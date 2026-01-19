// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz do dodawania/edycji rośliny.
    /// Pozwala ustawić nazwę, gatunek i częstotliwość podlewania.
    /// </summary>
    public partial class PlantEditorForm : Form
    {
        // Serwis trackingu
        private readonly TrackingService _trackingService;
        // Edytowana roślina (null = nowa)
        private readonly Plant _plant;
        // Czy tryb edycji
        private readonly bool _isEdit;

        /// <summary>
        /// Konstruktor formularza.
        /// </summary>
        public PlantEditorForm(TrackingService trackingService, Plant plant = null)
        {
            _trackingService = trackingService ?? throw new ArgumentNullException(nameof(trackingService));
            _plant = plant;
            _isEdit = plant != null;

            InitializeComponent();
            WireUpEvents();
            Text = _isEdit ? "Edit plant" : "Add plant";
            _btnSave.Text = _isEdit ? "Save" : "Add";
            _btnDelete.Visible = _isEdit;
            _btnDelete.Enabled = _isEdit;
            
            if (_isEdit)
            {
                LoadPlant();
            }
        }

        private void WireUpEvents()
        {
            _btnSave.Click += (_, _) => OnSave();
            _btnDelete.Click += (_, _) => OnDelete();
        }

        private void LoadPlant()
        {
            _txtName.Text = _plant.Name;
            _txtSpecies.Text = _plant.Species;
            if (_plant.WateringFrequency > 0)
            {
                _numFrequency.Value = Math.Min(_numFrequency.Maximum, Math.Max(_numFrequency.Minimum, _plant.WateringFrequency));
            }
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("Insert plant name.", "No data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            var frequency = (int)_numFrequency.Value;
            if (frequency <= 0)
            {
                MessageBox.Show("Frequency must be greater than zero.", "No data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            var species = string.IsNullOrWhiteSpace(_txtSpecies.Text) ? null : _txtSpecies.Text.Trim();

            if (_isEdit)
            {
                _plant.Name = _txtName.Text.Trim();
                _plant.Species = species;
                _plant.WateringFrequency = frequency;
                _trackingService.UpdatePlant(_plant);
            }
            else
            {
                var plant = new Plant
                {
                    Name = _txtName.Text.Trim(),
                    Species = species,
                    WateringFrequency = frequency,
                    AddedDate = DateTime.Now
                };
                _trackingService.AddPlant(plant);
            }
        }

        private void OnDelete()
        {
            if (!_isEdit || _plant == null)
                return;

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete the plant \"{_plant.Name}\"?",
                "Confirm deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            _trackingService.DeletePlant(_plant.PlantID);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

