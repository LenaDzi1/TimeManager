// ============================================================================
// EventService.cs
// Serwis zarządzania wydarzeniami: CRUD, planowanie, przeplanowanie.
// ============================================================================

#nullable enable


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TimeManager.Database;
using TimeManager.Models;

namespace TimeManager.Services
{
    /// <summary>
    /// Serwis do zarządzania wydarzeniami (CRUD + planowanie).
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Pobieranie wydarzeń z bazy danych (GetEvents, GetEventById, GetEventsWithSupply)
    /// - Dodawanie nowych wydarzeń (AddOrScheduleEvent)
    /// - Aktualizacja wydarzeń (UpdateEvent, UpdateEventBasic, UpdateEventWithReschedule)
    /// - Usuwanie wydarzeń (DeleteEvent)
    /// - Integracja z algorytmem planowania (EventSchedulingAlgorithm)
    /// - Automatyczne przeplanowanie nieukończonych eventów (RescheduleIncompleteEvents)
    /// 
    /// UWAGA: Wszystkie operacje uwzględniają ContextUserId - każdy użytkownik widzi tylko swoje eventy.
    /// </summary>
    public class EventService
    {
        #region Pobieranie wydarzeń

        /// <summary>
        /// Pobiera listę wydarzeń w podanym zakresie dat.
        /// 
        /// WAŻNE: Pobiera eventy które NAKŁADAJĄ SIĘ z zakresem, nie tylko te które ZACZYNAJĄ się w zakresie.
        /// To łapie eventy które zaczęły się przed startDate ale kończą po nim.
        /// 
        /// Automatycznie przelicza priorytety dla eventów bez ustawionego priorytetu.
        /// </summary>
        /// <param name="startDate">Początek zakresu</param>
        /// <param name="endDate">Koniec zakresu</param>
        /// <returns>Lista wydarzeń posortowana po StartDateTime</returns>
        public List<Event> GetEvents(DateTime startDate, DateTime endDate)
        {
            // Zapytanie SQL - filtrowanie po użytkowniku kontekstowym
            // Events z NULL UserId to stare dane - widoczne dla wszystkich
            string query = @"SELECT * FROM Events 
                           WHERE StartDateTime IS NOT NULL 
                             AND EndDateTime IS NOT NULL
                             AND StartDateTime < @EndDate 
                             AND EndDateTime > @StartDate
                             AND (UserId = @UserId OR UserId IS NULL)
                           ORDER BY StartDateTime";

            // Parametry zapytania
            var parameters = new[]
            {
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate),
                new SqlParameter("@UserId", UserSession.ContextUserId)
            };

            // Wykonaj zapytanie i mapuj wyniki
            var events = new List<Event>();
            var dataTable = DatabaseHelper.ExecuteQuery(query, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                // Mapuj wiersz na obiekt Event
                var ev = MapRowToEvent(row);

                // Automatyczne odświeżanie priorytetu względem deadline
                int oldPriority = ev.Priority;

                // Przeliczaj tylko jeśli priorytet nie był ustawiony (0)
                if (ev.Priority <= 0)
                {
                    ev.CalculatePriority();

                    // Zaktualizuj w bazie jeśli priorytet się zmienił
                    if (ev.EventID != 0 && ev.Priority != oldPriority)
                    {
                        UpdatePriorityOnly(ev.EventID, ev.Priority);
                    }
                }
                else
                {
                    // Zostaw ręcznie ustawiony priorytet
                    ev.Priority = oldPriority;
                }

                events.Add(ev);
            }


            return events;
        }

