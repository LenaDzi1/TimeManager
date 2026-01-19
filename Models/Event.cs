// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET

namespace TimeManager.Models
{
    /// <summary>
    /// Model eventu (zadania/aktywności) w systemie TimeManager.
    /// 
    /// Event może być:
    /// - Stały (SetEvent=true): nie może być przesunięty przez algorytm
    /// - Elastyczny: algorytm może go zaplanować i przesuwać
    /// - Cykliczny (IsRecurring=true): powtarza się co X dni
    /// 
    /// Priority obliczany jest przez algorytm na podstawie:
    /// - Deadline (im bliżej, tym wyższy priorytet)
    /// - AtLeast (minimum X razy w Y dni)
    /// - Kategorii (niektóre mają wyższy priorytet bazowy)
    /// </summary>
    public class Event
    {
        // Identyfikator w bazie danych
        public int EventID { get; set; }
        // Tytuł eventu
        public string Title { get; set; }
        // Opis eventu
        public string Description { get; set; }
        // Data i czas rozpoczęcia (nullable - może być ustawione przez algorytm)
        public DateTime? StartDateTime { get; set; }
        // Data i czas zakończenia
        public DateTime? EndDateTime { get; set; }
        // Data zakończenia (osobno od czasu)
        public DateTime? EndDate { get; set; }
        // Czas zakończenia (osobno od daty)
        public DateTime? EndTime { get; set; }
        // Czas trwania w minutach
        public int Duration { get; set; }
        // Priorytet obliczony przez algorytm (domyślnie 1)
        public int Priority { get; set; }
        // Kod koloru (hex, np. "#FF5733")
        public string ColorCode => GetDefaultColorForCategory(CategoryCode);
        // Kod kategorii (0=brak, 1-8 dla predefiniowanych kategorii)
        public int CategoryCode { get; set; }
        // Nazwa kategorii (do wyświetlania)
        public string Category { get; set; }
        // Czy musi być wykonany w dzień do określonej godziny
        public bool DoInDaytimeUntil { get; set; }
        // Godzina do której event powinien być wykonany
        public DateTime? DoInDaytimeUntilTime { get; set; }
        public int? TemplateID { get; set; }
        // Czy event został ukończony
        public bool IsCompleted { get; set; }
        // Czy event jest cykliczny
        public bool IsRecurring { get; set; }
        // Co ile dni się powtarza (dla cyklicznych)
        public int? RecurrenceDays { get; set; }

        // Data utworzenia
        public DateTime CreatedDate { get; set; }
        // Data ostatniej modyfikacji
        public DateTime ModifiedDate { get; set; }
        // Deadline - termin do którego event musi być wykonany
        public DateTime? Deadline { get; set; }
        
        // === Flagi opcji ===
        // Czy ma margines czasowy
        public bool HasMargin { get; set; }
        // Margines w minutach (+/-)
        public int? MarginMinutes { get; set; }
        // Czy jest ważny (tylko do kalkulacji priorytetu)
        public bool IsImportant { get; set; }
        // Czy jest pilny (tylko do kalkulacji priorytetu)
        public bool IsUrgent { get; set; }
        // Czy jest stały (nie może być przesunięty - praca, spotkania)
        public bool IsSetEvent { get; set; }

        /// <summary>
        /// Konstruktor domyślny - ustawia wartości początkowe.
        /// </summary>
        public Event()
        {
            StartDateTime = DateTime.Now;
            Duration = 60; // domyślnie 1 godzina
            Priority = 1; // domyślny priorytet

            CategoryCode = 0;
            Category = GetCategoryName(CategoryCode);
            IsCompleted = false;
            IsRecurring = false;
        }

        public static string GetCategoryName(int code)
        {
            return code switch
            {
                1 => "health",
                2 => "family",
                3 => "mentality",
                4 => "finance",
                5 => "work and career",
                6 => "relax",
                7 => "self development and education",
                8 => "friends and people",
                _ => "None",
            };
        }

