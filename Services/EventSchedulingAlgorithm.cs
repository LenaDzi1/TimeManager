// ============================================================================
// EventSchedulingAlgorithm.cs
// Algorytm planowania wydarzeń w kalendarzu: priorytety, interwały energetyczne.
// ============================================================================

#nullable enable


using System;
using System.Collections.Generic;
using System.Linq;
using TimeManager.Models;

namespace TimeManager.Services
{
    /// <summary>
    /// Algorytm planowania wydarzeń w kalendarzu.
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Planowanie elastycznych wydarzeń w optymalnych interwałach energetycznych
    /// - Wypychanie wydarzeń o niższym priorytecie gdy trzeba zrobić miejsce
    /// - Obsługa wydarzeń cyklicznych (recurring)
    /// - Automatyczne odkładanie nieukończonych wydarzeń
    /// 
    /// PRIORYTETY WYDARZEŃ:
    /// - P1: "W międzyczasie" - może być przesunięte przez ważniejsze
    /// - P2-P4: Normalne priorytety (ważne/pilne)
    /// - P5: Krytyczne - deadline w ciągu 1 dnia
    /// 
    /// INTERWAŁY ENERGETYCZNE (względem pobudki):
    /// - Interwał 1 (0h - 1.5h): Rozbudzenie - niska energia
    /// - Interwał 2 (1.5h - 5.5h): Szczyt produktywności
    /// - Interwał 3 (5.5h - 9h): Spadek po lunchu
    /// - Interwał 4 (9h - 12.5h): Drugie okno produktywności
    /// - Interwał 5 (12.5h - sen): Wieczór - odpoczynek
    /// </summary>
    public class EventSchedulingAlgorithm
    {
        #region Stałe i interwały energetyczne

        /// <summary>Koniec interwału 1: 1.5h po pobudce (rozbudzenie)</summary>
        private static readonly TimeSpan Interval1 = TimeSpan.FromHours(1.5);
        
        /// <summary>Koniec interwału 2: 5.5h po pobudce (szczyt produktywności)</summary>
        private static readonly TimeSpan Interval2 = TimeSpan.FromHours(5.5);
        
        /// <summary>Koniec interwału 3: 9h po pobudce (po lunchu)</summary>
        private static readonly TimeSpan Interval3 = TimeSpan.FromHours(9);
        
        /// <summary>Koniec interwału 4: 12.5h po pobudce (drugie okno)</summary>
        private static readonly TimeSpan Interval4 = TimeSpan.FromHours(12.5);

        #endregion

        #region Główna metoda planowania


        /// <summary>
        /// Główna metoda planowania wydarzeń w kalendarzu.
        /// 
        /// ALGORYTM:
        /// 1. Przelicza priorytety wszystkich wydarzeń (deadline może podbić priorytet do P5)
        /// 2. Rozdziela eventy na: stałe (SetEvent), istniejące z DB, nowe do zaplanowania
        /// 3. Sortuje niezaplanowane wg priorytetu (malejąco) i deadline'u (rosnąco)
        /// 4. Planuje każdy event szukając wolnego slota lub wypychając niższe priorytety
        /// 5. Przetwarza eventy cykliczne (recurring)
        /// 6. Przetwarza eventy "co najmniej X razy w Y dni"
        /// 7. Odracza nieukończone eventy (24h po końcu)
        /// 8. Przesuwa eventy P1 jeśli kolidują z ważniejszymi
        /// </summary>
        /// <param name="events">Lista wydarzeń do zaplanowania</param>
        /// <param name="startDate">Początek okna planowania</param>
        /// <param name="endDate">Koniec okna planowania</param>
        /// <returns>Lista zaplanowanych wydarzeń posortowana chronologicznie</returns>
        public List<Event> ScheduleEvents(List<Event> events, DateTime startDate, DateTime endDate)
        {
            // Lista wydarzeń które już mają przypisany czas
            var scheduledEvents = new List<Event>();
            // Lista wydarzeń które trzeba dopiero zaplanować
            var unscheduledEvents = new List<Event>();
            
            var now = DateTime.Now;

            // ==================== FAZA 0: USUWANIE EVENTÓW PO DEADLINE ====================
            // Eventy z przekroczonym deadline są natychmiast usuwane (nie przeplanowywane)
            var eventsToProcess = new List<Event>();
            foreach (var evt in events)
            {
                // Jeśli event ma deadline który już minął I event nie jest ukończony - USUŃ
                if (evt.Deadline.HasValue && evt.Deadline.Value < now && !evt.IsCompleted)
                {
                    // Nie dodajemy do żadnej listy = event znika z kalendarza
                    continue;
                }
                eventsToProcess.Add(evt);
            }

            // ==================== FAZA 1: PRZELICZENIE PRIORYTETÓW ====================
            // Dzień referencyjny do obliczania priorytetów (ważne dla deadline'ów)
            var refDay = startDate.Date;
            foreach (var evt in eventsToProcess)
            {
                // Przelicz priorytet - deadline może podbić nawet ręcznie ustawiony P2-P4 do P5
                evt.CalculatePriority(refDay);
            }

            // ==================== FAZA 2: KATEGORYZACJA WYDARZEŃ ====================
            foreach (var evt in eventsToProcess)
            {
                // KATEGORIA A: Wydarzenie stałe (SetEvent) z ustawionym czasem
                // Np. spotkanie w pracy, wizyta u lekarza - nie można przesunąć
                if (evt.StartDateTime.HasValue && evt.IsSetEvent)
                {
                    // Sprawdź czy nie koliduje z innym stałym wydarzeniem
                    if (scheduledEvents.Any(f => f.StartDateTime.HasValue &&
                                                 f.IsSetEvent &&
                                                 Overlaps(f, evt)))
                    {
                        // Konflikt stałych wydarzeń - błąd krytyczny
                        throw new InvalidOperationException("Fixed event conflicts with another fixed event.");
                    }

                    // Wypychaj elastyczne eventy które nachodzą na nowy SetEvent
                    var overlappingFlexible = new List<Event>();
                    foreach (var f in scheduledEvents)
                    {
                        if (!f.StartDateTime.HasValue || !f.EndDateTime.HasValue)
                        {
                            continue;
                        }
                        if (f.IsSetEvent)
                        {
                            continue;
                        }
                        
                        bool overlaps = Overlaps(f, evt);
                        
                        if (overlaps)
                        {
                            overlappingFlexible.Add(f);
                        }
                    }
                    
                    foreach (var flex in overlappingFlexible)
                    {
                        // Usuń z scheduled i dodaj do przeplanowania
                        scheduledEvents.Remove(flex);
                        flex.StartDateTime = null;
                        flex.EndDateTime = null;
                        unscheduledEvents.Add(flex);
                    }

                    // Dodaj do zaplanowanych (stałe eventy mają absolutny priorytet czasowy)
                    scheduledEvents.Add(evt);
                }
                // KATEGORIA B: Istniejące elastyczne wydarzenie z bazy danych
                // Ma już przypisany czas, ale może być przeplanowane jeśli trzeba
                else if (evt.EventID != 0 && evt.StartDateTime.HasValue && evt.EndDateTime.HasValue)
                {
                    // Sprawdź czy event z bazy NIE nakłada się z już dodanymi
                    bool hasConflict = false;
                    foreach (var existing in scheduledEvents)
                    {
                        if (existing.StartDateTime.HasValue && existing.EndDateTime.HasValue)
                        {
                            bool overlaps = Overlaps(existing, evt);
                            if (overlaps)
                            {
                                hasConflict = true;
                                break;
                            }
                        }
                    }
                    
                    if (hasConflict)
                    {
                        // Konflikt - wyczyść czas i dodaj do przeplanowania
                        evt.StartDateTime = null;
                        evt.EndDateTime = null;
                        unscheduledEvents.Add(evt);
                    }
                    else
                    {
                        scheduledEvents.Add(evt);
                    }
                }
                // KATEGORIA C: Nowe wydarzenie lub bez czasu - wymaga zaplanowania
                else
                {
                    // Wyczyść ewentualny czas z formularza - algorytm znajdzie optymalny slot
                    evt.StartDateTime = null;
                    evt.EndDateTime = null;
                    // Dodaj do kolejki do zaplanowania
                    unscheduledEvents.Add(evt);
                }
            }

            // ==================== FAZA 3: SORTOWANIE KOLEJKI PLANOWANIA ====================
            // Sortuj: najpierw najwyższy priorytet (P5 przed P1), potem najwcześniejszy deadline
            unscheduledEvents = unscheduledEvents
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.Deadline ?? DateTime.MaxValue)
                .ToList();

