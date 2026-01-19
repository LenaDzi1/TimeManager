// ============================================================================
// HomeLayoutModels.cs
// Modele układu domu: layout, piętra, ściany, kontenery i przedmioty.
// ============================================================================

#nullable enable

#region Importy
using System.Collections.Generic; // Kolekcje (List)
#endregion

namespace TimeManager.Models
{
    #region LayoutModel - Główny model layoutu

    /// <summary>
    /// Główny model layoutu domu.
    /// Przechowuje wymiary domyślne i listę pięter.
    /// </summary>
    public class LayoutModel
    {
        /// <summary>
        /// Unikalny identyfikator layoutu w bazie danych.
        /// Generowany automatycznie przez SQL Server (IDENTITY).
        /// </summary>
        public int LayoutId { get; set; }

        /// <summary>
        /// Domyślna szerokość domu w metrach.
        /// Używana jako wartość początkowa dla nowych pięter.
        /// </summary>
        public decimal DefaultWidthMeters { get; set; }

        /// <summary>
        /// Domyślna wysokość domu w metrach.
        /// Używana jako wartość początkowa dla nowych pięter.
        /// </summary>
        public decimal DefaultHeightMeters { get; set; }

        /// <summary>
        /// Lista pięter w domu.
        /// Każde piętro ma własne ściany i kontenery.
        /// </summary>
        public List<FloorModel> Floors { get; set; } = new();
    }

    #endregion

    #region FloorModel - Model piętra

    /// <summary>
    /// Model piętra w domu.
    /// Zawiera wymiary, ściany i kontenery.
    /// </summary>
    public class FloorModel
    {
        /// <summary>
        /// Unikalny identyfikator piętra w bazie danych.
        /// Generowany automatycznie przez SQL Server (IDENTITY).
        /// </summary>
        public int FloorId { get; set; }

        /// <summary>
        /// Numer piętra (0 = parter, 1 = pierwsze piętro, itd.).
        /// Używany do sortowania i wyświetlania.
        /// </summary>
        public int FloorNumber { get; set; }

        /// <summary>
        /// Opcjonalna nazwa piętra (np. "Parter", "Strych").
        /// Jeśli null, wyświetlany jest "Floor {FloorNumber + 1}".
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Szerokość piętra w metrach.
        /// Określa skalę rysowania na canvas.
        /// </summary>
        public decimal WidthMeters { get; set; }

        /// <summary>
        /// Wysokość piętra w metrach.
        /// Określa skalę rysowania na canvas.
        /// </summary>
        public decimal HeightMeters { get; set; }

        /// <summary>
        /// Lista ścian na piętrze.
        /// Ściany są rysowane jako linie na canvas.
        /// </summary>
        public List<WallModel> Walls { get; set; } = new();

        /// <summary>
        /// Lista kontenerów na piętrze.
        /// Kontenery przechowują przedmioty i mogą mieć podkontenery.
        /// </summary>
        public List<ContainerModel> Containers { get; set; } = new();
    }

    #endregion

    #region WallModel - Model ściany

    /// <summary>
    /// Model ściany (linia na planie piętra).
    /// Definiowana przez punkt początkowy i końcowy.
    /// </summary>
    public class WallModel
    {
        /// <summary>
        /// Unikalny identyfikator ściany w bazie danych.
        /// Generowany automatycznie przez SQL Server (IDENTITY).
        /// </summary>
        public int WallId { get; set; }

        /// <summary>
        /// Współrzędna X punktu początkowego ściany.
        /// Wartość w metrach (0 = lewa krawędź piętra).
        /// </summary>
        public float StartX { get; set; }

        /// <summary>
        /// Współrzędna Y punktu początkowego ściany.
        /// Wartość w metrach (0 = górna krawędź piętra).
        /// </summary>
        public float StartY { get; set; }

        /// <summary>
        /// Współrzędna X punktu końcowego ściany.
        /// Wartość w metrach.
        /// </summary>
        public float EndX { get; set; }

        /// <summary>
        /// Współrzędna Y punktu końcowego ściany.
        /// Wartość w metrach.
        /// </summary>
        public float EndY { get; set; }
    }

    #endregion

    #region ContainerModel - Model kontenera

