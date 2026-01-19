// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Collections.Generic; // Kolekcje generyczne
using System.Linq;              // LINQ
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz do dodawania/edycji przepisu kulinarnego.
    /// 
    /// Pozwala ustawić nazwę, opis, czas przygotowania,
    /// oraz listę składników z ilościami.
    /// </summary>
    public partial class AddRecipeForm : Form
    {
        // Serwis tracking (do zarządzania przepisami)
        private readonly TrackingService _trackingService;
        // Lista składników przepisu
        private List<RecipeIngredient> _ingredients = new List<RecipeIngredient>();
        // Istniejący przepis (null = nowy)
        private Recipe _existingRecipe;
        // Czy formularz jest w trybie edycji
        private bool _isEditMode;

        /// <summary>
        /// Konstruktor formularza - tryb dodawania nowego przepisu.
        /// </summary>
        public AddRecipeForm(TrackingService trackingService)
        {
            _trackingService = trackingService;
            _isEditMode = false;
            InitializeComponent();
            
            // Ustaw kolumny ListView i podepnij eventy
            _lstIngredients.Columns.Add("Product", 200);
            _lstIngredients.Columns.Add("Amount", 150);
            _lstIngredients.Columns.Add("Unit", 100);
            _cmbUnit.SelectedIndex = 0;
            
            _btnAddIngredient.Click += (s, e) => OnAddIngredient();
            _btnRemoveIngredient.Click += (s, e) => OnRemoveIngredient();
            _btnSave.Click += (s, e) => OnSave();
            
            LoadProducts();
        }

        /// <summary>
        /// Przełącza formularz w tryb edycji i wypełnia pola danymi przepisu.
        /// </summary>
        public void LoadRecipeForEdit(Recipe recipeToEdit)
        {
            if (recipeToEdit == null) throw new ArgumentNullException(nameof(recipeToEdit));

            _existingRecipe = recipeToEdit;
            _isEditMode = true;
            Text = "Edit Recipe";

            _txtName.Text = recipeToEdit.Name ?? string.Empty;
            _txtDescription.Text = recipeToEdit.Description ?? string.Empty;
            _numTime.Value = recipeToEdit.PreparationTimeMinutes;
            _numPortions.Value = recipeToEdit.NumberOfPortions;

            _ingredients = recipeToEdit.Ingredients?.ToList() ?? new List<RecipeIngredient>();
            RefreshIngredientsList();
        }
        private void LoadProducts()
        {
            var products = _trackingService.GetAllProducts();
            _cmbProduct.DataSource = products;
            _cmbProduct.DisplayMember = "Name";
            _cmbProduct.ValueMember = "ProductID";

            _cmbProduct.SelectedIndexChanged += (s, e) =>
            {
                if (_cmbProduct.SelectedItem is FoodProduct product && !string.IsNullOrEmpty(product.DefaultUnit))
                {
                    var unitIndex = _cmbUnit.Items.IndexOf(product.DefaultUnit);
                    if (unitIndex >= 0)
                        _cmbUnit.SelectedIndex = unitIndex;
                }
            };
        }

        private void OnAddIngredient()
        {
            if (_cmbProduct.SelectedItem is not FoodProduct product)
            {
                MessageBox.Show("Please select a product.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_numAmount.Value <= 0)
            {
                MessageBox.Show("Amount must be greater than zero.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_ingredients.Any(i => i.ProductID == product.ProductID))
            {
                MessageBox.Show("This ingredient is already in the list.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ingredient = new RecipeIngredient
            {
                ProductID = product.ProductID,
                ProductName = product.Name,
                Amount = _numAmount.Value,
                Unit = _cmbUnit.SelectedItem as string
            };

            _ingredients.Add(ingredient);
            RefreshIngredientsList();
        }

        private void OnRemoveIngredient()
        {
            if (_lstIngredients.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an ingredient to remove.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedItem = _lstIngredients.SelectedItems[0];
            if (selectedItem.Tag is RecipeIngredient ingredient)
            {
                _ingredients.Remove(ingredient);
                RefreshIngredientsList();
            }
        }

        private void RefreshIngredientsList()
        {
            _lstIngredients.Items.Clear();
            foreach (var ingredient in _ingredients)
            {
                var lvi = new ListViewItem(ingredient.ProductName);
                lvi.SubItems.Add(ingredient.Amount.ToString("0.00"));
                lvi.SubItems.Add(ingredient.Unit);
                lvi.Tag = ingredient;
                _lstIngredients.Items.Add(lvi);
            }
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("Please enter a recipe name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (_ingredients.Count == 0)
            {
                MessageBox.Show("Please add at least one ingredient.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (_isEditMode && _existingRecipe != null)
            {
                _existingRecipe.Name = _txtName.Text.Trim();
                _existingRecipe.Description = _txtDescription.Text.Trim();
                _existingRecipe.PreparationTimeMinutes = (int)_numTime.Value;
                _existingRecipe.NumberOfPortions = (int)_numPortions.Value;
                _existingRecipe.Ingredients = _ingredients;
                
                _trackingService.UpdateRecipe(_existingRecipe);
            }
            else
            {
                var recipe = new Recipe
                {
                    Name = _txtName.Text.Trim(),
                    Description = _txtDescription.Text.Trim(),
                    PreparationTimeMinutes = (int)_numTime.Value,
                    NumberOfPortions = (int)_numPortions.Value,
                    CreatedDate = DateTime.Now,
                    Ingredients = _ingredients
                };

                _trackingService.AddRecipe(recipe);
            }
        }
    }
}

