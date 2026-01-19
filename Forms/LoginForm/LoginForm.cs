// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET
using System.IO;                // Operacje plikowe
using System.Windows.Forms;     // Windows Forms
using TimeManager.Models;       // Modele aplikacji
using TimeManager.Services;     // Serwisy aplikacji

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz logowania i rejestracji użytkowników.
    /// 
    /// FUNKCJE:
    /// - Logowanie istniejących użytkowników
    /// - Rejestracja nowych kont (zawsze jako Kid)
    /// - Automatyczne tworzenie domyślnego konta admin
    /// 
    /// Nowe konta mają rolę Kid - admin może potem zmienić na User/Parent.
    /// </summary>
    public partial class LoginForm : Form
    {
        // Serwis użytkowników
        private readonly UserService _userService = new();
        // Czy jesteśmy w trybie rejestracji (true) czy logowania (false)
        private bool _isSignup = false;

        // Ścieżka do pliku z zapamiętanymi danymi logowania (legacy)
        // Używana przez MainForm/AdminPanel/Program, ale UI już tego nie wspiera
        private static readonly string CredentialsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TimeManager", "credentials.dat");

        // Nazwa zalogowanego użytkownika (ustawiana po pomyślnym logowaniu)
        public string LoggedInUserName { get; private set; }
        // Wybrana rola użytkownika
        public string SelectedRole { get; private set; }

        /// <summary>
        /// Konstruktor formularza logowania.
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();

            // Podepnij eventy przycisków
            _btnLogin.Click += BtnLogin_Click;
            _btnSignupMode.Click += (s, e) => ToggleMode();

            // Upewnij się że domyślne konto admin istnieje
            EnsureDefaultAdminExists();
        }

        /// <summary>
        /// Usuwa zapamiętane dane logowania (legacy).
        /// </summary>
        public static void ClearSavedCredentials()
        {
            try
            {
                if (File.Exists(CredentialsFile))
                    File.Delete(CredentialsFile);
            }
            catch
            {
            }
        }

        private void EnsureDefaultAdminExists()
        {
            try
            {
                _userService.FixRoleConstraint();

                _userService.EnsureAdminAccount("admin", "admin123", UserRoles.Administrator);
            }
            catch
            {
                // Ignoruj błąd - nie przerywaj pracy aplikacji
            }
        }

        private void ToggleMode()
        {
            _isSignup = !_isSignup;
            if (_isSignup)
            {
                _lblMode.Text = "Create a new account";
                _btnLogin.Text = "Sign up";
                _btnSignupMode.Text = "Back to login";
            }
            else
            {
                _lblMode.Text = "Log in to your account";
                _btnLogin.Text = "Log in";
                _btnSignupMode.Text = "Sign up instead";
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtUserName.Text) ||
                string.IsNullOrWhiteSpace(_txtPassword.Text))
            {
                MessageBox.Show("Please enter user name and password.", "Validation", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            LoggedInUserName = _txtUserName.Text.Trim();

            if (_isSignup)
            {
                HandleSignup();
            }
            else
            {
                HandleLogin();
            }
        }

        private void HandleLogin()
        {
            if (!_userService.ValidateUser(LoggedInUserName, _txtPassword.Text))
            {
                MessageBox.Show("Invalid username or password.", "Login failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var user = _userService.GetUser(LoggedInUserName);
            if (user == null)
            {
                MessageBox.Show("User not found.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UserSession.SetUser(user.UserId, user.UserName, user.Role, user.SleepStart, user.SleepEnd);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void HandleSignup()
        {
            if (_userService.UserExists(LoggedInUserName))
            {
                MessageBox.Show("This username is already taken.", "Sign up", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var role = UserRoles.Kid;
            var defaultStart = TimeSpan.FromHours(23);
            var defaultEnd = TimeSpan.FromHours(7);

            _userService.CreateUser(LoggedInUserName, _txtPassword.Text, role, defaultStart, defaultEnd);

            var user = _userService.GetUser(LoggedInUserName);
            UserSession.SetUser(user?.UserId ?? 0, LoggedInUserName, role, defaultStart, defaultEnd);

            MessageBox.Show("Account created! You are logged in as a Kid.\nAsk an Administrator to upgrade your role.",
                "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}



