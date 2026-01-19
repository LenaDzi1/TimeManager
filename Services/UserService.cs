// ============================================================================
// UserService.cs
// Serwis zarządzania użytkownikami: uwierzytelnianie, role, harmonogram snu.
// ============================================================================

#nullable enable

#region Importy
using System;                       // Podstawowe typy (TimeSpan)
using System.Collections.Generic;   // Kolekcje (List)
using System.Data;                  // Typy bazodanowe (DataRow)
using System.Data.SqlClient;        // Klient SQL Server
using System.Security.Cryptography; // SHA-256 hashowanie
using System.Text;                  // StringBuilder
using TimeManager.Database;         // DatabaseHelper
using TimeManager.Models;           // User, UserRoles
#endregion

namespace TimeManager.Services
{
    /// <summary>
    /// Serwis do zarządzania użytkownikami.
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Uwierzytelnianie (login, walidacja hasła SHA-256)
    /// - Zarządzanie kontami (CRUD)
    /// - Role użytkowników (Admin, User, Kid)
    /// - Hierarchia rodzic-dziecko (ParentId)
    /// - Harmonogram snu (SleepStart, SleepEnd)
    /// 
    /// BEZPIECZEŃSTWO:
    /// - Hasła przechowywane jako SHA-256 hash
    /// - Porównanie case-insensitive dla kompatybilności
    /// </summary>
    public class UserService
    {
        #region Migracje bazy danych

        /// <summary>
        /// Naprawia constraint w bazie danych dla kolumny Role.
        /// Usuwa istniejący CHECK constraint żeby akceptować nasze wartości ról.
        /// </summary>
        public void FixRoleConstraint()
        {
            try
            {
                const string dropConstraint = @"
                    DECLARE @constraintName NVARCHAR(200);
                    SELECT @constraintName = name 
                    FROM sys.check_constraints 
                    WHERE parent_object_id = OBJECT_ID('Users') 
                      AND COL_NAME(parent_object_id, parent_column_id) = 'Role';
                    IF @constraintName IS NOT NULL
                        EXEC('ALTER TABLE Users DROP CONSTRAINT ' + @constraintName);";

                DatabaseHelper.ExecuteNonQuery(dropConstraint);
            }
            catch
            {
                // Ignoruj błędy - constraint może nie istnieć
            }
        }

        #endregion

        #region Pobieranie użytkowników

