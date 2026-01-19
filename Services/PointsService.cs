using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TimeManager.Database;
using TimeManager.Models;

namespace TimeManager.Services
{
    public class PointsService
    {
        public PointsService()
        {
            //EnsureRewardHistoryTable();
        }

        
        /// <summary>
        /// Dodaje punkty do kategorii za bieżący miesiąc.
        /// </summary>
        public void AddCategoryPoints(string category, int points)
        {
            if (string.IsNullOrWhiteSpace(category))
                category = "None";
            category = category.Trim().ToLowerInvariant();

            string monthYear = DateTime.Now.ToString("yyyy-MM");
            
            // Sprawdź czy istnieje rekord dla tej kategorii i miesiąca
            string checkQuery = @"SELECT CategoryPointsID, Points FROM CategoryPoints 
                                 WHERE Category = @Category AND MonthYear = @MonthYear AND UserId = @UserId";
            
            var checkParams = new[]
            {
                new SqlParameter("@Category", category),
                new SqlParameter("@MonthYear", monthYear),
                new SqlParameter("@UserId", UserSession.ContextUserId)
            };

            var existingTable = DatabaseHelper.ExecuteQuery(checkQuery, checkParams);
            dynamic existing = null;
            if (existingTable.Rows.Count > 0)
            {
                var row = existingTable.Rows[0];
                existing = new { ID = Convert.ToInt32(row["CategoryPointsID"]), Points = Convert.ToInt32(row["Points"]) };
            }

            if (existing != null)
            {
                // Aktualizuj istniejący rekord
                string updateQuery = @"UPDATE CategoryPoints 
                                      SET Points = Points + @Points, UpdatedDate = GETDATE()
                                      WHERE CategoryPointsID = @ID AND UserId = @UserId";
                DatabaseHelper.ExecuteNonQuery(updateQuery, new[]
                {
                    new SqlParameter("@Points", points),
                    new SqlParameter("@ID", existing.ID),
                    new SqlParameter("@UserId", UserSession.ContextUserId)
                });
            }
            else
            {
                // Wstaw nowy rekord
                string insertQuery = @"INSERT INTO CategoryPoints (Category, Points, MonthYear, UserId)
                                      VALUES (@Category, @Points, @MonthYear, @UserId)";
                DatabaseHelper.ExecuteNonQuery(insertQuery, new[]
                {
                    new SqlParameter("@Category", category),
                    new SqlParameter("@Points", points),
                    new SqlParameter("@MonthYear", monthYear),
                    new SqlParameter("@UserId", UserSession.ContextUserId)
                });
            }
            
            // CategoryPoints służy do statystyk. TotalPoints przyznajemy tylko w momencie
            // ukończenia akcji (event/monitoring), inaczej punkty naliczają się podwójnie.
        }

        /// <summary>
        /// Pobiera punkty dla wszystkich kategorii z ostatnich 30 dni.
        /// </summary>
        public Dictionary<string, int> GetCategoryPointsForCurrentMonth()
        {
            // Pobierz dane z ostatnich 30 dni na podstawie UpdatedDate
            string query = @"SELECT Category, SUM(Points) as TotalPoints FROM CategoryPoints 
                           WHERE UpdatedDate >= DATEADD(day, -30, GETDATE()) AND UserId = @UserId
                           GROUP BY Category";

            var result = new Dictionary<string, int>();
            
            // Zainicjuj wszystkie kategorie wartością 0
            string[] categories = { "health", "family", "mentality", "finance", 
                                   "work and career", "relax", "self development and education", 
                                   "friends and people" };
            foreach (var cat in categories)
            {
                result[cat] = 0;
            }

            var dataTable = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", UserSession.ContextUserId));
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                string category = row["Category"].ToString();
                int points = Convert.ToInt32(row["TotalPoints"]);
                result[category] = points;
            }

            return result;
        }

        /// <summary>
        /// Dodaje punkty do łącznej sumy punktów użytkownika.
        /// </summary>
        public void AddTotalPoints(int points)
        {
            EnsureUserPointsRow();
            string query = @"UPDATE UserPoints SET TotalPoints = TotalPoints + @Points, LastUpdated = GETDATE()
                             WHERE UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(query, new[]
            {
                new SqlParameter("@Points", points),
                new SqlParameter("@UserId", UserSession.ContextUserId)
            });
        }

        /// <summary>
        /// Dodaje punkty do łącznej sumy punktów dla określonego użytkownika (ignoruje ContextUserId).
        /// </summary>
        public void AddTotalPointsForUser(int userId, int points)
        {
            EnsureUserPointsRow(userId);
            const string query = @"UPDATE UserPoints SET TotalPoints = TotalPoints + @Points, LastUpdated = GETDATE()
                                   WHERE UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(query, new[]
            {
                new SqlParameter("@Points", points),
                new SqlParameter("@UserId", userId)
            });
        }

