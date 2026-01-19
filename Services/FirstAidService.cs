// ============================================================================
// FirstAidService.cs
// Serwis zarządzania apteczką: leki, kontenery, harmonogramy dawkowania.
// ============================================================================

#nullable enable

#region Importy
using System;                   // Podstawowe typy (DateTime, Decimal)
using System.Collections.Generic; // Kolekcje (List)
using System.Data;              // Typy bazodanowe (DataRow, DataTable)
using System.Data.SqlClient;    // Klient SQL Server
using TimeManager.Database;     // DatabaseHelper
using TimeManager.Models;       // MedicineItem, MedicineContainer, etc.
#endregion

namespace TimeManager.Services
{
    /// <summary>
    /// Serwis do zarządzania apteczką pierwszej pomocy i monitorowaniem leków.
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - ZARZĄDZANIE LEKAMI (MedicineItems): dodawanie, usuwanie, przenoszenie
    /// - KONTENERY (MedicineContainers): organizacja leków w pudełkach
    /// - PRODUKTY (MedicineProducts): katalog typów leków
    /// - HARMONOGRAMY (MedicineSchedules): dawkowanie rano/wieczór
    /// - STATUS PRZYJĘCIA (MedicineIntakeStatus): śledzenie dawek
    /// 
    /// AUTOMATYZACJA:
    /// - Usuwa leki o ilości 0
    /// - Zmniejsza ilości po oznaczeniu dawki jako przyjętej
    /// - Dezaktywuje harmonogram gdy wszystkie dawki zostały przyjęte
    /// </summary>
    public class FirstAidService
    {
        #region Zarządzanie lekami (MedicineItems)

        /// <summary>
        /// Pobiera wszystkie leki z bazy danych.
        /// Automatycznie usuwa leki o ilości 0.
        /// </summary>
        /// <returns>Lista leków posortowana po dacie ważności i nazwie</returns>
        public List<MedicineItem> GetAllMedicineItems()
        {
            // Wyczyść leki o ilości 0
            RemoveZeroQuantityItems();
            string query = "SELECT * FROM MedicineItems ORDER BY ExpirationDate, Name";
            var items = new List<MedicineItem>();
            var dataTable = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(MapRowToMedicineItem(row));
            }

            return items;
        }

        /// <summary>
        /// Dodaje nowy lek do bazy danych.
        /// </summary>
        /// <param name="item">Lek do dodania</param>
        public void AddMedicineItem(MedicineItem item)
        {
            string query = @"INSERT INTO MedicineItems 
                (Name, Quantity, Unit, ExpirationDate, ContainerID, ProductID)
                VALUES (@Name, @Quantity, @Unit, @ExpirationDate, @ContainerID, @ProductID)";

            var parameters = new[]
            {
                new SqlParameter("@Name", item.Name),
                new SqlParameter("@Quantity", item.Quantity),
                new SqlParameter("@Unit", item.Unit),
                new SqlParameter("@ExpirationDate", (object?)item.ExpirationDate ?? DBNull.Value),
                new SqlParameter("@ContainerID", (object?)item.ContainerID ?? DBNull.Value),
                new SqlParameter("@ProductID", (object?)item.ProductID ?? DBNull.Value)
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Aktualizuje istniejący lek w bazie.
        /// Jeśli ilość <= 0, automatycznie usuwa lek.
        /// </summary>
        /// <param name="item">Lek do zaktualizowania</param>
        public void UpdateMedicineItem(MedicineItem item)
        {
            if (item == null)
                return;

            // Auto-usuń leki które się skończyły
            if (item.Quantity <= 0)
            {
                DeleteMedicineItem(item.ItemID);
                return;
            }

            string query = @"UPDATE MedicineItems SET 
                Name = @Name, Quantity = @Quantity, Unit = @Unit, ExpirationDate = @ExpirationDate,
                ContainerID = @ContainerID, ProductID = @ProductID,
                LastModified = GETDATE()
                WHERE ItemID = @ItemID";

            var parameters = new[]
            {
                new SqlParameter("@ItemID", item.ItemID),
                new SqlParameter("@Name", item.Name),
                new SqlParameter("@Quantity", item.Quantity),
                new SqlParameter("@Unit", item.Unit),
                new SqlParameter("@ExpirationDate", (object?)item.ExpirationDate ?? DBNull.Value),
                new SqlParameter("@ContainerID", (object?)item.ContainerID ?? DBNull.Value),
                new SqlParameter("@ProductID", (object?)item.ProductID ?? DBNull.Value)
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Usuwa lek z bazy danych.
        /// </summary>
        /// <param name="itemId">ID leku do usunięcia</param>
        public void DeleteMedicineItem(int itemId)
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM MedicineItems WHERE ItemID = @ItemID",
                new SqlParameter("@ItemID", itemId));
        }

        #endregion

        #region Kontenery (MedicineContainers)

        /// <summary>
        /// Pobiera wszystkie kontenery (pudełka, szuflady, itp.).
        /// </summary>
        /// <returns>Lista kontenerów posortowana po nazwie</returns>
        public List<MedicineContainer> GetContainers()
        {
            string query = "SELECT * FROM MedicineContainers ORDER BY Name";
            var table = DatabaseHelper.ExecuteQuery(query);

            var result = new List<MedicineContainer>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRowToContainer(row));
            }
            return result;
        }

