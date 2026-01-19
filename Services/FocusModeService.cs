// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Data.SqlClient;    // Klient SQL Server
using TimeManager.Database;     // Helper do bazy danych
using TimeManager.Models;       // Modele aplikacji

namespace TimeManager.Services
{
    /// <summary>
    /// Serwis do zarządzania trybem skupienia (Focus Mode).
    /// 
    /// Focus Mode to funkcja Pomodoro-like:
    /// - Użytkownik uruchamia sesję skupienia na określony czas
    /// - Podczas sesji może rejestrować przerwania
    /// - Po zakończeniu sesja jest zapisywana do bazy
    /// 
    /// Sesje mogą być powiązane z eventem (opcjonalnie).
    /// </summary>
    public class FocusModeService
    {
        // Aktywna sesja skupienia (null jeśli brak)
        private static FocusSession _activeSession = null;

        /// <summary>
        /// Sprawdza czy użytkownik jest w trybie skupienia.
        /// </summary>
        /// <returns>True jeśli jest aktywna sesja skupienia</returns>
        public bool IsInFocusMode()
        {
            return _activeSession != null && _activeSession.IsActive();
        }

        /// <summary>
        /// Rozpoczyna nową sesję skupienia.
        /// </summary>
        /// <param name="eventId">Opcjonalne ID eventu powiązanego z sesją</param>
        /// <param name="duration">Planowany czas trwania w minutach</param>
        /// <exception cref="InvalidOperationException">Jeśli już jest aktywna sesja</exception>
        public void StartFocusSession(int? eventId, int duration)
        {
            // Nie można mieć dwóch sesji naraz
            if (IsInFocusMode())
            {
                throw new InvalidOperationException("A focus session is already active.");
            }

            // Wstaw nową sesję do bazy
            string query = @"INSERT INTO FocusSessions (EventID, StartTime, PlannedDuration) 
                           VALUES (@EventID, GETDATE(), @PlannedDuration);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new[]
            {
                new SqlParameter("@EventID", (object)eventId ?? DBNull.Value),
                new SqlParameter("@PlannedDuration", duration)
            };

            // Pobierz ID nowej sesji
            var sessionId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(query, parameters));

            // Utwórz obiekt sesji
            _activeSession = new FocusSession
            {
                SessionID = sessionId,
                EventID = eventId,
                StartTime = DateTime.Now,
                PlannedDuration = duration
            };
        }

        /// <summary>
        /// Kończy aktywną sesję skupienia.
        /// </summary>
        /// <param name="wasInterrupted">Czy sesja została przerwana przedwcześnie</param>
        public void EndFocusSession(bool wasInterrupted = false)
        {
            // Brak aktywnej sesji - nic do kończenia
            if (_activeSession == null)
            {
                return;
            }

            // Ustaw dane końcowe
            _activeSession.ActualEndTime = DateTime.Now;

            // Zaktualizuj sesję w bazie
            string query = @"UPDATE FocusSessions SET 
                           ActualEndTime = GETDATE()
                           WHERE SessionID = @SessionID";

            var parameters = new[]
            {
                new SqlParameter("@SessionID", _activeSession.SessionID)
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);

            // Przyznaj punkty jeśli sesja nie była przerwana
            if (!wasInterrupted)
            {
                // Logika przyznawania punktów (do implementacji)
            }

            // Wyczyść aktywną sesję
            _activeSession = null;
        }
    }
}

