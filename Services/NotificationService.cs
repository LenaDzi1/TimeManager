// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Collections.Generic; // Kolekcje generyczne
using System.Linq;              // LINQ dla kolekcji
using System.Data;              // Typy bazodanowe
using System.Data.SqlClient;    // Klient SQL Server
using TimeManager.Database;     // Helper do bazy danych
using TimeManager.Models;       // Modele aplikacji

namespace TimeManager.Services
{
    /// <summary>
    /// Serwis do zarządzania powiadomieniami w aplikacji.
    /// 
    /// Główne funkcje:
    /// - TWORZENIE powiadomień (manualne, przypomnienia o eventach, śledzenie żywności/leków/roślin)
    /// - WYSYŁANIE/CZYTANIE - śledzenie statusu powiadomień
    /// - AUTOMATYCZNE POWIADOMIENIA - wygasające produkty, końcące się leki, podlewanie roślin
    /// - USTAWIENIA - konfiguracja auto-dodawania do kalendarza
    /// 
    /// Typy powiadomień: event, foodtracking, foodexpiry, medicinetracking, planttracker
    /// </summary>
    public class NotificationService
    {
     
        /// <summary>
        /// Tworzy nowe powiadomienie.
        /// </summary>
        /// <param name="title">Tytuł powiadomienia</param>
        /// <param name="message">Treść powiadomienia</param>
        /// <param name="type">Typ: event, foodtracking, foodexpiry, medicinetracking, planttracker</param>
        /// <param name="referenceId">Opcjonalne ID powiązanego obiektu</param>
        /// <param name="scheduledDateTime">Kiedy wyświetlić powiadomienie</param>
        public void CreateNotification(string title, string message, string type, int? referenceId, DateTime scheduledDateTime)
        {
            string query = @"INSERT INTO Notifications 
                (Title, Message, NotificationType, ReferenceID, ScheduledDateTime)
                VALUES (@Title, @Message, @NotificationType, @ReferenceID, @ScheduledDateTime)";

            var parameters = new[]
            {
                new SqlParameter("@Title", title),
                new SqlParameter("@Message", message),
                new SqlParameter("@NotificationType", type),
                new SqlParameter("@ReferenceID", (object)referenceId ?? DBNull.Value),
                new SqlParameter("@ScheduledDateTime", scheduledDateTime)
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Oznacza powiadomienie jako przeczytane.
        /// </summary>
        public void MarkAsRead(int notificationId)
        {
            string query = "UPDATE Notifications SET IsRead = 1 WHERE NotificationID = @NotificationID";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@NotificationID", notificationId));
        }

        /// <summary>
        /// Pobiera wszystkie powiadomienia (historia).
        /// </summary>
        /// <returns>Lista wszystkich powiadomień posortowana od najnowszych</returns>
        public List<Notification> GetAllNotifications()
        {
            const string query = @"SELECT * FROM Notifications ORDER BY CreatedDate DESC";
            var notifications = new List<Notification>();
            var dataTable = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                notifications.Add(MapRowToNotification(row));
            }

            return notifications;
        }

        /// <summary>
        /// Pobiera aktywne (nieprzeczytane) powiadomienia dla wskaźnika.
        /// </summary>
        /// <returns>Lista nieprzeczytanych powiadomień</returns>
        public List<Notification> GetActiveNotifications()
        {
            const string query = @"SELECT * FROM Notifications 
                                   WHERE IsRead = 0 
                                   ORDER BY ScheduledDateTime DESC";
            var notifications = new List<Notification>();
            var dataTable = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                notifications.Add(MapRowToNotification(row));
            }

            return notifications;
        }

        public void DeleteNotification(int notificationId)
        {
            const string query = "DELETE FROM Notifications WHERE NotificationID = @NotificationID";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@NotificationID", notificationId));
        }