            // ==================== FAZA 4: PLANOWANIE ELASTYCZNYCH WYDARZEŃ ====================
            foreach (var evt in unscheduledEvents)
            {
                // Elastyczne eventy szukają do skutku (lub do deadline)
                // - Eventy BEZ deadline: szukamy do 365 dni (praktycznie bez limitu)
                // - Eventy Z deadline: szukamy do deadline (bo potem i tak są usuwane)
                DateTime searchEnd;
                if (evt.Deadline.HasValue)
                {
                    // Ma deadline - to jest granica szukania
                    searchEnd = evt.Deadline.Value.Date.AddDays(1);
                }
                else
                {
                    // Bez deadline - szukaj do 365 dni (praktycznie bez limitu)
                    searchEnd = DateTime.Today.AddDays(365);
                }
                
                // Znajdź slot dla eventu - może wypychać eventy o niższym priorytecie
                ScheduleFlexibleEventWithDisplacement(evt, scheduledEvents, startDate, searchEnd);
            }

            // ==================== FAZA 5: POST-PROCESSING ====================
            // Przetwórz eventy cykliczne - stwórz kopie dla kolejnych wystąpień
            scheduledEvents = ProcessRecurringEvents(scheduledEvents, startDate, endDate);
            // Odrocz eventy nieukończone ponad 24h po planowanym końcu
            scheduledEvents = PostponeIncompleteEvents(scheduledEvents);
            // Przesuń eventy P1 jeśli kolidują z ważniejszymi (inline z RescheduleDisplacedEvents)
            var hardEvents = scheduledEvents
                .Where(e => e.StartDateTime.HasValue && (e.Priority > 1 || e.IsSetEvent))
                .ToList();
            var conflictingP1s = scheduledEvents
                .Where(e => e.Priority == 1 && !e.IsSetEvent &&
                       e.StartDateTime.HasValue && e.EndDateTime.HasValue &&
                       hardEvents.Any(h => Overlaps(e, h)))
                .ToList();
                
            if (conflictingP1s.Count > 0)
            {
                foreach (var p1 in conflictingP1s)
                    scheduledEvents.Remove(p1);
                RescheduleDisplacedEvents(conflictingP1s, scheduledEvents, startDate, endDate);
            }

            // ==================== FAZA 6: ZWRÓĆ WYNIK ====================
            // Zwróć listę posortowaną chronologicznie
            return scheduledEvents
                .OrderBy(e => e.GetStartDateTime())
                .ToList();
        }

        #endregion

        #region Wykrywanie konfliktów

        /// <summary>
        /// Sprawdza czy istniejące wydarzenie powinno być pominięte przy sprawdzaniu konfliktów.
        /// 
        /// Konsoliduje całą logikę pomijania w jednym miejscu, żeby:
        /// - IsFree, GetBlockingEvents i GetNextStartIfBlocked były spójne
        /// - Uniknąć duplikacji kodu
        /// 
        /// POMINIĘTE SĄ:
        /// - Ten sam event (referencja lub ID)
        /// - Eventy bez ustawionego czasu
        /// - Elastyczne P1 - są "niewidzialne" dla ważniejszych eventów
        /// </summary>
        /// <param name="candidate">Event który próbujemy zaplanować</param>
        /// <param name="existing">Istniejący event który sprawdzamy</param>
        /// <returns>True jeśli existing powinien być pominięty (nie blokuje candidate)</returns>
        private bool ShouldSkipEvent(Event candidate, Event existing)
        {
            // Pomiń samego siebie (ta sama referencja w pamięci)
            if (ReferenceEquals(existing, candidate))
                return true;

            // Pomiń eventy bez ustawionego czasu (nie mogą blokować)
            if (!existing.StartDateTime.HasValue || !existing.EndDateTime.HasValue)
                return true;

            // Pomiń ten sam event po ID (jeśli ID != 0, czyli zapisany w DB)
            if (existing.EventID == candidate.EventID && candidate.EventID != 0)
                return true;

            // Elastyczne P1 są "niewidzialne" dla ważniejszych eventów
            // (P1 to eventy "w międzyczasie" - mogą być przesunięte)
            // Elastyczne P1 = Priority == 1 ORAZ nie jest stałym eventem
            bool existingIsP1Flex = existing.Priority == 1 && !existing.IsSetEvent;
            // Kandydat jest "ważniejszy" jeśli ma Priority > 1 lub jest stałym eventem
            bool candidateIsAboveP1 = candidate.Priority > 1 || candidate.IsSetEvent;
            // Jeśli existing to P1 elastyczny, a candidate jest ważniejszy - pomiń existing
            if (existingIsP1Flex && candidateIsAboveP1)
                return true;

            // Nie pomijaj - existing może blokować candidate
            return false;
        }

        #endregion

        #region Obsługa wypychania

        /// <summary>
        /// Przeplanowuje wypchniete eventy (dowolnego priorytetu).
        /// 
        /// Uniwersalna funkcja wywoływana gdy eventy zostały wyparte przez ważniejszy.
        /// Obsługuje zarówno P1 jak i wyższe priorytety wypchniete przez P5/SetEvent.
        /// 
        /// ALGORYTM:
        /// 1. Sortuje wg priorytetu (P5 najpierw, potem deadline)
        /// 2. Dla każdego: wyczyść czas i przeplanuj
        /// 3. Upewnij się że event jest na liście (nawet bez czasu)
        /// </summary>
        /// <param name="displacedEvents">Lista wypchnietych eventów do przeplanowania</param>
        /// <param name="scheduledEvents">Lista wszystkich zaplanowanych eventów</param>
        /// <param name="scheduleStart">Początek okna szukania</param>
        /// <param name="scheduleEnd">Koniec okna szukania</param>
        private void RescheduleDisplacedEvents(
            List<Event> displacedEvents,
            List<Event> scheduledEvents,
            DateTime scheduleStart,
            DateTime scheduleEnd)
        {
            if (displacedEvents.Count == 0)
                return;
            
            // Sortuj wg priorytetu (P5 najpierw), potem deadline
            var sorted = displacedEvents
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.Deadline ?? DateTime.MaxValue)
                .ToList();

