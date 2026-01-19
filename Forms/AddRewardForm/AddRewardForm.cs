// Import przestrzeni nazw
using System.Windows.Forms;     // Windows Forms

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz do dodawania nowej nagrody.
    /// Pozwala ustawić nazwę, opis i koszt w punktach.
    /// </summary>
    public partial class AddRewardForm : Form
    {
        // Nazwa nagrody (z pola tekstowego)
        public string RewardName => _txtName.Text;
        // Opis nagrody
        public string Description => _txtDescription.Text;
        // Koszt w punktach
        public int PointsCost => (int)_numPointsCost.Value;

        /// <summary>
        /// Konstruktor formularza.
        /// </summary>
        public AddRewardForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler przycisku zapisz - walidacja i zapis.
        /// </summary>
        private void OnSaveClick(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("Please enter a name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }
        }
    }
}
