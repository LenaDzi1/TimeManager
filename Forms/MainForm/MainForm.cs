// ============================================================================
// MainForm.cs
// Główny formularz aplikacji TimeManager - zarządza nawigacją i widokami.
// ============================================================================

#region Importy
using System;                           // Podstawowe typy .NET (EventArgs)
using System.Drawing;                   // Grafika (Color, Point, Size)
using System.Drawing.Drawing2D;         // Zaawansowana grafika (GraphicsPath dla okrągłej kropki)
using System.Windows.Forms;             // Windows Forms (Form, Button, Timer)
using TimeManager.Services;             // Serwisy: Event, Tracking, Notification, FocusMode
using TimeManager.Models;               // Modele: UserSession, Notification
using TimeManager.Forms.Notifications;  // Formularze powiadomień
#endregion

namespace TimeManager.Forms
{
    /// <summary>
    /// Główny formularz aplikacji TimeManager.
    /// 
    /// GŁÓWNE ODPOWIEDZIALNOŚCI:
    /// - Nawigacja między widokami (Kalendarz, Tracking, Statystyki, Home Browser)
    /// - Top bar z przyciskami nawigacyjnymi i ikonami (powiadomienia, konto)
    /// - Timer sprawdzający powiadomienia co 30 sekund
    /// - Funkcja "View As" dla rodziców (podgląd kalendarza dziecka)
    /// - Obsługa ról użytkowników (Kid ma ograniczenia)
    /// 
    /// PRZEPŁYW STARTOWY:
    /// 1. Inicjalizacja serwisów (Event, Tracking, Notification, FocusMode)
    /// 2. Konfiguracja top bar i kropki powiadomień
    /// 3. Setup "View As" selector (dla rodziców z dziećmi)
    /// 4. Aplikacja ograniczeń roli
    /// 5. Start timera powiadomień
    /// 6. Pokazanie widoku kalendarza (domyślny)
    /// </summary>
    public partial class MainForm : Form
    {
        #region Pola prywatne - Serwisy

        /// <summary>Serwis do operacji na eventach (CRUD, planowanie).</summary>
        private EventService _eventService;

        /// <summary>Serwis do trackingu (jedzenie, leki, rośliny).</summary>
        private TrackingService _trackingService;

        /// <summary>Serwis powiadomień (tworzenie, pobieranie, usuwanie).</summary>
        private NotificationService _notificationService;

        /// <summary>Serwis trybu skupienia (focus mode).</summary>
        private FocusModeService _focusModeService;

        /// <summary>Serwis użytkowników (dla funkcji View As).</summary>
        private UserService _userService;

        #endregion

        #region Pola prywatne - UI

        /// <summary>Timer do cyklicznego sprawdzania nowych powiadomień.</summary>
        private Timer _notificationCheckTimer;

        /// <summary>Aktualnie podświetlony przycisk nawigacji.</summary>
        private Button _activeNavButton;

        #endregion

        #region Konstruktor

        /// <summary>
        /// Konstruktor głównego formularza.
        /// Inicjalizuje wszystkie serwisy, konfiguruje UI i pokazuje kalendarz.
        /// </summary>
        public MainForm()
        {
            // Inicjalizuj kontrolki z Designer.cs
            InitializeComponent();

            // Konfiguracja wyglądu top bar
            ConfigureTopBarButtons();
            ConfigureNotificationDot();

            // Inicjalizacja serwisów aplikacji
            InitializeServices();

            // Podepnij handlery kliknięć przycisków
            ConnectEvents();

            // Konfiguracja "View As" (podgląd kalendarza dziecka dla rodziców)
            SetupViewAsSelector();

            // Zastosuj ograniczenia oparte na roli użytkownika
            ApplyRoleRestrictions();

            // Start timera sprawdzającego powiadomienia co 30 sekund
            StartNotificationChecker();

            // Pokaż domyślny widok - kalendarz
            ShowCalendarView();
        }

        #endregion

        #region Inicjalizacja serwisów

        /// <summary>
        /// Tworzy instancje wszystkich serwisów aplikacji.
        /// Wywoływane raz podczas inicjalizacji formularza.
        /// </summary>
        private void InitializeServices()
        {
            _eventService = new EventService();
            _trackingService = new TrackingService();
            _notificationService = new NotificationService();
            _focusModeService = new FocusModeService();
            _userService = new UserService();
        }

