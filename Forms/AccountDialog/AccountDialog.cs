// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Dialog konta użytkownika.
    /// Pozwala zmienić hasło, ustawić harmonogram snu i wylogować się.
    /// </summary>
    public partial class AccountDialog : Form
    {
        // Serwis użytkowników
        private readonly UserService _userService = new();

        // Czy użytkownik chce się wylogować
        public bool LogoutRequested { get; private set; }

        /// <summary>
        /// Konstruktor dialogu konta.
        /// </summary>
        public AccountDialog()
        {
            InitializeComponent();
            WireUpEvents();
            Load += (_, _) => LoadUserData();
        }

        /// <summary>
        /// Podpina eventy kontrolek.
        /// </summary>
        private void WireUpEvents()
        {
            _btnSave.Click += (_, _) => SaveChanges();
            _btnLogout.Click += (_, _) => Logout();
            _sleepCircle.SleepScheduleChanged += (_, _) => UpdateSleepSummary();
        }

        private void LoadUserData()
        {
            var userName = UserSession.UserName ?? "User";
            _lblGreeting.Text = $"Hi {userName}!";

            // Wyświetl rolę użytkownika
            var roleDisplay = UserSession.Role ?? "Unknown";
            _lblRole.Text = $"Role: {roleDisplay}";

            var user = _userService.GetUser(userName);
            if (user != null)
            {
                if (user.SleepStart.HasValue)
                {
                    _sleepCircle.SleepStart = user.SleepStart.Value;
                }
                if (user.SleepEnd.HasValue)
                {
                    _sleepCircle.SleepEnd = user.SleepEnd.Value;
                }
                UpdateSleepSummary();
                return;
            }

            // Domyślne wartości
            _sleepCircle.SleepStart = TimeSpan.FromHours(23);
            _sleepCircle.SleepEnd = TimeSpan.FromHours(7);
            UpdateSleepSummary();
        }

        private void SaveChanges()
        {
            var userName = UserSession.UserName;
            if (string.IsNullOrWhiteSpace(userName))
            {
                MessageBox.Show("No user is logged in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var user = _userService.GetUser(userName);
            if (user == null)
            {
                MessageBox.Show("User record not found in database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrWhiteSpace(_txtPassword.Text) || !string.IsNullOrWhiteSpace(_txtPasswordRepeat.Text))
            {
                if (!string.Equals(_txtPassword.Text, _txtPasswordRepeat.Text, StringComparison.Ordinal))
                {
                    MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _userService.UpdatePassword(userName, _txtPassword.Text);
            }

            _userService.UpdateSleepSchedule(userName, _sleepCircle.SleepStart, _sleepCircle.SleepEnd);
            UserSession.UpdateSleep(_sleepCircle.SleepStart, _sleepCircle.SleepEnd);

            MessageBox.Show("Changes saved.", "Account", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _txtPassword.Clear();
            _txtPasswordRepeat.Clear();
        }

        private void Logout()
        {
            LogoutRequested = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void UpdateSleepSummary()
        {
            var duration = _sleepCircle.GetDuration();
            var startText = FormatTime(_sleepCircle.SleepStart);
            var endText = FormatTime(_sleepCircle.SleepEnd);
            _lblSleepSummary.Text = $"You sleep {duration.TotalHours:0.#}h  {startText} - {endText}";
        }

        private static string FormatTime(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }
    }
}