    /// <summary>
    /// Model kontenera (np. szafka, półka, lodówka).
    /// Kontener jest prostokątem na planie i może zawierać przedmioty.
    /// </summary>
    public class ContainerModel
    {
        /// <summary>
        /// Unikalny identyfikator kontenera w bazie danych.
        /// Generowany automatycznie przez SQL Server (IDENTITY).
        /// Wartości ujemne = tymczasowe ID (przed zapisem do DB).
        /// </summary>
        public int ContainerId { get; set; }

        /// <summary>
        /// Nazwa kontenera (np. "Lodówka kuchenna", "Szafa główna").
        /// Wyświetlana w UI i wynikach wyszukiwania.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Opcjonalny tag pokoju (np. "Kuchnia", "Sypialnia").
        /// Używany do grupowania kontenerów.
        /// </summary>
        public string? RoomTag { get; set; }

        /// <summary>
        /// Współrzędna X lewego górnego rogu kontenera (punkt A).
        /// Wartość w metrach.
        /// </summary>
        public float PointAX { get; set; }

        /// <summary>
        /// Współrzędna Y lewego górnego rogu kontenera (punkt A).
        /// Wartość w metrach.
        /// </summary>
        public float PointAY { get; set; }

        /// <summary>
        /// Współrzędna X prawego dolnego rogu kontenera (punkt B).
        /// Wartość w metrach.
        /// </summary>
        public float PointBX { get; set; }

        /// <summary>
        /// Współrzędna Y prawego dolnego rogu kontenera (punkt B).
        /// Wartość w metrach.
        /// </summary>
        public float PointBY { get; set; }

        /// <summary>
        /// Czy kontener jest widoczny na planie.
        /// False = ukryty (np. schowany pod innym).
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// ID kontenera nadrzędnego (null = kontener główny).
        /// Pozwala na hierarchię: szafka -> półka -> pudełko.
        /// </summary>
        public int? ParentContainerId { get; set; }

        /// <summary>
        /// Lista przedmiotów w kontenerze.
        /// Każdy przedmiot ma nazwę, ilość i jednostkę.
        /// </summary>
        public List<ItemInventoryModel> Items { get; set; } = new();
    }

    #endregion

    #region ItemInventoryModel - Model przedmiotu

    /// <summary>
    /// Model przedmiotu w inwentarzu kontenera.
    /// Przechowuje informacje o lokalizacji i ilości.
    /// </summary>
    public class ItemInventoryModel
    {
        /// <summary>
        /// Unikalny identyfikator rekordu inwentarza w bazie danych.
        /// Generowany automatycznie przez SQL Server (IDENTITY).
        /// </summary>
        public int InventoryId { get; set; }

        /// <summary>
        /// ID przedmiotu z tabeli Items.
        /// Łączy inwentarz z definicją przedmiotu.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Nazwa przedmiotu (np. "Mleko", "Książka").
        /// Kopiowana z tabeli Items dla szybkiego dostępu.
        /// </summary>
        public string ItemName { get; set; } = string.Empty;

        /// <summary>
        /// Ilość przedmiotu w kontenerze.
        /// Może być ułamkowa (np. 0.5 kg).
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Jednostka miary (np. "pcs", "kg", "l").
        /// Domyślnie "pcs" (sztuki).
        /// </summary>
        public string Unit { get; set; } = "pcs";

        /// <summary>
        /// Nazwa kontenera (denormalizowana dla wyszukiwania).
        /// Pozwala wyświetlić lokalizację bez JOIN-a.
        /// </summary>
        public string? ContainerName { get; set; }

        /// <summary>
        /// Numer piętra (denormalizowany dla wyszukiwania).
        /// Pozwala wyświetlić lokalizację bez JOIN-a.
        /// </summary>
        public int FloorNumber { get; set; }

        /// <summary>
        /// Tag pokoju (denormalizowany dla wyszukiwania).
        /// Pozwala wyświetlić lokalizację bez JOIN-a.
        /// </summary>
        public string? RoomTag { get; set; }

        /// <summary>
        /// ID kontenera zawierającego przedmiot.
        /// Używany do nawigacji do kontenera.
        /// </summary>
        public int ContainerId { get; set; }
    }

    #endregion
}