        #endregion

        #region Konfiguracja powiadomień

        /// <summary>
        /// Konfiguruje czerwoną kropkę wskazującą nieprzeczytane powiadomienia.
        /// Kropka jest okrągła (10x10 px) i pozycjonowana przy przycisku dzwonka.
        /// </summary>
        private void ConfigureNotificationDot()
        {
            if (_notificationDot == null) return;

            // Ustaw rozmiar i kolor kropki
            _notificationDot.Size = new Size(10, 10);
            _notificationDot.BackColor = Color.Red;
            _notificationDot.Visible = false; // Ukryta domyślnie

            // Uczyń kropkę okrągłą używając Region
            var path = new GraphicsPath();
            path.AddEllipse(0, 0, 10, 10);
            _notificationDot.Region = new Region(path);

            // Upewnij się że jest na wierzchu
            _notificationDot.BringToFront();
        }

        /// <summary>
        /// Uruchamia timer sprawdzający powiadomienia co 30 sekund.
        /// Przy starcie od razu sprawdza i aktualizuje kropkę.
        /// </summary>
        private void StartNotificationChecker()
        {
            // Timer sprawdza co 30 sekund
            _notificationCheckTimer = new Timer();
            _notificationCheckTimer.Interval = 30000; // 30 sekund
            _notificationCheckTimer.Tick += (s, e) => UpdateNotificationDot();
            _notificationCheckTimer.Start();

            // Początkowe sprawdzenie - od razu po zalogowaniu
            // (żeby czerwona kropka była widoczna natychmiast)
            TryRefreshTrackingNotifications();
            UpdateNotificationDot();
        }

        /// <summary>
        /// Próbuje wygenerować powiadomienia o trackingu (wygasające produkty, itp.).
        /// Ignoruje błędy - to funkcja pomocnicza.
        /// </summary>
        private void TryRefreshTrackingNotifications()
        {
            try
            {
                _notificationService?.CheckAndCreateTrackingNotifications();
            }
            catch
            {
                // Ignoruj błędy generowania powiadomień
            }
        }

        /// <summary>
        /// Aktualizuje widoczność czerwonej kropki powiadomień.
        /// Widoczna = są nieprzeczytane powiadomienia.
        /// </summary>
        private void UpdateNotificationDot()
        {
            try
            {
                if (_notificationService != null && _notificationDot != null)
                {
                    var notifications = _notificationService.GetActiveNotifications();
                    _notificationDot.Visible = notifications != null && notifications.Count > 0;
                }
            }
            catch
            {
                // Ignoruj błędy sprawdzania powiadomień
            }
        }

        #endregion

        #region Konfiguracja ról i ograniczeń

        /// <summary>
        /// Stosuje ograniczenia UI oparte na roli użytkownika.
        /// Aktualnie: wszystkie przyciski włączone (ograniczenia wewnątrz widoków).
        /// </summary>
        private void ApplyRoleRestrictions()
        {
            // Statystyki dostępne dla wszystkich (z ograniczeniami wewnątrz)
            _btnStatistics.Enabled = true;
            _btnStatistics.BackColor = Color.FromArgb(52, 73, 94);
        }

        #endregion

        #region Konfiguracja Top Bar

        /// <summary>
        /// Konfiguruje wszystkie przyciski w top bar.
        /// Lewe przyciski: nawigacja (tekstowe).
        /// Prawe przyciski: ikony (emoji).
        /// </summary>
        private void ConfigureTopBarButtons()
        {
            // === LEWA STRONA - przyciski nawigacyjne z tekstem ===
            ConfigureNavButton(_btnToggleMenu, "Home Browser");
            ConfigureNavButton(_btnTopCalendar, "Calendar");
            ConfigureNavButton(_btnFridgeKitchen, "Tracking");
            ConfigureNavButton(_btnStatistics, "Statistics");

            // === PRAWA STRONA - przyciski ikonowe z emoji ===
            ConfigureRightIconButton(_btnNotifications, "🔔"); // Dzwonek
            ConfigureRightIconButton(_btnAccount, "👤");       // Osoba

            // Pozycjonowanie lewych przycisków (z odstępami)
            int x = 12;
            _btnToggleMenu.Location = new Point(x, 10);
            x += _btnToggleMenu.Width + 8;
            _btnTopCalendar.Location = new Point(x, 10);
            x += _btnTopCalendar.Width + 8;
            _btnFridgeKitchen.Location = new Point(x, 10);
            x += _btnFridgeKitchen.Width + 8;
            _btnStatistics.Location = new Point(x, 10);

            // Pozycjonowanie prawych przycisków (od prawej krawędzi)
            int rightStartX = _topBar.Width - 12 - 44; // Prawa krawędź - padding - szerokość przycisku
            _btnAccount.Location = new Point(rightStartX, 10);
            _btnAccount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnNotifications.Location = new Point(rightStartX - 54, 10);
            _btnNotifications.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Domyślne podświetlenie - kalendarz (domyślny widok)
            HighlightNavButton(_btnTopCalendar);
        }

