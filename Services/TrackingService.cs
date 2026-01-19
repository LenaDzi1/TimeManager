// ============================================================================
// TrackingService.cs
// Serwis śledzenia: jedzenie, rośliny, nawyki, przepisy, lista zakupów.
// ============================================================================

#nullable enable

#region Importy
using System;                   // Podstawowe typy (DateTime, Decimal)
using System.Collections.Generic; // Kolekcje (List)
using System.Data;              // DataRow, DataTable
using System.Data.SqlClient;    // SqlParameter, SqlCommand
using TimeManager.Database;     // DatabaseHelper
using TimeManager.Models;       // FridgeItem, Plant, HabitStep, etc.
#endregion

namespace TimeManager.Services
{
    /// <summary>
    /// Serwis do śledzenia różnych aspektów codziennego życia.
    /// 
    /// MODUŁY:
    /// - Jedzenie: FridgeItems, FoodContainers, FoodProducts
    /// - Rośliny: Plants, historia podlewania
    /// - Nawyki: HabitCategories, HabitSteps, HabitCompletions
    /// - Przepisy: Recipes, RecipeIngredients
    /// - Lista zakupów: ShoppingList
    /// 
    /// INTEGRACJE:
    /// - PointsService: nagradzanie za wykonane nawyki
    /// </summary>
    public class TrackingService
    {
        #region Pola i serwisy

        /// <summary>Serwis do obsługi punktów użytkownika.</summary>
        private readonly PointsService _pointsService = new PointsService();

        #endregion

        #region Zarządzanie lodówką (FridgeItems)

        /// <summary>
        /// Pobiera wszystkie produkty spożywcze z bazy.
        /// </summary>
        public List<FridgeItem> GetAllFridgeItems()
        {
            string query = "SELECT * FROM FridgeItems ORDER BY ExpirationDate, Name";
            var items = new List<FridgeItem>();
            var dataTable = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                items.Add(MapRowToFridgeItem(row));
            }

            return items;
        }