        /// <summary>
        /// Czyści punkty kategorii zebrane w ostatnich 30 dniach (nie wpływa na łączne punkty użytkownika).
        /// </summary>
        public void ClearCategoryPointsLast30Days()
        {
            const string query = @"DELETE FROM CategoryPoints WHERE UpdatedDate >= DATEADD(day, -30, GETDATE()) AND UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@UserId", UserSession.ContextUserId));
        }

        /// <summary>
        /// Pobiera łączną liczbę punktów dostępnych do wymiany.
        /// </summary>
        public int GetTotalPoints()
        {
            EnsureUserPointsRow();
            string query = "SELECT TotalPoints FROM UserPoints WHERE UserId = @UserId";
            var result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@UserId", UserSession.ContextUserId));
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Realizuje nagrodę atomowo. Zwraca false przy niewystarczających punktach lub już zrealizowanej nagrodzie.
        /// </summary>
        public bool RedeemReward(int rewardId)
        {
            const string query = @"
DECLARE @cost INT;
DECLARE @rewardName NVARCHAR(255);
SELECT @cost = PointsCost, @rewardName = Name FROM Rewards WHERE RewardID = @RewardID AND UserId = @UserId;
IF @cost IS NULL
BEGIN
    SELECT -2 AS ResultCode; -- not found or already redeemed
    RETURN;
END

DECLARE @current INT;
SELECT @current = TotalPoints FROM UserPoints WHERE UserId = @UserId;
IF @current < @cost
BEGIN
    SELECT -1 AS ResultCode; -- insufficient points
    RETURN;
END

BEGIN TRAN;

    UPDATE UserPoints
        SET TotalPoints = TotalPoints - @cost, LastUpdated = GETDATE()
        WHERE UserId = @UserId;

    INSERT INTO RewardHistory (UserId, RewardName, PointsSpent, RedeemedDate)
    VALUES (@UserId, @rewardName, @cost, GETDATE());

COMMIT TRAN;

SELECT 1 AS ResultCode; -- success
";

            var result = DatabaseHelper.ExecuteScalar(query, new[]
            {
                new SqlParameter("@RewardID", rewardId),
                new SqlParameter("@UserId", UserSession.ContextUserId)
            });
            if (result == null) return false;

            var code = Convert.ToInt32(result);
            return code == 1;
        }

        /// <summary>
        /// Dodaje nową nagrodę.
        /// </summary>
        public int AddReward(string name, string description, int pointsCost)
        {
            string query = @"INSERT INTO Rewards (Name, Description, PointsCost, UserId)
                            VALUES (@Name, @Description, @PointsCost, @UserId);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            var result = DatabaseHelper.ExecuteScalar(query, new[]
            {
                new SqlParameter("@Name", name),
                new SqlParameter("@Description", description ?? string.Empty),
                new SqlParameter("@PointsCost", pointsCost),
                new SqlParameter("@UserId", UserSession.ContextUserId)
            });

            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Pobiera wszystkie nagrody.
        /// </summary>
        public List<Reward> GetRewards()
        {
            string query = @"SELECT RewardID, Name, Description, PointsCost, CreatedDate 
                             FROM Rewards WHERE UserId = @UserId ORDER BY PointsCost ASC";
            var rewards = new List<Reward>();

            var dataTable = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", UserSession.ContextUserId));
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                rewards.Add(new Reward
                {
                    RewardID = Convert.ToInt32(row["RewardID"]),
                    Name = row["Name"].ToString(),
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    PointsCost = Convert.ToInt32(row["PointsCost"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                });
            }

            return rewards;
        }

        /// <summary>
        /// Aktualizuje istniejącą nagrodę.
        /// </summary>
        public void UpdateReward(int rewardId, string name, string description, int pointsCost)
        {
            string query = @"UPDATE Rewards 
                           SET Name = @Name, Description = @Description, PointsCost = @PointsCost
                           WHERE RewardID = @RewardID AND UserId = @UserId";
            
            DatabaseHelper.ExecuteNonQuery(query, new[]
            {
                new SqlParameter("@RewardID", rewardId),
                new SqlParameter("@Name", name),
                new SqlParameter("@Description", description ?? string.Empty),
                new SqlParameter("@PointsCost", pointsCost),
                new SqlParameter("@UserId", UserSession.ContextUserId)
            });
        }

        /// <summary>
        /// Usuwa nagrodę.
        /// </summary>
        public void DeleteReward(int rewardId)
        {
            string query = @"DELETE FROM Rewards WHERE RewardID = @RewardID AND UserId = @UserId";
            
            DatabaseHelper.ExecuteNonQuery(query, new[]
            {
                new SqlParameter("@RewardID", rewardId),
                new SqlParameter("@UserId", UserSession.ContextUserId)
            });
            
        }

        /// <summary>
        /// Pobiera historię wymienionych nagród dla użytkownika.
        /// </summary>
        public List<RedeemedReward> GetRedeemedRewards(int userId)
        {
            string query = @"SELECT HistoryID, RewardName, PointsSpent, RedeemedDate 
                             FROM RewardHistory 
                             WHERE UserId = @UserId 
                             ORDER BY RedeemedDate DESC";
            
            var result = new List<RedeemedReward>();
            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId));

            foreach (System.Data.DataRow row in table.Rows)
            {
                result.Add(new RedeemedReward
                {
                    HistoryID = Convert.ToInt32(row["HistoryID"]),
                    RewardName = row["RewardName"].ToString(),
                    PointsSpent = Convert.ToInt32(row["PointsSpent"]),
                    RedeemedDate = Convert.ToDateTime(row["RedeemedDate"])
                });
            }
            return result;
        }

        /// <summary>
        /// Czyści historię wymienionych nagród (część 'Clear Statistics').
        /// </summary>
        public void ClearRedeemedHistory()
        {
            const string query = "DELETE FROM RewardHistory WHERE UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@UserId", UserSession.ContextUserId));
        }

        private void EnsureUserPointsRow()
        {
            EnsureUserPointsRow(UserSession.ContextUserId);
        }

        private void EnsureUserPointsRow(int userId)
        {
            const string checkQuery = "SELECT 1 FROM UserPoints WHERE UserId = @UserId";
            var exists = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter("@UserId", userId));
            if (exists == null)
            {
                const string insert = "INSERT INTO UserPoints (UserId, TotalPoints, LastUpdated) VALUES (@UserId, 0, GETDATE())";
                DatabaseHelper.ExecuteNonQuery(insert, new SqlParameter("@UserId", userId));
            }
        }
    }
}

