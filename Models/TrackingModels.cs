// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Collections.Generic; // Kolekcje generyczne

namespace TimeManager.Models
{
    /// <summary>
    /// Modele do śledzenia: jedzenie, leki, rośliny, przepisy.
    /// </summary>

    /// <summary>
    /// Model produktu spożywczego w lodówce/spiżarni.
    /// </summary>
    public class FridgeItem
    {
        // ID produktu
        public int ItemID { get; set; }
        // Nazwa produktu
        public string Name { get; set; }
        // Ilość
        public decimal Quantity { get; set; }
        // Jednostka (szt, kg, l, itp.)
        public string Unit { get; set; }
        // Data ważności
        public DateTime? ExpirationDate { get; set; }

        // Data dodania
        public DateTime AddedDate { get; set; }
        // Data ostatniej modyfikacji
        public DateTime LastModified { get; set; }

        // Opcjonalne referencje do kontenerów/produktów
        public int? ContainerID { get; set; }
        public int? ProductID { get; set; }

        /// <summary>
        /// Sprawdza czy produkt wkrótce traci ważność (3 dni).
        /// </summary>
        public bool IsExpiringSoon()
        {
            if (ExpirationDate.HasValue)
            {
                return ExpirationDate.Value <= DateTime.Now.AddDays(3) && !IsExpired();
            }
            return false;
        }

        public bool IsExpired()
        {
            if (ExpirationDate.HasValue)
            {
                return ExpirationDate.Value < DateTime.Now;
            }
            return false;
        }

        public string ExpireState
        {
            get
            {
                if (IsExpired()) return "Expired";
                if (IsExpiringSoon()) return "About to expire";
                return "Fresh";
            }
        }
    }

    public class FoodContainer
    {
        public int ContainerID { get; set; }
        public string Name { get; set; }   
        public string Type { get; set; }   
    }

    public class FoodProduct
    {
        public int ProductID { get; set; }
        public string Name { get; set; }          
        public string DefaultUnit { get; set; }   
    }



    public class Plant
    {
        public int PlantID { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public int WateringFrequency { get; set; }
        public DateTime? LastWateredDate { get; set; }
        public DateTime? NextWateringDate { get; set; }
        public DateTime AddedDate { get; set; }

        public bool NeedsWatering()
        {
            if (NextWateringDate.HasValue)
            {
                return NextWateringDate.Value <= DateTime.Now;
            }
            return true;
        }

        public void UpdateWateringSchedule()
        {
            LastWateredDate = DateTime.Now;
            NextWateringDate = DateTime.Now.AddDays(WateringFrequency);
        }
    }

    public class MedicineSchedule
    {
        public int ScheduleID { get; set; }
        public int MedicineItemID { get; set; }
        public string MedicineName { get; set; }
        public decimal? MorningDose { get; set; }
        public decimal? EveningDose { get; set; }
        public int IntervalDays { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class MedicineIntakeStatus
    {
        public int StatusID { get; set; }
        public int ScheduleID { get; set; }
        public DateTime Date { get; set; }
        public bool MorningTaken { get; set; }
        public bool EveningTaken { get; set; }
    }

    public class Notification
    {
        public int NotificationID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public int? ReferenceID { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public bool IsSent { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class FocusSession
    {
        public int SessionID { get; set; }
        public int? EventID { get; set; }
        public DateTime StartTime { get; set; }
        public int PlannedDuration { get; set; }
        public DateTime? ActualEndTime { get; set; }


        public bool IsActive()
        {
            return !ActualEndTime.HasValue;
        }

        public TimeSpan GetRemainingTime()
        {
            var plannedEnd = StartTime.AddMinutes(PlannedDuration);
            var remaining = plannedEnd - DateTime.Now;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    public class Recipe
    {
        public int RecipeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PreparationTimeMinutes { get; set; }
        public int NumberOfPortions { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsScheduled { get; set; }
        public List<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
    }

    public class RecipeIngredient
    {
        public int RecipeIngredientID { get; set; }
        public int RecipeID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; } 
        public decimal Amount { get; set; }
        public string Unit { get; set; }
    }

    public class MedicineItem
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public DateTime AddedDate { get; set; }
        public DateTime LastModified { get; set; }
        public int? ContainerID { get; set; }
        public int? ProductID { get; set; }

        public bool IsExpiringSoon()
        {
            if (ExpirationDate.HasValue)
            {
                return ExpirationDate.Value <= DateTime.Now.AddDays(30) && !IsExpired();
            }
            return false;
        }

        public bool IsExpired()
        {
            if (ExpirationDate.HasValue)
            {
                return ExpirationDate.Value < DateTime.Now;
            }
            return false;
        }

        public string ExpireState
        {
            get
            {
                if (IsExpired()) return "Expired";
                if (IsExpiringSoon()) return "Expiring soon";
                return "Valid";
            }
        }
    }

    public class MedicineContainer
    {
        public int ContainerID { get; set; }
        public string Name { get; set; }
    }

    public class MedicineProduct
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string DefaultUnit { get; set; }
    }

    public class ShoppingListItem
    {
        public int ShoppingItemID { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
        public int? RecipeID { get; set; }
        public DateTime AddedDate { get; set; }

    }
}
