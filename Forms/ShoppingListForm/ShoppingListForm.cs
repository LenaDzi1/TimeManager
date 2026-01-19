// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Drawing;           // Grafika i kolory
using System.Windows.Forms;     // Windows Forms
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz listy zakupów.
    /// Wyświetla brakujące produkty (jedzenie i leki) potrzebne do przepisów.
    /// </summary>
    public partial class ShoppingListForm : Form
    {
        // Serwis listy zakupów
        private readonly ShoppingListService _shoppingListService;

        /// <summary>
        /// Konstruktor formularza.
        /// </summary>
        public ShoppingListForm(ShoppingListService shoppingListService)
        {
            _shoppingListService = shoppingListService ?? throw new ArgumentNullException(nameof(shoppingListService));
            InitializeComponent();
            
            // Podepnij eventy (przeniesione z Designer dla kompatybilności z VS Designer)
            _btnRefresh.Click += (sender, args) => LoadShoppingList();
            _btnClose.Click += (sender, args) => Close();
            
            // Force Setup Columns
            _listView.View = View.Details;
            _listView.GridLines = true;
            _listView.FullRowSelect = true;
            _listView.Columns.Clear();
            _listView.Columns.Add("Name", 150);
            _listView.Columns.Add("Amount", 80);
            _listView.Columns.Add("Unit", 80);
            _listView.Columns.Add("Type", 100);
            _listView.Columns.Add("Source", 100);

            LoadShoppingList();
        }
        private void LoadShoppingList()
        {
            _listView.Items.Clear();

            try
            {
                var missingItems = _shoppingListService.CalculateMissingItems();

                if (missingItems.Count == 0)
                {
                    var item = new ListViewItem("No missing items");
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("");
                    item.SubItems.Add("All items are available");
                    item.ForeColor = Color.Gray;
                    _listView.Items.Add(item);
                }
                else
                {
                    foreach (var shoppingItem in missingItems)
                    {
                        var item = new ListViewItem(shoppingItem.Name);
                        item.SubItems.Add(shoppingItem.Amount.ToString("0.00"));
                        item.SubItems.Add(shoppingItem.Unit);
                        item.SubItems.Add(shoppingItem.Type);
                        item.SubItems.Add(shoppingItem.Source);

                        // Koloruj według typu
                        if (shoppingItem.Type == "Medicine")
                        {
                            item.BackColor = Color.FromArgb(255, 240, 240);
                        }
                        else
                        {
                            item.BackColor = Color.FromArgb(240, 255, 240);
                        }

                        _listView.Items.Add(item);
                    }
                }
            }
            catch 
            {}
        }
    }
}