            foreach (var displaced in sorted)
            {
                // Wyczyść stary czas
                displaced.StartDateTime = null;
                displaced.EndDateTime = null;

                // Rozszerz okno szukania (do 14 dni od teraz)
                var searchStart = DateTime.Now;
                var searchEnd = scheduleEnd.AddDays(14);

                // Ogranicz do deadline jeśli istnieje
                if (displaced.Deadline.HasValue && displaced.Deadline.Value < searchEnd)
                    searchEnd = displaced.Deadline.Value;

                // Tylko jeśli okno ma sens
                if (searchEnd > searchStart)
                {
                    ScheduleFlexibleEventWithDisplacement(displaced, scheduledEvents, searchStart, searchEnd);
                }

                // Upewnij się że event jest na liście (nawet bez czasu)
                AddUnique(scheduledEvents, displaced);
            }
        }

        #endregion

        #region Główne planowanie

        /// <summary>
        /// Planuje elastyczne wydarzenie, potencjalnie wypychając eventy o niższym priorytecie.
        /// 
        /// ALGORYTM:
        /// 1. Dla każdego dnia w oknie planowania:
        ///    a) Przelicz priorytet eventu względem tego dnia (deadline może zmieniać priorytet)
        ///    b) Pobierz preferowane interwały energetyczne dla tego priorytetu
        ///    c) Dla każdego interwału:
        ///       - FAZA 1: Szukaj wolnego slota (bez wypychania)
        ///       - FAZA 2: Jeśli P3+ i nie znaleziono - próbuj wypychać niższe priorytety
        ///    d) Jeśli wszystkie interwały zawiodły - przejdź do następnego dnia
        /// 2. Jeśli nie znaleziono w żadnym dniu - zostaw bez czasu
        /// 
        /// SPECJALNE PRZYPADKI:
        /// - P5 (deadline): Próbuje wypychać NAJPIERW, przed szukaniem wolnego slota
        /// - P1 (w międzyczasie): Nie wypycha innych, tylko szuka wolnego miejsca
        /// - DoInDaytimeUntil: Ogranicza interwał do ustawionej godziny
        /// </summary>
        /// <param name="evt">Wydarzenie do zaplanowania</param>
        /// <param name="scheduledEvents">Lista już zaplanowanych wydarzeń</param>
        /// <param name="startDate">Początek okna planowania</param>
        /// <param name="endDate">Koniec okna planowania</param>
        private void ScheduleFlexibleEventWithDisplacement(Event evt, List<Event> scheduledEvents, DateTime startDate, DateTime endDate)
        {
            // Pobierz godziny snu użytkownika (dla określenia aktywnych godzin dnia)
            var wakeSleep = GetSleepWindow();

            // Dzień od którego szukamy (nie wcześniej niż dziś)
            var day = startDate.Date;
            // Ostatni dzień okna planowania
            var endDay = endDate.Date;

            // Nie planuj w przeszłości - zacznij od dziś
            if (day < DateTime.Today)
                day = DateTime.Today;

            // Licznik dni do debugowania
            int dayCount = 0;

            while (day <= endDay)
            {
                dayCount++;

                // Przelicz priorytet względem dnia, który właśnie sprawdzamy
                evt.CalculatePriority(day);

                // Preferencje interwałów zależą od priorytetu, więc muszą być po CalculatePriority(day)
                var preferredIntervals = GetIntervalPreference(evt.Priority);

                var intervals = BuildDailyIntervals(day, wakeSleep.wake, wakeSleep.sleep);

                foreach (var intervalId in preferredIntervals)
                {
                    if (!intervals.TryGetValue(intervalId, out var range))
                        continue;

                    var rangeStart = range.start;
                    var rangeEnd = range.end;

                    // P5: najpierw próbuj wywłaszczyć w tym interwale
                    if (evt.Priority == 5)
                    {
                        // Zastosuj ograniczenie DoInDaytimeUntil dla displacement
                        DateTime effectiveEnd = rangeEnd;
                        if (evt.DoInDaytimeUntil && evt.DoInDaytimeUntilTime.HasValue)
                        {
                            var limitTime = day.Date.Add(evt.DoInDaytimeUntilTime.Value.TimeOfDay);
                            if (rangeStart >= limitTime)
                                continue;
                            if (effectiveEnd > limitTime)
                                effectiveEnd = limitTime;
                        }

                        if (TryDisplaceAndSchedule(evt, scheduledEvents, rangeStart, effectiveEnd, startDate, endDate))
                            return;
                    }

                    // -------- FAZA 1 (WOLNY SLOT) w tym interwale --------
                    DateTime slotEffectiveEnd = rangeEnd;
                     if (evt.DoInDaytimeUntil && evt.DoInDaytimeUntilTime.HasValue)
                    {
                        var limitTime = day.Date.Add(evt.DoInDaytimeUntilTime.Value.TimeOfDay);
                        if (rangeStart >= limitTime)
                            continue;
                        if (slotEffectiveEnd > limitTime)
                            slotEffectiveEnd = limitTime;
                    }

                    var slot = FindSlotInRange(evt, scheduledEvents, rangeStart, slotEffectiveEnd, null);

                    if (slot.HasValue)
                    {
                        var proposedEnd = slot.Value.AddMinutes(evt.Duration);

                        int margin = evt.HasMargin ? (evt.MarginMinutes ?? 0) : 0;

                        if (IsFree(evt, scheduledEvents, slot.Value, proposedEnd, margin))
                        {
                            evt.StartDateTime = slot.Value;
                            evt.EndDateTime = proposedEnd;
                            AddUnique(scheduledEvents, evt);
                            return;
                        }
                    }

                    // -------- FAZA 2 (WYPYCHANIE) w TYM SAMYM interwale --------
                    // (bez P5, bo P5 robi displacement wcześniej)
                    if (evt.Priority >= 3 && evt.Priority != 5)
                    {
                        try
                        {
                            if (TryDisplaceAndSchedule(evt, scheduledEvents, rangeStart, slotEffectiveEnd, startDate, endDate))
                                return;
                        }
                        catch (Exception)
                        {
                            // Ignoruj błędy - kontynuuj do następnego interwału
                        }
                    }

                    // Jeśli się nie udało w tym interwale -> idziemy do następnego interwału
                }

                // FAZA 3: Nie udało się zaplanować w tym dniu - próbuj następny dzień
                bool isSingleDayWindow = (endDate.Date - startDate.Date).TotalDays <= 1;

                if (evt.Priority == 5 && isSingleDayWindow)
                {
                    break;
                }

                day = day.AddDays(1);
            }

            // Nie udało się zaplanować - zostaw bez czasu
            evt.StartDateTime = null;
            evt.EndDateTime = null;

            AddUnique(scheduledEvents, evt);
            
            // Znajdź P1 nadeptane przez evt i przeplanuj je
            if (evt.StartDateTime.HasValue && (evt.Priority > 1 || evt.IsSetEvent))
            {
                var overlappedP1 = scheduledEvents
                    .Where(e => e.Priority == 1 && !e.IsSetEvent &&
                           e.StartDateTime.HasValue && e.EndDateTime.HasValue &&
                           !ReferenceEquals(e, evt) && Overlaps(e, evt))
                    .ToList();
                    
                if (overlappedP1.Count > 0)
                {
                    foreach (var p1 in overlappedP1)
                        scheduledEvents.Remove(p1);
                    RescheduleDisplacedEvents(overlappedP1, scheduledEvents, startDate, endDate);
                }
            }

        }


        /// <summary>
        /// Próbuje wypchnąć eventy o niższym priorytecie i zaplanować kandydata w ich miejsce.
        /// 
        /// ALGORYTM:
        /// 1. Iteruje przez interwał co 15 minut szukając slotu
        /// 2. Dla każdego slotu:
        ///    a) Znajdź blokujące eventy (GetBlockingEvents)
        ///    b) Jeśli brak blokerów i slot wolny - wstaw kandydata
        ///    c) Jeśli są blokery - sprawdź czy WSZYSTKIE można wypchnąć:
        ///       - Muszą być elastyczne (nie SetEvent)
        ///       - Muszą mieć niższy priorytet LUB być P1 (które zawsze można wypchnąć)
        ///       - Muszą mieć czas >= kandydata (żeby było miejsce)
        ///    d) Jeśli tak - usuń blokery, wstaw kandydata, spróbuj przeplanować blokery
        /// 3. Jeśli nie znaleziono slota - zwróć false
        /// 
        /// BEZPIECZEŃSTWO:
        /// - Rollback jeśli po wstawieniu kandydat nadal koliduje
        /// - Wypychane eventy trafiają na listę do przeplanowania
        /// - P1 mają rozszerzony zakres szukania (do 7 dni od deadline)
        /// </summary>
        /// <param name="candidate">Event który próbujemy wstawić</param>
        /// <param name="scheduledEvents">Lista zaplanowanych eventów</param>
        /// <param name="rangeStart">Początek interwału do sprawdzenia</param>
        /// <param name="rangeEnd">Koniec interwału</param>
        /// <param name="scheduleStart">Początek całego okna planowania (do przeplanowania wypchnietych)</param>
        /// <param name="scheduleEnd">Koniec całego okna planowania</param>
        /// <returns>True jeśli udało się zaplanować, false w przeciwnym razie</returns>
        private bool TryDisplaceAndSchedule(
            Event candidate,
            List<Event> scheduledEvents,
            DateTime rangeStart,
            DateTime rangeEnd,
            DateTime scheduleStart,
            DateTime scheduleEnd)
        {
            try
            {
                // Oblicz całkowity wymagany czas (duration + marginesy z obu stron)
                int duration = candidate.Duration;
                int margin = candidate.HasMargin ? candidate.MarginMinutes ?? 0 : 0;
                int total = duration + (margin * 2);

                // Jeśli interwał jest pusty lub odwrócony - nie ma co szukać
                if (rangeEnd <= rangeStart)
                    return false;

                DateTime current = rangeStart;

                // Nie planuj w przeszłości
                if (current < DateTime.Now)
                    current = DateTime.Now;

                // Zeruj sekundy
                current = new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute, 0);

                if (current.AddMinutes(total) > rangeEnd)
                    return false;

                scheduledEvents.RemoveAll(e => ReferenceEquals(e, candidate));

                while (current.AddMinutes(total) <= rangeEnd)
                {
                    // WAŻNE: nie pozwól, żeby "stary" czas kandydata przeciekał między iteracjami
                    candidate.StartDateTime = null;
                    candidate.EndDateTime = null;

                    var slotStart = current.AddMinutes(margin);
                    
                    // Gap po długim evencie (>=45min ciągłej pracy -> 5min przerwy)
                    int requiredGap = GetRequiredGapBefore(slotStart, scheduledEvents);
                    if (requiredGap > 0)
                    {
                        slotStart = slotStart.AddMinutes(requiredGap);
                        // Przesuń current żeby w następnej iteracji nie liczyć gapa ponownie
                        current = current.AddMinutes(requiredGap);
                    }
                    
                    var slotEnd = slotStart.AddMinutes(duration);

                    // Znajdź blokujące eventy (licząc marginesy/przerwy)
                    var blocking = GetBlockingEvents( candidate, 
                        scheduledEvents, slotStart, slotEnd,
                        excludeEventId: candidate.EventID,
                        candidateMargin: margin
                    ).ToList();

                    // 1) Jeśli nikt nie blokuje: slot musi być wolny -> wstawiamy
                    if (blocking.Count == 0)
                    {
                        if (!IsFree(candidate, scheduledEvents, slotStart, slotEnd, margin))
                        {
                            current = current.AddMinutes(15);
                            continue;
                        }

                        candidate.StartDateTime = slotStart;
                        candidate.EndDateTime = slotEnd;
                        scheduledEvents.Add(candidate);
                        
                        // Znajdź P1 nadeptane przez candidate i przeplanuj je
                        if (candidate.Priority > 1 || candidate.IsSetEvent)
                        {
                            var overlappedP1 = scheduledEvents
                                .Where(e => e.Priority == 1 && !e.IsSetEvent &&
                                       e.StartDateTime.HasValue && e.EndDateTime.HasValue &&
                                       !ReferenceEquals(e, candidate) && Overlaps(e, candidate))
                                .ToList();
                                
                            if (overlappedP1.Count > 0)
                            {
                                foreach (var p1 in overlappedP1)
                                    scheduledEvents.Remove(p1);
                                RescheduleDisplacedEvents(overlappedP1, scheduledEvents, scheduleStart, scheduleEnd);
                            }
                        }

                        return true;
                    }

                    // 2) Są blokujący: możemy wypchnąć tylko niższy priorytet i nie set/promise
                    bool canDisplaceAll = blocking.All(b =>
                        !b.IsSetEvent &&
                        (
                            // P1 jest "podevent": każdy kandydat >1 może go wywłaszczyć zawsze
                            (b.Priority == 1 && candidate.Priority > 1)
                            ||
                            // Wyższy priorytet może wypychać niższy tylko jeśli bloker ma dłuższy/równy czas
                            (b.Priority < candidate.Priority && b.Duration >= candidate.Duration)
                        )
                    );



                    if (!canDisplaceAll)
                    {
                        // Skocz do końca NAJDALSZEGO blokera (zamiast +15min)
                        var maxEnd = blocking.Max(b => b.EndDateTime ?? DateTime.MinValue);
                        if (maxEnd > current)
                            current = maxEnd;
                        else
                            current = current.AddMinutes(1);
                        continue;
                    }

                    // Zapamiętaj oryginalne czasy do rollback
                    var removed = new List<(Event evt, DateTime? start, DateTime? end)>();
                    foreach (var b in blocking)
                    {
                        removed.Add((b, b.StartDateTime, b.EndDateTime));
                        scheduledEvents.Remove(b);
                    }

                    // Po usunięciu blokujących slot musi być wolny
                    if (!IsFree(candidate, scheduledEvents, slotStart, slotEnd, margin))
                    {
                        foreach (var r in removed)
                        {
                            r.evt.StartDateTime = r.start;
                            r.evt.EndDateTime = r.end;
                            scheduledEvents.Add(r.evt);
                        }

                        current = current.AddMinutes(15);
                        continue;
                    }

                    // Wstaw kandydata
                    candidate.StartDateTime = slotStart;
                    candidate.EndDateTime = slotEnd;
                    scheduledEvents.Add(candidate);

                    // Spróbuj przeplanować wypchnięte eventy używając uniwersalnej funkcji
                    var displaced = removed.Select(r => r.evt).ToList();
                    RescheduleDisplacedEvents(displaced, scheduledEvents, scheduleStart, scheduleEnd);



                    // Finalne sprawdzenie: po wszystkim candidate nadal nie może kolidować
                    if (!IsFree(candidate, scheduledEvents, candidate.StartDateTime.Value, candidate.EndDateTime.Value, margin))
                    {
                        scheduledEvents.Remove(candidate);
                        candidate.StartDateTime = null;
                        candidate.EndDateTime = null;

                        foreach (var r in removed)
                        {
                            r.evt.StartDateTime = r.start;
                            r.evt.EndDateTime = r.end;
                            scheduledEvents.Add(r.evt);
                        }

                        current = current.AddMinutes(15);
                        continue;
                    }

                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Znajdowanie slotów

        /// <summary>
        /// Pobiera listę wydarzeń które blokują dany slot czasowy.
        /// 
        /// Używa tej samej logiki co IsFree dla spójności. Różnica:
        /// - IsFree zwraca bool (czy slot jest wolny)
        /// - GetBlockingEvents zwraca LISTĘ blokerów (do wypychania)
        /// 
        /// Uwzględnia marginesy zarówno kandydata jak i istniejących eventów.
        /// </summary>
        /// <param name="candidate">Event który próbujemy zaplanować</param>
        /// <param name="scheduledEvents">Lista zaplanowanych eventów do sprawdzenia</param>
        /// <param name="slotStart">Początek sprawdzanego slota</param>
        /// <param name="slotEnd">Koniec slota</param>
        /// <param name="excludeEventId">ID eventu do pominięcia (gdy edytujemy istniejący)</param>
        /// <param name="candidateMargin">Margines kandydata w minutach</param>
        /// <returns>Lista eventów które blokują slot</returns>
        private List<Event> GetBlockingEvents(Event candidate, List<Event> scheduledEvents, DateTime slotStart, DateTime slotEnd, int excludeEventId, int candidateMargin = 0)
        {
            // Lista blokerów do zwrócenia
            var blocking = new List<Event>();
            
            // Rozszerz okno konfliktu o margines kandydata (z obu stron)
            var windowStart = slotStart.AddMinutes(-candidateMargin);
            var windowEnd = slotEnd.AddMinutes(candidateMargin);

            // Sprawdź każdy istniejący event
            foreach (var existing in scheduledEvents)
            {
                // Użyj wspólnej logiki pomijania eventów
                if (ShouldSkipEvent(candidate, existing))
                    continue;

                // Dodatkowy check dla excludeEventId (gdy edytujemy event, nie chcemy żeby blokował sam siebie)
                if (existing.EventID == excludeEventId && excludeEventId != 0)
                    continue;

                // Pobierz efektywny zakres czasowy (z marginesem) istniejącego eventu
                var (exStart, exEnd) = existing.GetEffectiveTimeRange();
              
                // Sprawdź czy zakresy się nakładają
                if (windowStart < exEnd && windowEnd > exStart)
                {
                    // Event blokuje slot - dodaj do listy
                    blocking.Add(existing);
                }
            }

            return blocking;
        }



        /// <summary>
        /// Buduje słownik interwałów energetycznych dla danego dnia.
        /// 
        /// Interwały są względne do godziny pobudki użytkownika:
        /// - Interwał 1: 0h - 1.5h po pobudce (rozbudzenie)
        /// - Interwał 2: 1.5h - 5.5h (szczyt produktywności)
        /// - Interwał 3: 5.5h - 9h (spadek po lunchu)
        /// - Interwał 4: 9h - 12.5h (drugie okno produktywności)
        /// - Interwał 5: 12.5h - sen (wieczór)
        /// </summary>
        /// <param name="day">Dzień dla którego budujemy interwały</param>
        /// <param name="wake">Godzina pobudki (TimeSpan od północy)</param>
        /// <param name="sleep">Godzina snu (TimeSpan od północy)</param>
        /// <returns>Słownik: numer interwału -> (początek, koniec)</returns>
        private Dictionary<int, (DateTime start, DateTime end)> BuildDailyIntervals(DateTime day, TimeSpan wake, TimeSpan sleep)
        {
            // Oblicz DateTime pobudki
            var wakeDt = day.Date.Add(wake);
            // Oblicz DateTime snu (uwzględnij przypadek gdy sen jest po północy)
            var sleepDt = (wake < sleep) ? day.Date.Add(sleep) : day.Date.AddDays(1).Add(sleep);

            // Buduj słownik interwałów
            var intervals = new Dictionary<int, (DateTime start, DateTime end)>();
            intervals[1] = (wakeDt, wakeDt + Interval1);
            intervals[2] = (wakeDt + Interval1, wakeDt + Interval2);
            intervals[3] = (wakeDt + Interval2, wakeDt + Interval3);
            intervals[4] = (wakeDt + Interval3, wakeDt + Interval4);
            intervals[5] = (wakeDt + Interval4, sleepDt);
            return intervals;
        }

        /// <summary>
        /// Zwraca listę preferowanych interwałów energetycznych dla danego priorytetu.
        /// 
        /// Eventy o wyższym priorytecie są planowane w interwałach wysokiej energii.
        /// Kolejność listy oznacza preferencję (pierwszy = najbardziej preferowany).
        /// </summary>
        /// <param name="priority">Priorytet eventu (1-5)</param>
        /// <returns>Lista numerów interwałów w kolejności preferencji</returns>
        private List<int> GetIntervalPreference(int priority)
        {
            return priority switch
            {
                5 => new List<int> { 2, 4, 3, 5 },
                4 => new List<int> { 2, 4 },
                3 => new List<int> { 4, 3 },
                2 => new List<int> { 3 },
                1 => new List<int> { 2, 4, 3, 5 },
                _ => new List<int> { 1, 2, 3, 4, 5 }
            };
        }


        /// <summary>
        /// Pobiera okno czasowe snu użytkownika (godzina pobudki i zaśnięcia).
        /// 
        /// Pobiera dane dla CONTEXT user (może być dziecko, nie zalogowany użytkownik).
        /// Fallback: sesja użytkownika, potem domyślne wartości (7:00 - 23:00).
        /// </summary>
        /// <returns>Tuple: (godzina pobudki, godzina snu)</returns>
        private (TimeSpan wake, TimeSpan sleep) GetSleepWindow()
        {
            // Pobierz harmonogram snu dla użytkownika kontekstowego
            try
            {
                var userService = new UserService();
                var user = userService.GetUserById(UserSession.ContextUserId);
                if (user?.SleepStart != null && user.SleepEnd != null)
                {
                    // SleepEnd = pobudka, SleepStart = zaśnięcie
                    return (user.SleepEnd.Value, user.SleepStart.Value);
                }
            }
            catch
            {
                // W przypadku błędu - użyj domyślnych z sesji
            }
            
            // Fallback: wartości z sesji lub domyślne
            var wake = UserSession.SleepEnd ?? TimeSpan.FromHours(7);
            var sleep = UserSession.SleepStart ?? TimeSpan.FromHours(23);
            return (wake, sleep);
        }

        /// <summary>
        /// Dodaje event do listy tylko jeśli jeszcze go tam nie ma.
        /// Sprawdza po referencji LUB po EventID (jeśli != 0).
        /// </summary>
        /// <param name="list">Lista do której dodajemy</param>
        /// <param name="evt">Event do dodania</param>
        private static void AddUnique(List<Event> list, Event evt)
        {
            // Sprawdź czy event już jest na liście
            if (list.Any(e =>
                ReferenceEquals(e, evt) ||
                (evt.EventID != 0 && e.EventID == evt.EventID)))
                return;

            // Dodaj jeśli nie ma
            list.Add(evt);
        }

        /// <summary>
        /// Znajduje następny potencjalny start slotu jeśli obecny jest zablokowany.
        /// 
        /// Optymalizacja: zamiast sprawdzać co 15 min, "skacze" do końca blokującego eventu.
        /// 
        /// Jeśli slot jest wolny => zwraca null (nie trzeba skakać).
        /// Jeśli blokowany => zwraca zaokrąglony koniec najpóźniejszego blockera.
        /// 
        /// WAŻNE: Sprawdza również czy slot nie wchodzi w okno snu!
        /// </summary>
        /// <param name="candidate">Event który próbujemy zaplanować</param>
        /// <param name="existingEvents">Lista istniejących eventów</param>
        /// <param name="slotStart">Początek sprawdzanego slotu</param>
        /// <param name="slotEnd">Koniec slotu</param>
        /// <param name="candidateMargin">Margines kandydata w minutach</param>
        /// <param name="blocker">OUT: event który blokuje slot (jeśli jakiś)</param>
        /// <returns>Czas do którego skoczyć lub null jeśli slot wolny</returns>
        private DateTime? GetNextStartIfBlocked(Event candidate, List<Event> existingEvents,
            DateTime slotStart, DateTime slotEnd, int candidateMargin, out Event? blocker)
        {
            blocker = null;
            // Rozszerz okno konfliktu o margines kandydata
            var windowStart = slotStart.AddMinutes(-candidateMargin);
            var windowEnd = slotEnd.AddMinutes(candidateMargin);

            // Szukaj najpóźniejszego końca blokującego eventu
            DateTime? maxBlockingEnd = null;
            Event? maxBlocker = null;

            foreach (var existing in existingEvents)
            {
                // Użyj wspólnej logiki pomijania
                if (ShouldSkipEvent(candidate, existing))
                    continue;

                // Pobierz efektywny zakres czasowy (z marginesem)
                var (exStart, exEnd) = existing.GetEffectiveTimeRange();

                // Sprawdź konflikt z oknem kandydata
                if (windowStart < exEnd && windowEnd > exStart)
                {
                    // Zapamiętaj najpóźniejszy koniec blockera
                    if (!maxBlockingEnd.HasValue || exEnd > maxBlockingEnd.Value)
                    {
                        maxBlockingEnd = exEnd;
                        maxBlocker = existing;
                    }
                }
            }

            // Jeśli brak blockerów - slot jest wolny
            if (!maxBlockingEnd.HasValue)
                return null;

            // Zwróć koniec blockera (algorytm skoczy do tego miejsca)
            blocker = maxBlocker;
            return maxBlockingEnd.Value;
        }


        /// <summary>
        /// Szuka wolnego slotu dla eventu w podanym zakresie czasowym.
        /// 
        /// ALGORYTM:
        /// 1. Zaczyna od rangeStart (lub teraz, jeśli rangeStart w przeszłości)
        /// 2. Zaokrągla do 15 min
        /// 3. Dla każdego potencjalnego slotu:
        ///    a) Sprawdź IsFree
        ///    b) Jeśli wolny - zwróć
        ///    c) Jeśli zajęty - skocz do końca blockera (optymalizacja)
        /// 4. Jeśli nie znaleziono w zakresie - zwróć null
        /// </summary>
        /// <param name="evt">Event do zaplanowania</param>
        /// <param name="existingEvents">Lista istniejących eventów</param>
        /// <param name="rangeStart">Początek zakresu szukania</param>
        /// <param name="rangeEnd">Koniec zakresu</param>
        /// <param name="minStartHint">Opcjonalna podpowiedź minimum startu</param>
        /// <returns>DateTime początku wolnego slotu lub null</returns>
        private DateTime? FindSlotInRange(Event evt, List<Event> existingEvents, DateTime rangeStart, DateTime rangeEnd, DateTime? minStartHint = null)
        {
            // Oblicz wymagany czas
            var duration = evt.Duration;
            var margin = evt.HasMargin ? evt.MarginMinutes ?? 0 : 0;
            var total = duration + (margin * 2);

            var current = rangeStart;
            var now = DateTime.Now;
            
            // KRYTYCZNE: Nigdy nie planuj eventów w przeszłości - niezależnie od dnia
            // Jeśli zakres jest całkowicie w przeszłości, zwróć null natychmiast
            if (rangeEnd <= now)
                return null;
            
            // Jeśli zakres zaczyna się przed teraz, przesuń start do teraz
            if (current < now)
            {
                current = now;
            }
            
            // Uwzględnij hint minimalnego startu (z UI) - nigdy nie wchodź przed hint
            if (minStartHint.HasValue && minStartHint.Value > current)
            {
                current = minStartHint.Value;
            }
            
            // Podwójne sprawdzenie: jeśli po wszystkich korektach, aktualny czas jest za końcem zakresu, brak slotu
            if (current >= rangeEnd)
                return null;
            
            // Zeruj sekundy
            current = new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute, 0);

            while (current.AddMinutes(total) <= rangeEnd)
            {
                var slotStart = current.AddMinutes(margin);

                // Ewentualny dodatkowy gap po długim evencie
                int requiredGap = GetRequiredGapBefore(slotStart, existingEvents);
                if (requiredGap > 0)
                {
                    slotStart = slotStart.AddMinutes(requiredGap);
                    // WAŻNE: przesuń current też, żeby w następnej iteracji nie liczyć gapa ponownie
                    current = current.AddMinutes(requiredGap);
                }

                var slotEnd = slotStart.AddMinutes(duration);

                // Jeśli po korektach nie mieści się w interwale
                if (slotEnd > rangeEnd)
                {
                    current = current.AddMinutes(15);
                    continue;
                }

                // Zamiast IsFree + current+=15 -> skaczemy do końca blokera
                var jumpTo = GetNextStartIfBlocked(evt, existingEvents, slotStart, slotEnd, margin, out var blocker);

                if (!jumpTo.HasValue)
                {
                    // Wolne
                    if (slotStart >= DateTime.Now)
                        return slotStart;

                    // Safety
                    current = current.AddMinutes(15);
                    continue;
                }

                // WAŻNE: jumpTo może być <= current w dziwnych przypadkach, więc zabezpieczenie:
                if (jumpTo.Value <= current)
                    current = current.AddMinutes(1);
                else
                    current = jumpTo.Value;
            }

            return null;
        }

        /// <summary>
        /// Sprawdza czy dwa eventy nakładają się czasowo.
        /// Uwzględnia marginesy obu eventów.
        /// </summary>
        /// <param name="a">Pierwszy event</param>
        /// <param name="b">Drugi event</param>
        /// <returns>True jeśli eventy się nakładają</returns>
        private bool Overlaps(Event a, Event b)
        {
            // Oba eventy muszą mieć czas
            if (!a.StartDateTime.HasValue || !a.EndDateTime.HasValue ||
                !b.StartDateTime.HasValue || !b.EndDateTime.HasValue)
                return false;

            // Pobierz zakresy czasowe
            var aStart = a.GetStartDateTime();
            var aEnd = a.GetEndDateTime();
            var bStart = b.GetStartDateTime();
            var bEnd = b.GetEndDateTime();

            // Rozszerz zakresy o marginesy (jeśli ustawione)
            if (a.HasMargin && a.MarginMinutes.HasValue)
            {
                aStart = aStart.AddMinutes(-a.MarginMinutes.Value);
                aEnd = aEnd.AddMinutes(a.MarginMinutes.Value);
            }
            if (b.HasMargin && b.MarginMinutes.HasValue)
            {
                bStart = bStart.AddMinutes(-b.MarginMinutes.Value);
                bEnd = bEnd.AddMinutes(b.MarginMinutes.Value);
            }

            // Sprawdź nakładanie: A zaczyna przed końcem B ORAZ A kończy po początku B
            return aStart < bEnd && aEnd > bStart;
        }

        /// <summary>
        /// Sprawdza czy slot jest wolny dla danego kandydata.
        /// Deleguje do GetBlockingEvents() i sprawdza czy lista jest pusta.
        /// </summary>
        /// <param name="candidate">Event który próbujemy zaplanować</param>
        /// <param name="existingEvents">Lista istniejących eventów</param>
        /// <param name="start">Początek slotu</param>
        /// <param name="end">Koniec slotu</param>
        /// <param name="margin">Margines kandydata w minutach</param>
        /// <param name="excludeEventId">ID eventu do pominięcia (domyślnie 0)</param>
        /// <returns>True jeśli slot jest wolny</returns>
        private bool IsFree(Event candidate, List<Event> existingEvents, DateTime start, DateTime end, int margin, int excludeEventId = 0)
        {
            return GetBlockingEvents(candidate, existingEvents, start, end, excludeEventId, margin).Count == 0;
        }

        #endregion

        #region Metody pomocnicze

        /// <summary>
        /// Oblicza wymagany gap przed slotem na podstawie poprzedzających eventów.
        /// </summary>
        private int GetRequiredGapBefore(DateTime slotStart, List<Event> existingEvents)
        {
            // Szukaj ciągłego "streaka" eventów bez przerwy przed slotStart.
            // Jeśli całkowity czas streaka >= 45 min -> wymagaj 5 min przerwy po zakończeniu streaka.

            // Pobierz eventy z czasem, posortowane po końcu
            var timed = existingEvents
                .Where(e => e.StartDateTime.HasValue && e.EndDateTime.HasValue)
                .Select(e =>
                {
                    var (start, end) = e.GetEffectiveTimeRange();
                    return new { e, start, end };
                })
                .OrderBy(x => x.end)
                .ToList();

            // Znajdź event, który kończy się NAJBLIŻEJ przed slotStart (<= slotStart)
            var last = timed.LastOrDefault(x => x.end <= slotStart);
            if (last == null)
                return 0;

            // Budujemy streak wstecz: dopóki kolejne eventy "stykają się" (brak wolnego czasu)
            // Przyjmujemy tolerancję 0..1 min dla bezpieczeństwa danych
            const int glueToleranceMinutes = 1;

            DateTime streakStart = last.start;
            DateTime streakEnd = last.end;
            int streakMinutes = (int)(streakEnd - streakStart).TotalMinutes;

            int i = timed.IndexOf(last) - 1;
            while (i >= 0)
            {
                var prev = timed[i];

                // Czy prev kończy się "praktycznie" w momencie startu aktualnego streaka? (brak przerwy)
                var gap = (streakStart - prev.end).TotalMinutes;
                if (gap > glueToleranceMinutes)
                    break;

                // Doklejamy prev do streaka
                streakStart = prev.start;
                streakMinutes = (int)(streakEnd - streakStart).TotalMinutes;

                i--;
            }

            if (streakMinutes < 45)
                return 0;

            // Streak >=45 -> wymagamy 5 min przerwy po jego końcu
            var requiredStart = streakEnd.AddMinutes(5);

            if (slotStart >= requiredStart)
                return 0;

            var result = (int)(requiredStart - slotStart).TotalMinutes;
            return result;
        }

        #endregion

        #region Post-processing (cykliczne, odraczanie)

        /// <summary>
        /// Przetwarza eventy cykliczne (recurring) - tworzy kopie dla kolejnych wystąpień.
        /// 
        /// Dla każdego eventu z IsRecurring = true i RecurrenceDays ustawionym:
        /// 1. Zaczyna od następnego wystąpienia (start + RecurrenceDays)
        /// 2. Tworzy kopie eventu dla każdego wystąpienia w oknie planowania
        /// 3. Pomija wykluczone dni tygodnia (DontDoOnDayOfWeek)
        /// 
        /// UWAGA: Tworzy tylko "głupie" kopie - bez sprawdzania konfliktów.
        /// Konflikt rozwiązuje się przy następnym uruchomieniu algorytmu.
        /// </summary>
        /// <param name="events">Lista wydarzeń do przetworzenia</param>
        /// <param name="startDate">Początek okna planowania</param>
        /// <param name="endDate">Koniec okna planowania</param>
        /// <returns>Lista z dodanymi kopiami eventów cyklicznych</returns>
        private List<Event> ProcessRecurringEvents(List<Event> events, DateTime startDate, DateTime endDate)
        {
            // Stwórz nową listę z oryginalnymi eventami
            var newEvents = new List<Event>(events);
            // Znajdź eventy cykliczne z ustawionym interwałem
            var recurringEvents = events.Where(e => e.IsRecurring && e.RecurrenceDays.HasValue).ToList();
            
            foreach (var recurring in recurringEvents)
            {
                if (!recurring.StartDateTime.HasValue)
                    continue;
                
                var currentDate = recurring.GetStartDateTime().AddDays(recurring.RecurrenceDays!.Value);
                
                while (currentDate <= endDate)
                {
                    
                    var newEvent = new Event
                    {
                        Title = recurring.Title,
                        Description = recurring.Description,
                        StartDateTime = currentDate,
                        Duration = recurring.Duration,
                        CategoryCode = recurring.CategoryCode,
                        Category = Event.GetCategoryName(recurring.CategoryCode),
                    
                        Priority = recurring.Priority,
                        IsImportant = recurring.IsImportant,
                        IsUrgent = recurring.IsUrgent,
                        IsSetEvent = recurring.IsSetEvent,
                
                        DoInDaytimeUntil = recurring.DoInDaytimeUntil,
                        DoInDaytimeUntilTime = recurring.DoInDaytimeUntilTime,
          
                        HasMargin = recurring.HasMargin,
                        MarginMinutes = recurring.MarginMinutes,
              
                        IsRecurring = true,
                        RecurrenceDays = recurring.RecurrenceDays,
                        TemplateID = recurring.TemplateID
                    };
                    
                    newEvent.EndDateTime = newEvent.GetEndDateTime();
                    
                    // Sprawdź czy slot jest wolny przed dodaniem cyklicznego eventu
                    int margin = newEvent.HasMargin ? newEvent.MarginMinutes ?? 0 : 0;
                    if (IsFree(newEvent, newEvents, newEvent.GetStartDateTime(), newEvent.GetEndDateTime(), margin))
                    {
                        newEvents.Add(newEvent);
                    }
                    // Jeśli nie wolne, pomiń to wystąpienie (nie dodawaj nakładającego się eventu)
                    
                    currentDate = currentDate.AddDays(recurring.RecurrenceDays.Value);
                }
            }
            
            return newEvents;
        }

       

        /// <summary>
        /// Odkłada nieukończone eventy które minęły o ponad 24 godziny.
        /// 
        /// ALGORYTM:
        /// 1. Dla każdego nieukończonego eventu:
        ///    a) Sprawdź czy minęło 24h od planowanego KOŃCA
        ///    b) Jeśli tak i event może być przesunięty - wyczyść czas
        ///    c) Dodaj do listy do przeplanowania
        /// 2. Przeplanuj wszystkie odłożone eventy na następne 7 dni
        /// 
        /// UWAGA: Stałe eventy (SetEvent) i obietnice nie są odkładane.
        /// </summary>
        /// <param name="events">Lista wydarzeń do przetworzenia</param>
        /// <returns>Lista z przeplanowanymi eventami</returns>
        private List<Event> PostponeIncompleteEvents(List<Event> events)
        {
            var now = DateTime.Now;
            // Eventy które nie wymagają przeplanowania
            var updatedEvents = new List<Event>();
            // Eventy do przeplanowania
            var eventsToReschedule = new List<Event>();
            
            foreach (var evt in events)
            {
                // Ukończone eventy - pozostaw bez zmian
                if (evt.IsCompleted)
                {
                    updatedEvents.Add(evt);
                    continue;
                }
                
                // Sprawdź czy czas KOŃCA eventu minął i nie jest ukończony (24h po planowanym końcu)
                if (evt.EndDateTime.HasValue)
                {
                    var eventEndTime = evt.GetEndDateTime();
                    var hoursSinceEnd = (now - eventEndTime).TotalHours;
                    
                    // Jeśli minęło 24 godzin od planowanego czasu KOŃCA i event nie jest ukończony
                    if (hoursSinceEnd >= 24)
                    {
                        // Odłóż event jeśli może być odłożony (nie jest stałym set-event ani obietnicą)
                        if (evt.CanBePostponed())
                        {
                            // Jeśli event ma deadline który już minął - USUŃ go (nie przeplanowuj)
                            if (evt.Deadline.HasValue && evt.Deadline.Value < now)
                            {
                                // Nie dodajemy do żadnej listy = event znika
                                continue;
                            }
                            
                            // Wyczyść stary czas żeby algorytm mógł przeplanować
                            evt.StartDateTime = null;
                            evt.EndDateTime = null;
                            
                            // Dodaj do listy do przeplanowania
                            eventsToReschedule.Add(evt);
                        }
                        else
                        {
                            // Nie można odłożyć (stały event lub obietnica) - zostaw jak jest ale oznacz do uwagi
                            updatedEvents.Add(evt);
                        }
                    }
                    else
                    {
                        updatedEvents.Add(evt);
                    }
                }
                else if (evt.StartDateTime.HasValue)
                {
                    // Ma start ale nie ma end - sprawdź czy 24h minęło od szacowanego końca (start + duration)
                    var estimatedEnd = evt.StartDateTime.Value.AddMinutes(evt.Duration);
                    var hoursSinceEnd = (now - estimatedEnd).TotalHours;
                    
                    if (hoursSinceEnd >= 24 && evt.CanBePostponed())
                    {
                        // Jeśli event ma deadline który już minął - USUŃ go
                        if (evt.Deadline.HasValue && evt.Deadline.Value < now)
                        {
                            continue;
                        }
                        
                        evt.StartDateTime = null;
                        evt.EndDateTime = null;
                        eventsToReschedule.Add(evt);
                    }
                    else
                    {
                        updatedEvents.Add(evt);
                    }
                }
                else
                {
                    // Brak ustawionego czasu start/end - dodaj jak jest
                    updatedEvents.Add(evt);
                }
            }
            
            foreach (var evt in eventsToReschedule)
            {
                evt.CalculatePriority(now.Date);
            }
            
            var sortedToReschedule = eventsToReschedule
                .OrderByDescending(e => e.Priority)
                .ThenBy(e => e.Deadline ?? DateTime.MaxValue)
                .ToList();
            
            foreach (var evt in sortedToReschedule)
            {
                // Znajdź nowy slot czasowy zaczynając od teraz, szukając do 14 dni do przodu
                var scheduleStart = now;
                var scheduleEnd = now.AddDays(14);
                
                // Użyj nowej metody displacement
                ScheduleFlexibleEventWithDisplacement(evt, updatedEvents, scheduleStart, scheduleEnd);
                
                // Event jest dodawany do updatedEvents wewnątrz metody jeśli pomyślnie zaplanowany
                // Jeśli nie zaplanowany, dodaj z powrotem bez czasu
                if (!evt.StartDateTime.HasValue)
                {
                    evt.ModifiedDate = now;
                    if (!updatedEvents.Contains(evt))
                        updatedEvents.Add(evt);
                }
                else
                {
                    evt.ModifiedDate = now;
                }
            }
            
            return updatedEvents;
        }

        #endregion
    }
}