        public static string GetDefaultColorForCategory(int code)
        {
            // Kolory zsynchronizowane z wykresem (StatisticsView)
            switch (code)
            {
                case 1: return "#9DE284"; // zdrowie
                case 2: return "#ECAA54"; // rodzina (żółty)
                case 3: return "#94E5F3"; // mentalność
                case 4: return "#839EF6"; // finanse (turkus)
                case 5: return "#FF7676"; // praca i kariera (granat)
                case 6: return "#FFE374"; // relaks (pomarańczowy)
                case 7: return "#C898DB"; // rozwój osobisty i edukacja
                case 8: return "#EA6591"; // przyjaciele i ludzie (magenta)
                default: return "#9CACAE"; // brak/szary
            }
        }

        public DateTime GetEndDateTime()
        {
            if (EndDateTime.HasValue)
                return EndDateTime.Value;
            
            if (EndDate.HasValue && EndTime.HasValue)
                return EndDate.Value.Date.Add(EndTime.Value.TimeOfDay);
            
            if (StartDateTime.HasValue)
                return StartDateTime.Value.AddMinutes(Duration);
            
            return DateTime.Now.AddMinutes(Duration);
        }
        
        public DateTime GetStartDateTime()
        {
            return StartDateTime ?? DateTime.Now;
        }

        /// <summary>
        /// Zwraca efektywny zakres czasowy z zastosowanym marginesem.
        /// </summary>
        public (DateTime start, DateTime end) GetEffectiveTimeRange()
        {
            var start = GetStartDateTime();
            var end = GetEndDateTime();
            if (HasMargin && MarginMinutes.HasValue)
            {
                start = start.AddMinutes(-MarginMinutes.Value);
                end = end.AddMinutes(MarginMinutes.Value);
            }
            return (start, end);
        }

        public int CalculatePriority()
        {
            // Domyślne zachowanie dla "teraz"
            return CalculatePriority(DateTime.Now);
        }

        public int CalculatePriority(DateTime referenceDate)
        {
            // Stałe eventy zachowują priorytet 1 ale są obsługiwane osobno przez scheduler
            if (IsSetEvent)
            {
                Priority = 1;
                return Priority;
            }

            // 1) Bazowy priorytet:
            // - jeśli user ustawił ręcznie P2-P4 i nie ma flag (ważne/obietnica/pilne),
            //   to to traktujemy jako "manualny priorytet"
            // - inaczej wyliczamy z flag jak wcześniej
            bool hasAnyFlag = IsImportant || IsUrgent;

            int priority;
            if (!hasAnyFlag && Priority >= 2 && Priority <= 4)
            {
                // ręczny priorytet z UI/DB
                priority = Priority;
            }
            else
            {
                priority = 1; 
                if (IsImportant) priority += 2;
                if (IsUrgent) priority += 1;

                // Ograniczenie do 4 dla normalnego przepływu
                priority = Math.Min(priority, 4);
            }

            // 2) Deadline ma NAJWIĘKSZY priorytet w kontekście planowania:
            //    jeśli dzień planowania (referenceDate) jest w dniu deadlinu lub dzień przed,
            //    ustawiamy P5 niezależnie od ręcznego P2-P4.
            if (Deadline.HasValue)
            {
                // Deadline traktujemy jako KONIEC DNIA (więc liczymy po samych datach)
                var deadlineDate = Deadline.Value.Date;
                var baseDate = referenceDate.Date;

                var daysUntilDeadline = (deadlineDate - baseDate).Days;

                if (daysUntilDeadline <= 1)
                {
                    priority = 5;
                }
            }

            Priority = priority;
            return priority;
        }
        
        /// <summary>
        /// Określa, czy wydarzenie może zostać przełożone na inny termin.
        /// Wydarzenie może być przełożone, jeśli nie jest wydarzeniem stałym (SetEvent).
        /// </summary>
        /// <returns>True jeśli wydarzenie może zostać przełożone, false w przeciwnym razie.</returns>
        public bool CanBePostponed()
        {
            return !IsSetEvent;
        }

    }
}