        /// <summary>
        /// Konfiguruje wygląd przycisku nawigacyjnego (lewa strona top bar).
        /// </summary>
        /// <param name="btn">Przycisk do skonfigurowania</param>
        /// <param name="text">Tekst do wyświetlenia</param>
        private void ConfigureNavButton(Button btn, string text)
        {
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(150, 150, 150); // Szara ramka
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(176, 224, 230); // Jasnoniebieski hover
            btn.BackColor = Color.FromArgb(200, 230, 245); // Jasny błękit
            btn.ForeColor = Color.FromArgb(30, 80, 120);   // Ciemnoniebieski tekst
            btn.Cursor = Cursors.Hand;
            btn.TabStop = false; // Zapobiega pokazywaniu focusa
        }

        /// <summary>
        /// Konfiguruje wygląd przycisku ikonowego (prawa strona top bar).
        /// </summary>
        /// <param name="btn">Przycisk do skonfigurowania</param>
        /// <param name="emoji">Emoji do wyświetlenia</param>
        private void ConfigureRightIconButton(Button btn, string emoji)
        {
            btn.Text = emoji;
            btn.Font = new Font("Segoe UI", 14, FontStyle.Regular); // Większe emoji
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 128, 185); // Ciemniejszy niebieski
            btn.BackColor = Color.FromArgb(52, 152, 219); // Niebieski
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Podświetla wybrany przycisk nawigacji (aktywny widok).
        /// Resetuje styl pozostałych przycisków.
        /// </summary>
        /// <param name="btn">Przycisk do podświetlenia</param>
        private void HighlightNavButton(Button btn)
        {
            // Resetuj wszystkie przyciski do domyślnego stylu
            foreach (var navBtn in new[] { _btnToggleMenu, _btnTopCalendar, _btnFridgeKitchen, _btnStatistics })
            {
                navBtn.BackColor = Color.FromArgb(200, 230, 245);
                navBtn.ForeColor = Color.FromArgb(30, 80, 120);
                navBtn.FlatAppearance.BorderColor = Color.FromArgb(150, 150, 150);
                navBtn.FlatAppearance.BorderSize = 1;
            }

            // Podświetl wybrany przycisk - ciemniejszy niebieski
            btn.BackColor = Color.FromArgb(52, 152, 219);
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = Color.FromArgb(41, 128, 185);
            btn.FlatAppearance.BorderSize = 2;

            _activeNavButton = btn;
        }

        #endregion

        #region "View As" - Podgląd dziecka

        /// <summary>
        /// Konfiguruje ComboBox "View As" dla rodziców.
        /// Pozwala rodzicowi przeglądać kalendarz swojego dziecka.
        /// 
        /// WIDOCZNOŚĆ:
        /// - Tylko dla użytkowników z rolą User
        /// - Tylko jeśli mają przypisane dzieci (Kids)
        /// </summary>
        private void SetupViewAsSelector()
        {
            // Odepnij handler żeby nie duplikować przy re-loginie
            if (_cmbViewAs != null)
                _cmbViewAs.SelectedIndexChanged -= CmbViewAs_SelectedIndexChanged;

            // Domyślnie ukryj kontrolki
            if (_lblViewAs != null) _lblViewAs.Visible = false;
            if (_cmbViewAs != null)
            {
                _cmbViewAs.Visible = false;
                _cmbViewAs.Items.Clear();
            }

            // Kids i admini nie widzą "View As"
            if (!UserSession.IsUser)
                return;

            // Sprawdź czy użytkownik ma dzieci
            var children = _userService.GetChildren(UserSession.UserId);
            if (children == null || children.Count == 0)
                return;

            // Pokaż kontrolki
            if (_lblViewAs != null) _lblViewAs.Visible = true;
            if (_cmbViewAs != null) _cmbViewAs.Visible = true;

            // Wypełnij ComboBox: "Ja" + lista dzieci
            _cmbViewAs.Items.Add(new ViewAsOption(UserSession.UserId, $"Me ({UserSession.UserName})"));

            foreach (var child in children)
            {
                _cmbViewAs.Items.Add(new ViewAsOption(child.UserId, $"Child: {child.UserName}"));
            }

            // Wybierz aktualny kontekst
            int ctx = UserSession.ContextUserId;
            for (int i = 0; i < _cmbViewAs.Items.Count; i++)
            {
                if (_cmbViewAs.Items[i] is ViewAsOption opt && opt.UserId == ctx)
                {
                    _cmbViewAs.SelectedIndex = i;
                    break;
                }
            }
            if (_cmbViewAs.SelectedIndex == -1 && _cmbViewAs.Items.Count > 0)
                _cmbViewAs.SelectedIndex = 0;

            // Podepnij handler zmiany
            _cmbViewAs.SelectedIndexChanged += CmbViewAs_SelectedIndexChanged;
        }

