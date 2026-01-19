// ============================================================================
// HabitModels.cs
// Modele nawyków: kategorie, kroki i rekordy ukończenia.
// ============================================================================

#region Importy
using System;                   // Podstawowe typy (DateTime)
#endregion

namespace TimeManager.Models
{
    #region HabitCategory - Kategoria nawyków

    /// <summary>
    /// Kategoria nawyków użytkownika (np. Zdrowie, Praca, Sport).
    /// Grupuje powiązane kroki nawyków.
    /// </summary>
    public class HabitCategory
    {
        /// <summary>
        /// Unikalny identyfikator kategorii w bazie danych.
        /// Generowany automatycznie przez SQL Server (IDENTITY).
        /// </summary>
        public int HabitCategoryId { get; set; }

        /// <summary>
        /// ID użytkownika - właściciela kategorii.
        /// Każdy użytkownik może mieć własne kategorie.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nazwa kategorii (np. "Zdrowie", "Praca", "Sport").
        /// Wyświetlana jako zakładka w UI.
        /// </summary>
        public string Name { get; set; }
    }

    #endregion

    #region HabitStep - Krok nawyku

    /// <summary>
    /// Krok nawyku - konkretna aktywność do wykonania.
    /// Może mieć zakres dat, powtarzalność i punkty za wykonanie.
    /// </summary>
    public class HabitStep
    {
        /// <summary>
        /// Unikalny identyfikator kroku w bazie danych.
        /// Generowany automatycznie przez SQL Server (IDENTITY).
        /// </summary>
        public int HabitStepId { get; set; }

        /// <summary>
        /// ID kategorii nawyków, do której należy krok.
        /// Łączy krok z kategorią (FK).
        /// </summary>
        public int HabitCategoryId { get; set; }

        /// <summary>
        /// ID użytkownika - właściciela kroku.
        /// Denormalizowane dla szybkiego filtrowania.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nazwa kroku (np. "Ćwiczenia poranne", "Picie wody").
        /// Wyświetlana jako karta w UI.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Data rozpoczęcia śledzenia nawyku (opcjonalna).
        /// Null = aktywny od zawsze.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Data zakończenia śledzenia nawyku (opcjonalna).
        /// Null = aktywny na zawsze.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Powtarzaj co X dni (opcjonalne).
        /// Null = powtarzaj codziennie.
        /// Przykład: 7 = co tydzień.
        /// </summary>
        public int? RepeatEveryDays { get; set; }

        /// <summary>
        /// Opis kroku (opcjonalny).
        /// Dodatkowe szczegóły o nawyku.
        /// </summary>
        public string Description { get; set; }



        /// <summary>
        /// Czy podzielić wykonanie na rano/wieczór.
        /// True = dwa checkboxy (Day + Night).
        /// False = jeden checkbox (cały dzień).
        /// </summary>
        public bool UseDaytimeSplit { get; set; }

        /// <summary>
        /// Szacowana liczba wykonań (opcjonalna).
        /// Używana do śledzenia postępu.
        /// </summary>
        public int? EstimatedOccurrences { get; set; }

        /// <summary>
        /// Pozostałe wykonania (opcjonalne).
        /// Odliczane przy każdym ukończeniu.
        /// </summary>
        public int? RemainingOccurrences { get; set; }

        /// <summary>
        /// Punkty za wykonanie (opcjonalne).
        /// Nagroda dodawana do konta użytkownika.
        /// </summary>
        public int? PointsReward { get; set; }
    }

    #endregion

    #region HabitCompletion - Rekord ukończenia

    /// <summary>
    /// Rekord ukończenia kroku nawyku.
    /// Zapisuje kiedy i jaka część (rano/wieczór) została wykonana.
    /// </summary>
    public class HabitCompletion
    {
        /// <summary>
        /// ID kroku nawyku, który został ukończony.
        /// Łączy rekord z krokiem (FK).
        /// </summary>
        public int HabitStepId { get; set; }

        /// <summary>
        /// Data ukończenia (tylko data, bez czasu).
        /// Używana do sprawdzenia czy nawyk wykonany danego dnia.
        /// </summary>
        public DateTime CompletionDate { get; set; }

        /// <summary>
        /// Część dnia: null/"" = cały dzień, "Day" = rano, "Night" = wieczór.
        /// Używane gdy UseDaytimeSplit = true.
        /// </summary>
        public string Part { get; set; }
    }

    #endregion
}
