// ============================================================================
// AdminPanelForm.cs
// Panel administracyjny do zarządzania użytkownikami aplikacji.
// Dostępny tylko dla roli Administrator.
// ============================================================================

using System;                   // Podstawowe typy .NET (Convert, Exception)
using System.Drawing;           // Grafika (Point, Size, Color)
using System.Windows.Forms;     // Windows Forms (Form, DataGridView, itp.)
using TimeManager.Models;       // UserRoles, UserSession - modele użytkowników
using TimeManager.Services;     // UserService - operacje na użytkownikach

namespace TimeManager.Forms
{
    /// <summary>
    /// Panel administracyjny do zarządzania użytkownikami aplikacji.
    /// 
    /// DOSTĘP: Tylko dla użytkowników z rolą Administrator.
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Wyświetlanie listy wszystkich użytkowników z ich rolami
    /// - Zmiana roli użytkownika (Kid → User → Administrator)
    /// - Usuwanie użytkowników (z wyjątkiem siebie i domyślnego admina)
    /// - Zmiana hasła dowolnego użytkownika
    /// - Przypisywanie rodzica do kont dzieci (dla roli Kid)
    /// 
    /// WAŻNE OGRANICZENIA:
    /// - Nie można usunąć własnego konta
    /// - Nie można usunąć domyślnego konta "admin"
    /// - Nie można zmienić własnej roli jeśli jesteś jedynym adminem
    /// </summary>
    public partial class AdminPanelForm : Form
    {
        #region Pola prywatne

        /// <summary>
        /// Serwis do operacji na użytkownikach (CRUD, zmiana hasła, ról).
        /// Inicjalizowany przy tworzeniu formularza.
        /// </summary>
        private readonly UserService _userService = new();

        #endregion

        #region Konstruktor

        /// <summary>
        /// Konstruktor panelu administracyjnego.
        /// Inicjalizuje kontrolki, wypełnia ComboBox z rolami, ładuje listę użytkowników.
        /// </summary>
        public AdminPanelForm()
        {
            // Inicjalizuj kontrolki z Designer.cs
            InitializeComponent();

            // Wypełnij ComboBox dostępnymi rolami
            _cmbNewRole.Items.Clear();
            _cmbNewRole.Items.Add(UserRoles.Kid);           // Dziecko - ograniczone uprawnienia
            _cmbNewRole.Items.Add(UserRoles.User);          // Zwykły użytkownik
            _cmbNewRole.Items.Add(UserRoles.Administrator); // Administrator
            _cmbNewRole.SelectedIndex = 1; // Domyślnie: User

            // Podepnij handlery zdarzeń
            WireUpEvents();

            // Załaduj listę użytkowników
            LoadUsers();
        }

        #endregion

        #region Inicjalizacja

        /// <summary>
        /// Podpina handlery zdarzeń do kontrolek.
        /// Wywoływane raz podczas inicjalizacji formularza.
        /// </summary>
        private void WireUpEvents()
        {
            // Zmiana zaznaczenia w tabeli - aktualizuj widoczność "Assign Parent"
            _gridUsers.SelectionChanged += GridUsers_SelectionChanged;

            // Przyciski akcji
            _btnChangeRole.Click += BtnChangeRole_Click;        // Zmień rolę
            _btnDeleteUser.Click += BtnDeleteUser_Click;        // Usuń użytkownika
            _btnChangePassword.Click += BtnChangePassword_Click;// Zmień hasło
            _btnRefresh.Click += (_, _) => LoadUsers();         // Odśwież listę
            _btnLogout.Click += (_, _) => LogoutAndShowLogin(); // Wyloguj
            _btnAssignParent.Click += BtnAssignParent_Click;    // Przypisz rodzica
        }

