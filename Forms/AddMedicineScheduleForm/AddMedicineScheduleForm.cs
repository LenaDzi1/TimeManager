// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz do dodawania/edycji harmonogramu przyjmowania leków.
    /// 
    /// Pozwala ustawić:
    /// - Lek do monitorowania
    /// - Dawki rano i wieczorem
    /// - Interwał (co ile dni)
    /// - Datę rozpoczęcia i opcjonalną datę zakończenia
    /// </summary>
    public partial class AddMedicineScheduleForm : Form
    {
        // Serwis apteczki
        private readonly FirstAidService _firstAidService;
        // Istniejący harmonogram (null = nowy)
        private readonly MedicineSchedule _existing;

        /// <summary>
        /// Konstruktor dla nowego harmonogramu.
        /// </summary>
        public AddMedicineScheduleForm(FirstAidService firstAidService)
        {
            _firstAidService = firstAidService ?? throw new ArgumentNullException(nameof(firstAidService));
            InitializeComponent();
            WireUpEvents();
            Text = "Add Medicine Schedule";
            LoadMedicines();
        }

        /// <summary>
        /// Konstruktor do edycji istniejącego harmonogramu.
        /// </summary>
        public AddMedicineScheduleForm(FirstAidService firstAidService, MedicineSchedule existing)
        {
            _firstAidService = firstAidService ?? throw new ArgumentNullException(nameof(firstAidService));
            _existing = existing ?? throw new ArgumentNullException(nameof(existing));
            InitializeComponent();
            WireUpEvents();
            Text = "Edit Medicine Schedule";
            _btnDelete.Visible = _existing.ScheduleID > 0;
            LoadMedicines();
            Prefill();
        }

        /// <summary>
        /// Podpina eventy kontrolek.
        /// </summary>
        private void WireUpEvents()
        {
            _chkEnd.CheckedChanged += (_, _) => _dtpEnd.Enabled = _chkEnd.Checked;
            _btnSave.Click += (_, _) => OnSave();
            _btnDelete.Click += (_, _) => OnDelete();
        }
        
        private void OnDelete()
        {
            if (_existing == null || _existing.ScheduleID <= 0)
                return;
            
            var result = MessageBox.Show(
                "Are you sure you want to delete this medicine schedule?",
                "Delete Medicine Schedule",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            
            if (result != DialogResult.Yes)
                return;
            
            try
            {
                _firstAidService.DeleteMedicineSchedule(_existing.ScheduleID);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while deleting:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadMedicines()
        {
            var items = _firstAidService.GetAllMedicineItems();
            _cmbMedicine.DataSource = items;
            _cmbMedicine.DisplayMember = "Name";
            _cmbMedicine.ValueMember = "ItemID";

            if (_existing != null)
            {
                _cmbMedicine.SelectedValue = _existing.MedicineItemID;
            }
        }

        private void OnSave()
        {
            if (_cmbMedicine.SelectedItem is not MedicineItem medicine)
            {
                MessageBox.Show("Select a medicine from the list.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            var morning = _numMorning.Value > 0 ? _numMorning.Value : (decimal?)null;
            var evening = _numEvening.Value > 0 ? _numEvening.Value : (decimal?)null;
            if (morning == null && evening == null)
            {
                MessageBox.Show("Enter a dose for morning or evening.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            var schedule = new MedicineSchedule
            {
                ScheduleID = _existing?.ScheduleID ?? 0,
                MedicineItemID = medicine.ItemID,
                MedicineName = medicine.Name,
                MorningDose = morning,
                EveningDose = evening,
                IntervalDays = (int)_numInterval.Value,
                StartDate = _dtpStart.Value.Date,
                EndDate = _chkEnd.Checked ? _dtpEnd.Value.Date : (DateTime?)null,
                IsActive = true
            };

            if (_existing == null)
            {
                _firstAidService.AddSchedule(schedule);
            }
            else
            {
                _firstAidService.UpdateSchedule(schedule);
            }
        }

        private void Prefill()
        {
            if (_existing == null)
                return;

            _cmbMedicine.SelectedValue = _existing.MedicineItemID;
            _numInterval.Value = Math.Clamp(_existing.IntervalDays, (int)_numInterval.Minimum, (int)_numInterval.Maximum);
            _numMorning.Value = _existing.MorningDose.HasValue
                ? Math.Min(_existing.MorningDose.Value, _numMorning.Maximum)
                : 0;
            _numEvening.Value = _existing.EveningDose.HasValue
                ? Math.Min(_existing.EveningDose.Value, _numEvening.Maximum)
                : 0;
            _dtpStart.Value = _existing.StartDate;
            if (_existing.EndDate.HasValue)
            {
                _chkEnd.Checked = true;
                _dtpEnd.Enabled = true;
                _dtpEnd.Value = _existing.EndDate.Value;
            }
        }
    }
}

