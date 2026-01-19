#nullable enable
// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz do edycji istniejącej nagrody.
    /// Pozwala zmienić nazwę, opis i koszt w punktach.
    /// </summary>
    public partial class EditRewardForm : Form
    {
        // Edytowana nagroda
        private readonly Reward _reward;

        // Nazwa nagrody (z pola tekstowego)
        public string RewardName => _txtName.Text;
        // Opis nagrody
        public string Description => _txtDescription.Text;
        // Koszt w punktach
        public int PointsCost => (int)_numPointsCost.Value;

        /// <summary>
        /// Konstruktor formularza edycji nagrody.
        /// </summary>
        public EditRewardForm(Reward reward)
        {
            _reward = reward ?? throw new ArgumentNullException(nameof(reward));
            InitializeComponent();

            _txtName.Text = _reward.Name ?? string.Empty;
            _txtDescription.Text = _reward.Description ?? string.Empty;
            _numPointsCost.Value = Math.Max(_numPointsCost.Minimum, Math.Min(_numPointsCost.Maximum, _reward.PointsCost));
        }

        private void OnSaveClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }

        private void OnDeleteClick(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort; // Sygnał do usunięcia
        }
    }
}