        public void DeleteFridgeItem(int itemId)
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM FridgeItems WHERE ItemID = @ItemID",
                new SqlParameter("@ItemID", itemId));
        }
        public List<FoodContainer> GetContainers(string type)
        {
            string query = "SELECT * FROM FoodContainers WHERE Type = @Type ORDER BY Name";
            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@Type", type));

            var result = new List<FoodContainer>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRowToContainer(row));
            }
            return result;
        }

        public int AddContainer(FoodContainer container)
        {
            const string query = @"INSERT INTO FoodContainers (Name, Type)
                                   VALUES (@Name, @Type);
                                   SELECT SCOPE_IDENTITY();";

            var idObj = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Name", container.Name),
                new SqlParameter("@Type", container.Type));

            return Convert.ToInt32(idObj);
        }

        public void DeleteContainerWithItems(int containerId)
        {
            // 1) najpierw usuwamy produkty z kontenera
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM FridgeItems WHERE ContainerID = @id",
                new SqlParameter("@id", containerId));

            // 2) potem sam kontener
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM FoodContainers WHERE ContainerID = @id",
                new SqlParameter("@id", containerId));
        }


        public List<FoodProduct> GetAllProducts()
        {
            var table = DatabaseHelper.ExecuteQuery("SELECT * FROM FoodProducts ORDER BY Name");
            var result = new List<FoodProduct>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRowToProduct(row));
            }
            return result;
        }

        public int AddProduct(FoodProduct product)
        {
            const string query = @"INSERT INTO FoodProducts (Name)
                                   VALUES (@Name);
                                   SELECT SCOPE_IDENTITY();";

            var idObj = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Name", product.Name));
             
            return Convert.ToInt32(idObj);
        }
        public bool ProductIsUsed(int productId)
        {
            const string query = "SELECT COUNT(*) FROM FridgeItems WHERE ProductID = @id";
            var count = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@id", productId));
            return Convert.ToInt32(count) > 0;
        }

        public bool DeleteProduct(int productId)
        {
            // Jeśli produkt jest używany - nie usuwamy
            if (ProductIsUsed(productId))
                return false;

            const string query = "DELETE FROM FoodProducts WHERE ProductID = @id";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@id", productId));
            return true;
        }


        public List<FridgeItem> GetFridgeItemsByContainer(int containerId)
        {
            const string query = @"SELECT * FROM FridgeItems 
                                   WHERE ContainerID = @ContainerID
                                   ORDER BY ExpirationDate, Name";

            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ContainerID", containerId));
            var result = new List<FridgeItem>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRowToFridgeItem(row));
            }
            return result;
        }

        public FridgeItem? GetFridgeItemById(int itemId)
        {
            const string query = "SELECT * FROM FridgeItems WHERE ItemID = @ItemID";
            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ItemID", itemId));

            if (table.Rows.Count == 0)
                return null;

            return MapRowToFridgeItem(table.Rows[0]);
        }

        // Przenieś przedmiot do innego kontenera, opcjonalnie dzieląc ilość
        public void MoveFridgeItem(int itemId, int targetContainerId, decimal amountToMove)
        {
            var item = GetFridgeItemById(itemId);
            if (item == null)
                return;

            if (amountToMove <= 0)
                return;

            if (amountToMove >= item.Quantity)
            {
                // Przenieś cały przedmiot
                item.ContainerID = targetContainerId;
                UpdateFridgeItem(item);
                return;
            }

            // Rozdziel przedmiot: zmniejsz oryginalny, utwórz nowy dla przeniesionej ilości
            decimal remaining = item.Quantity - amountToMove;
            item.Quantity = remaining;
            UpdateFridgeItem(item);

            var moved = new FridgeItem
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
            AddFridgeItem(moved);
        }

        #endregion



        #region Zarządzanie roślinami (Plants)

        /// <summary>
        /// Pobiera wszystkie rośliny posortowane wg daty podlewania.
        /// </summary>
        public List<Plant> GetAllPlants()
        {
            string query = "SELECT * FROM Plants ORDER BY NextWateringDate, Name";
            var plants = new List<Plant>();
            var dataTable = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                plants.Add(MapRowToPlant(row));
            }

            return plants;
        }





        public void WaterPlant(int plantId)
        {
            var plant = GetPlantById(plantId);
            if (plant != null)
            {
                plant.UpdateWateringSchedule();

                string query = @"UPDATE Plants SET 
                    LastWateredDate = @LastWateredDate, NextWateringDate = @NextWateringDate 
                    WHERE PlantID = @PlantID";

                var parameters = new[]
                {
                    new SqlParameter("@PlantID", plantId),
                    new SqlParameter("@LastWateredDate", plant.LastWateredDate),
                    new SqlParameter("@NextWateringDate", plant.NextWateringDate)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);

            }
        }

        public void DeletePlant(int plantId)
        {
            const string query = "DELETE FROM Plants WHERE PlantID = @PlantID";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@PlantID", plantId));
        }

        #endregion

        #region Zarządzanie nawykami (Habits)

        /// <summary>
        /// Pobiera kategorie nawyków użytkownika.
        /// </summary>
        public List<HabitCategory> GetHabitCategories(int userId)
        {
            const string query = @"SELECT HabitCategoryId, UserId, Name
                                   FROM HabitCategories
                                   WHERE UserId = @UserId
                                   ORDER BY Name";

            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId));
            var result = new List<HabitCategory>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRowToHabitCategory(row));
            }
            return result;
        }

        public int AddHabitCategory(int userId, string name)
        {
            const string query = @"INSERT INTO HabitCategories (UserId, Name)
                                   VALUES (@UserId, @Name);
                                   SELECT SCOPE_IDENTITY();";

            var idObj = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Name", name));

            return Convert.ToInt32(idObj);
        }

        // Habits - steps
        public List<HabitStep> GetHabitSteps(int habitCategoryId, int userId)
        {
            const string query = @"SELECT HabitStepId, HabitCategoryId, UserId, Name, StartDate, EndDate,
                                          RepeatEveryDays, Description, UseDaytimeSplit,
                                          EstimatedOccurrences, RemainingOccurrences, PointsReward
                                   FROM HabitSteps
                                   WHERE HabitCategoryId = @HabitCategoryId AND UserId = @UserId
                                   ORDER BY StartDate, Name";

            var table = DatabaseHelper.ExecuteQuery(query,
                new SqlParameter("@HabitCategoryId", habitCategoryId),
                new SqlParameter("@UserId", userId));

            var result = new List<HabitStep>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(MapRowToHabitStep(row));
            }
            return result;
        }

        public int AddHabitStep(HabitStep step)
        {
            EnsureOccurrences(step);

            const string query = @"INSERT INTO HabitSteps
                (HabitCategoryId, UserId, Name, StartDate, EndDate, RepeatEveryDays, Description, UseDaytimeSplit, EstimatedOccurrences, RemainingOccurrences, PointsReward)
                VALUES (@HabitCategoryId, @UserId, @Name, @StartDate, @EndDate, @RepeatEveryDays, @Description, @UseDaytimeSplit, @EstimatedOccurrences, @RemainingOccurrences, @PointsReward);
                SELECT SCOPE_IDENTITY();";

            var idObj = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@HabitCategoryId", step.HabitCategoryId),
                new SqlParameter("@UserId", step.UserId),
                new SqlParameter("@Name", step.Name),
                new SqlParameter("@StartDate", (object?)step.StartDate ?? DBNull.Value),
                new SqlParameter("@EndDate", (object?)step.EndDate ?? DBNull.Value),
                new SqlParameter("@RepeatEveryDays", (object?)step.RepeatEveryDays ?? DBNull.Value),
                new SqlParameter("@Description", (object?)step.Description ?? DBNull.Value),

                new SqlParameter("@UseDaytimeSplit", step.UseDaytimeSplit),
                new SqlParameter("@EstimatedOccurrences", (object?)step.EstimatedOccurrences ?? DBNull.Value),
                new SqlParameter("@RemainingOccurrences", (object?)step.RemainingOccurrences ?? DBNull.Value),
                new SqlParameter("@PointsReward", (object?)step.PointsReward ?? DBNull.Value));

            return Convert.ToInt32(idObj);
        }

        public void UpdateHabitCategory(int id, string name)
        {
            const string query = "UPDATE HabitCategories SET Name = @Name WHERE HabitCategoryId = @Id";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Name", name), new SqlParameter("@Id", id));
        }

        public void DeleteHabitCategory(int id)
        {
            // Najpierw usuń kroki
            const string deleteSteps = "DELETE FROM HabitSteps WHERE HabitCategoryId = @Id";
            DatabaseHelper.ExecuteNonQuery(deleteSteps, new SqlParameter("@Id", id));

            // Następnie usuń kategorię
            const string deleteCat = "DELETE FROM HabitCategories WHERE HabitCategoryId = @Id";
            DatabaseHelper.ExecuteNonQuery(deleteCat, new SqlParameter("@Id", id));
        }

        public void UpdateHabitStep(HabitStep step)
        {
            // Wymuś ponowne obliczenie Estimated
            int estimated = 1;
            if (step.StartDate.HasValue && step.EndDate.HasValue)
            {
                 var span = (step.EndDate.Value - step.StartDate.Value).TotalDays;
                 var repeat = step.RepeatEveryDays.GetValueOrDefault(0);
                 if (repeat > 0) estimated = (int)(span / repeat) + 1;
                 else estimated = (int)span + 1;
                 if (estimated < 1) estimated = 1;
            }
            else if (step.RepeatEveryDays.HasValue && step.RepeatEveryDays.Value > 0)
            {
                estimated = 1;
            }
            step.EstimatedOccurrences = estimated;

            // Oblicz pozostałe na podstawie rzeczywistych ukończeń w zakresie
            int completedCount = GetCompletionCount(step.HabitStepId, step.StartDate, step.EndDate);
            step.RemainingOccurrences = Math.Max(0, estimated - completedCount);

            const string query = @"UPDATE HabitSteps SET
                Name = @Name,
                StartDate = @StartDate,
                EndDate = @EndDate,
                RepeatEveryDays = @RepeatEveryDays,
                Description = @Description,

                UseDaytimeSplit = @UseDaytimeSplit,
                EstimatedOccurrences = @EstimatedOccurrences,
                RemainingOccurrences = @RemainingOccurrences,
                PointsReward = @PointsReward
                WHERE HabitStepId = @HabitStepId AND UserId = @UserId";

            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@HabitStepId", step.HabitStepId),
                new SqlParameter("@UserId", step.UserId),
                new SqlParameter("@Name", step.Name),
                new SqlParameter("@StartDate", (object?)step.StartDate ?? DBNull.Value),
                new SqlParameter("@EndDate", (object?)step.EndDate ?? DBNull.Value),
                new SqlParameter("@RepeatEveryDays", (object?)step.RepeatEveryDays ?? DBNull.Value),
                new SqlParameter("@Description", (object?)step.Description ?? DBNull.Value),
                new SqlParameter("@UseDaytimeSplit", step.UseDaytimeSplit),
                new SqlParameter("@EstimatedOccurrences", (object?)step.EstimatedOccurrences ?? DBNull.Value),
                new SqlParameter("@RemainingOccurrences", (object?)step.RemainingOccurrences ?? DBNull.Value),
                new SqlParameter("@PointsReward", (object?)step.PointsReward ?? DBNull.Value));
        }

        private int GetCompletionCount(int habitStepId, DateTime? start, DateTime? end)
        {
             // Policz ile 'Full' ukończeń istnieje w zakresie dat
             string query = @"SELECT COUNT(*) FROM HabitStepCompletions 
                              WHERE HabitStepId = @HabitStepId 
                              AND Part IS NULL"; // Part IS NULL oznacza pełne ukończenie
             
             if (start.HasValue)
                 query += " AND CompletionDate >= @Start";
             if (end.HasValue)
                 query += " AND CompletionDate <= @End";

             var parameters = new List<SqlParameter> { new SqlParameter("@HabitStepId", habitStepId) };
             if (start.HasValue) parameters.Add(new SqlParameter("@Start", start.Value.Date));
             if (end.HasValue) parameters.Add(new SqlParameter("@End", end.Value.Date));

             return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters.ToArray()));
        }

        public void DeleteHabitStep(int habitStepId, int userId)
        {
            const string query = @"DELETE FROM HabitSteps 
                                   WHERE HabitStepId = @HabitStepId AND UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@HabitStepId", habitStepId),
                new SqlParameter("@UserId", userId));
        }

        public List<HabitCompletion> GetHabitCompletions(DateTime date, int userId)
        {
            const string query = @"SELECT c.HabitStepId, c.CompletionDate, c.Part
                                   FROM HabitStepCompletions c
                                   INNER JOIN HabitSteps s ON c.HabitStepId = s.HabitStepId
                                   WHERE s.UserId = @UserId AND c.CompletionDate = @Date";

            var table = DatabaseHelper.ExecuteQuery(query,
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Date", date.Date));

            var list = new List<HabitCompletion>();
            foreach (DataRow row in table.Rows)
            {
                list.Add(new HabitCompletion
                {
                    HabitStepId = Convert.ToInt32(row["HabitStepId"]),
                    CompletionDate = Convert.ToDateTime(row["CompletionDate"]),
                    Part = row.IsNull("Part") ? null : row["Part"].ToString()
                });
            }
            return list;
        }

        public void MarkHabitCompletion(int habitStepId, int userId, DateTime date, string part, bool useDaytimeSplit)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            bool completedNow = false;
            bool fullExistedBefore;

            using (var cmd = new SqlCommand(@"SELECT COUNT(1) FROM HabitStepCompletions WHERE HabitStepId=@HabitStepId AND CompletionDate=@Date AND Part IS NULL", connection, tran))
            {
                cmd.Parameters.AddWithValue("@HabitStepId", habitStepId);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                fullExistedBefore = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }

            // Wstaw częściowe/pełne ukończenie jeśli nie istnieje
            string insertCompletion = @"IF NOT EXISTS (SELECT 1 FROM HabitStepCompletions WHERE HabitStepId=@HabitStepId AND CompletionDate=@Date AND ISNULL(Part,'') = ISNULL(@Part,''))
                                        BEGIN
                                            INSERT INTO HabitStepCompletions (HabitStepId, CompletionDate, Part)
                                            VALUES (@HabitStepId, @Date, @Part);
                                        END";
            using (var cmd = new SqlCommand(insertCompletion, connection, tran))
            {
                cmd.Parameters.AddWithValue("@HabitStepId", habitStepId);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                cmd.Parameters.AddWithValue("@Part", (object?)part ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }

            // Re-check full completion after the insert above
            bool fullExistsAfter;
            using (var cmd = new SqlCommand(@"SELECT COUNT(1) FROM HabitStepCompletions WHERE HabitStepId=@HabitStepId AND CompletionDate=@Date AND Part IS NULL", connection, tran))
            {
                cmd.Parameters.AddWithValue("@HabitStepId", habitStepId);
                cmd.Parameters.AddWithValue("@Date", date.Date);
                fullExistsAfter = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }

            // Jeśli użytkownik zaznaczył pełne ukończenie bezpośrednio (part == null), liczymy to jako ukończenie wykonania
            // nawet gdy UseDaytimeSplit = true.
            if (string.IsNullOrEmpty(part))
            {
                completedNow = !fullExistedBefore && fullExistsAfter;
            }
            else if (useDaytimeSplit)
            {
                // jeśli istnieją zarówno dzień jak i noc, dodaj pełne
                bool hasDay, hasNight;
                using (var cmd = new SqlCommand(@"SELECT 
                        SUM(CASE WHEN Part='Day' THEN 1 ELSE 0 END) as DayCnt,
                        SUM(CASE WHEN Part='Night' THEN 1 ELSE 0 END) as NightCnt
                        FROM HabitStepCompletions 
                        WHERE HabitStepId=@HabitStepId AND CompletionDate=@Date", connection, tran))
                {
                    cmd.Parameters.AddWithValue("@HabitStepId", habitStepId);
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    using var r = cmd.ExecuteReader();
                    r.Read();
                    hasDay = Convert.ToInt32(r["DayCnt"]) > 0;
                    hasNight = Convert.ToInt32(r["NightCnt"]) > 0;
                }

                if (hasDay && hasNight && !fullExistedBefore)
                {
                    using var cmdInsertFull = new SqlCommand(insertCompletion, connection, tran);
                    cmdInsertFull.Parameters.AddWithValue("@HabitStepId", habitStepId);
                    cmdInsertFull.Parameters.AddWithValue("@Date", date.Date);
                    cmdInsertFull.Parameters.AddWithValue("@Part", DBNull.Value);
                    cmdInsertFull.ExecuteNonQuery();
                    completedNow = true;
                }
            }

            if (completedNow)
            {
                // Jeśli RemainingOccurrences jest NULL dla starego rekordu, ale krok ma EndDate, oblicz teraz
                // aby zachowanie "auto ukończenie -> usuń krok" działało dla istniejących wierszy DB.
                using (var cmdEnsure = new SqlCommand(@"
                    SELECT StartDate, EndDate, RepeatEveryDays, EstimatedOccurrences, RemainingOccurrences
                    FROM HabitSteps
                    WHERE HabitStepId=@HabitStepId AND UserId=@UserId;", connection, tran))
                {
                    cmdEnsure.Parameters.AddWithValue("@HabitStepId", habitStepId);
                    cmdEnsure.Parameters.AddWithValue("@UserId", userId);
                    using var r = cmdEnsure.ExecuteReader();
                    if (r.Read())
                    {
                        var endDate = r.IsDBNull(1) ? (DateTime?)null : r.GetDateTime(1).Date;
                        var remainingDb = r.IsDBNull(4) ? (int?)null : r.GetInt32(4);
                        if (remainingDb == null)
                        {
                            var startDate = r.IsDBNull(0) ? date.Date : r.GetDateTime(0).Date;
                            var repeatEveryDays = r.IsDBNull(2) ? (int?)null : r.GetInt32(2);
                            // Jeśli nie ma EndDate, traktujemy to jako krok jednorazowy.
                            var estimated = endDate.HasValue
                                ? ComputeEstimatedOccurrences(startDate, endDate.Value, repeatEveryDays)
                                : 1;
                            r.Close();

                            using var cmdUpd = new SqlCommand(@"
                                UPDATE HabitSteps
                                SET EstimatedOccurrences = @Estimated,
                                    RemainingOccurrences = @Estimated
                                WHERE HabitStepId=@HabitStepId AND UserId=@UserId;", connection, tran);
                            cmdUpd.Parameters.AddWithValue("@Estimated", estimated);
                            cmdUpd.Parameters.AddWithValue("@HabitStepId", habitStepId);
                            cmdUpd.Parameters.AddWithValue("@UserId", userId);
                            cmdUpd.ExecuteNonQuery();
                        }
                    }
                }

                // decrement remaining
                using var cmdDec = new SqlCommand(@"
                    UPDATE HabitSteps
                    SET RemainingOccurrences = CASE 
                        WHEN RemainingOccurrences IS NULL THEN NULL
                        WHEN RemainingOccurrences > 0 THEN RemainingOccurrences - 1
                        ELSE RemainingOccurrences END
                    WHERE HabitStepId = @HabitStepId AND UserId=@UserId;
                    SELECT RemainingOccurrences, PointsReward FROM HabitSteps WHERE HabitStepId=@HabitStepId AND UserId=@UserId;", connection, tran);
                cmdDec.Parameters.AddWithValue("@HabitStepId", habitStepId);
                cmdDec.Parameters.AddWithValue("@UserId", userId);

                using var reader = cmdDec.ExecuteReader();
                int? remaining = null;
                int points = 0;
                if (reader.Read())
                {
                    remaining = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                    points = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                }
                reader.Close();

                if (remaining.HasValue && remaining.Value <= 0)
                {
                    // Pobierz nazwę kroku przed usunięciem do komunikatu
                    string stepName = "";
                    using (var cmdName = new SqlCommand(@"SELECT Name FROM HabitSteps WHERE HabitStepId=@HabitStepId AND UserId=@UserId", connection, tran))
                    {
                        cmdName.Parameters.AddWithValue("@HabitStepId", habitStepId);
                        cmdName.Parameters.AddWithValue("@UserId", userId);
                        stepName = cmdName.ExecuteScalar()?.ToString() ?? "habit";
                    }

                    // usuń krok; ukończenia kaskadują
                    using var cmdDel = new SqlCommand(@"DELETE FROM HabitSteps WHERE HabitStepId=@HabitStepId AND UserId=@UserId", connection, tran);
                    cmdDel.Parameters.AddWithValue("@HabitStepId", habitStepId);
                    cmdDel.Parameters.AddWithValue("@UserId", userId);
                    cmdDel.ExecuteNonQuery();

                    if (points > 0)
                    {
                        _pointsService.AddTotalPointsForUser(userId, points);
                    }

                    // Zatwierdź przed pokazaniem messagebox (żeby transakcja nie była otwarta)
                    tran.Commit();
                    
                    // Pokaż komunikat o ukończeniu
                    System.Windows.Forms.MessageBox.Show(
                        $"Congratulations! You completed all occurrences of \"{stepName}\"!" +
                        (points > 0 ? $"\n\nYou earned {points} points!" : ""),
                        "Habit Completed",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                    return; // Transakcja już zatwierdzona
                }
            }

            tran.Commit();
        }

        private static int ComputeEstimatedOccurrences(DateTime start, DateTime end, int? repeatEveryDays)
        {
            if (end.Date < start.Date)
                return 1;

            var repeat = repeatEveryDays.GetValueOrDefault(0);
            if (repeat > 0)
            {
                int count = 0;
                var current = start.Date;
                while (current <= end.Date)
                {
                    count++;
                    current = current.AddDays(repeat);
                }
                return Math.Max(1, count);
            }

            // Brak ustawionego powtarzania: traktuj jako "codziennie" (włącznie), spójne z EnsureOccurrences()
            var spanDays = (end.Date - start.Date).TotalDays;
            var estimated = (int)spanDays + 1;
            return Math.Max(1, estimated);
        }

        private static HabitCategory MapRowToHabitCategory(DataRow row)
        {
            return new HabitCategory
            {
                HabitCategoryId = Convert.ToInt32(row["HabitCategoryId"]),
                UserId = Convert.ToInt32(row["UserId"]),
                Name = row["Name"]?.ToString()
            };
        }

        private static HabitStep MapRowToHabitStep(DataRow row)
        {
            return new HabitStep
            {
                HabitStepId = Convert.ToInt32(row["HabitStepId"]),
                HabitCategoryId = Convert.ToInt32(row["HabitCategoryId"]),
                UserId = Convert.ToInt32(row["UserId"]),
                Name = row["Name"]?.ToString(),
                StartDate = row.IsNull("StartDate") ? (DateTime?)null : Convert.ToDateTime(row["StartDate"]),
                EndDate = row.IsNull("EndDate") ? (DateTime?)null : Convert.ToDateTime(row["EndDate"]),
                RepeatEveryDays = row.IsNull("RepeatEveryDays") ? (int?)null : Convert.ToInt32(row["RepeatEveryDays"]),
                Description = row.IsNull("Description") ? null : row["Description"].ToString(),
                UseDaytimeSplit = !row.IsNull("UseDaytimeSplit") && Convert.ToBoolean(row["UseDaytimeSplit"]),
                EstimatedOccurrences = row.IsNull("EstimatedOccurrences") ? (int?)null : Convert.ToInt32(row["EstimatedOccurrences"]),
                RemainingOccurrences = row.IsNull("RemainingOccurrences") ? (int?)null : Convert.ToInt32(row["RemainingOccurrences"]),
                PointsReward = row.IsNull("PointsReward") ? (int?)null : Convert.ToInt32(row["PointsReward"])
            };
        }

        private static void EnsureOccurrences(HabitStep step)
        {
            // Jeśli już ustawione, zachowaj aktualne wartości
            if (step.EstimatedOccurrences.HasValue && step.RemainingOccurrences.HasValue)
                return;

            int estimated = 1;

            if (step.StartDate.HasValue && step.EndDate.HasValue)
            {
                var span = (step.EndDate.Value - step.StartDate.Value).TotalDays;
                var repeat = step.RepeatEveryDays.GetValueOrDefault(0);
                if (repeat > 0)
                {
                    estimated = (int)(span / repeat) + 1;
                }
                else
                {
                    estimated = (int)span + 1;
                }
                if (estimated < 1) estimated = 1;
            }
            else if (step.RepeatEveryDays.HasValue && step.RepeatEveryDays.Value > 0)
            {
                estimated = 1;
            }

            step.EstimatedOccurrences = estimated;
            step.RemainingOccurrences = estimated;
        }

        public Plant? GetPlantById(int plantId)
        {
            string query = "SELECT * FROM Plants WHERE PlantID = @PlantID";
            var dataTable = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@PlantID", plantId));

            if (dataTable.Rows.Count > 0)
            {
                return MapRowToPlant(dataTable.Rows[0]);
            }

            return null;
        }

        public int AddPlant(Plant plant)
        {
            string query = @"INSERT INTO Plants (Name, Species, WateringFrequency, LastWateredDate, NextWateringDate, AddedDate)
                            VALUES (@Name, @Species, @WateringFrequency, @LastWateredDate, @NextWateringDate, @AddedDate);
                            SELECT SCOPE_IDENTITY();";

            var plantId = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Name", plant.Name),
                new SqlParameter("@Species", (object?)plant.Species ?? DBNull.Value),
                new SqlParameter("@WateringFrequency", plant.WateringFrequency),
                new SqlParameter("@LastWateredDate", (object?)plant.LastWateredDate ?? DBNull.Value),
                new SqlParameter("@NextWateringDate", (object?)plant.NextWateringDate ?? DBNull.Value),
                new SqlParameter("@AddedDate", plant.AddedDate));

            return Convert.ToInt32(plantId);
        }

        public void UpdatePlant(Plant plant)
        {
            string query = @"UPDATE Plants SET Name = @Name, Species = @Species, 
                                WateringFrequency = @WateringFrequency, LastWateredDate = @LastWateredDate, 
                                NextWateringDate = @NextWateringDate
                            WHERE PlantID = @PlantID";

            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@PlantID", plant.PlantID),
                new SqlParameter("@Name", plant.Name),
                new SqlParameter("@Species", (object?)plant.Species ?? DBNull.Value),
                new SqlParameter("@WateringFrequency", plant.WateringFrequency),
                new SqlParameter("@LastWateredDate", (object?)plant.LastWateredDate ?? DBNull.Value),
                new SqlParameter("@NextWateringDate", (object?)plant.NextWateringDate ?? DBNull.Value));
        }

        public int AddFridgeItem(FridgeItem item)
        {
            string query = @"INSERT INTO FridgeItems (Name, Quantity, Unit, ExpirationDate, AddedDate, LastModified, ContainerID, ProductID)
                            VALUES (@Name, @Quantity, @Unit, @ExpirationDate, @AddedDate, @LastModified, @ContainerID, @ProductID);
                            SELECT SCOPE_IDENTITY();";

            var itemId = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Name", item.Name),
                new SqlParameter("@Quantity", item.Quantity),
                new SqlParameter("@Unit", item.Unit),
                new SqlParameter("@ExpirationDate", (object?)item.ExpirationDate ?? DBNull.Value),
                new SqlParameter("@AddedDate", item.AddedDate),
                new SqlParameter("@LastModified", item.LastModified),
                new SqlParameter("@ContainerID", (object?)item.ContainerID ?? DBNull.Value),
                new SqlParameter("@ProductID", (object?)item.ProductID ?? DBNull.Value));

            return Convert.ToInt32(itemId);
        }

        public void UpdateFridgeItem(FridgeItem item)
        {
            string query = @"UPDATE FridgeItems 
                            SET Name = @Name, Quantity = @Quantity, Unit = @Unit, 
                                ExpirationDate = @ExpirationDate, LastModified = @LastModified, 
                                ContainerID = @ContainerID, ProductID = @ProductID
                            WHERE ItemID = @ItemID";

            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@ItemID", item.ItemID),
                new SqlParameter("@Name", item.Name),
                new SqlParameter("@Quantity", item.Quantity),
                new SqlParameter("@Unit", item.Unit),
                new SqlParameter("@ExpirationDate", (object?)item.ExpirationDate ?? DBNull.Value),
                new SqlParameter("@LastModified", item.LastModified),
                new SqlParameter("@ContainerID", (object?)item.ContainerID ?? DBNull.Value),
                new SqlParameter("@ProductID", (object?)item.ProductID ?? DBNull.Value));
        }

        public void MoveFridgeItem(int itemId, int newContainerId)
        {
            string query = @"UPDATE FridgeItems SET ContainerID = @NewContainerID, LastModified = @LastModified WHERE ItemID = @ItemID";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@NewContainerID", newContainerId),
                new SqlParameter("@LastModified", DateTime.Now),
                new SqlParameter("@ItemID", itemId));
        }

        #endregion

        #region Mapowanie wierszy (Helper methods)

        /// <summary>
        /// Mapuje wiersz bazy danych na obiekt FridgeItem.
        /// </summary>
        private FridgeItem MapRowToFridgeItem(DataRow row)
        {
            return new FridgeItem
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

        private FoodContainer MapRowToContainer(DataRow row)
        {
            return new FoodContainer
            {
                ContainerID = Convert.ToInt32(row["ContainerID"]),
                Name = row["Name"].ToString(),
                Type = row["Type"].ToString()
            };
        }

        private FoodProduct MapRowToProduct(DataRow row)
        {
            return new FoodProduct
            {
                ProductID = Convert.ToInt32(row["ProductID"]),
                Name = row["Name"].ToString(),
                DefaultUnit = "pcs"
            };
        }





        private Plant MapRowToPlant(DataRow row)
        {
            return new Plant
            {
                PlantID = Convert.ToInt32(row["PlantID"]),
                Name = row["Name"].ToString(),
                Species = row["Species"]?.ToString(),
           
                WateringFrequency = Convert.ToInt32(row["WateringFrequency"]),
                LastWateredDate = row["LastWateredDate"] != DBNull.Value ? Convert.ToDateTime(row["LastWateredDate"]) : (DateTime?)null,
                NextWateringDate = row["NextWateringDate"] != DBNull.Value ? Convert.ToDateTime(row["NextWateringDate"]) : (DateTime?)null,
       
                AddedDate = Convert.ToDateTime(row["AddedDate"])
            };
        }

        #endregion

        #region Zarządzanie przepisami (Recipes)

        /// <summary>
        /// Pobiera wszystkie przepisy.
        /// </summary>
        public List<Recipe> GetAllRecipes()

        {
            string query = "SELECT * FROM Recipes ORDER BY Name";
            var recipes = new List<Recipe>();
            var dataTable = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                var recipe = MapRowToRecipe(row);
                recipe.Ingredients = GetRecipeIngredients(recipe.RecipeID);
                recipes.Add(recipe);
            }

            return recipes;
        }

        public Recipe? GetRecipeById(int recipeId)
        {
            string query = "SELECT * FROM Recipes WHERE RecipeID = @RecipeID";
            var dataTable = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@RecipeID", recipeId));

            if (dataTable.Rows.Count == 0)
                return null;

            var recipe = MapRowToRecipe(dataTable.Rows[0]);
            recipe.Ingredients = GetRecipeIngredients(recipe.RecipeID);
            return recipe;
        }

        public int AddRecipe(Recipe recipe)
        {
            string query = @"INSERT INTO Recipes (Name, Description, PreparationTimeMinutes, NumberOfPortions, CreatedDate)
                            VALUES (@Name, @Description, @PreparationTimeMinutes, @NumberOfPortions, @CreatedDate);
                            SELECT SCOPE_IDENTITY();";

            var recipeIdObj = DatabaseHelper.ExecuteScalar(query,
                new SqlParameter("@Name", recipe.Name),
                new SqlParameter("@Description", (object)recipe.Description ?? DBNull.Value),
                new SqlParameter("@PreparationTimeMinutes", recipe.PreparationTimeMinutes),
                new SqlParameter("@NumberOfPortions", recipe.NumberOfPortions),
                new SqlParameter("@CreatedDate", recipe.CreatedDate));

            int recipeId = Convert.ToInt32(recipeIdObj);

            // Dodaj składniki
            foreach (var ingredient in recipe.Ingredients)
            {
                string ingredientQuery = @"INSERT INTO RecipeIngredients (RecipeID, ProductID, ProductName, Amount, Unit)
                                          VALUES (@RecipeID, @ProductID, @ProductName, @Amount, @Unit)";
                DatabaseHelper.ExecuteNonQuery(ingredientQuery,
                    new SqlParameter("@RecipeID", recipeId),
                    new SqlParameter("@ProductID", ingredient.ProductID),
                    new SqlParameter("@ProductName", ingredient.ProductName ?? ""),
                    new SqlParameter("@Amount", ingredient.Amount),
                    new SqlParameter("@Unit", ingredient.Unit));
            }

            return recipeId;
        }

        public void UpdateRecipe(Recipe recipe)
        {
            string query = @"UPDATE Recipes 
                           SET Name = @Name, Description = @Description, 
                               PreparationTimeMinutes = @PreparationTimeMinutes, 
                               NumberOfPortions = @NumberOfPortions
                           WHERE RecipeID = @RecipeID";

            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@RecipeID", recipe.RecipeID),
                new SqlParameter("@Name", recipe.Name),
                new SqlParameter("@Description", (object)recipe.Description ?? DBNull.Value),
                new SqlParameter("@PreparationTimeMinutes", recipe.PreparationTimeMinutes),
                new SqlParameter("@NumberOfPortions", recipe.NumberOfPortions));

            // Usuń stare składniki i dodaj nowe
            string deleteIngredientsQuery = "DELETE FROM RecipeIngredients WHERE RecipeID = @RecipeID";
            DatabaseHelper.ExecuteNonQuery(deleteIngredientsQuery, new SqlParameter("@RecipeID", recipe.RecipeID));

            // Dodaj zaktualizowane składniki
            foreach (var ingredient in recipe.Ingredients)
            {
                string ingredientQuery = @"INSERT INTO RecipeIngredients (RecipeID, ProductID, ProductName, Amount, Unit)
                                          VALUES (@RecipeID, @ProductID, @ProductName, @Amount, @Unit)";
                DatabaseHelper.ExecuteNonQuery(ingredientQuery,
                    new SqlParameter("@RecipeID", recipe.RecipeID),
                    new SqlParameter("@ProductID", ingredient.ProductID),
                    new SqlParameter("@ProductName", ingredient.ProductName ?? ""),
                    new SqlParameter("@Amount", ingredient.Amount),
                    new SqlParameter("@Unit", ingredient.Unit));
            }
        }

        public void DeleteRecipe(int recipeId)
        {
            // Składniki zostaną usunięte automatycznie przez CASCADE
            string query = "DELETE FROM Recipes WHERE RecipeID = @RecipeID";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@RecipeID", recipeId));
        }

        private List<RecipeIngredient> GetRecipeIngredients(int recipeId)
        {
            string query = @"SELECT ri.*, p.Name as ProductName 
                            FROM RecipeIngredients ri
                            INNER JOIN FoodProducts p ON ri.ProductID = p.ProductID
                            WHERE ri.RecipeID = @RecipeID";
            var dataTable = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@RecipeID", recipeId));

            var ingredients = new List<RecipeIngredient>();
            foreach (DataRow row in dataTable.Rows)
            {
                ingredients.Add(MapRowToRecipeIngredient(row));
            }

            return ingredients;
        }

        private Recipe MapRowToRecipe(DataRow row)
        {
            return new Recipe
            {
                RecipeID = Convert.ToInt32(row["RecipeID"]),
                Name = row["Name"].ToString(),
                Description = row.Table.Columns.Contains("Description") && row["Description"] != DBNull.Value
                    ? row["Description"].ToString() : null,
                PreparationTimeMinutes = Convert.ToInt32(row["PreparationTimeMinutes"]),
                NumberOfPortions = Convert.ToInt32(row["NumberOfPortions"]),
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                IsScheduled = row.Table.Columns.Contains("IsScheduled") && row["IsScheduled"] != DBNull.Value
                    ? Convert.ToBoolean(row["IsScheduled"]) : false
            };
        }
        
        public void SetRecipeScheduled(int recipeId, bool isScheduled)
        {
            // Ensure IsScheduled column exists
            EnsureRecipeScheduledColumn();
            
            string query = "UPDATE Recipes SET IsScheduled = @IsScheduled WHERE RecipeID = @RecipeID";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@IsScheduled", isScheduled),
                new SqlParameter("@RecipeID", recipeId));
        }
        
        private void EnsureRecipeScheduledColumn()
        {
            try
            {
                const string checkQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Recipes') AND name = 'IsScheduled')
                    BEGIN
                        ALTER TABLE Recipes ADD IsScheduled BIT NOT NULL DEFAULT 0
                    END";
                DatabaseHelper.ExecuteNonQuery(checkQuery);
            }
            catch
            {
                // Kolumna może już istnieć
            }
        }


        private RecipeIngredient MapRowToRecipeIngredient(DataRow row)
        {
            return new RecipeIngredient
            {
                RecipeIngredientID = Convert.ToInt32(row["RecipeIngredientID"]),
                RecipeID = Convert.ToInt32(row["RecipeID"]),
                ProductID = Convert.ToInt32(row["ProductID"]),
                ProductName = row.Table.Columns.Contains("ProductName") ? row["ProductName"].ToString() : null,
                Amount = Convert.ToDecimal(row["Amount"]),
                Unit = row["Unit"].ToString()
            };
        }

        #endregion

        #region Lista zakupów (Shopping List)


        public decimal GetAvailableIngredientAmount(string productName, string unit)
        {
            decimal totalAmount = 0;
            string normalizedUnit = unit.ToLower().Trim();
            
            // Pobierz kompatybilne jednostki i współczynniki konwersji
            var compatibleUnits = GetCompatibleUnits(normalizedUnit);

            foreach (var (compatibleUnit, conversionFactor) in compatibleUnits)
            {
                // 1) Sprawdź tabelę FridgeItems (przechowywanie Food Tracking)
                const string fridgeQuery = @"
                    SELECT Quantity, Unit
                    FROM FridgeItems 
                    WHERE LOWER(Name) = LOWER(@Name) 
                      AND LOWER(Unit) = LOWER(@Unit)
                      AND (ExpirationDate IS NULL OR ExpirationDate >= GETDATE())";

                var fridgeTable = DatabaseHelper.ExecuteQuery(fridgeQuery,
                    new SqlParameter("@Name", productName),
                    new SqlParameter("@Unit", compatibleUnit));

                foreach (DataRow row in fridgeTable.Rows)
                {
                    decimal quantity = Convert.ToDecimal(row["Quantity"]);
                    totalAmount += quantity * conversionFactor;
                }

                // 2) Sprawdź tabele ItemInventory + Items (kontenery Home Layout: lodówka, zamrażarka, spiżarnia)
                const string inventoryQuery = @"
                    SELECT ii.Quantity, ii.Unit
                    FROM ItemInventory ii
                    INNER JOIN Items i ON ii.ItemID = i.ItemID
                    WHERE LOWER(i.Name) = LOWER(@Name) 
                      AND LOWER(ii.Unit) = LOWER(@Unit)";

                var inventoryTable = DatabaseHelper.ExecuteQuery(inventoryQuery,
                    new SqlParameter("@Name", productName),
                    new SqlParameter("@Unit", compatibleUnit));

                foreach (DataRow row in inventoryTable.Rows)
                {
                    decimal quantity = Convert.ToDecimal(row["Quantity"]);
                    totalAmount += quantity * conversionFactor;
                }
            }

            return totalAmount;
        }

        /// <summary>
        /// Zwraca listę kompatybilnych jednostek ze współczynnikami konwersji do jednostki docelowej.
        /// Np. jeśli cel to "g", zwraca [("g", 1), ("kg", 1000)]
        /// </summary>
        private List<(string unit, decimal factor)> GetCompatibleUnits(string targetUnit)
        {
            var result = new List<(string, decimal)>();
            string normalized = targetUnit.ToLower().Trim();

            // Zawsze dołącz dokładną jednostkę ze współczynnikiem 1
            result.Add((normalized, 1m));

            // Konwersje wagi: g ↔ kg
            if (normalized == "g")
            {
                result.Add(("kg", 1000m)); // 1 kg = 1000 g
            }
            else if (normalized == "kg")
            {
                result.Add(("g", 0.001m)); // 1 g = 0.001 kg
            }

            // Konwersje objętości: ml ↔ l
            if (normalized == "ml")
            {
                result.Add(("l", 1000m)); // 1 l = 1000 ml
            }
            else if (normalized == "l")
            {
                result.Add(("ml", 0.001m)); // 1 ml = 0.001 l
            }

            return result;
        }

        public void AddToShoppingList(string productName, decimal amount, string unit, int? recipeId)
        {
            //EnsureShoppingListTable();

            // Sprawdź czy produkt już istnieje na liście zakupów (niezakupiony)
            const string checkQuery = @"
                SELECT ShoppingItemID, Amount 
                FROM ShoppingList 
                WHERE ProductName = @Name AND Unit = @Unit";

            var existingTable = DatabaseHelper.ExecuteQuery(checkQuery,
                new SqlParameter("@Name", productName),
                new SqlParameter("@Unit", unit));

            if (existingTable.Rows.Count > 0)
            {
                // Zaktualizuj istniejący wpis
                int existingId = Convert.ToInt32(existingTable.Rows[0]["ShoppingItemID"]);
                decimal existingAmount = Convert.ToDecimal(existingTable.Rows[0]["Amount"]);

                const string updateQuery = "UPDATE ShoppingList SET Amount = @Amount WHERE ShoppingItemID = @Id";
                DatabaseHelper.ExecuteNonQuery(updateQuery,
                    new SqlParameter("@Amount", existingAmount + amount),
                    new SqlParameter("@Id", existingId));
            }
            else
            {
                // Wstaw nowy wpis
                const string insertQuery = @"
                    INSERT INTO ShoppingList (ProductName, Amount, Unit, RecipeID)
                    VALUES (@Name, @Amount, @Unit, @RecipeID)";

                DatabaseHelper.ExecuteNonQuery(insertQuery,
                    new SqlParameter("@Name", productName),
                    new SqlParameter("@Amount", amount),
                    new SqlParameter("@Unit", unit),
                    new SqlParameter("@RecipeID", (object?)recipeId ?? DBNull.Value));
            }
        }

        public void RemoveFromShoppingListByRecipe(int recipeId)
        {
            //EnsureShoppingListTable();

            const string query = "DELETE FROM ShoppingList WHERE RecipeID = @RecipeID";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@RecipeID", recipeId));
        }

        public List<ShoppingListItem> GetShoppingListItems()
        {
          //  EnsureShoppingListTable();

            const string query = @"
                SELECT ShoppingItemID, ProductName, Amount, Unit, RecipeID, AddedDate
                FROM ShoppingList
                ORDER BY AddedDate DESC";

            var table = DatabaseHelper.ExecuteQuery(query);
            var items = new List<ShoppingListItem>();

            foreach (DataRow row in table.Rows)
            {
                items.Add(new ShoppingListItem
                {
                    ShoppingItemID = row["ShoppingItemID"] != DBNull.Value ? Convert.ToInt32(row["ShoppingItemID"]) : 0,
                    ProductName = row["ProductName"] != DBNull.Value ? row["ProductName"].ToString() : "",
                    Amount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0m,
                    Unit = row["Unit"] != DBNull.Value ? row["Unit"].ToString() : "",
                    RecipeID = row.Table.Columns.Contains("RecipeID") && row["RecipeID"] != DBNull.Value ? Convert.ToInt32(row["RecipeID"]) : (int?)null,
                    AddedDate = row.Table.Columns.Contains("AddedDate") && row["AddedDate"] != DBNull.Value ? Convert.ToDateTime(row["AddedDate"]) : DateTime.MinValue
                });
            }

            return items;
        }

        #endregion
    }
}