        /// <summary>
        /// Pobiera wszystkich użytkowników (dla panelu admina).
        /// </summary>
        /// <returns>Lista użytkowników posortowana po nazwie</returns>
        public List<User> GetAllUsers()
        {
            const string query = "SELECT UserId, UserName, Role, PasswordHash, SleepStart, SleepEnd, ParentId FROM Users ORDER BY UserName";
            var users = new List<User>();
            var dataTable = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dataTable.Rows)
            {
                users.Add(new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    UserName = row["UserName"].ToString(),
                    Role = row["Role"]?.ToString() ?? "Kid",
                    PasswordHash = row["PasswordHash"]?.ToString() ?? "",
                    SleepStart = row["SleepStart"] != DBNull.Value ? (TimeSpan?)row["SleepStart"] : null,
                    SleepEnd = row["SleepEnd"] != DBNull.Value ? (TimeSpan?)row["SleepEnd"] : null,
                    ParentId = row["ParentId"] != DBNull.Value ? (int?)Convert.ToInt32(row["ParentId"]) : null
                });
            }
            return users;
        }

        /// <summary>
        /// Pobiera użytkownika po nazwie.
        /// </summary>
        /// <param name="userName">Nazwa użytkownika</param>
        /// <returns>User lub null jeśli nie znaleziono</returns>
        public User? GetUser(string userName)
        {
            const string query = @"SELECT TOP 1 UserId, UserName, Role, PasswordHash, SleepStart, SleepEnd, ParentId 
                                   FROM Users WHERE UserName = @UserName";
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserName", userName);
            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            // Obsłuż brakującą rolę (stare dane)
            int? parentId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6);
            var role = reader.IsDBNull(2) ? null : reader.GetString(2);
            // Jeśli Role brakuje - dedukuj: Ma ParentId => Kid, brak => User
            role ??= parentId.HasValue ? UserRoles.Kid : UserRoles.User;

            return new User
            {
                UserId = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Role = role,
                PasswordHash = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                SleepStart = reader.IsDBNull(4) ? null : reader.GetTimeSpan(4),
                SleepEnd = reader.IsDBNull(5) ? null : reader.GetTimeSpan(5),
                ParentId = parentId
            };
        }

        /// <summary>
        /// Pobiera użytkownika po ID.
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <returns>User lub null jeśli nie znaleziono</returns>
        public User? GetUserById(int userId)
        {
            const string query = @"SELECT TOP 1 UserId, UserName, Role, PasswordHash, SleepStart, SleepEnd, ParentId 
                                   FROM Users WHERE UserId = @UserId";
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            connection.Open();
            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            int? parentId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6);
            var role = reader.IsDBNull(2) ? null : reader.GetString(2);
            role ??= parentId.HasValue ? UserRoles.Kid : UserRoles.User;

            return new User
            {
                UserId = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Role = role,
                PasswordHash = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                SleepStart = reader.IsDBNull(4) ? null : reader.GetTimeSpan(4),
                SleepEnd = reader.IsDBNull(5) ? null : reader.GetTimeSpan(5),
                ParentId = parentId
            };
        }

        /// <summary>
        /// Sprawdza czy użytkownik o danej nazwie istnieje.
        /// </summary>
        /// <param name="userName">Nazwa użytkownika</param>
        /// <returns>True jeśli użytkownik istnieje</returns>
        public bool UserExists(string userName)
        {
            const string query = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName";
            var count = (int)DatabaseHelper.ExecuteScalar(query, new SqlParameter("@UserName", userName));
            return count > 0;
        }

        /// <summary>
        /// Pobiera listę dzieci danego rodzica.
        /// </summary>
        /// <param name="parentId">ID rodzica</param>
        /// <returns>Lista kont dzieci</returns>
        public List<User> GetChildren(int parentId)
        {
            const string query = @"SELECT UserId, UserName, Role, PasswordHash, SleepStart, SleepEnd, ParentId 
                                   FROM Users WHERE ParentId = @ParentId ORDER BY UserName";
            var users = new List<User>();
            var table = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ParentId", parentId));

            foreach (DataRow row in table.Rows)
            {
                users.Add(new User
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    UserName = row["UserName"].ToString(),
                    Role = row["Role"]?.ToString() ?? "Kid",
                    PasswordHash = row["PasswordHash"]?.ToString() ?? "",
                    SleepStart = row["SleepStart"] != DBNull.Value ? (TimeSpan?)row["SleepStart"] : null,
                    SleepEnd = row["SleepEnd"] != DBNull.Value ? (TimeSpan?)row["SleepEnd"] : null,
                    ParentId = row["ParentId"] != DBNull.Value ? (int?)Convert.ToInt32(row["ParentId"]) : null
                });
            }
            return users;
        }

        #endregion

        #region Uwierzytelnianie

        /// <summary>
        /// Waliduje dane logowania użytkownika.
        /// Porównuje hash podanego hasła z hashem w bazie.
        /// </summary>
        /// <param name="userName">Nazwa użytkownika</param>
        /// <param name="password">Hasło w czystym tekście</param>
        /// <returns>True jeśli dane są poprawne</returns>
        public bool ValidateUser(string userName, string password)
        {
            const string query = "SELECT PasswordHash FROM Users WHERE UserName = @UserName";
            var result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@UserName", userName)) as string;

            if (result == null)
                return false;

            // Porównaj hashe (case-insensitive dla kompatybilności)
            return string.Equals(result, ComputeHash(password), StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Zarządzanie kontami

        /// <summary>
        /// Tworzy nowego użytkownika.
        /// </summary>
        /// <param name="userName">Nazwa użytkownika</param>
        /// <param name="password">Hasło w czystym tekście (zostanie zahashowane)</param>
        /// <param name="role">Rola użytkownika (Admin, User, Kid)</param>
        /// <param name="defaultSleepStart">Opcjonalna godzina zaśnięcia</param>
        /// <param name="defaultSleepEnd">Opcjonalna godzina pobudki</param>
        /// <param name="parentId">Opcjonalne ID rodzica (dla kont dzieci)</param>
        public void CreateUser(string userName, string password, string role, 
            TimeSpan? defaultSleepStart = null, TimeSpan? defaultSleepEnd = null, int? parentId = null)
        {
            const string query = @"INSERT INTO Users (UserName, PasswordHash, Role, SleepStart, SleepEnd, ParentId)
                                   VALUES (@UserName, @PasswordHash, @Role, @SleepStart, @SleepEnd, @ParentId)";

            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@UserName", userName),
                new SqlParameter("@PasswordHash", ComputeHash(password)),
                new SqlParameter("@Role", role),
                new SqlParameter("@SleepStart", (object?)defaultSleepStart ?? DBNull.Value),
                new SqlParameter("@SleepEnd", (object?)defaultSleepEnd ?? DBNull.Value),
                new SqlParameter("@ParentId", (object?)parentId ?? DBNull.Value));
        }

        /// <summary>
        /// Aktualizuje rolę użytkownika (tylko admin).
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <param name="newRole">Nowa rola (Admin, User, Kid)</param>
        public void UpdateUserRole(int userId, string newRole)
        {
            const string query = "UPDATE Users SET Role = @Role WHERE UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@Role", newRole),
                new SqlParameter("@UserId", userId));
        }

        /// <summary>
        /// Usuwa użytkownika (tylko admin).
        /// </summary>
        /// <param name="userId">ID użytkownika do usunięcia</param>
        public void DeleteUser(int userId)
        {
            const string query = "DELETE FROM Users WHERE UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@UserId", userId));
        }

        /// <summary>
        /// Aktualizuje hasło użytkownika po nazwie.
        /// </summary>
        /// <param name="userName">Nazwa użytkownika</param>
        /// <param name="newPassword">Nowe hasło w czystym tekście</param>
        public void UpdatePassword(string userName, string newPassword)
        {
            const string query = @"UPDATE Users SET PasswordHash = @PasswordHash WHERE UserName = @UserName";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@PasswordHash", ComputeHash(newPassword)),
                new SqlParameter("@UserName", userName));
        }

        /// <summary>
        /// Aktualizuje hasło użytkownika po ID (tylko admin).
        /// </summary>
        /// <param name="userId">ID użytkownika</param>
        /// <param name="newPassword">Nowe hasło w czystym tekście</param>
        public void UpdateUserPassword(int userId, string newPassword)
        {
            const string query = @"UPDATE Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@PasswordHash", ComputeHash(newPassword)),
                new SqlParameter("@UserId", userId));
        }

        /// <summary>
        /// Aktualizuje harmonogram snu użytkownika.
        /// </summary>
        /// <param name="userName">Nazwa użytkownika</param>
        /// <param name="sleepStart">Godzina zaśnięcia</param>
        /// <param name="sleepEnd">Godzina pobudki</param>
        public void UpdateSleepSchedule(string userName, TimeSpan sleepStart, TimeSpan sleepEnd)
        {
            const string query = @"UPDATE Users SET SleepStart = @SleepStart, SleepEnd = @SleepEnd WHERE UserName = @UserName";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@SleepStart", sleepStart),
                new SqlParameter("@SleepEnd", sleepEnd),
                new SqlParameter("@UserName", userName));
        }

        /// <summary>
        /// Przypisuje rodzica do konta dziecka.
        /// </summary>
        /// <param name="childId">ID konta dziecka</param>
        /// <param name="parentId">ID rodzica (lub null żeby usunąć powiązanie)</param>
        public void AssignParent(int childId, int? parentId)
        {
            const string query = "UPDATE Users SET ParentId = @ParentId WHERE UserId = @ChildId";
            DatabaseHelper.ExecuteNonQuery(query,
                new SqlParameter("@ParentId", (object?)parentId ?? DBNull.Value),
                new SqlParameter("@ChildId", childId));
        }

        #endregion

        #region Konto admina

        /// <summary>
        /// Zapewnia że konto admina istnieje.
        /// 
        /// Jeśli nie istnieje - tworzy je z domyślnym hasłem.
        /// Jeśli istnieje - NIE resetuje hasła, tylko upewnia się że rola jest poprawna.
        /// Dzięki temu admin może zmienić swoje hasło i nie zostanie ono zresetowane.
        /// </summary>
        /// <param name="userName">Nazwa admina</param>
        /// <param name="password">Domyślne hasło (używane tylko przy tworzeniu)</param>
        /// <param name="role">Rola (powinna być "Admin")</param>
        public void EnsureAdminAccount(string userName, string password, string role)
        {
            if (UserExists(userName))
            {
                // Użytkownik istnieje - aktualizuj rolę jeśli trzeba, NIE resetuj hasła
                const string query = @"UPDATE Users SET Role = @Role WHERE UserName = @UserName AND Role != @Role";
                DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@Role", role),
                    new SqlParameter("@UserName", userName));
            }
            else
            {
                // Utwórz nowego admina z domyślnym hasłem
                CreateUser(userName, password, role, TimeSpan.FromHours(23), TimeSpan.FromHours(7));
            }
        }

        #endregion

        #region Hashowanie haseł

        /// <summary>
        /// Oblicza SHA-256 hash hasła.
        /// </summary>
        /// <param name="password">Hasło w czystym tekście</param>
        /// <returns>Hash jako string hex (lowercase)</returns>
        private static string ComputeHash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        #endregion
    }
}
