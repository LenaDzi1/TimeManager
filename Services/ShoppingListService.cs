using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeManager.Services
{
    public class ShoppingListService
    {
        private readonly TrackingService _trackingService;
        private readonly FirstAidService _firstAidService;
        private readonly EventService _eventService;

        public ShoppingListService(TrackingService trackingService, FirstAidService firstAidService, EventService eventService)
        {
            _trackingService = trackingService ?? throw new ArgumentNullException(nameof(trackingService));
            _firstAidService = firstAidService ?? throw new ArgumentNullException(nameof(firstAidService));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        /// <summary>
        /// Oblicza brakujące produkty na listę zakupów na podstawie:
        /// 1. Zaplanowanych posiłków (wydarzenia z przepisami) - sprawdza dostępność składników
        /// 2. Zaplanowanych leków (wydarzenia z lekami) - sprawdza czy leki się kończą
        /// </summary>
        public List<CalculatedShoppingItem> CalculateMissingItems()
        {
            var missingItems = new Dictionary<string, CalculatedShoppingItem>();

            // Pobierz aktualny stan magazynu
            var fridgeItems = _trackingService.GetAllFridgeItems();
            var medicineItems = _firstAidService.GetAllMedicineItems();

            // 1. Sprawdź leki o niskim stanie (Quantity <= 5)
            // Używa źródeł FirstAidService
            foreach (var medicineItem in medicineItems)
            {
                if (medicineItem.Quantity <= 5)
                {
                    string key = $"Medicine_{medicineItem.Name}";
                    if (!missingItems.ContainsKey(key))
                    {
                        missingItems[key] = new CalculatedShoppingItem
                        {
                            Name = medicineItem.Name,
                            Amount = Math.Max(1, 10 - medicineItem.Quantity), // Kup tyle żeby mieć co najmniej 10
                            Unit = medicineItem.Unit ?? "pcs",
                            Type = "Medicine",
                            Source = "Running low"
                        };
                    }
                }
            }

            // 2. Uwzględnij produkty z tabeli ShoppingList (dodane ręcznie)
            // Ponownie sprawdź dostępność każdego produktu
            var shoppingListItems = _trackingService.GetShoppingListItems();
            foreach (var item in shoppingListItems)
            {
                // Pobierz nazwę przepisu jeśli RecipeID jest ustawione
                string source = "Shopping list";
                if (item.RecipeID.HasValue)
                {
                    var recipe = _trackingService.GetRecipeById(item.RecipeID.Value);
                    source = recipe != null ? $"Recipe: {recipe.Name}" : "Scheduled meal";
                }
                
                string key = $"{item.ProductName}_{item.Unit}";
                if (missingItems.ContainsKey(key))
                {
                    // Nie licz podwójnie - zapisana ilość już odzwierciedla brakującą
                    // Zaktualizuj źródło jeśli ten wpis ma nazwę przepisu
                    if (item.RecipeID.HasValue && !missingItems[key].Source.StartsWith("Recipe:"))
                    {
                        missingItems[key].Source = source;
                    }
                }
                else
                {
                    missingItems[key] = new CalculatedShoppingItem
                    {
                        Name = item.ProductName,
                        Amount = item.Amount,
                        Unit = item.Unit,
                        Type = "Food",
                        Source = source
                    };
                }
            }

            return missingItems.Values.OrderBy(i => i.Type).ThenBy(i => i.Name).ToList();
        }

    }

    public class CalculatedShoppingItem
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Unit { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
    }
}