        /// <summary>
        /// Ładuje listę użytkowników z bazy danych do DataGridView.
        /// Wypełnia również ComboBox z potencjalnymi rodzicami (dla Kids).
        /// </summary>
        private void LoadUsers()
        {
            // Wyczyść istniejące wiersze
            _gridUsers.Rows.Clear();

            // Utwórz kolumny jeśli jeszcze nie istnieją
            if (_gridUsers.Columns.Count == 0)
            {
                _gridUsers.Columns.Add("UserId", "ID");
                _gridUsers.Columns.Add("UserName", "Username");
                _gridUsers.Columns.Add("Role", "Role");
                _gridUsers.Columns.Add("ParentId", "Parent ID");

                // Ustaw szerokości kolumn
                _gridUsers.Columns["UserId"].Width = 50;
                _gridUsers.Columns["UserName"].Width = 150;
                _gridUsers.Columns["Role"].Width = 100;
                _gridUsers.Columns["ParentId"].Width = 80;
            }

            try
            {
                // Pobierz wszystkich użytkowników z bazy
                var users = _userService.GetAllUsers();

                // Dodaj każdego użytkownika jako wiersz w tabeli
                foreach (var user in users)
                {
                    _gridUsers.Rows.Add(
                        user.UserId,
                        user.UserName,
                        user.Role,
                        user.ParentId.HasValue ? user.ParentId.Value.ToString() : ""
                    );
                }

                // Wypełnij ComboBox potencjalnymi rodzicami
                // (tylko użytkownicy z rolą User mogą być rodzicami)
                _cmbAssignParent.Items.Clear();
                _cmbAssignParent.Items.Add("(none)"); // Opcja "brak rodzica"

                foreach (var u in users)
                {
                    if (u.Role == UserRoles.User)
                        _cmbAssignParent.Items.Add(new ComboItem(u.UserName, u.UserId));
                }
                _cmbAssignParent.SelectedIndex = 0;

                // Aktualizuj widoczność sekcji "Assign Parent"
                UpdateAssignParentVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Zarządzanie rolami

        /// <summary>
        /// Handler przycisku "Change Role" - zmienia rolę wybranego użytkownika.
        /// 
        /// OGRANICZENIA:
        /// - Nie można zmienić własnej roli jeśli jesteś jedynym administratorem
        /// - Wymaga potwierdzenia przed zmianą
        /// </summary>
        private void BtnChangeRole_Click(object sender, EventArgs e)
        {
            // Sprawdź czy wybrano użytkownika
            if (_gridUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pobierz dane wybranego użytkownika
            var row = _gridUsers.SelectedRows[0];
            var userId = Convert.ToInt32(row.Cells["UserId"].Value);
            var userName = row.Cells["UserName"].Value.ToString();
            var currentRole = row.Cells["Role"].Value.ToString();
            var newRole = _cmbNewRole.SelectedItem.ToString();

            // === WALIDACJA: Ochrona przed utratą dostępu admin ===
            // Nie pozwól na zmianę własnej roli jeśli jesteś jedynym adminem
            if (userName == UserSession.UserName && currentRole == UserRoles.Administrator)
            {
                // Policz ilu jest adminów
                var adminCount = 0;
                foreach (DataGridViewRow r in _gridUsers.Rows)
                {
                    if (r.Cells["Role"].Value?.ToString() == UserRoles.Administrator)
                        adminCount++;
                }

                // Jeśli to jedyny admin i próbuje zmienić swoją rolę - zablokuj
                if (adminCount <= 1 && newRole != UserRoles.Administrator)
                {
                    MessageBox.Show("Cannot change your own role - you are the only administrator.",
                        "Cannot Change", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Poproś o potwierdzenie
            var result = MessageBox.Show($"Change {userName}'s role from {currentRole} to {newRole}?",
                "Confirm Role Change", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Zaktualizuj rolę w bazie
                    _userService.UpdateUserRole(userId, newRole);

                    // Odśwież listę
                    LoadUsers();

                    MessageBox.Show("Role updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating role: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Usuwanie użytkowników

        /// <summary>
        /// Handler przycisku "Delete User" - usuwa wybranego użytkownika.
        /// 
        /// OGRANICZENIA:
        /// - Nie można usunąć własnego konta (zalogowanego użytkownika)
        /// - Nie można usunąć domyślnego konta "admin"
        /// - Wymaga potwierdzenia przed usunięciem
        /// </summary>
        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            // Sprawdź czy wybrano użytkownika
            if (_gridUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pobierz dane wybranego użytkownika
            var row = _gridUsers.SelectedRows[0];
            var userId = Convert.ToInt32(row.Cells["UserId"].Value);
            var userName = row.Cells["UserName"].Value.ToString();

            // === WALIDACJA: Nie usuwaj siebie ===
            if (userName == UserSession.UserName)
            {
                MessageBox.Show("You cannot delete your own account while logged in.",
                    "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // === WALIDACJA: Nie usuwaj domyślnego admina ===
            if (userName == "admin")
            {
                MessageBox.Show("Cannot delete the default admin account.",
                    "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Poproś o potwierdzenie (z ostrzeżeniem o nieodwracalności)
            var result = MessageBox.Show(
                $"Are you sure you want to delete user '{userName}'?\nThis action cannot be undone.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Usuń użytkownika z bazy
                    _userService.DeleteUser(userId);

                    // Odśwież listę
                    LoadUsers();

                    MessageBox.Show("User deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting user: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        #region Wylogowanie

        /// <summary>
        /// Wylogowuje administratora i pokazuje formularz logowania.
        /// 
        /// PO ZALOGOWANIU:
        /// - Jeśli nowy user to admin → odśwież panel i pokaż
        /// - Jeśli nowy user to nie-admin → otwórz MainForm i zamknij panel
        /// - Jeśli anulowano logowanie → zamknij aplikację
        /// </summary>
        private void LogoutAndShowLogin()
        {
            // Wyczyść zapisane dane logowania i wyloguj
            LoginForm.ClearSavedCredentials();
            UserSession.Logout();

            // Ukryj panel admina
            Hide();

            // Pokaż formularz logowania
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK && UserSession.IsAuthenticated)
                {
                    if (UserSession.IsAdministrator)
                    {
                        // Nowy admin - odśwież listę i pokaż panel
                        LoadUsers();
                        Show();
                    }
                    else
                    {
                        // Nie-admin - otwórz główną aplikację
                        using (var main = new MainForm())
                        {
                            main.ShowDialog(this);
                        }
                        Close();
                    }
                }
                else
                {
                    // Anulowano logowanie - zamknij aplikację
                    Close();
                }
            }
        }

        /// <summary>
        /// Obsługuje zamykanie formularza - czyści zapisane dane logowania.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Wyczyść dane logowania przy zamknięciu panelu
            LoginForm.ClearSavedCredentials();
            base.OnFormClosing(e);
        }

        #endregion

        #region Przypisywanie rodzica

        /// <summary>
        /// Handler przycisku "Assign Parent" - przypisuje rodzica do konta dziecka.
        /// 
        /// OGRANICZENIA:
        /// - Działa tylko dla użytkowników z rolą Kid
        /// - Rodzic musi mieć rolę User
        /// </summary>
        private void BtnAssignParent_Click(object sender, EventArgs e)
        {
            // Sprawdź czy wybrano użytkownika
            if (_gridUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a child (Kid) row first.", "Assign Parent",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Pobierz dane wybranego użytkownika
            var row = _gridUsers.SelectedRows[0];
            var userId = Convert.ToInt32(row.Cells["UserId"].Value);
            var role = row.Cells["Role"].Value?.ToString();

            // === WALIDACJA: Tylko Kids mogą mieć rodzica ===
            if (role != UserRoles.Kid)
            {
                MessageBox.Show("Only Kid accounts can be assigned to a parent.", "Assign Parent",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Pobierz ID wybranego rodzica z ComboBox
            int? parentId = null;
            if (_cmbAssignParent.SelectedItem is ComboItem item)
            {
                parentId = item.Value;
            }
            else if (_cmbAssignParent.SelectedIndex > 0 && _cmbAssignParent.SelectedItem is string)
            {
                // Nieoczekiwany przypadek - zostaw null
                parentId = null;
            }

            try
            {
                // Przypisz rodzica w bazie
                _userService.AssignParent(userId, parentId);

                // Odśwież listę
                LoadUsers();

                MessageBox.Show("Parent assigned.", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error assigning parent: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handler zmiany zaznaczenia w tabeli użytkowników.
        /// Aktualizuje widoczność sekcji "Assign Parent".
        /// </summary>
        private void GridUsers_SelectionChanged(object sender, EventArgs e)
        {
            UpdateAssignParentVisibility();
        }

        /// <summary>
        /// Pokazuje/ukrywa sekcję "Assign Parent" w zależności od roli wybranego użytkownika.
        /// Sekcja jest widoczna tylko gdy wybrany użytkownik ma rolę Kid.
        /// </summary>
        private void UpdateAssignParentVisibility()
        {
            bool show = false;

            // Sprawdź czy wybrany użytkownik ma rolę Kid
            if (_gridUsers.SelectedRows.Count > 0)
            {
                var role = _gridUsers.SelectedRows[0].Cells["Role"].Value?.ToString();
                show = role == UserRoles.Kid;
            }

            // Pokaż/ukryj kontrolki przypisywania rodzica
            _lblParentInfo.Visible = show;
            _cmbAssignParent.Visible = show;
            _btnAssignParent.Visible = show;
        }

        #endregion

        #region Zmiana hasła

        /// <summary>
        /// Handler przycisku "Change Password" - otwiera dialog zmiany hasła.
        /// 
        /// DIALOG ZAWIERA:
        /// - Pole na nowe hasło
        /// - Pole na potwierdzenie hasła
        /// 
        /// WALIDACJA:
        /// - Hasło nie może być puste
        /// - Hasło musi mieć min. 4 znaki
        /// - Oba pola muszą się zgadzać
        /// </summary>
        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            // Sprawdź czy wybrano użytkownika
            if (_gridUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user first.", "No User Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Pobierz dane wybranego użytkownika
            var row = _gridUsers.SelectedRows[0];
            var userId = Convert.ToInt32(row.Cells["UserId"].Value);
            var userName = row.Cells["UserName"].Value.ToString();

            // === TWORZENIE DIALOGU PROGRAMOWO ===
            using (var dialog = new Form())
            {
                // Konfiguracja okna
                dialog.Text = $"Change Password for {userName}";
                dialog.Size = new Size(350, 180);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                // === POLE: Nowe hasło ===
                var lblPassword = new Label
                {
                    Text = "New Password:",
                    Location = new Point(20, 25),
                    AutoSize = true
                };
                dialog.Controls.Add(lblPassword);

                var txtPassword = new TextBox
                {
                    Location = new Point(20, 50),
                    Width = 290,
                    PasswordChar = '*' // Ukryj hasło
                };
                dialog.Controls.Add(txtPassword);

                // === POLE: Potwierdzenie hasła ===
                var lblConfirm = new Label
                {
                    Text = "Confirm Password:",
                    Location = new Point(20, 80),
                    AutoSize = true
                };
                dialog.Controls.Add(lblConfirm);

                var txtConfirm = new TextBox
                {
                    Location = new Point(140, 77),
                    Width = 170,
                    PasswordChar = '*'
                };
                dialog.Controls.Add(txtConfirm);

                // === PRZYCISKI ===
                var btnOk = new Button
                {
                    Text = "Change",
                    DialogResult = DialogResult.OK,
                    Location = new Point(130, 110),
                    Size = new Size(80, 28),
                    BackColor = Color.FromArgb(52, 152, 219), // Niebieski
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnOk.FlatAppearance.BorderSize = 0;
                dialog.Controls.Add(btnOk);

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(220, 110),
                    Size = new Size(80, 28)
                };
                dialog.Controls.Add(btnCancel);

                // Ustaw domyślne przyciski
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                // === OBSŁUGA WYNIKU ===
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    var newPassword = txtPassword.Text;
                    var confirmPassword = txtConfirm.Text;

                    // --- WALIDACJA: Hasło niepuste ---
                    if (string.IsNullOrWhiteSpace(newPassword))
                    {
                        MessageBox.Show("Password cannot be empty.", "Invalid Password",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // --- WALIDACJA: Min. 4 znaki ---
                    if (newPassword.Length < 4)
                    {
                        MessageBox.Show("Password must be at least 4 characters long.", "Invalid Password",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // --- WALIDACJA: Hasła się zgadzają ---
                    if (newPassword != confirmPassword)
                    {
                        MessageBox.Show("Passwords do not match.", "Invalid Password",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    try
                    {
                        // Zaktualizuj hasło w bazie
                        _userService.UpdateUserPassword(userId, newPassword);

                        MessageBox.Show($"Password for '{userName}' has been updated successfully!",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating password: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region Klasy pomocnicze

        /// <summary>
        /// Klasa pomocnicza do przechowywania par (tekst, wartość) w ComboBox.
        /// Używana do wyświetlania nazw użytkowników z ukrytym ID.
        /// </summary>
        private class ComboItem
        {
            /// <summary>Tekst wyświetlany w ComboBox (nazwa użytkownika).</summary>
            public string Text { get; }

            /// <summary>Wartość ukryta (ID użytkownika).</summary>
            public int Value { get; }

            /// <summary>Tworzy nowy element ComboBox.</summary>
            public ComboItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            /// <summary>Zwraca tekst do wyświetlenia w ComboBox.</summary>
            public override string ToString() => Text;
        }

        #endregion
    }
}