        public void CheckAndCreateTrackingNotifications()
        {
            var trackingService = new TrackingService();
            var firstAidService = new FirstAidService();
            var eventService = new EventService();
            var shoppingListService = new ShoppingListService(trackingService, firstAidService, eventService);

            var missingItems = shoppingListService.CalculateMissingItems();
            if (missingItems.Any() && !NotificationExists("foodtracking", null))
            {
                CreateNotification(
                    "Food tracker",
                    "There are missing items in your kitchen.",
                    "foodtracking",
                    null,
                    DateTime.Now);
            }
        
            // Próg jest zdefiniowany w FridgeItem.IsExpiringSoon() (obecnie 3 dni).
            var fridgeItems = trackingService.GetAllFridgeItems();
            foreach (var item in fridgeItems)
            {
                if (!item.ExpirationDate.HasValue)
                    continue;

                // Sprawdź czy wkrótce wygasa LUB już wygasło
                if (!item.IsExpiringSoon() && !item.IsExpired())
                    continue;

                if (NotificationExists("foodexpiry", item.ItemID))
                    continue;

                var daysLeft = (item.ExpirationDate.Value.Date - DateTime.Now.Date).Days;
                string msg;
                
                if (daysLeft < 0)
                    msg = $"\"{item.Name}\" has EXPIRED!";
                else if (daysLeft == 0)
                    msg = $"\"{item.Name}\" expires today.";
                else
                    msg = $"\"{item.Name}\" expires in {daysLeft} day(s).";

                CreateNotification(
                    item.Name,
                    msg,
                    "foodexpiry",
                    item.ItemID,
                    DateTime.Now);
            }

            // 2) MedicineTracking: ilość <= 3 (z apteczki) LUB wygasłe
            var medicineItems = firstAidService.GetAllMedicineItems();
            foreach (var item in medicineItems)
            {
                // Sprawdź niski stan
                bool isLow = item.Quantity <= 3;
                // Sprawdź wygasłe/wygasające
                bool isExpiring = item.ExpirationDate.HasValue && (item.IsExpiringSoon() || item.IsExpired());

                if (!isLow && !isExpiring) continue;

                if (NotificationExists("medicinetracking", item.ItemID))
                    continue;

                string msg;
                if (isExpiring)
                {
                     var daysLeft = (item.ExpirationDate!.Value.Date - DateTime.Now.Date).Days;
                     if (daysLeft < 0) msg = $"Medicine \"{item.Name}\" has EXPIRED!";
                     else if (daysLeft == 0) msg = $"Medicine \"{item.Name}\" expires today.";
                     else msg = $"Medicine \"{item.Name}\" expires in {daysLeft} days.";
                }
                else
                {
                    msg = $"The medicine \"{item.Name}\" is ending soon.";
                }

                CreateNotification(
                    item.Name,
                    msg,
                    "medicinetracking",
                    item.ItemID,
                    DateTime.Now);
            }

            // 3) PlantTracker
            var plants = trackingService.GetAllPlants();
            foreach (var plant in plants)
            {
                if (plant.NeedsWatering() && !NotificationExists("planttracker", plant.PlantID))
                {
                    CreateNotification(
                        plant.Name,
                        $"{plant.Name} needs watering!",
                        "planttracker",
                        plant.PlantID,
                        DateTime.Now);
                }
            }
        }

        private Notification MapRowToNotification(DataRow row)
        {
            return new Notification
            {
                NotificationID = Convert.ToInt32(row["NotificationID"]),
                Title = row["Title"].ToString(),
                Message = row["Message"].ToString(),
                NotificationType = row["NotificationType"].ToString(),
                ReferenceID = row["ReferenceID"] != DBNull.Value ? Convert.ToInt32(row["ReferenceID"]) : (int?)null,
                ScheduledDateTime = Convert.ToDateTime(row["ScheduledDateTime"]),
                IsSent = Convert.ToBoolean(row["IsSent"]),
                IsRead = Convert.ToBoolean(row["IsRead"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"])
            };
        }

        private bool NotificationExists(string type, int? referenceId)
        {
            const string query = @"SELECT COUNT(1) FROM Notifications 
                                   WHERE NotificationType = @Type 
                                     AND ((@RefId IS NULL AND ReferenceID IS NULL) OR ReferenceID = @RefId)";

            var countObj = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Type", type),
                new SqlParameter("@RefId", (object)referenceId ?? DBNull.Value));

            return countObj != null && int.TryParse(countObj.ToString(), out var count) && count > 0;
        }
    }
}