        /// <summary>
        /// Pobiera eventy z bazy I uruchamia scheduler żeby naprawić nakładające się eventy.
        /// Zapisuje poprawione czasy do bazy danych.
        /// </summary>
        public List<Event> GetEventsWithScheduling(DateTime startDate, DateTime endDate)
        {
            var events = GetEvents(startDate, endDate);
            
            if (events.Count == 0)
                return events;
            
            // Sprawdź czy są nakładające się eventy
            bool hasOverlaps = false;
            for (int i = 0; i < events.Count && !hasOverlaps; i++)
            {
                for (int j = i + 1; j < events.Count && !hasOverlaps; j++)
                {
                    var a = events[i];
                    var b = events[j];
                    
                    if (a.StartDateTime.HasValue && a.EndDateTime.HasValue &&
                        b.StartDateTime.HasValue && b.EndDateTime.HasValue)
                    {
                        if (a.StartDateTime.Value < b.EndDateTime.Value && 
                            a.EndDateTime.Value > b.StartDateTime.Value)
                        {
                            hasOverlaps = true;
                        }
                    }
                }
            }
            
            if (!hasOverlaps)
                return events;
            
            // Uruchom scheduler żeby naprawić nakładania
            var scheduler = new EventSchedulingAlgorithm();
            var scheduleFrom = startDate.AddDays(-1);
            var scheduleTo = endDate.AddDays(7);
            var scheduled = scheduler.ScheduleEvents(events, scheduleFrom, scheduleTo);
            
            // Zapisz poprawione czasy do bazy
            using var conn = DatabaseHelper.OpenConnection();
            using var tx = conn.BeginTransaction();
            
            try
            {
                foreach (var e in scheduled.Where(x => x.EventID != 0))
                {
                    var original = events.FirstOrDefault(o => o.EventID == e.EventID);
                    if (original == null) continue;
                    
                    bool changed = original.StartDateTime != e.StartDateTime || original.EndDateTime != e.EndDateTime;
                    
                    if (changed && e.StartDateTime.HasValue && e.EndDateTime.HasValue)
                    {
                        const string sql = @"UPDATE Events SET StartDateTime = @Start, EndDateTime = @End, ModifiedDate = GETDATE() WHERE EventID = @EventID;";
                        using var cmd = new SqlCommand(sql, conn, tx);
                        cmd.Parameters.AddWithValue("@Start", e.StartDateTime.Value);
                        cmd.Parameters.AddWithValue("@End", e.EndDateTime.Value);
                        cmd.Parameters.AddWithValue("@EventID", e.EventID);
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                return events;
            }
            
            return scheduled
                .Where(e => e.StartDateTime.HasValue && e.EndDateTime.HasValue)
                .Where(e => e.StartDateTime!.Value < endDate && e.EndDateTime!.Value > startDate)
                .OrderBy(e => e.StartDateTime)
                .ToList();
        }

        /// <summary>
        /// Pobiera pojedynczy event po ID.
        /// </summary>
        /// <param name="eventId">ID eventu do pobrania</param>
        /// <returns>Event lub null jeśli nie znaleziono</returns>
        public Event? GetEventById(int eventId)
        {
            string query = "SELECT * FROM Events WHERE EventID = @EventID";
            var dataTable = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@EventID", eventId));

            if (dataTable.Rows.Count > 0)
            {
                return MapRowToEvent(dataTable.Rows[0]);
            }

            return null;
        }

        #endregion

        #region Walidacja i sprawdzanie konfliktów

        #endregion

        #region Dodawanie i planowanie wydarzeń

        /// <summary>
        /// Dodaje nowy event i planuje go używając algorytmu.
        /// 
        /// RÓŻNICA OD AddEvent:
        /// - AddEvent: tylko dodaje do bazy bez planowania [ZAKOMENTOWANE]
        /// - AddOrScheduleEvent: dodaje I uruchamia algorytm planowania
        /// 
        /// ALGORYTM:
        /// 1. Waliduje event (czas, duration)
        /// 2. Przelicza priorytet i ustawia domyślne wartości
        /// 3. Pobiera istniejące eventy z bazy
        /// 4. Uruchamia algorytm planowania na całej liście (z nowym eventem)
        /// 5. TRANSAKCYJNIE zapisuje wszystkie zmiany czasu do bazy
        /// 6. Przeplanowuje kolidujące elastyczne jeśli dodano stały
        /// </summary>
        /// <param name="newEvent">Nowy event do dodania i zaplanowania</param>
        /// <param name="scheduleFrom">Początek okna planowania</param>
        /// <param name="scheduleTo">Koniec okna planowania</param>
        /// <returns>ID nowo dodanego eventu</returns>
        public int AddOrScheduleEvent(Event newEvent, DateTime scheduleFrom, DateTime scheduleTo)
        {
            // Walidacja: koniec musi być po początku
            if (newEvent.StartDateTime.HasValue && newEvent.EndDateTime.HasValue &&
                newEvent.EndDateTime.Value <= newEvent.StartDateTime.Value)
                throw new InvalidOperationException("End time must be later than start time.");

            // Oblicz Duration jeśli nie ustawiony
            if (newEvent.EndDateTime.HasValue && newEvent.StartDateTime.HasValue && newEvent.Duration == 0)
                newEvent.Duration = (int)(newEvent.EndDateTime.Value - newEvent.StartDateTime.Value).TotalMinutes;

            // Domyślny Duration jeśli nadal 0
            if (newEvent.Duration == 0)
                newEvent.Duration = 120;

            // Przelicz priorytet i ustaw domyślne wartości
            newEvent.CalculatePriority();
            newEvent.Category = newEvent.CategoryCode.ToString();

            // Inicjalizacja algorytmu planowania
            var scheduler = new EventSchedulingAlgorithm();

            using var conn = DatabaseHelper.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                // 1) Pobierz eventy z DB w zakresie (UWAGA: overlap z zakresem)
                // Rozszerzamy zakres o 7 dni w obie strony żeby złapać wywłaszczone eventy
                // które mogły zostać przesunięte poza oryginalny zakres planowania
                var extendedStart = scheduleFrom.AddDays(-7);
                var extendedEnd = scheduleTo.AddDays(7);
                var existingEvents = GetEventsForUserInRangeTx(conn, tx, UserSession.ContextUserId, extendedStart, extendedEnd);

                var existingById = existingEvents
                    .Where(e => e.EventID != 0)
                    .ToDictionary(e => e.EventID, e => new
                    {
                        e.StartDateTime,
                        e.EndDateTime,
                        e.Priority,
                        e.Duration
                    });

                // Wywłaszczanie elastycznych przez stałe: wszystkie flexible nachodzące na fixed idą do "unscheduled"
                if (newEvent.IsSetEvent &&
                    newEvent.StartDateTime.HasValue && newEvent.EndDateTime.HasValue)
                {
                    var fixedStart = newEvent.StartDateTime.Value;
                    var fixedEnd = newEvent.EndDateTime.Value;

                    foreach (var e in existingEvents)
                    {
                        // Tylko elastyczne (nie set/promise), które mają czas i nachodzą
                        if (e.StartDateTime.HasValue && e.EndDateTime.HasValue &&
                            !e.IsSetEvent &&
                            fixedStart < e.EndDateTime.Value &&
                            fixedEnd > e.StartDateTime.Value)
                        {
                            e.StartDateTime = null;
                            e.EndDateTime = null;
                        }
                    }
                }
                
                // Elastyczny nie może nachodzić na stały - wyczyść czas żeby scheduler znalazł wolny slot
                if (!newEvent.IsSetEvent &&
                    newEvent.StartDateTime.HasValue && newEvent.EndDateTime.HasValue)
                {
                    var flexStart = newEvent.StartDateTime.Value;
                    var flexEnd = newEvent.EndDateTime.Value;
                    
                    // Sprawdź czy nachodzi na jakikolwiek SetEvent
                    bool overlapsWithFixed = existingEvents.Any(e => 
                        e.IsSetEvent &&
                        e.StartDateTime.HasValue && e.EndDateTime.HasValue &&
                        flexStart < e.EndDateTime.Value &&
                        flexEnd > e.StartDateTime.Value);
                    
                    if (overlapsWithFixed)
                    {
                        // Wyczyść czas - scheduler znajdzie wolny slot
                        newEvent.StartDateTime = null;
                        newEvent.EndDateTime = null;
                    }
                }
                
                var originalRef = newEvent;
                var allEvents = new List<Event>(existingEvents) { newEvent };

                // 3) Odpal scheduler (P5 zrobi displacement w algorytmie)
                var scheduled = scheduler.ScheduleEvents(allEvents, scheduleFrom, scheduleTo);

                Event? scheduledNew =
                    scheduled.FirstOrDefault(e => ReferenceEquals(e, originalRef))
                    ?? scheduled.FirstOrDefault(e =>
                        e.EventID == 0 &&
                        string.Equals(e.Title, newEvent.Title, StringComparison.Ordinal) &&
                        e.Duration == newEvent.Duration &&
                        e.CategoryCode == newEvent.CategoryCode &&
                        e.Priority == newEvent.Priority);

                // Jeśli scheduler "zgubił" instancję nowego eventu - traktuj jak brak możliwości zapisu
                if (scheduledNew == null)
                {
                    // Dla P5: komunikat
                    if (newEvent.Priority == 5)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            "Cannot save event with priority 5.\n\n" +
                            "The scheduler could not properly place the event in your schedule.\n" +
                            "Change the parameters (duration, deadline, priority) or free up space in the calendar.",
                            "Cannot save event",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Warning
                        );

                        throw new InvalidOperationException("P5 rejected: scheduler did not resolve new event instance.");
                    }

                    // Dla innych priorytetów - wyjątek techniczny
                    throw new InvalidOperationException("Scheduler did not return the new event instance.");
                }

                // Jeśli scheduler nie dał czasu (a to nie P1)
                if (scheduledNew.Priority >= 2 && !scheduledNew.StartDateTime.HasValue)
                {
                    bool isSingleDayWindow =
                        scheduleFrom.Date == scheduleTo.Date ||
                        (scheduleTo.Date - scheduleFrom.Date).TotalDays <= 1;

                    // Specjalna obsługa priorytetu 5
                    if (scheduledNew.Priority == 5 && isSingleDayWindow)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            "Cannot save event with priority 5.\n\n" +
                            "No space in today's schedule.\n\n" +
                            "Change the parameters (duration, deadline, priority) " +
                            "or remove/move other events.",
                            "No space in calendar",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Warning
                        );

                        // NIE zapisujemy eventu, przerywamy operację
                        throw new InvalidOperationException("P5 rejected: no slot in single-day window.");
                    }

