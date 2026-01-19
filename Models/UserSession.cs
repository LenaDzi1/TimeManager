// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET

namespace TimeManager.Models
{
    /// <summary>
    /// Stałe ról użytkowników w systemie.
    /// </summary>
    public static class UserRoles
    {
        // Dziecko - ograniczony dostęp
        public const string Kid = "Kid";
        // Rodzic/użytkownik - pełny dostęp
        public const string User = "User";
        // Administrator - zarządzanie użytkownikami
        public const string Administrator = "Administrator";
    }

    /// <summary>
    /// Statyczna klasa przechowująca informacje o aktualnie zalogowanym użytkowniku.
    /// 
    /// Używana globalnie w całej aplikacji do sprawdzania uprawnień
    /// i kontekstu użytkownika (np. rodzic może przełączyć się na widok dziecka).
    /// </summary>
    public static class UserSession
    {
        // ID zalogowanego użytkownika
        public static int UserId { get; private set; }
        // Nazwa zalogowanego użytkownika
        public static string UserName { get; private set; }
        // Rola: "Kid", "User", "Administrator"
        public static string Role { get; private set; }
        // ID podglądanego użytkownika (dla rodzica przełączonego na dziecko)
        private static int? _viewedUserId;
        // Godzina rozpoczęcia snu
        public static TimeSpan? SleepStart { get; private set; }
        // Godzina zakończenia snu
        public static TimeSpan? SleepEnd { get; private set; }
        // Czy użytkownik jest zalogowany
        public static bool IsAuthenticated { get; private set; }

        // === Helpery sprawdzania roli ===
        public static bool IsKid => Role == UserRoles.Kid;
        public static bool IsUser => Role == UserRoles.User;
        public static bool IsAdministrator => Role == UserRoles.Administrator;
  

        /// <summary>
        /// ID aktualnego kontekstu użytkownika (własne lub wybranego dziecka).
        /// </summary>
        public static int ContextUserId => _viewedUserId ?? UserId;

        /// <summary>
        /// Ustawia podglądanego użytkownika (dla rodziców podglądających dzieci).
        /// </summary>
        public static void SetViewedUser(int? userId)
        {
            if (IsUser && userId.HasValue)
            {
                _viewedUserId = userId.Value;
            }
            else
            {
                _viewedUserId = null;
            }
        }

        public static void SetUser(int userId, string userName, string role, TimeSpan? sleepStart = null, TimeSpan? sleepEnd = null)
        {
            UserId = userId;
            UserName = userName;
            Role = role;
            _viewedUserId = null;
            SleepStart = sleepStart;
            SleepEnd = sleepEnd;
            IsAuthenticated = true;
        }

        public static void Logout()
        {
            UserId = 0;
            UserName = null;
            Role = null;
            _viewedUserId = null;
            SleepStart = null;
            SleepEnd = null;
            IsAuthenticated = false;
        }

        public static void UpdateSleep(TimeSpan? sleepStart, TimeSpan? sleepEnd)
        {
            SleepStart = sleepStart;
            SleepEnd = sleepEnd;
        }
    }
}



