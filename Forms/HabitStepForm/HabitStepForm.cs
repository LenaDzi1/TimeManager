using System;                   // Podstawowe typy .NET
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz do dodawania/edycji kroku nawyku (HabitStep).
    /// 
    /// Pozwala ustawić nazwę, opis, punkty za wykonanie,
    /// oraz tryb dzień/noc (DaytimeSplit).
    /// </summary>
    public partial class HabitStepForm : Form
    {
        // Krok nawyku (edytowany lub nowy)
        private readonly HabitStep _step;
        // Serwis trackingu
        private readonly TrackingService _trackingService;
        // ID użytkownika (do usuwania)
        private readonly int? _userId;

        // Zwraca edytowany krok nawyku
        public HabitStep Result => _step;

        /// <summary>
        /// Główny konstruktor formularza.
        /// </summary>
        public HabitStepForm(TrackingService trackingService, int? userId, HabitStep step = null)
        {
            _step = step != null ? Clone(step) : new HabitStep { UseDaytimeSplit = false };
            _trackingService = trackingService;
            _userId = userId;

            InitializeComponent();
            WireUpEvents();
            Text = step == null ? "Add habit step" : "Edit habit step";
            _btnDelete.Visible = _trackingService != null && _userId.HasValue && _step != null && _step.HabitStepId > 0;
            LoadData();
        }

        /// <summary>
        /// Podpina eventy kontrolek.
        /// </summary>
        private void WireUpEvents()
        {
            _chkRepeat.CheckedChanged += (_, _) => _numRepeat.Enabled = _chkRepeat.Checked;
            _chkHasEndDate.CheckedChanged += (_, _) => _dtEnd.Enabled = _chkHasEndDate.Checked;
            _btnOk.Click += (_, _) => OnSave();
            _btnDelete.Click += (_, _) => OnDelete();
        }

        private void LoadData()
        {
            if (_step == null)
                return;

            _txtName.Text = _step.Name ?? string.Empty;
            _txtDescription.Text = _step.Description ?? string.Empty;
            _chkDaytimeSplit.Checked = _step.UseDaytimeSplit;
            if (_step.PointsReward.HasValue && _step.PointsReward.Value >= _numPoints.Minimum && _step.PointsReward.Value <= _numPoints.Maximum)
            {
                _numPoints.Value = _step.PointsReward.Value;
            }

            _dtStart.Value = _step.StartDate ?? DateTime.Today;

            if (_step.EndDate.HasValue)
            {
                _chkHasEndDate.Checked = true;
                _dtEnd.Enabled = true;
                _dtEnd.Value = _step.EndDate.Value;
            }
            else
            {
                _chkHasEndDate.Checked = false;
                _dtEnd.Enabled = false;
            }

            if (_step.RepeatEveryDays.HasValue && _step.RepeatEveryDays.Value >= _numRepeat.Minimum && _step.RepeatEveryDays.Value <= _numRepeat.Maximum)
            {
                _chkRepeat.Checked = true;
                _numRepeat.Value = _step.RepeatEveryDays.Value;
            }
            else
            {
                _chkRepeat.Checked = false;
                _numRepeat.Enabled = false;
            }
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("Name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            _step.Name = _txtName.Text.Trim();
            _step.StartDate = _dtStart.Value.Date;
            _step.EndDate = _chkHasEndDate.Checked ? _dtEnd.Value.Date : (DateTime?)null;
            _step.RepeatEveryDays = _chkRepeat.Checked ? (int?)_numRepeat.Value : null;
            _step.Description = string.IsNullOrWhiteSpace(_txtDescription.Text) ? null : TrimToMax(_txtDescription.Text.Trim(), 200);
            _step.UseDaytimeSplit = _chkDaytimeSplit.Checked;
            _step.PointsReward = _numPoints.Value > 0 ? (int?)_numPoints.Value : null;
        }

        private void OnDelete()
        {
            if (_trackingService == null || !_userId.HasValue)
                return;
            if (_step == null || _step.HabitStepId <= 0)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the habit step \"{_step.Name}\"?",
                "Confirm deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                _trackingService.DeleteHabitStep(_step.HabitStepId, _userId.Value);
                DialogResult = DialogResult.Abort;
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

        private static HabitStep Clone(HabitStep source)
        {
            return new HabitStep
            {
                HabitStepId = source.HabitStepId,
                HabitCategoryId = source.HabitCategoryId,
                UserId = source.UserId,
                Name = source.Name,
                StartDate = source.StartDate,
                EndDate = source.EndDate,
                RepeatEveryDays = source.RepeatEveryDays,
                Description = source.Description,
                UseDaytimeSplit = source.UseDaytimeSplit,
                PointsReward = source.PointsReward,
                EstimatedOccurrences = source.EstimatedOccurrences,
                RemainingOccurrences = source.RemainingOccurrences
            };
        }

        private static string TrimToMax(string text, int max)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Length <= max ? text : text.Substring(0, max);
        }
    }
}