                    // Pozostałe priorytety - standardowe zachowanie
                    throw new InvalidOperationException(
                        $"Could not find available time slot for event '{scheduledNew.Title}'.");
                }


                // 5) UPDATE dla istniejących eventów, które scheduler przesunął
                foreach (var e in scheduled.Where(x => x.EventID != 0))
                {
                    // Eventy bez czasu będą obsłużone osobno (krok 5a)
                    if (!e.StartDateTime.HasValue || !e.EndDateTime.HasValue)
                        continue;
                    
                    // Sprawdź czy mamy oryginalne dane do porównania
                    if (existingById.TryGetValue(e.EventID, out var orig))
                    {
                        // Event był w oryginalnym zapytaniu - porównaj i zaktualizuj jeśli się zmienił
                        bool changed =
                            orig.StartDateTime != e.StartDateTime ||
                            orig.EndDateTime != e.EndDateTime ||
                            orig.Priority != e.Priority ||
                            orig.Duration != e.Duration;

                        if (!changed)
                            continue;
                    }
                    // Event NIE był w zakresie zapytania ale został wywłaszczony do nowego czasu
                    // To się zdarza gdy scheduler przesuwa eventy poza oryginalne okno
                    // MUSIMY je też zaktualizować!

                    UpdateEventTimesTx(conn, tx, e.EventID, e.StartDateTime, e.EndDateTime, e.Duration, e.Priority);
                }


                // 6) INSERT dla eventów nowych (EventID==0): tu zapiszemy zarówno "newEvent", jak i ewentualne klony recurring
                // Żeby uniknąć podwójnych insertów, nadajemy ID od razu w obiekcie.
                int newEventId = 0;

                foreach (var e in scheduled.Where(x => x.EventID == 0))
                {
                    // Jeśli to "nowy event z formularza" i ma czas -> zapisujemy tak jak jest
                    int id = InsertEventTx(conn, tx, e);
                    e.EventID = id;

                    if (ReferenceEquals(e, scheduledNew))
                        newEventId = id;
                }

                // Fallback: czasem przez ReferenceEquals może nie trafić (np. jeśli gdzieś robisz kopie)
                if (newEventId == 0 && scheduledNew.EventID != 0)
                    newEventId = scheduledNew.EventID;

                if (newEventId == 0)
                    throw new InvalidOperationException("Insert failed: new event id not resolved.");

                tx.Commit();
                return newEventId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private int InsertEventTx(SqlConnection conn, SqlTransaction tx, Event evt)
        {
            // Minimalna korekta Duration jeśli mamy czasy
            if (evt.Duration == 0 && evt.StartDateTime.HasValue && evt.EndDateTime.HasValue)
                evt.Duration = (int)(evt.EndDateTime.Value - evt.StartDateTime.Value).TotalMinutes;
            if (evt.Duration == 0)
                evt.Duration = 120;

            // Upewnij się że priority/weight spójne
            if (evt.Priority == 0)
                evt.CalculatePriority();



            string query = @"INSERT INTO Events 
        (Title, Description, StartDateTime, EndDateTime, Duration, Category, Priority,
         Deadline,
         IsSetEvent, DoInDaytimeUntil, DoInDaytimeUntilTime, UserId, CreatedDate, ModifiedDate)
        VALUES
        (@Title, @Description, @StartDateTime, @EndDateTime, @Duration,  @Category, @Priority,
         @Deadline,
         @IsSetEvent, @DoInDaytimeUntil, @DoInDaytimeUntilTime, @UserId, GETDATE(), GETDATE());
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var cmd = new SqlCommand(query, conn, tx);

            cmd.Parameters.AddWithValue("@Title", evt.Title ?? string.Empty);
            cmd.Parameters.AddWithValue("@Description", (object?)evt.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDateTime", (object?)evt.StartDateTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDateTime", (object?)evt.EndDateTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Duration", evt.Duration);
            cmd.Parameters.AddWithValue("@Category", evt.Category ?? "0");

            cmd.Parameters.AddWithValue("@Priority", evt.Priority);
            cmd.Parameters.AddWithValue("@Deadline", (object?)evt.Deadline ?? DBNull.Value);
          
            cmd.Parameters.AddWithValue("@IsSetEvent", evt.IsSetEvent);
            cmd.Parameters.AddWithValue("@DoInDaytimeUntil", evt.DoInDaytimeUntil);
            cmd.Parameters.AddWithValue("@DoInDaytimeUntilTime", evt.DoInDaytimeUntilTime.HasValue ? (object)evt.DoInDaytimeUntilTime.Value.TimeOfDay : DBNull.Value);
            
            cmd.Parameters.AddWithValue("@UserId", UserSession.ContextUserId);

            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        private void UpdatePriorityOnly(int eventId, int newPriority)
        {
            const string sql = @"
UPDATE Events
SET Priority = @Priority,
    ModifiedDate = GETDATE()
WHERE EventID = @EventID;";

            DatabaseHelper.ExecuteNonQuery(sql,
                new SqlParameter("@Priority", newPriority),
                new SqlParameter("@EventID", eventId));
        }


        private void UpdateEventTimesTx(SqlConnection conn, SqlTransaction tx, int eventId,
            DateTime? start, DateTime? end, int duration, int priority)
        {
            const string sql = @"
UPDATE Events
SET StartDateTime = @Start,
    EndDateTime   = @End,
    Duration      = @Duration,
    Priority      = @Priority,
    ModifiedDate  = GETDATE()
WHERE EventID = @EventID;";

            using var cmd = new SqlCommand(sql, conn, tx);
            cmd.Parameters.AddWithValue("@Start", (object?)start ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@End", (object?)end ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Duration", duration);
            cmd.Parameters.AddWithValue("@Priority", priority);
            cmd.Parameters.AddWithValue("@EventID", eventId);

            cmd.ExecuteNonQuery();
        }



        private List<Event> GetEventsForUserInRangeTx(SqlConnection conn, SqlTransaction tx, int userId, DateTime startDate, DateTime endDate)
        {
            // Pobieramy eventy, które OVERLAP z zakresem (tak jak GetEvents)
            string query = @"SELECT * FROM Events
                     WHERE StartDateTime IS NOT NULL
                       AND EndDateTime IS NOT NULL
                       AND StartDateTime < @EndDate
                       AND EndDateTime > @StartDate
                       AND (UserId = @UserId OR UserId IS NULL)
                     ORDER BY StartDateTime";

            using var cmd = new SqlCommand(query, conn, tx);
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            cmd.Parameters.AddWithValue("@UserId", userId);

            var list = new List<Event>();
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToEvent(row));

            return list;
        }

        #endregion

        #region Aktualizacja wydarzeń

        /// <summary>
        /// Aktualizuje event w bazie danych.
        /// </summary>
        public void UpdateEvent(Event evt)
        {
            if (evt.StartDateTime.HasValue && evt.EndDateTime.HasValue &&
                evt.EndDateTime.Value <= evt.StartDateTime.Value)
            {
                throw new InvalidOperationException("End time must be later than start time.");
            }

            // Aktualizuj Duration jeśli EndDateTime się zmienił
            if (evt.EndDateTime.HasValue && evt.StartDateTime.HasValue)
            {
                evt.Duration = (int)(evt.EndDateTime.Value - evt.StartDateTime.Value).TotalMinutes;
            }

            string query = @"UPDATE Events SET 
                Title = @Title,
                Description = @Description,
                StartDateTime = @StartDateTime,
                EndDateTime = @EndDateTime,
                Duration = @Duration,
                ColorCode = @ColorCode,
                Category = @Category,
                IsCompleted = @IsCompleted,
                IsRecurring = @IsRecurring,
                Priority = @Priority,
                Deadline = @Deadline,
                HasMargin = @HasMargin,
                MarginMinutes = @MarginMinutes,
                IsSetEvent = @IsSetEvent,
                DoInDaytimeUntil = @DoInDaytimeUntil,
                DoInDaytimeUntilTime = @DoInDaytimeUntilTime,
                RecurrenceDays = @RecurrenceDays,
                ModifiedDate = GETDATE()
            WHERE EventID = @EventID";

            // Category w DB jest kodem (0-8). W UI czasem trzymamy nazwę.
            // Upewnij się, że do SQL idzie kod jako string.
            string categoryValue = evt.Category;
            if (string.IsNullOrWhiteSpace(categoryValue))
            {
                categoryValue = evt.CategoryCode.ToString();
            }
            else if (!int.TryParse(categoryValue, out _))
            {
                var resolved = 0;
                for (int code = 0; code <= 8; code++)
                {
                    if (string.Equals(Event.GetCategoryName(code), categoryValue, StringComparison.OrdinalIgnoreCase))
                    {
                        resolved = code;
                        break;
                    }
                }
                evt.CategoryCode = resolved;
                categoryValue = resolved.ToString();
            }

            var parameters = new[]
            {
                new SqlParameter("@EventID", evt.EventID),
                new SqlParameter("@Title", evt.Title ?? string.Empty),
                new SqlParameter("@Description", (object?)evt.Description ?? DBNull.Value),
                new SqlParameter("@StartDateTime", (object?)evt.StartDateTime ?? DBNull.Value),
                new SqlParameter("@EndDateTime", (object?)evt.EndDateTime ?? DBNull.Value),
                new SqlParameter("@Duration", evt.Duration),
                new SqlParameter("@ColorCode", evt.ColorCode ?? "#7F8C8D"),
                new SqlParameter("@Category", categoryValue),
                new SqlParameter("@IsCompleted", evt.IsCompleted),
                new SqlParameter("@IsRecurring", evt.IsRecurring),

                new SqlParameter("@Priority", evt.Priority),
                new SqlParameter("@Deadline", (object?)evt.Deadline ?? DBNull.Value),
                new SqlParameter("@HasMargin", evt.HasMargin),
                new SqlParameter("@MarginMinutes", (object?)evt.MarginMinutes ?? DBNull.Value),
                new SqlParameter("@IsSetEvent", evt.IsSetEvent),
                new SqlParameter("@DoInDaytimeUntil", evt.DoInDaytimeUntil),
                new SqlParameter("@DoInDaytimeUntilTime", evt.DoInDaytimeUntilTime.HasValue ? (object)evt.DoInDaytimeUntilTime.Value.TimeOfDay : DBNull.Value),
                new SqlParameter("@RecurrenceDays", (object?)evt.RecurrenceDays ?? DBNull.Value)
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
            }
            catch (SqlException ex)
            {
                // Jeśli kolumny nie istnieją, użyj podstawowego update
                if (ex.Message.Contains("Invalid column name"))
                {
                    UpdateEventBasic(evt);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Aktualizuje event z reschedule innych eventów przy zmianie czasu.
        /// 
        /// SetEvent: Traktuje jak dodanie nowego - wypycha elastyczne eventy.
        /// Priority Event: Force-schedule w wybranym czasie, rehome innych.
        /// </summary>
        public void UpdateEventWithReschedule(Event evt, DateTime oldStart, DateTime oldEnd, DateTime newStart, DateTime newEnd)
        {
            // Sprawdź czy czas się zmienił
            bool timeChanged = oldStart != newStart || oldEnd != newEnd;
            
            if (!timeChanged)
            {
                // Bez zmiany czasu - normalna aktualizacja
                UpdateEvent(evt);
                return;
            }

            // Walidacja
            if (newEnd <= newStart)
                throw new InvalidOperationException("End time must be later than start time.");

            using var conn = DatabaseHelper.OpenConnection();
            using var tx = conn.BeginTransaction();

            try
            {
                // Pobierz eventy w SZEROKIM zakresie - scheduler będzie szukać do 14 dni
                // Musimy mieć wszystkie SetEventy widoczne jako blokery
                var extendedStart = DateTime.Today.AddDays(-1);
                var extendedEnd = DateTime.Today.AddDays(15);
                var existingEvents = GetEventsForUserInRangeTx(conn, tx, UserSession.ContextUserId, extendedStart, extendedEnd);

                // Usuń aktualny event z listy (będziemy go aktualizować osobno)
                existingEvents = existingEvents.Where(e => e.EventID != evt.EventID).ToList();

                if (evt.IsSetEvent)
                {
                    // === SetEvent: Wypychaj elastyczne eventy ===
                    foreach (var e in existingEvents)
                    {
                        // Sprawdź kolizję
                        if (!e.StartDateTime.HasValue || !e.EndDateTime.HasValue)
                            continue;

                        bool overlaps = newStart < e.EndDateTime.Value && newEnd > e.StartDateTime.Value;
                        
                        if (overlaps)
                        {
                            if (e.IsSetEvent)
                            {
                                // Kolizja z innym SetEvent - błąd
                                throw new InvalidOperationException(
                                    $"Cannot schedule: conflicts with fixed event '{e.Title}' ({e.StartDateTime:HH:mm}-{e.EndDateTime:HH:mm})");
                            }
                            
                            // Elastyczny event - wyczyść czas (będzie przeplanowany)
                            e.StartDateTime = null;
                            e.EndDateTime = null;
                        }
                    }

                    // Ustaw nowy czas
                    evt.StartDateTime = newStart;
                    evt.EndDateTime = newEnd;
                    evt.Duration = (int)(newEnd - newStart).TotalMinutes;

                    // Przeplanuj wyparte eventy
                    var scheduler = new EventSchedulingAlgorithm();
                    var allEvents = new List<Event>(existingEvents) { evt };
                    var scheduled = scheduler.ScheduleEvents(allEvents, DateTime.Today, DateTime.Today.AddDays(14));

                    // Zaktualizuj eventy w DB
                    foreach (var e in scheduled.Where(x => x.EventID != 0 && x.EventID != evt.EventID))
                    {
                        if (e.StartDateTime.HasValue && e.EndDateTime.HasValue)
                        {
                            UpdateEventTimesTx(conn, tx, e.EventID, e.StartDateTime, e.EndDateTime, e.Duration, e.Priority);
                        }
                    }
                }
                else
                {
                    // === Priority Event: Force-schedule, rehome innych ===
                    
                    // Sprawdź kolizję z SetEvents - to jest błąd
                    foreach (var e in existingEvents)
                    {
                        if (!e.StartDateTime.HasValue || !e.EndDateTime.HasValue)
                            continue;

                        bool overlaps = newStart < e.EndDateTime.Value && newEnd > e.StartDateTime.Value;
                        
                        if (overlaps && e.IsSetEvent)
                        {
                            throw new InvalidOperationException(
                                $"Cannot schedule: conflicts with fixed event '{e.Title}' ({e.StartDateTime:HH:mm}-{e.EndDateTime:HH:mm})");
                        }
                    }

                    // Ustaw nowy czas (force-schedule)
                    evt.StartDateTime = newStart;
                    evt.EndDateTime = newEnd;
                    evt.Duration = (int)(newEnd - newStart).TotalMinutes;

                    // Znajdź elastyczne eventy do rehome
                    var eventsToRehome = existingEvents.Where(e =>
                        e.StartDateTime.HasValue && e.EndDateTime.HasValue &&
                        !e.IsSetEvent &&
                        newStart < e.EndDateTime.Value && newEnd > e.StartDateTime.Value
                    ).ToList();

                    // Wyczyść ich czas
                    foreach (var e in eventsToRehome)
                    {
                        e.StartDateTime = null;
                        e.EndDateTime = null;
                    }

                    // Przeplanuj
                    var scheduler = new EventSchedulingAlgorithm();
                    var allEvents = new List<Event>(existingEvents) { evt };
                    var scheduled = scheduler.ScheduleEvents(allEvents, DateTime.Today, DateTime.Today.AddDays(14));

                    // Zaktualizuj eventy w DB
                    foreach (var e in scheduled.Where(x => x.EventID != 0 && x.EventID != evt.EventID))
                    {
                        if (e.StartDateTime.HasValue && e.EndDateTime.HasValue)
                        {
                            UpdateEventTimesTx(conn, tx, e.EventID, e.StartDateTime, e.EndDateTime, e.Duration, e.Priority);
                        }
                    }
                }

                // Zaktualizuj główny event
                UpdateEventTx(conn, tx, evt);

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private void UpdateEventTx(SqlConnection conn, SqlTransaction tx, Event evt)
        {
            string query = @"UPDATE Events SET 
                Title = @Title,
                Description = @Description,
                StartDateTime = @StartDateTime,
                EndDateTime = @EndDateTime,
                Duration = @Duration,
                Priority = @Priority,
                Category = @Category,
                DoInDaytimeUntil = @DoInDaytimeUntil,
                DoInDaytimeUntilTime = @DoInDaytimeUntilTime,
                ModifiedDate = GETDATE()
            WHERE EventID = @EventID";

            // Category w DB jest kodem (0-8) jako string
            string categoryValue = evt.CategoryCode.ToString();
            if (evt.CategoryCode == 0 && !string.IsNullOrWhiteSpace(evt.Category))
            {
                for (int code = 1; code <= 8; code++)
                {
                    if (string.Equals(Event.GetCategoryName(code), evt.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        categoryValue = code.ToString();
                        break;
                    }
                }
            }

            using var cmd = new SqlCommand(query, conn, tx);
            cmd.Parameters.AddWithValue("@EventID", evt.EventID);
            cmd.Parameters.AddWithValue("@Title", evt.Title ?? "");
            cmd.Parameters.AddWithValue("@Description", (object?)evt.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StartDateTime", (object?)evt.StartDateTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDateTime", (object?)evt.EndDateTime ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Duration", evt.Duration);
            cmd.Parameters.AddWithValue("@Priority", evt.Priority);
            cmd.Parameters.AddWithValue("@Category", categoryValue);
            cmd.Parameters.AddWithValue("@DoInDaytimeUntil", evt.DoInDaytimeUntil);
            cmd.Parameters.AddWithValue("@DoInDaytimeUntilTime", evt.DoInDaytimeUntilTime.HasValue ? (object)evt.DoInDaytimeUntilTime.Value.TimeOfDay : DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        private void UpdateEventBasic(Event evt)
        {
            // Fallback do podstawowego update jeśli nowe kolumny jeszcze nie istnieją
            string query = @"UPDATE Events SET 
                Title = @Title,
                Description = @Description,
                StartDateTime = @StartDateTime,
                EndDateTime = @EndDateTime,
                Duration = @Duration,
                Category = @Category,
                ModifiedDate = GETDATE()
            WHERE EventID = @EventID";

            // Category w DB jest kodem (0-8) jako string.
            // Jeśli evt.Category to nazwa, przekonwertuj na kod.
            string categoryValue = evt.CategoryCode.ToString();
            if (evt.CategoryCode == 0 && !string.IsNullOrWhiteSpace(evt.Category))
            {
                // Spróbuj odzyskać kod z nazwy
                for (int code = 1; code <= 8; code++)
                {
                    if (string.Equals(Event.GetCategoryName(code), evt.Category, StringComparison.OrdinalIgnoreCase))
                    {
                        categoryValue = code.ToString();
                        break;
                    }
                }
            }

            var parameters = new[]
            {
                new SqlParameter("@EventID", evt.EventID),
                new SqlParameter("@Title", evt.Title ?? string.Empty),
                new SqlParameter("@Description", (object?)evt.Description ?? DBNull.Value),
                new SqlParameter("@StartDateTime", (object?)evt.StartDateTime ?? DBNull.Value),
                new SqlParameter("@EndDateTime", (object?)evt.EndDateTime ?? DBNull.Value),
                new SqlParameter("@Duration", evt.Duration),
                new SqlParameter("@Category", categoryValue),
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        #endregion

        #region Usuwanie wydarzeń

        /// <summary>
        /// Usuwa event z bazy danych.
        /// 
        /// WAŻNE: Obsługuje powiązania:
        /// 1. Ustawia LinkedEventID na NULL dla eventów które wskazywały na usuwany
        /// 2. Usuwa powiązane FocusSessions
        /// 3. Promises table ma ON DELETE CASCADE - usuwa się automatycznie
        /// </summary>
        /// <param name="eventId">ID eventu do usunięcia</param>
        public void DeleteEvent(int eventId)
        {
          
            // Usuń FocusSessions które wskazują na ten event
            string deleteFocusSessionsQuery = "DELETE FROM FocusSessions WHERE EventID = @EventID";
            DatabaseHelper.ExecuteNonQuery(deleteFocusSessionsQuery, new SqlParameter("@EventID", eventId));

            // Usuń sam event
            // Promises table ma ON DELETE CASCADE - usuwa automatycznie
            string query = "DELETE FROM Events WHERE EventID = @EventID";
            DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@EventID", eventId));
        }

        public List<Event> GetLinkedEvents(int eventId)
        {
            string query = "SELECT * FROM Events WHERE LinkedEventID = @EventID ORDER BY StartDateTime";
            var events = new List<Event>();
            var dataTable = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@EventID", eventId));

            foreach (DataRow row in dataTable.Rows)
            {
                events.Add(MapRowToEvent(row));
            }

            return events;
        }

        #endregion

        #region Przeplanowanie nieukończonych

        /// <summary>
        /// Sprawdza i przeplanowuje nieukończone eventy które minęły 24+ godzin temu.
        /// Wywoływane przy starcie aplikacji lub okresowo.
        /// </summary>
        public void RescheduleIncompleteEvents()
        {
            var now = DateTime.Now;
            var scheduleFrom = now;
            var scheduleTo = now.AddDays(14);
            
            // Pobierz wszystkie eventy z ostatnich 14 dni które mogą wymagać przeplanowania
            var eventsToCheck = GetEvents(now.AddDays(-14), now);
            
            // Filtruj do nieukończonych eventów które są 24+ godzin po czasie zakończenia
            var incompleteEvents = eventsToCheck
                .Where(e => !e.IsCompleted && 
                            e.EndDateTime.HasValue && 
                            (now - e.GetEndDateTime()).TotalHours >= 24 &&
                            e.CanBePostponed())
                .ToList();
            
            if (incompleteEvents.Count == 0)
            {
                return;
            }
            
            // Pobierz aktualne eventy do sprawdzenia konfliktów
            var currentEvents = GetEvents(scheduleFrom, scheduleTo);
            
            var scheduler = new EventSchedulingAlgorithm();

            foreach (var evt in incompleteEvents)
            {
                // Wyczyść stare czasy żeby algorytm mógł przeplanować
                evt.StartDateTime = null;
                evt.EndDateTime = null;
                
                // Dodaj do listy do planowania
                var allEvents = new List<Event>(currentEvents) { evt };
                
                try
                {
                    var scheduled = scheduler.ScheduleEvents(allEvents, scheduleFrom, scheduleTo);
                    
                    // Znajdź przeplanowany event i zaktualizuj go w bazie
                    var rescheduled = scheduled.FirstOrDefault(e => e.EventID == evt.EventID);
                    if (rescheduled != null && rescheduled.StartDateTime.HasValue)
                    {
                        UpdateEvent(rescheduled);
                        currentEvents.Add(rescheduled);
                    }
                }
                catch (Exception)
                {
                    // Ignoruj błędy - kontynuuj z następnym eventem
                }
            }
        }

        #endregion

        #region Mapowanie danych

        /// <summary>
        /// Mapuje wiersz DataRow na obiekt Event.
        /// Obsługuje zarówno stare jak i nowe kolumny bazy danych.
        /// </summary>
        private Event MapRowToEvent(DataRow row)
        {
            var evt = new Event
            {
                EventID = Convert.ToInt32(row["EventID"]),
                Title = row["Title"].ToString(),
                Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                StartDateTime = row["StartDateTime"] != DBNull.Value ? (DateTime?)row["StartDateTime"] : null,
                EndDateTime = row["EndDateTime"] != DBNull.Value ? (DateTime?)row["EndDateTime"] : null,
                Duration = Convert.ToInt32(row["Duration"]),
                Priority = Convert.ToInt32(row["Priority"]),
                Category = row["Category"] != DBNull.Value ? row["Category"].ToString() : "0",
                CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                ModifiedDate = row.Table.Columns.Contains("ModifiedDate") && row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]) : DateTime.MinValue,
                Deadline = row.Table.Columns.Contains("Deadline") && row["Deadline"] != DBNull.Value ? (DateTime?)row["Deadline"] : null,
                IsSetEvent = row.Table.Columns.Contains("IsSetEvent") ? Convert.ToBoolean(row["IsSetEvent"]) : false,
                DoInDaytimeUntil = row.Table.Columns.Contains("DoInDaytimeUntil") && row["DoInDaytimeUntil"] != DBNull.Value ? Convert.ToBoolean(row["DoInDaytimeUntil"]) : false,
                DoInDaytimeUntilTime = row.Table.Columns.Contains("DoInDaytimeUntilTime") && row["DoInDaytimeUntilTime"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)row["DoInDaytimeUntilTime"]) : (DateTime?)null,
         };

            // Parsuj CategoryCode
            if (int.TryParse(evt.Category, out int code))
            {
                evt.CategoryCode = code;
                evt.Category = Event.GetCategoryName(code);
            }
            
            // Oblicz flagi legacy (opcjonalnie)
            evt.IsImportant = (evt.Priority == 3 || evt.Priority == 4 || evt.Priority == 5);
            evt.IsUrgent = (evt.Priority == 2 || evt.Priority == 4 || evt.Priority == 5);

            return evt;
        }

        #endregion
    }
}
