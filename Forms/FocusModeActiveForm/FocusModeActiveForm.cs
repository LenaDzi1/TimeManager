// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Drawing;           // Grafika i kolory
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz aktywnej sesji Focus Mode (tryb skupienia).
    /// 
    /// Wyświetla timer odliczający czas na wykonanie zadania.
    /// Pozwala na ukończenie, anulowanie lub pauzowanie sesji.
    /// Po zakończeniu aktualizuje trackowanie i punkty.
    /// </summary>
    public partial class FocusModeActiveForm : Form
    {
        // Event do którego należy sesja Focus
        private readonly Event _event;
        // Serwis Focus Mode
        private readonly FocusModeService _focusService;
        // Timer do odliczania czasu
        private readonly Timer _timer;
        // Czas rozpoczęcia sesji
        private DateTime _startTime;
        // Planowany czas trwania w minutach
        private int _durationMinutes;
        // Licznik upłyniętych sekund
        private int _elapsedSeconds;

        /// <summary>
        /// Konstruktor formularza Focus Mode.
        /// </summary>
        public FocusModeActiveForm(Event evt, FocusModeService focusService)
        {
            _event = evt ?? throw new ArgumentNullException(nameof(evt));
            _focusService = focusService ?? throw new ArgumentNullException(nameof(focusService));
            _durationMinutes = evt.Duration > 0 ? evt.Duration : 30;
            _elapsedSeconds = 0;
            _startTime = DateTime.Now;

            InitializeComponent();
            WireUpEvents();

            // Ustaw tytuł i początkowe wartości kontrolek
            Text = $"Focus Mode - {_event.Title}";
            _lblEventTitle.Text = _event.Title ?? "Focus Session";
            _lblDuration.Text = $"Duration: {_durationMinutes} minutes";
            _progressBar.Maximum = _durationMinutes * 60;
            _progressBar.Value = 0;

            // Uruchom timer (1 sekunda)
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += Timer_Tick;
            _timer.Start();

            UpdateTimeDisplay();
        }

        /// <summary>
        /// Podpina eventy przycisków.
        /// </summary>
        private void WireUpEvents()
        {
            _btnComplete.Click += BtnComplete_Click;
            _btnCancel.Click += BtnCancel_Click;
            _btnPause.Click += BtnPause_Click;
            _buttonPanel.Resize += (_, __) => CenterButtons();
            _progressPanel.Resize += (_, __) => CenterContent();
            this.Resize += (_, __) => ScaleTimerFont();
        }

        private void ScaleTimerFont()
        {
            if (_lblTimeRemaining == null) return;

            var heightRatio = (float)(this.ClientSize.Height - 300) / 200f;
            heightRatio = Math.Max(0f, Math.Min(1f, heightRatio));
            var fontSize = 36f + (heightRatio * 36f);

            try
            {
                _lblTimeRemaining.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                CenterContent();
            }
            catch { }
        }

        private void CenterContent()
        {
            if (_progressPanel == null || _lblTimeRemaining == null) return;

            var panelWidth = _progressPanel.ClientSize.Width;
            var panelHeight = _progressPanel.ClientSize.Height;

            var spacing = 10;
            var timerHeight = _lblTimeRemaining.Height;
            var progressHeight = _progressBar?.Height ?? 25;
            var elapsedHeight = _lblElapsed?.Height ?? 20;
            var percentHeight = _lblPercentage?.Height ?? 25;
            var totalHeight = timerHeight + progressHeight + elapsedHeight + percentHeight + (spacing * 3);

            var startY = Math.Max(10, (panelHeight - totalHeight) / 2);
            var centerX = panelWidth / 2;

            _lblTimeRemaining.Location = new Point(centerX - (_lblTimeRemaining.Width / 2), startY);

            if (_progressBar != null)
            {
                var progressWidth = panelWidth - 40;
                _progressBar.Size = new Size(progressWidth, 25);
                _progressBar.Location = new Point(20, startY + timerHeight + spacing);
            }

            if (_lblElapsed != null)
            {
                _lblElapsed.Location = new Point(centerX - (_lblElapsed.Width / 2), startY + timerHeight + progressHeight + (spacing * 2));
            }

            if (_lblPercentage != null)
            {
                _lblPercentage.Location = new Point(centerX - (_lblPercentage.Width / 2), startY + timerHeight + progressHeight + elapsedHeight + (spacing * 3));
            }
        }

        private void CenterButtons()
        {
            if (_buttonPanel == null || _btnComplete == null || _btnPause == null || _btnCancel == null) return;

            var panelWidth = _buttonPanel.ClientSize.Width;
            var buttonWidth = _btnComplete.Width;
            const int minSpacing = 20;
            const int maxSpacing = 60;

            var totalButtonsWidth = buttonWidth * 3;
            var availableSpace = panelWidth - totalButtonsWidth - 60;
            var spacing = Math.Max(minSpacing, Math.Min(maxSpacing, availableSpace / 2));

            var totalWidth = (buttonWidth * 3) + (spacing * 2);
            var startX = (panelWidth - totalWidth) / 2;

            _btnComplete.Location = new Point(startX, _btnComplete.Location.Y);
            _btnPause.Location = new Point(startX + buttonWidth + spacing, _btnPause.Location.Y);
            _btnCancel.Location = new Point(startX + (buttonWidth + spacing) * 2, _btnCancel.Location.Y);
        }

        /// <summary>
        /// Tick timera - aktualizuje wyświetlany czas.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            _elapsedSeconds++;

            if (_elapsedSeconds >= _durationMinutes * 60)
            {
                _timer.Stop();
                OnSessionComplete();
                return;
            }

            _progressBar.Value = Math.Min(_elapsedSeconds, _progressBar.Maximum);
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            int remainingSeconds = (_durationMinutes * 60) - _elapsedSeconds;
            int mins = remainingSeconds / 60;
            int secs = remainingSeconds % 60;
            _lblTimeRemaining.Text = $"{mins:D2}:{secs:D2}";

            int elapsedMins = _elapsedSeconds / 60;
            int elapsedSecs = _elapsedSeconds % 60;
            _lblElapsed.Text = $"Elapsed: {elapsedMins:D2}:{elapsedSecs:D2}";

            double percentage = (_elapsedSeconds * 100.0) / (_durationMinutes * 60);
            _lblPercentage.Text = $"{percentage:F0}%";
        }

        private void OnSessionComplete()
        {
            _focusService.EndFocusSession(false);
            MessageBox.Show(
                "Focus session completed! Great work!",
                "Session Complete",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnComplete_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            var result = MessageBox.Show(
                "Mark this focus session as complete?",
                "Complete Session",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _focusService.EndFocusSession(false);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                _timer.Start();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            var result = MessageBox.Show(
                "Cancel the focus session?",
                "Cancel Session",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                _focusService.EndFocusSession(true);
                DialogResult = DialogResult.Cancel;
                Close();
            }
            else
            {
                _timer.Start();
            }
        }

        private bool _isPaused = false;
        private void BtnPause_Click(object sender, EventArgs e)
        {
            if (_isPaused)
            {
                _timer.Start();
                _btnPause.Text = "⏸ Pause";
                _isPaused = false;
            }
            else
            {
                _timer.Stop();
                _btnPause.Text = "▶ Resume";
                _isPaused = true;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timer?.Stop();
            _timer?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
