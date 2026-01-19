// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz szczegółów przepisu kulinarnego.
    /// Wyświetla składniki, czas przygotowania i pozwala zaplanować posiłek.
    /// </summary>
    public partial class RecipeDetailsForm : Form
    {
        // Serwis trackingu
        private readonly TrackingService _trackingService;
        // Wyświetlany przepis
        private readonly Recipe _recipe;
        // Oryginalna liczba porcji (do skalowania)
        private int _originalPortions;
        // Serwisy pomocnicze
        private FirstAidService _firstAidService;
        private EventService _eventService;

        /// <summary>
        /// Konstruktor formularza szczegółów przepisu.
        /// </summary>
        public RecipeDetailsForm(TrackingService trackingService, Recipe recipe)
        {
            _trackingService = trackingService;
            _recipe = recipe;
            _originalPortions = recipe.NumberOfPortions;
            _firstAidService = new FirstAidService();
            _eventService = new EventService();
            InitializeComponent();

            _lstIngredients.Columns.Add("Product", 250);
            _lstIngredients.Columns.Add("Amount", 150);
            _lstIngredients.Columns.Add("Unit", 100);

            _btnEdit.Click += (_, __) => OnEditRecipe();
            _btnDelete.Click += (_, __) => OnDeleteRecipe();
            _btnPlanEating.Click += (_, __) => OnPlanEating();
            _numPortions.ValueChanged += (_, __) => UpdateIngredientAmounts();

            LoadRecipeDetails();
        }
        private void LoadRecipeDetails()
        {
            _lblName.Text = _recipe.Name;
            _lblDescription.Text = string.IsNullOrWhiteSpace(_recipe.Description) ? "(No description)" : _recipe.Description;
            _lblTime.Text = _recipe.PreparationTimeMinutes.ToString() + " minutes";
            _numPortions.Value = _recipe.NumberOfPortions;

            UpdateIngredientAmounts();
            UpdatePlanEatingButton();
        }

        private void UpdatePlanEatingButton()
        {
            if (_recipe.IsScheduled)
            {
                _btnPlanEating.Text = "Unschedule";
                _btnPlanEating.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
                _btnPlanEating.BackColor = System.Drawing.Color.FromArgb(250, 200, 200);
                _btnPlanEating.ForeColor = System.Drawing.Color.FromArgb(192, 57, 43);
                _btnPlanEating.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(231, 76, 60);
                _btnPlanEating.Enabled = true;
            }
            else
            {
                _btnPlanEating.Text = "Plan Eating";
                _btnPlanEating.BackColor = System.Drawing.Color.FromArgb(200, 240, 210);
                _btnPlanEating.ForeColor = System.Drawing.Color.FromArgb(40, 120, 60);
                _btnPlanEating.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(160, 220, 180);
                _btnPlanEating.Enabled = true;
            }
        }

        private void UpdateIngredientAmounts()
        {
            _lstIngredients.Items.Clear();

            int currentPortions = (int)_numPortions.Value;
            decimal portionRatio = currentPortions / (decimal)_originalPortions;

            foreach (var ingredient in _recipe.Ingredients)
            {
                decimal scaledAmount = ingredient.Amount * portionRatio;

                var lvi = new ListViewItem(ingredient.ProductName);
                lvi.SubItems.Add(scaledAmount.ToString("0.00"));
                lvi.SubItems.Add(ingredient.Unit);
                _lstIngredients.Items.Add(lvi);
            }
        }

        private void OnDeleteRecipe()
        {
            if (_recipe.IsScheduled)
            {
                MessageBox.Show(
                    "You cannot delete a scheduled recipe.\nPlease unschedule it first using 'Unschedule' button.",
                    "Cannot Delete Recipe",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            var result = MessageBox.Show(
                $"Are you sure you want to delete the recipe \"{_recipe.Name}\"?",
                "Delete Recipe",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                _trackingService.DeleteRecipe(_recipe.RecipeID);
                MessageBox.Show(
                    "Recipe deleted successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while deleting the recipe:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnPlanEating()
        {
            try
            {
                if (_recipe.IsScheduled)
                {
                    // Usuń przepis z harmonogramu
                    _trackingService.SetRecipeScheduled(_recipe.RecipeID, false);
                    _recipe.IsScheduled = false;

                    _trackingService.RemoveFromShoppingListByRecipe(_recipe.RecipeID);

                    MessageBox.Show(
                        $"Meal \"{_recipe.Name}\" has been unscheduled.\nRelated items have been removed from the shopping list.",
                        "Meal Unscheduled",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    // Zaplanuj przepis
                    int portions = (int)_numPortions.Value;

                    _trackingService.SetRecipeScheduled(_recipe.RecipeID, true);
                    _recipe.IsScheduled = true;

                    CheckIngredientsAndAddToShoppingList(portions);

                    MessageBox.Show(
                        $"Meal \"{_recipe.Name}\" has been scheduled for {portions} portion(s).\nMissing ingredients have been added to the shopping list.",
                        "Meal Scheduled",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                UpdatePlanEatingButton();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CheckIngredientsAndAddToShoppingList(int portions)
        {
            decimal portionRatio = portions / (decimal)_originalPortions;

            foreach (var ingredient in _recipe.Ingredients)
            {
                decimal requiredAmount = ingredient.Amount * portionRatio;

                decimal availableAmount = _trackingService.GetAvailableIngredientAmount(
                    ingredient.ProductName, ingredient.Unit);

                decimal missingAmount = requiredAmount - availableAmount;

                if (missingAmount > 0)
                {
                    _trackingService.AddToShoppingList(
                        ingredient.ProductName,
                        missingAmount,
                        ingredient.Unit,
                        _recipe.RecipeID);
                }
            }
        }

        private void OnEditRecipe()
        {
            using var editForm = new AddRecipeForm(_trackingService);
            editForm.LoadRecipeForEdit(_recipe);
            if (editForm.ShowDialog(this) == DialogResult.OK)
            {
                // Odśwież przepis z bazy danych
                var updatedRecipe = _trackingService.GetRecipeById(_recipe.RecipeID);
                if (updatedRecipe != null)
                {
                    _recipe.Name = updatedRecipe.Name;
                    _recipe.Description = updatedRecipe.Description;
                    _recipe.PreparationTimeMinutes = updatedRecipe.PreparationTimeMinutes;
                    _recipe.NumberOfPortions = updatedRecipe.NumberOfPortions;
                    _recipe.Ingredients = updatedRecipe.Ingredients;
                    _originalPortions = updatedRecipe.NumberOfPortions;

                    LoadRecipeDetails();

                    MessageBox.Show("Recipe updated successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}