        /// <summary>
        /// Dodaje nowy kontener.
        /// </summary>
        /// <param name="container">Kontener do dodania</param>
        /// <returns>ID nowego kontenera</returns>
        public int AddContainer(MedicineContainer container)
        {
            const string query = @"INSERT INTO MedicineContainers (Name)
                                   VALUES (@Name);
                                   SELECT SCOPE_IDENTITY();";

            var idObj = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Name", container.Name));

            return Convert.ToInt32(idObj);
        }


        /// <summary>
        /// Usuwa kontener WRAZ z wszystkimi lekami w nim.
        /// </summary>
        /// <param name="containerId">ID kontenera do usunięcia</param>
        public void DeleteContainerWithItems(int containerId)
        {
            // Najpierw usuń wszystkie leki w kontenerze
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM MedicineItems WHERE ContainerID = @id",
                new SqlParameter("@id", containerId));

            // Potem usuń sam kontener
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM MedicineContainers WHERE ContainerID = @id",
                new SqlParameter("@id", containerId));
        }

        #endregion

        #region Produkty (MedicineProducts)

        /// <summary>
        /// Pobiera katalog produktów lekowych.
        /// Produkty to definicje typów leków (np. "Ibuprofen", "Paracetamol").
        /// </summary>
        /// <returns>Lista produktów posortowana po nazwie</returns>
        public List<MedicineProduct> GetAllProducts()
        {
            var table = DatabaseHelper.ExecuteQuery("SELECT * FROM MedicineProducts ORDER BY Name");
            var result = new List<MedicineProduct>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRowToProduct(row));
            }
            return result;
        }

        /// <summary>
        /// Dodaje nowy produkt lekowy do katalogu.
        /// </summary>
        /// <param name="product">Produkt do dodania</param>
        /// <returns>ID nowego produktu</returns>
        public int AddProduct(MedicineProduct product)
        {
            const string query = @"INSERT INTO MedicineProducts (Name)
                                   VALUES (@Name);
                                   SELECT SCOPE_IDENTITY();";

            var idObj = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Name", product.Name));

            return Convert.ToInt32(idObj);
        }

        /// <summary>
        /// Sprawdza czy produkt jest używany przez jakieś leki.
        /// Jeśli tak, nie można go usunąć.
        /// </summary>
        /// <param name="productId">ID produktu do sprawdzenia</param>
        /// <returns>True jeśli produkt jest używany</returns>
        public bool ProductIsUsed(int productId)
        {
            const string query = "SELECT COUNT(*) FROM MedicineItems WHERE ProductID = @id";
            var count = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@id", productId));
            return Convert.ToInt32(count) > 0;
        }

        /// <summary>
        /// Usuwa produkt z katalogu.
        /// Nie można usunąć produktu który jest używany przez leki.
        /// </summary>
        /// <param name="productId">ID produktu do usunięcia</param>
        /// <returns>True jeśli usunięto, false jeśli produkt jest w użyciu</returns>
        public bool DeleteProduct(int productId)
        {
            // Nie można usunąć produktu w użyciu
            if (ProductIsUsed(productId))
                return false;

            const string query = "DELETE FROM MedicineProducts WHERE ProductID = @id";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@id", productId));
            return true;
        }

        /// <summary>
        /// Pobiera leki w danym kontenerze.
        /// Automatycznie usuwa leki o ilości 0.
        /// </summary>
        /// <param name="containerId">ID kontenera</param>
        /// <returns>Lista leków posortowana po dacie ważności i nazwie</returns>
        public List<MedicineItem> GetMedicineItemsByContainer(int containerId)
        {
            // Wyczyść leki o ilości 0 w tym kontenerze
            RemoveZeroQuantityItems(containerId);
            const string query = @"SELECT * FROM MedicineItems 
                                   WHERE ContainerID = @ContainerID
                                   ORDER BY ExpirationDate, Name";

            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ContainerID", containerId));
            var result = new List<MedicineItem>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRowToMedicineItem(row));
            }
            return result;
        }

        /// <summary>
        /// Pobiera pojedynczy lek po ID.
        /// </summary>
        /// <param name="itemId">ID leku</param>
        /// <returns>Lek lub null</returns>
        public MedicineItem? GetMedicineItemById(int itemId)
        {
            const string query = "SELECT * FROM MedicineItems WHERE ItemID = @ItemID";
            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ItemID", itemId));

            if (table.Rows.Count == 0)
                return null;

            return MapRowToMedicineItem(table.Rows[0]);
        }

        /// <summary>
        /// Przenosi lek (lub część ilości) do innego kontenera.
        /// 
        /// Jeśli przenoszona ilość >= całkowitej - przenosi cały lek.
        /// Jeśli mniej - rozdziela: zmniejsza oryginalny i tworzy nowy wpis w docelowym kontenerze.
        /// </summary>
        /// <param name="itemId">ID leku do przeniesienia</param>
        /// <param name="targetContainerId">ID docelowego kontenera</param>
        /// <param name="amountToMove">Ilość do przeniesienia</param>
        public void MoveMedicineItem(int itemId, int targetContainerId, decimal amountToMove)
        {
            var item = GetMedicineItemById(itemId);
            if (item == null)
                return;

            if (amountToMove <= 0)
                return;

            if (amountToMove >= item.Quantity)
            {
                // Przenieś cały przedmiot
                item.ContainerID = targetContainerId;
                UpdateMedicineItem(item);
                return;
            }

            // Rozdziel przedmiot: zmniejsz oryginalny, utwórz nowy dla przenoszonej ilości
            decimal remaining = item.Quantity - amountToMove;
            item.Quantity = remaining;
            UpdateMedicineItem(item);

            var moved = new MedicineItem
            {
                Name = item.Name,
                ProductID = item.ProductID,
                Quantity = amountToMove,
                Unit = item.Unit,
                ExpirationDate = item.ExpirationDate,
                ContainerID = targetContainerId,
                AddedDate = DateTime.Now,
                LastModified = DateTime.Now
            };
            AddMedicineItem(moved);
        }

        #endregion

        #region Harmonogramy i monitorowanie (MedicineSchedules)

        /// <summary>
        /// Pobiera aktywne harmonogramy dawkowania.
        /// </summary>
        public List<MedicineSchedule> GetActiveSchedules()
        {
            const string query = @"SELECT ms.*, mi.Name AS MedicineName
                                   FROM MedicineSchedules ms
                                   INNER JOIN MedicineItems mi ON mi.ItemID = ms.MedicineItemID
                                   WHERE ms.IsActive = 1";

            var table = DatabaseHelper.ExecuteQuery(query);
            var schedules = new List<MedicineSchedule>();
            foreach (DataRow row in table.Rows)
            {
                schedules.Add(MapRowToSchedule(row));
            }

            return schedules;
        }

        public int AddSchedule(MedicineSchedule schedule)
        {
            const string query = @"INSERT INTO MedicineSchedules
                (MedicineItemID, MorningDose, EveningDose, IntervalDays, StartDate, EndDate, IsActive)
                VALUES (@MedicineItemID, @MorningDose, @EveningDose, @IntervalDays, @StartDate, @EndDate, @IsActive);
                SELECT SCOPE_IDENTITY();";

            var idObj = DatabaseHelper.ExecuteScalar(
                query,
                new SqlParameter("@MedicineItemID", schedule.MedicineItemID),
                new SqlParameter("@MorningDose", (object?)schedule.MorningDose ?? DBNull.Value),
                new SqlParameter("@EveningDose", (object?)schedule.EveningDose ?? DBNull.Value),
                new SqlParameter("@IntervalDays", schedule.IntervalDays),
                new SqlParameter("@StartDate", schedule.StartDate.Date),
                new SqlParameter("@EndDate", (object?)schedule.EndDate ?? DBNull.Value),
                new SqlParameter("@IsActive", schedule.IsActive));

            return Convert.ToInt32(idObj);
        }

        public void UpdateSchedule(MedicineSchedule schedule)
        {
            const string query = @"UPDATE MedicineSchedules
                                   SET MedicineItemID = @MedicineItemID,
                                       MorningDose = @MorningDose,
                                       EveningDose = @EveningDose,
                                       IntervalDays = @IntervalDays,
                                       StartDate = @StartDate,
                                       EndDate = @EndDate,
                                       IsActive = @IsActive
                                   WHERE ScheduleID = @ScheduleID";

            DatabaseHelper.ExecuteNonQuery(
                query,
                new SqlParameter("@MedicineItemID", schedule.MedicineItemID),
                new SqlParameter("@MorningDose", (object?)schedule.MorningDose ?? DBNull.Value),
                new SqlParameter("@EveningDose", (object?)schedule.EveningDose ?? DBNull.Value),
                new SqlParameter("@IntervalDays", schedule.IntervalDays),
                new SqlParameter("@StartDate", schedule.StartDate.Date),
                new SqlParameter("@EndDate", (object?)schedule.EndDate ?? DBNull.Value),
                new SqlParameter("@IsActive", schedule.IsActive),
                new SqlParameter("@ScheduleID", schedule.ScheduleID));
        }

        public void DeleteMedicineSchedule(int scheduleId)
        {
            // Najpierw usuń powiązane rekordy statusu dawkowania
            const string deleteStatus = @"DELETE FROM MedicineIntakeStatus WHERE ScheduleID = @ScheduleID";
            DatabaseHelper.ExecuteNonQuery(deleteStatus, new SqlParameter("@ScheduleID", scheduleId));

            // Potem usuń sam harmonogram
            const string deleteSchedule = @"DELETE FROM MedicineSchedules WHERE ScheduleID = @ScheduleID";
            DatabaseHelper.ExecuteNonQuery(deleteSchedule, new SqlParameter("@ScheduleID", scheduleId));
        }

        public MedicineSchedule? GetScheduleById(int scheduleId)
        {
            const string query = @"SELECT ms.*, mi.Name AS MedicineName
                                   FROM MedicineSchedules ms
                                   INNER JOIN MedicineItems mi ON mi.ItemID = ms.MedicineItemID
                                   WHERE ms.ScheduleID = @ScheduleID";

            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ScheduleID", scheduleId));
            if (table.Rows.Count == 0)
                return null;

            return MapRowToSchedule(table.Rows[0]);
        }

        public MedicineIntakeStatus? GetIntakeStatus(int scheduleId, DateTime date)
        {
            // Porównaj tylko po dacie (działa nawet gdy kolumna [Date] jest typu DATETIME z komponentem czasu)
            const string query = @"SELECT TOP 1 * FROM MedicineIntakeStatus
                                   WHERE ScheduleID = @ScheduleID AND CAST([Date] AS DATE) = @Date";
            var table = DatabaseHelper.ExecuteQuery(query,
                new SqlParameter("@ScheduleID", scheduleId),
                new SqlParameter("@Date", date.Date));

            if (table.Rows.Count == 0)
                return null;

            return MapRowToIntakeStatus(table.Rows[0]);
        }

        public bool SetIntakeStatus(int scheduleId, DateTime date, bool? morningTaken, bool? eveningTaken)
        {
            var existing = GetIntakeStatus(scheduleId, date);
            bool oldMorning = existing?.MorningTaken ?? false;
            bool oldEvening = existing?.EveningTaken ?? false;

            bool morning = morningTaken ?? oldMorning;
            bool evening = eveningTaken ?? oldEvening;

            // Zwaliduj ilość przed zapisem statusu / dekrementacją
            if (!HasEnoughQuantityForToggle(scheduleId, oldMorning, morning, oldEvening, evening))
            {
                return false;
            }

            if (existing == null)
            {
                const string insert = @"INSERT INTO MedicineIntakeStatus (ScheduleID, [Date], MorningTaken, EveningTaken)
                                        VALUES (@ScheduleID, @Date, @MorningTaken, @EveningTaken)";
                DatabaseHelper.ExecuteNonQuery(insert,
                    new SqlParameter("@ScheduleID", scheduleId),
                    new SqlParameter("@Date", date.Date),
                    new SqlParameter("@MorningTaken", morning),
                    new SqlParameter("@EveningTaken", evening));
            }
            else
            {
                const string update = @"UPDATE MedicineIntakeStatus
                                        SET MorningTaken = @MorningTaken,
                                            EveningTaken = @EveningTaken
                                        WHERE StatusID = @StatusID";
                DatabaseHelper.ExecuteNonQuery(update,
                    new SqlParameter("@StatusID", existing.StatusID),
                    new SqlParameter("@MorningTaken", morning),
                    new SqlParameter("@EveningTaken", evening));
            }

            ApplyDoseDecrements(scheduleId, oldMorning, morning, oldEvening, evening);
            
            // Sprawdź czy wszystkie dawki zostały zakończone dla harmonogramów z datą końcową
            CheckAndCompleteSchedule(scheduleId);
            
            return true;
        }
        
        /// <summary>
        /// Sprawdza czy wszystkie wymagane dawki zostały przyjęte dla harmonogramu z datą końcową.
        /// Jeśli tak, dezaktywuje harmonogram i wyświetla komunikat o zakończeniu.
        /// </summary>
        private void CheckAndCompleteSchedule(int scheduleId)
        {
            var schedule = GetScheduleById(scheduleId);
            if (schedule == null || !schedule.EndDate.HasValue || !schedule.IsActive)
                return;
            
            // Oblicz wszystkie wymagane daty dla tego harmonogramu
            var allDates = new List<DateTime>();
            var current = schedule.StartDate.Date;
            var end = schedule.EndDate.Value.Date;
            
            while (current <= end)
            {
                allDates.Add(current);
                current = current.AddDays(schedule.IntervalDays);
            }
            
            // Sprawdź czy wszystkie dawki zostały przyjęte dla wszystkich dat
            foreach (var d in allDates)
            {
                var status = GetIntakeStatus(scheduleId, d);
                
                // Sprawdź poranną dawkę jeśli wymagana
                if (schedule.MorningDose.HasValue && schedule.MorningDose.Value > 0)
                {
                    if (status == null || !status.MorningTaken)
                        return; // Nie wszystkie ukończone
                }
                
                // Sprawdź wieczorną dawkę jeśli wymagana
                if (schedule.EveningDose.HasValue && schedule.EveningDose.Value > 0)
                {
                    if (status == null || !status.EveningTaken)
                        return; // Nie wszystkie ukończone
                }
            }
            
            // Wszystkie dawki ukończone - dezaktywuj harmonogram
            schedule.IsActive = false;
            UpdateSchedule(schedule);
            
            // Pokaż komunikat o zakończeniu
            System.Windows.Forms.MessageBox.Show(
                $"Congratulations! You completed all doses for \"{schedule.MedicineName}\"!\n\n" +
                "The monitoring has been automatically ended.",
                "Medication Monitoring Completed",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }

        #endregion

        #region Mapowanie wierszy (Helper methods)

        /// <summary>
        /// Mapuje wiersz DataRow na obiekt MedicineItem.
        /// </summary>
        private MedicineItem MapRowToMedicineItem(DataRow row)
        {
            return new MedicineItem
            {
                ItemID = Convert.ToInt32(row["ItemID"]),
                Name = row["Name"].ToString(),
                Quantity = Convert.ToDecimal(row["Quantity"]),
                Unit = row["Unit"].ToString(),
                ExpirationDate = row.Table.Columns.Contains("ExpirationDate") && row["ExpirationDate"] != DBNull.Value
                    ? Convert.ToDateTime(row["ExpirationDate"]) : (DateTime?)null,
                AddedDate = row.Table.Columns.Contains("AddedDate") && row["AddedDate"] != DBNull.Value
                    ? Convert.ToDateTime(row["AddedDate"]) : DateTime.Now,
                LastModified = row.Table.Columns.Contains("LastModified") && row["LastModified"] != DBNull.Value
                    ? Convert.ToDateTime(row["LastModified"]) : DateTime.Now,
                ContainerID = row.Table.Columns.Contains("ContainerID") && row["ContainerID"] != DBNull.Value
                    ? (int?)Convert.ToInt32(row["ContainerID"]) : null,
                ProductID = row.Table.Columns.Contains("ProductID") && row["ProductID"] != DBNull.Value
                    ? (int?)Convert.ToInt32(row["ProductID"]) : null
            };
        }

        private MedicineContainer MapRowToContainer(DataRow row)
        {
            return new MedicineContainer
            {
                ContainerID = Convert.ToInt32(row["ContainerID"]),
                Name = row["Name"].ToString()
            };
        }

        private MedicineProduct MapRowToProduct(DataRow row)
        {
            return new MedicineProduct
            {
                ProductID = Convert.ToInt32(row["ProductID"]),
                Name = row["Name"].ToString(),
                DefaultUnit = row.Table.Columns.Contains("DefaultUnit") ? row["DefaultUnit"].ToString() : null
            };
        }

        private MedicineSchedule MapRowToSchedule(DataRow row)
        {
            return new MedicineSchedule
            {
                ScheduleID = Convert.ToInt32(row["ScheduleID"]),
                MedicineItemID = Convert.ToInt32(row["MedicineItemID"]),
                MedicineName = row.Table.Columns.Contains("MedicineName") ? row["MedicineName"].ToString() : string.Empty,
                MorningDose = row["MorningDose"] != DBNull.Value ? Convert.ToDecimal(row["MorningDose"]) : (decimal?)null,
                EveningDose = row["EveningDose"] != DBNull.Value ? Convert.ToDecimal(row["EveningDose"]) : (decimal?)null,
                IntervalDays = Convert.ToInt32(row["IntervalDays"]),
                StartDate = Convert.ToDateTime(row["StartDate"]),
                EndDate = row["EndDate"] != DBNull.Value ? Convert.ToDateTime(row["EndDate"]) : (DateTime?)null,
                IsActive = Convert.ToBoolean(row["IsActive"])
            };
        }

        private MedicineIntakeStatus MapRowToIntakeStatus(DataRow row)
        {
            return new MedicineIntakeStatus
            {
                StatusID = Convert.ToInt32(row["StatusID"]),
                ScheduleID = Convert.ToInt32(row["ScheduleID"]),
                Date = Convert.ToDateTime(row["Date"]),
                MorningTaken = row["MorningTaken"] != DBNull.Value && Convert.ToBoolean(row["MorningTaken"]),
                EveningTaken = row["EveningTaken"] != DBNull.Value && Convert.ToBoolean(row["EveningTaken"])
            };
        }

        private bool HasEnoughQuantityForToggle(int scheduleId, bool oldMorning, bool newMorning, bool oldEvening, bool newEvening)
        {
            var schedule = GetScheduleById(scheduleId);
            if (schedule == null)
                return false;

            var medicine = GetMedicineItemById(schedule.MedicineItemID);
            if (medicine == null)
                return false;

            decimal required = 0;
            if (!oldMorning && newMorning && schedule.MorningDose.HasValue)
                required += schedule.MorningDose.Value;
            if (!oldEvening && newEvening && schedule.EveningDose.HasValue)
                required += schedule.EveningDose.Value;

            return medicine.Quantity >= required;
        }

        private void ApplyDoseDecrements(int scheduleId, bool oldMorning, bool newMorning, bool oldEvening, bool newEvening)
        {
            var schedule = GetScheduleById(scheduleId);
            if (schedule == null)
                return;

            var medicine = GetMedicineItemById(schedule.MedicineItemID);
            if (medicine == null)
                return;

            decimal newQuantity = medicine.Quantity;

            // Delta poranna: +dawka jeśli zaznaczone, -dawka jeśli odznaczone
            if (schedule.MorningDose.HasValue)
            {
                var delta = (newMorning ? 1 : 0) - (oldMorning ? 1 : 0);
                newQuantity -= delta * schedule.MorningDose.Value;
            }

            // Delta wieczorna
            if (schedule.EveningDose.HasValue)
            {
                var delta = (newEvening ? 1 : 0) - (oldEvening ? 1 : 0);
                newQuantity -= delta * schedule.EveningDose.Value;
            }

            if (newQuantity < 0)
                newQuantity = 0;

            if (newQuantity != medicine.Quantity)
            {
                medicine.Quantity = newQuantity;
                UpdateMedicineItem(medicine);
            }
        }

        private void RemoveZeroQuantityItems(int? containerId = null)
        {
            if (containerId.HasValue)
            {
                DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM MedicineItems WHERE Quantity <= 0 AND ContainerID = @ContainerID",
                    new SqlParameter("@ContainerID", containerId.Value));
            }
            else
            {
                DatabaseHelper.ExecuteNonQuery(
                    "DELETE FROM MedicineItems WHERE Quantity <= 0");
            }
        }

        #endregion
    }
}