        /// <summary>
        /// Handler zmiany wyboru w ComboBox "View As".
        /// Przełącza kontekst użytkownika i odświeża widok.
        /// </summary>
        private void CmbViewAs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cmbViewAs?.SelectedItem is ViewAsOption opt)
            {
                if (opt.UserId == UserSession.UserId)
                    UserSession.SetViewedUser(null); // Przełącz na siebie
                else
                    UserSession.SetViewedUser(opt.UserId); // Przełącz na dziecko

                // Odśwież aktualny widok
                RefreshCurrentView();
            }
        }

        /// <summary>
        /// Odświeża aktualnie wyświetlany widok po zmianie kontekstu.
        /// </summary>
        private void RefreshCurrentView()
        {
            if (_mainPanel.Controls.Count > 0)
            {
                var currentControl = _mainPanel.Controls[0];
                if (currentControl is CalendarView)
                {
                    ShowCalendarView();
                }
                else if (currentControl is TrackingView)
                {
                    ShowTrackingView();
                }
                else if (currentControl is StatisticsView)
                {
                    ShowStatisticsView();
                }
            }
        }

        /// <summary>
        /// Klasa pomocnicza dla ComboBox "View As".
        /// Przechowuje ID użytkownika i tekst do wyświetlenia.
        /// </summary>
        private class ViewAsOption
        {
            public int UserId { get; }
            public string Text { get; }
            public ViewAsOption(int id, string text) { UserId = id; Text = text; }
            public override string ToString() => Text;
        }

        #endregion

        #region Podpinanie zdarzeń

        /// <summary>
        /// Podpina handlery kliknięć do przycisków nawigacyjnych.
        /// </summary>
        private void ConnectEvents()
        {
            _btnToggleMenu.Click += (s, e) => ShowHomeBrowserView();
            _btnTopCalendar.Click += (s, e) => ShowCalendarView();
            _btnFridgeKitchen.Click += (s, e) => ShowTrackingView();
            _btnStatistics.Click += (s, e) => ShowStatisticsView();
            _btnNotifications.Click += (s, e) => ShowNotificationsForm();
            _btnAccount.Click += (s, e) => ShowAccountMenu();
        }

        #endregion

        #region Nawigacja - Widoki

        /// <summary>
        /// Pokazuje widok kalendarza (domyślny widok aplikacji).
        /// </summary>
        private void ShowCalendarView()
        {
            _mainPanel.Controls.Clear();
            var calendar = new CalendarView(_eventService)
            {
                Dock = DockStyle.Fill
            };
            _mainPanel.Controls.Add(calendar);
            HighlightNavButton(_btnTopCalendar);
        }

        /// <summary>
        /// Pokazuje widok trackingu (jedzenie, leki, rośliny).
        /// </summary>
        /// <param name="targetSection">Opcjonalna sekcja do nawigacji (fridge, medication, plant)</param>
        /// <param name="referenceId">Opcjonalne ID elementu do podświetlenia</param>
        private void ShowTrackingView(string targetSection = null, int? referenceId = null)
        {
            _mainPanel.Controls.Clear();
            var tracking = new TrackingView(_trackingService, _eventService, null)
            {
                Dock = DockStyle.Fill
            };

            // Jeśli podano sekcję - nawiguj do niej
            if (!string.IsNullOrEmpty(targetSection))
            {
                tracking.NavigateToNotification(targetSection, referenceId);
            }

            _mainPanel.Controls.Add(tracking);
            HighlightNavButton(_btnFridgeKitchen);
        }

        /// <summary>
        /// Pokazuje widok statystyk.
        /// </summary>
        private void ShowStatisticsView()
        {
            _mainPanel.Controls.Clear();
            var statistics = new StatisticsView
            {
                Dock = DockStyle.Fill
            };
            _mainPanel.Controls.Add(statistics);
            HighlightNavButton(_btnStatistics);
        }

        /// <summary>
        /// Pokazuje widok Home Browser (przeglądarka domu).
        /// </summary>
        private void ShowHomeBrowserView()
        {
            _mainPanel.Controls.Clear();
            var browser = new HomeBrowserForm
            {
                Dock = DockStyle.Fill
            };
            _mainPanel.Controls.Add(browser);
            HighlightNavButton(_btnToggleMenu);
        }

        #endregion

        #region Konto użytkownika

        /// <summary>
        /// Pokazuje dialog konta użytkownika.
        /// Po zamknięciu sprawdza czy użytkownik chce się wylogować.
        /// </summary>
        private void ShowAccountMenu()
        {
            using var dialog = new AccountDialog();
            var result = dialog.ShowDialog(this);

            if (dialog.LogoutRequested)
            {
                HandleLogoutFlow();
                return;
            }
        }

        /// <summary>
        /// Obsługuje proces wylogowania użytkownika.
        /// 
        /// PRZEPŁYW:
        /// 1. Wyczyść zapisane dane logowania
        /// 2. Wyloguj z sesji
        /// 3. Pokaż formularz logowania
        /// 4. Po zalogowaniu:
        ///    - Admin → otwórz AdminPanel i zamknij MainForm
        ///    - User/Kid → odśwież MainForm
        ///    - Anulowano → zamknij aplikację
        /// </summary>
        private void HandleLogoutFlow()
        {
            // Wyczyść zapisane dane logowania
            LoginForm.ClearSavedCredentials();

            // Wyloguj użytkownika
            UserSession.Logout();
            Hide();

            // Pokaż formularz logowania
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK && UserSession.IsAuthenticated)
                {
                    if (UserSession.IsAdministrator)
                    {
                        // Admin → otwórz panel admina zamiast MainForm
                        Hide();
                        using (var admin = new AdminPanelForm())
                        {
                            admin.ShowDialog(this);
                        }
                        Close();
                        return;
                    }

                    // User/Kid → odśwież Main Form
                    ApplyRoleRestrictions();
                    SetupViewAsSelector();
                    Show();
                    ShowCalendarView();

                    // Odśwież powiadomienia po re-loginie
                    TryRefreshTrackingNotifications();
                    UpdateNotificationDot();
                }
                else
                {
                    // Anulowano logowanie → zamknij aplikację
                    Close();
                }
            }
        }

        #endregion

        #region Powiadomienia

        /// <summary>
        /// Pokazuje formularz powiadomień jako dialog.
        /// Po zamknięciu aktualizuje kropkę powiadomień.
        /// </summary>
        private void ShowNotificationsForm()
        {
            using var form = new NotificationsForm(_notificationService, HandleNotificationNavigation);
            form.ShowDialog(this);

            // Aktualizuj kropkę (powiadomienia mogły być odrzucone)
            UpdateNotificationDot();
        }

        /// <summary>
        /// Obsługuje nawigację z powiadomienia do odpowiedniego widoku.
        /// </summary>
        /// <param name="notification">Powiadomienie które wywołało nawigację</param>
        private void HandleNotificationNavigation(Notification notification)
        {
            var type = (notification.NotificationType ?? string.Empty).ToLowerInvariant();

            switch (type)
            {
                case "foodtracking":
                case "food":
                case "fridge":
                case "foodexpiry":
                    ShowTrackingView("fridge", notification.ReferenceID);
                    break;

                case "medicinetracking":
                case "medicine":
                case "medication":
                    ShowTrackingView("medication", notification.ReferenceID);
                    break;

                case "planttracker":
                case "plant":
                    ShowTrackingView("plant", notification.ReferenceID);
                    break;

                default:
                    MessageBox.Show("No associated view for this notification.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        #endregion
    }
}
