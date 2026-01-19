// ============================================================================
// CalendarView.cs
// Główny widok kalendarza aplikacji TimeManager.
// Obsługuje tryby: Month, Week, Day oraz godziny snu.
// ============================================================================

#nullable enable

#region Importy
using System;                   // Podstawowe typy .NET (DateTime, Math)
using System.Collections.Generic; // Kolekcje (List, Dictionary)
using System.Drawing;           // Grafika (Color, Point, Size)
using System.Linq;              // LINQ (Where, OrderBy)
using System.Windows.Forms;     // Windows Forms (UserControl, Panel)
using System.Reflection;        // Reflection (do ustawienia DoubleBuffered)
using TimeManager.Models;       // Modele (Event, UserSession)
using TimeManager.Services;     // Serwisy (EventService, UserService)
#endregion

namespace TimeManager.Forms
{
    /// <summary>
    /// Główny widok kalendarza aplikacji TimeManager.
    /// 
    /// TRYBY WYŚWIETLANIA:
    /// - Month: widok miesięczny (siatka 6x7 dni z numerami tygodni)
    /// - Week: widok tygodniowy (7 kolumn × 24 godziny)
    /// - Day: widok dzienny (1 kolumna × 24 godziny)
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Nawigacja między datami (prev/next/today)
    /// - Dodawanie/edycja/usuwanie eventów
    /// - Quick Tasks (lista szybkich zadań P1)
    /// - Wyświetlanie godzin snu (ukośne paski)
    /// - Zmiana pierwszego dnia tygodnia
    /// 
    /// ARCHITEKTURA:
    /// - _calendarPanelHost: panel-host dla aktywnego widoku
    /// - _monthViewPanel, _weekViewPanel, _dayViewPanel: panele widoków
    /// - SetActiveView(): przełącza aktywny panel
    /// </summary>
    public partial class CalendarView : UserControl
    {
        #region Pola prywatne - Serwisy

        /// <summary>Serwis eventów (CRUD, scheduling).</summary>
        private readonly EventService? _eventService;

        /// <summary>Serwis użytkowników (do pobierania harmonogramu snu).</summary>
        private readonly UserService _userService = new();

        #endregion

        #region Pola prywatne - Stan widoku

        /// <summary>Aktualnie wyświetlana data (centrum nawigacji).</summary>
        private DateTime _currentDate;

        /// <summary>Tryb widoku: "month", "week", "day".</summary>
        private string _viewMode = "month";

        /// <summary>Pierwszy dzień tygodnia (domyślnie poniedziałek).</summary>
        private DayOfWeek _firstDayOfWeek = DayOfWeek.Monday;

        /// <summary>Flaga czy kalendarz został już zainicjalizowany.</summary>
        private bool _isInitialized = false;

        #endregion

        #region Pola prywatne - Panele widoków

        /// <summary>Panel widoku miesięcznego.</summary>
        private Panel? _monthViewPanel;

        /// <summary>Panel widoku tygodniowego.</summary>
        private Panel? _weekViewPanel;

        /// <summary>Panel widoku dziennego.</summary>
        private Panel? _dayViewPanel;

        /// <summary>Panel-host z designera dla aktywnego widoku.</summary>
        private Panel? _calendarPanelHost;

        /// <summary>Panel kalendarza w widoku miesiąca.</summary>
        private Panel? _monthCalendarPanel;

        /// <summary>Panel nagłówków dni w widoku miesiąca.</summary>
        private Panel? _monthDayHeadersPanel;

        /// <summary>Przycisk wyboru pierwszego dnia tygodnia w widoku miesiąca.</summary>
        private Button? _monthBtnWeekStartSelector;

        /// <summary>Panel overlay wyświetlany podczas ładowania.</summary>
        private Panel? _loadingOverlay;

        #endregion

        #region Konstruktory

        /// <summary>
        /// Konstruktor bezparametrowy - tylko dla DESIGNERA.
        /// Nie inicjalizuje serwisów ani widoków runtime.
        /// </summary>
        public CalendarView()
        {
            InitializeComponent();
            // W trybie design nic więcej nie robimy
        }


        /// <summary>
        /// Konstruktor runtime - inicjalizuje kalendarz z serwisem eventów.
        /// 
        /// KOLEJNOŚĆ INICJALIZACJI:
        /// 1. InitializeComponent() (przez this())
        /// 2. Zachowaj panel-host z designera
        /// 3. Utwórz panele widoków (Month/Week/Day)
        /// 4. Skonfiguruj double-buffering
        /// 5. Podepnij handlery zdarzeń
        /// 6. Przesuń nieukończone eventy (jeśli minęło 24h+)
        /// </summary>
        /// <param name="eventService">Serwis eventów (wymagany)</param>
        public CalendarView(EventService eventService) : this()
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
            _currentDate = DateTime.Now;

            // Zachowaj panel z designera jako host dla dynamicznych widoków
            _calendarPanelHost = _calendarPanel;
            InitializeViewPanels();

            if (!DesignMode)
            {
                _toolTip = new ToolTip();
                ConfigureDoubleBuffering();
                WireUpEvents();

                // Automatycznie przesuń nieukończone eventy które są 24+ godzin po czasie końca
                try
                {
                    _eventService.RescheduleIncompleteEvents();
                }
                catch
                {
                    // Jeśli przesunięcie się nie uda, nie blokuj ładowania kalendarza
                }

                // Renderuj dopiero gdy kontrolka stanie się widoczna (optymalizacja)
                this.VisibleChanged += CalendarView_VisibleChanged;

                // Przerenderuj przy zmianie rozmiaru
                this.Resize += CalendarView_Resize;
            }
        }

        #endregion

        #region Inicjalizacja

        /// <summary>
        /// Handler zdarzenia VisibleChanged - inicjalizuje kalendarz przy pierwszym wyświetleniu.
        /// </summary>
        private void CalendarView_VisibleChanged(object? sender, EventArgs e)
        {
            // Renderuj kalendarz gdy stanie się widoczny po raz pierwszy
            if (this.Visible && !_isInitialized)
            {
                InitializeCalendarView();
            }
        }

        /// <summary>
        /// Inicjalizuje widok kalendarza.
        /// Jeśli panel jest za mały (< 200px), czeka z timerem aż layout się ustabilizuje.
        /// </summary>
        private void InitializeCalendarView()
        {
            // Sprawdź czy mamy prawidłowy rozmiar (panel musi być min. 200x200)
            if (_calendarPanelHost == null || _calendarPanelHost.Width < 200 || _calendarPanelHost.Height < 200)
            {
                // Rozmiar jeszcze nie gotowy - poczekaj na layout
                var timer = new Timer();
                timer.Interval = 50; // 50ms opóźnienie
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    timer.Dispose();

                    // Sprawdź ponownie czy można zainicjalizować
                    if (!_isInitialized)
                    {
                        if (_calendarPanelHost != null && _calendarPanelHost.Width >= 200 && _calendarPanelHost.Height >= 200)
                        {
                            _isInitialized = true;
                            RenderInitialCalendar();
                        }
                        else
                        {
                            // Wciąż nie gotowe - spróbuj ponownie
                            InitializeCalendarView();
                        }
                    }
                };
                timer.Start();
                return;
            }

            _isInitialized = true;
            RenderInitialCalendar();
        }

        /// <summary>
        /// Renderuje początkowy kalendarz (widok miesięczny).
        /// </summary>
        private void RenderInitialCalendar()
        {
            // Wymuś obliczenie layoutu przed renderowaniem
            if (_calendarPanelHost != null)
            {
                _calendarPanelHost.PerformLayout();
            }

            // Ustaw widok i wyrenderuj
            ChangeViewMode("month");
            UpdateWeekStartSelectorPosition();
            SetupWeekStartSelectorHover();
        }

        /// <summary>
        /// Handler zdarzenia Resize - przerenderowuje kalendarz przy zmianie rozmiaru.
        /// </summary>
        private void CalendarView_Resize(object? sender, EventArgs e)
        {
            // Jeśli jeszcze nie zainicjalizowany ale teraz ma prawidłowy rozmiar, zainicjalizuj
            if (!_isInitialized && this.Visible && _calendarPanelHost != null &&
                _calendarPanelHost.Width >= 200 && _calendarPanelHost.Height >= 200)
            {
                _isInitialized = true;
                RenderInitialCalendar();
                return;
            }

            // Wycentruj etykietę z datą względem panelu kalendarza
            CenterHeaderLabel();

            // Przerenderuj tylko jeśli już zainicjalizowany i ma prawidłowy rozmiar
            if (_isInitialized && this.Visible && this.Width > 100 && this.Height > 100)
            {
                LoadCalendar();
            }
        }

        /// <summary>
        /// Centruje etykietę z datą (_lblCurrentDate) względem panelu kalendarza.
        /// </summary>
        private void CenterHeaderLabel()
        {
            if (_lblCurrentDate == null || _calendarPanelHost == null || _headerPanel == null) return;

            // Wycentruj względem szerokości panelu kalendarza, nie całego nagłówka
            var calendarPanelWidth = _calendarPanelHost.Width;
            var calendarPanelLeft = _calendarPanelHost.Left;
            var labelWidth = _lblCurrentDate.Width;

            // Oblicz pozycję środka względem headerPanel
            var centerX = calendarPanelLeft + (calendarPanelWidth / 2) - (labelWidth / 2);
            _lblCurrentDate.Location = new Point(centerX, _lblCurrentDate.Location.Y);
        }

        /// <summary>
        /// Konfiguruje double-buffering dla kontrolki i panelu kalendarza.
        /// Minimalizuje migotanie przy renderowaniu.
        /// </summary>
        private void ConfigureDoubleBuffering()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            // Włącz double-buffering dla panelu kalendarza przez reflection
            if (_calendarPanel != null)
            {
                try
                {
                    typeof(Panel).InvokeMember(
                        "DoubleBuffered",
                        BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        _calendarPanel,
                        new object[] { true });
                }
                catch
                {
                    // Jeśli się nie uda (np. w designerze) - po prostu pomiń
                }
            }
        }

        /// <summary>
        /// Podpina wszystkie handlery zdarzeń (tylko w runtime).
        /// </summary>
        private void WireUpEvents()
        {
            if (DesignMode) return;

            // Przyciski nawigacji
            if (_btnPrevious != null) _btnPrevious.Click += BtnPrevious_Click;
            if (_btnNext != null) _btnNext.Click += BtnNext_Click;
            if (_btnToday != null) _btnToday.Click += BtnToday_Click;

            // Przycisk dodawania eventów
            if (_btnAddEvent != null) _btnAddEvent.Click += BtnAddEvent_Click;

            // Przyciski zmiany trybu widoku
            if (_btnMonthView != null) _btnMonthView.Click += BtnMonthView_Click;
            if (_btnWeekView != null) _btnWeekView.Click += BtnWeekView_Click;
            if (_btnDayView != null) _btnDayView.Click += BtnDayView_Click;

            // Quick Tasks i wybór pierwszego dnia tygodnia
            if (_btnQuickTasks != null) _btnQuickTasks.Click += BtnQuickTasks_Click;
            if (_btnWeekStartSelector != null) _btnWeekStartSelector.Click += BtnWeekStartSelector_Click;

            // Checkbox godzin snu
            if (_chkShowSleepHours != null) _chkShowSleepHours.CheckedChanged += ChkShowSleepHours_CheckedChanged;
        }

        #endregion

        private void ChkShowSleepHours_CheckedChanged(object? sender, EventArgs e)
        {
            // Odśwież widok aby pokazać/ukryć godziny snu
            LoadCalendar();
        }

        #region Handlery kliknięć

        private void BtnPrevious_Click(object? sender, EventArgs e) => NavigatePrevious();
        private void BtnNext_Click(object? sender, EventArgs e) => NavigateNext();
        private void BtnToday_Click(object? sender, EventArgs e) => NavigateToday();
        private void BtnMonthView_Click(object? sender, EventArgs e) => ChangeViewMode("month");
        private void BtnWeekView_Click(object? sender, EventArgs e) => ChangeViewMode("week");
        private void BtnDayView_Click(object? sender, EventArgs e) => ChangeViewMode("day");
        private void BtnAddEvent_Click(object? sender, EventArgs e) => ShowAddEventDialog();
        private void BtnQuickTasks_Click(object? sender, EventArgs e) => ShowQuickTasks();

        private void ShowQuickTasks()
        {
            if (_eventService == null) return;

            var (from, toExclusive) = GetCurrentViewRange();
            // Użyj GetEvents() zamiast GetEventsWithScheduling() dla szybszego ładowania
            // Scheduler uruchamia się tylko przy dodawaniu/edycji eventów
            var events = _eventService.GetEvents(from, toExclusive);

            // "Szybkie zadania": niskie priorytety i NIE ustalone (ustawione) zdarzenia.
            var quick = events
                .Where(ev => ev.Priority == 1 && !ev.IsSetEvent)
                .OrderBy(ev => ev.StartDateTime ?? DateTime.MaxValue)
                .ToList();

            using var dlg = new QuickTasksForm(quick, from, toExclusive.AddDays(-1));
            dlg.ShowDialog(this);
        }

        private (DateTime from, DateTime toExclusive) GetCurrentViewRange()
        {
            if (_viewMode == "day")
            {
                var d = _currentDate.Date;
                return (d, d.AddDays(1));
            }

            if (_viewMode == "week")
            {
                int firstDayOffset = ((int)_firstDayOfWeek + 6) % 7;
                int currentDayOffset = ((int)_currentDate.DayOfWeek + 6) % 7;
                var weekStart = _currentDate.AddDays(-currentDayOffset + firstDayOffset).Date;
                if (weekStart > _currentDate.Date) weekStart = weekStart.AddDays(-7);
                return (weekStart, weekStart.AddDays(7));
            }

            // miesiąc
            var first = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            return (first, first.AddMonths(1));
        }

        private void BtnWeekStartSelector_Click(object? sender, EventArgs e)
        {
            // Pokaż menu kontekstowe z wyborem początku tygodnia
            ContextMenuStrip menu = new ContextMenuStrip();

            string[] dayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            DayOfWeek[] dayOfWeeks = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                                      DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };

            for (int i = 0; i < 7; i++)
            {
                int index = i; // Przechwyć dla zamknięcia
                ToolStripMenuItem item = new ToolStripMenuItem(dayNames[i]);
                item.Click += (s, args) =>
                {
                    _firstDayOfWeek = dayOfWeeks[index];
                    UpdateWeekStartSelectorPosition();
                    SetupWeekStartSelectorHover(); // Zaktualizuj hover na nowym pierwszym dniu
                    LoadCalendar(); // Przeładuj kalendarz z nowym początkiem tygodnia
                    menu.Close();
                };
                menu.Items.Add(item);
            }

            // Pokaż menu POD kontrolką (przyciskiem lub labelem)
            // Użyj sender jako kontrolki do pozycjonowania, lub fallback na _dayHeaderLabels[0]
            Control? positionControl = (sender as Control) ?? (_btnWeekStartSelector as Control) ?? (_dayHeaderLabels != null ? _dayHeaderLabels[0] as Control : null);
            if (positionControl != null)
            {
                Point location = positionControl.PointToScreen(new Point(0, positionControl.Height));
                menu.Show(location);
            }
            else
            {
                // Ostateczny fallback - pokaż menu w miejscu kursora
                menu.Show(Cursor.Position);
            }
        }

        private void UpdateWeekStartSelectorPosition()
        {
            if (_dayHeadersPanel == null || _btnWeekStartSelector == null) return;

            // Przycisk zawsze na pierwszym dniu (pozycja 0), niezale nie od wybranego pocz tku tygodnia
            // Przycisk po lewej stronie, obok numerów tygodni
            _btnWeekStartSelector.Location = new Point(5, 5);
            _btnWeekStartSelector.BringToFront();
        }

        private void SetupWeekStartSelectorHover()
        {
            if (_dayHeaderLabels == null || _dayHeaderLabels[0] == null) return;

            // Użyj wspólnego tooltip (utwórz jeśli nie istnieje)
            if (_toolTip == null)
            {
                _toolTip = new ToolTip();
            }

            _toolTip.SetToolTip(_dayHeaderLabels[0], "Kliknij, aby zmienic poczatek tygodnia");
            if (_btnWeekStartSelector != null)
            {
                _toolTip.SetToolTip(_btnWeekStartSelector, "Kliknij, aby zmienic poczatek tygodnia");
            }

            // Usuń poprzednie event handlery jeśli istnieją
            _dayHeaderLabels[0].MouseEnter -= DayHeaderLabel0_MouseEnter;
            _dayHeaderLabels[0].MouseLeave -= DayHeaderLabel0_MouseLeave;
            _dayHeaderLabels[0].Click -= DayHeaderLabel0_Click;

            // Dodaj nowe event handlery
            _dayHeaderLabels[0].MouseEnter += DayHeaderLabel0_MouseEnter;
            _dayHeaderLabels[0].MouseLeave += DayHeaderLabel0_MouseLeave;
            _dayHeaderLabels[0].Click += DayHeaderLabel0_Click; // Kliknięcie w label również otwiera menu

            // Również na przycisku, aby nie znikał gdy mysz jest nad nim
            if (_btnWeekStartSelector != null)
            {
                _btnWeekStartSelector.MouseEnter -= BtnWeekStartSelector_MouseEnter;
                _btnWeekStartSelector.MouseLeave -= BtnWeekStartSelector_MouseLeave;

                _btnWeekStartSelector.MouseEnter += BtnWeekStartSelector_MouseEnter;
                _btnWeekStartSelector.MouseLeave += BtnWeekStartSelector_MouseLeave;
            }
        }

        private void DayHeaderLabel0_Click(object? sender, EventArgs e)
        {
            // Kliknięcie w pierwszy label dnia również otwiera menu
            BtnWeekStartSelector_Click(_btnWeekStartSelector, e);
        }

        private void DayHeaderLabel0_MouseEnter(object? sender, EventArgs e)
        {
            // Przycisk jest teraz zawsze widoczny, tylko przesuń na wierzch
            if (_btnWeekStartSelector != null)
            {
                _btnWeekStartSelector.BringToFront();
            }
        }

        private void DayHeaderLabel0_MouseLeave(object? sender, EventArgs e)
        {
            // Przycisk jest teraz zawsze widoczny
        }

        private void BtnWeekStartSelector_MouseEnter(object? sender, EventArgs e)
        {
            // Przycisk jest teraz zawsze widoczny
        }

        private void BtnWeekStartSelector_MouseLeave(object? sender, EventArgs e)
        {
            // Przycisk jest teraz zawsze widoczny
        }

        #endregion

        #region Zarządzanie widokami

        /// <summary>
        /// Zmienia tryb widoku kalendarza (month/week/day).
        /// Aktualizuje kolory przycisków i przerenderowuje kalendarz.
        /// </summary>
        /// <param name="mode">Tryb widoku: "month", "week" lub "day"</param>
        private void ChangeViewMode(string mode)
        {
            _viewMode = mode;

            // Aktualizuj kolory przycisków widoku
            var activeColor = Color.FromArgb(200, 225, 255);
            var inactiveColor = Color.White;

            if (_btnMonthView != null)
            {
                _btnMonthView.BackColor = mode == "month" ? activeColor : inactiveColor;
                _btnMonthView.FlatAppearance.BorderColor = (_viewMode == "month") ? Color.FromArgb(52, 152, 219) : Color.FromArgb(180, 180, 180);
                _btnMonthView.ForeColor = (_viewMode == "month") ? Color.FromArgb(52, 152, 219) : Color.FromArgb(100, 100, 100);
            }
            if (_btnWeekView != null)
            {
                _btnWeekView.BackColor = mode == "week" ? activeColor : inactiveColor;
                _btnWeekView.FlatAppearance.BorderColor = (_viewMode == "week") ? Color.FromArgb(52, 152, 219) : Color.FromArgb(180, 180, 180);
                _btnWeekView.ForeColor = (_viewMode == "week") ? Color.FromArgb(52, 152, 219) : Color.FromArgb(100, 100, 100);
            }
            if (_btnDayView != null)
            {
                _btnDayView.BackColor = mode == "day" ? activeColor : inactiveColor;
                _btnDayView.FlatAppearance.BorderColor = (_viewMode == "day") ? Color.FromArgb(52, 152, 219) : Color.FromArgb(180, 180, 180);
                _btnDayView.ForeColor = (_viewMode == "day") ? Color.FromArgb(52, 152, 219) : Color.FromArgb(100, 100, 100);
            }

            SetActiveView(mode);

            // Wymuś aktualizację układu po zmianie widoku
            if (_calendarPanelHost != null)
            {
                _calendarPanelHost.PerformLayout();
            }

            // Zawsze renderuj kalendarz po zmianie trybu widoku
            LoadCalendar();
        }

        /// <summary>
        /// Inicjalizuje panele widoków (zastępuje puste UserControls).
        /// </summary>
        private void InitializeViewPanels()
        {
            // === Month View Panel ===
            _monthViewPanel = new Panel { Dock = DockStyle.Fill };
            _monthCalendarPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _monthDayHeadersPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.WhiteSmoke
            };
            _monthBtnWeekStartSelector = new Button
            {
                Location = new Point(5, 5),
                Size = new Size(24, 24),
                Text = ">>",
                Visible = true,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent
            };
            _monthBtnWeekStartSelector.FlatAppearance.BorderSize = 0;
            _monthCalendarPanel.Controls.Add(_monthDayHeadersPanel);
            _monthCalendarPanel.Controls.Add(_monthBtnWeekStartSelector);
            _monthViewPanel.Controls.Add(_monthCalendarPanel);

            // === Week View Panel ===
            _weekViewPanel = new Panel { Dock = DockStyle.Fill };
            var weekCalendarPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            var weekDayHeadersPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.WhiteSmoke
            };
            weekCalendarPanel.Controls.Add(weekDayHeadersPanel);
            _weekViewPanel.Controls.Add(weekCalendarPanel);

            // === Day View Panel ===
            _dayViewPanel = new Panel { Dock = DockStyle.Fill };
            var dayCalendarPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            _dayViewPanel.Controls.Add(dayCalendarPanel);
        }

        /// <summary>
        /// Przełącza hostowany widok (month/week/day) i mapuje używane kontrolki.
        /// </summary>
        private void SetActiveView(string mode)
        {
            if (_calendarPanelHost == null) return;

            Panel? view = mode switch
            {
                "week" => _weekViewPanel,
                "day" => _dayViewPanel,
                _ => _monthViewPanel
            };

            if (view == null) return;

            _calendarPanelHost.Controls.Clear();
            view.Dock = DockStyle.Fill;
            _calendarPanelHost.Controls.Add(view);

            if (mode == "month")
            {
                _calendarPanel = _monthCalendarPanel;
                _dayHeadersPanel = _monthDayHeadersPanel;
                _btnWeekStartSelector = _monthBtnWeekStartSelector;
            }
            else if (mode == "week")
            {
                // Pobierz panele z weekViewPanel
                _calendarPanel = _weekViewPanel?.Controls[0] as Panel ?? _weekViewPanel;
                _dayHeadersPanel = _calendarPanel?.Controls.Count > 0 ? _calendarPanel.Controls[0] as Panel : null;
                _btnWeekStartSelector = null;
            }
            else // dzień
            {
                _calendarPanel = _dayViewPanel?.Controls[0] as Panel ?? _dayViewPanel;
                _dayHeadersPanel = null;
                _btnWeekStartSelector = null;
            }

            // Zostawiamy przyciski nawigacji z hosta (Month/Week/Day/TODAY/Prev/Next/Add)
        }

        private void NavigatePrevious()
        {
            switch (_viewMode)
            {
                case "month":
                    _currentDate = _currentDate.AddMonths(-1);
                    break;
                case "week":
                    _currentDate = _currentDate.AddDays(-7);
                    break;
                case "day":
                    _currentDate = _currentDate.AddDays(-1);
                    break;
            }

            LoadCalendar();
        }

        private void NavigateNext()
        {
            switch (_viewMode)
            {
                case "month":
                    _currentDate = _currentDate.AddMonths(1);
                    break;
                case "week":
                    _currentDate = _currentDate.AddDays(7);
                    break;
                case "day":
                    _currentDate = _currentDate.AddDays(1);
                    break;
            }

            LoadCalendar();
        }

        private void NavigateToday()
        {
            _currentDate = DateTime.Now;
            LoadCalendar();
        }

        private void LoadCalendar()
        {
            // Pokaż overlay ładowania (żeby UI nie wyglądał na zamrożony)
            ShowLoadingOverlay();
            
            try
            {
                // Nie usuwaj wszystkich kontrolek - zachowaj kontrolki utworzone w designerze
                var controlsToRemove = new List<Control>();
            foreach (Control ctrl in _calendarPanel.Controls)
            {
                // Zachowaj kontrolki utworzone w designerze (nagłówki dni, komórki dni, numery tygodni)
                bool isDesignerControl = ctrl == _dayHeadersPanel;

                if (_dayCells != null && _dayCells.Contains(ctrl))
                    isDesignerControl = true;

                if (_weekNumberLabels != null && _weekNumberLabels.Contains(ctrl))
                    isDesignerControl = true;

                if (_btnWeekStartSelector != null && ctrl == _btnWeekStartSelector)
                    isDesignerControl = true;

                if (!isDesignerControl)
                {
                    controlsToRemove.Add(ctrl);
                }
            }
            foreach (var ctrl in controlsToRemove)
            {
                _calendarPanel.Controls.Remove(ctrl);
            }

            // Wymuś aktualizację układu przed renderowaniem
            _calendarPanel.SuspendLayout();

            // Upewnij się, że panel kalendarza ma uchwyt przed utworzeniem kontrolek podrzędnych
            if (!_calendarPanel.IsHandleCreated && _calendarPanel.Visible)
            {
                try
                {
                    _calendarPanel.CreateControl();
                }
                catch
                {
                    _calendarPanel.ResumeLayout(false);
                    return;
                }
            }

            UpdateDateLabel();

            switch (_viewMode)
            {
                case "month":
                    RenderMonthView();
                    break;
                case "week":
                    RenderWeekView();
                    break;
                case "day":
                    RenderDayView();
                    break;
            }

            _calendarPanel.ResumeLayout(true);

            // Po wznowieniu układu bezpiecznie ustaw widoczność dla komórek widoku miesięcznego
            if (_viewMode == "month" && _dayCells != null)
            {
                for (int i = 0; i < 42; i++)
                {
                    if (_dayCells[i] != null && !_dayCells[i].IsDisposed && _calendarPanel.Controls.Contains(_dayCells[i]))
                    {
                        try
                        {
                            if (_dayCells[i].Controls.Count > 0 && _calendarPanel.IsHandleCreated)
                            {
                                _dayCells[i].Visible = true;
                            }
                        }
                        catch (System.ComponentModel.Win32Exception)
                        {
                        }
                        catch (System.ObjectDisposedException)
                        {
                        }
                    }
                }
            }

            }
            finally
            {
                // Ukryj overlay po załadowaniu
                HideLoadingOverlay();
            }
        }

        private void UpdateDateLabel()
        {
            switch (_viewMode)
            {
                case "month":
                    _lblCurrentDate.Text = _currentDate.ToString("MMMM yyyy");
                    break;

                case "week":
                    int firstDayOffset = ((int)_firstDayOfWeek + 6) % 7;
                    int currentDayOffset = ((int)_currentDate.DayOfWeek + 6) % 7;
                    var weekStart = _currentDate.AddDays(-currentDayOffset + firstDayOffset);
                    if (weekStart > _currentDate) weekStart = weekStart.AddDays(-7);
                    var weekEnd = weekStart.AddDays(6);
                    _lblCurrentDate.Text = $"{weekStart:MMM dd} - {weekEnd:MMM dd, yyyy}";
                    break;

                case "day":
                    _lblCurrentDate.Text = _currentDate.ToString("dddd, MMMM dd, yyyy");
                    break;
            }
        }

        /// <summary>
        /// Pokazuje nakładkę "Ładowanie..." na kalendarzu.
        /// </summary>
        private void ShowLoadingOverlay()
        {
            if (_loadingOverlay != null) return;
            if (_calendarPanelHost == null) return;

            _loadingOverlay = new Panel
            {
                BackColor = Color.FromArgb(200, 255, 255, 255), // Półprzezroczysty biały
                Dock = DockStyle.Fill,
                Name = "loadingOverlay"
            };

            var loadingLabel = new Label
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // Wycentruj tekst
            _loadingOverlay.Controls.Add(loadingLabel);
            _loadingOverlay.Resize += (s, e) =>
            {
                loadingLabel.Location = new Point(
                    (_loadingOverlay.Width - loadingLabel.Width) / 2,
                    (_loadingOverlay.Height - loadingLabel.Height) / 2);
            };

            _calendarPanelHost.Controls.Add(_loadingOverlay);
            _loadingOverlay.BringToFront();
            _loadingOverlay.Refresh();
            Application.DoEvents(); // Wymuś przerysowanie UI
        }

        /// <summary>
        /// Ukrywa overlay ładowania.
        /// </summary>
        private void HideLoadingOverlay()
        {
            if (_loadingOverlay == null) return;

            _calendarPanelHost?.Controls.Remove(_loadingOverlay);
            _loadingOverlay.Dispose();
            _loadingOverlay = null;
        }

        #endregion

        #region Widok miesięczny (Month View)

        /// <summary>
        /// Renderuje widok miesięczny kalendarza.
        /// 
        /// STRUKTURA:
        /// - Siatka 6x7 komórek (tygodnie x dni)
        /// - Nagłówki dni tygodnia (Mon-Sun)
        /// - Numery tygodni po lewej (I, II, III...)
        /// - Eventy w komórkach (max 3 + "more")
        /// </summary>
        private void RenderMonthView()
        {
            if (_eventService == null) return;
            if (_dayHeadersPanel == null) return;

            int panelWidth = _calendarPanelHost != null ? _calendarPanelHost.ClientSize.Width : _calendarPanel.ClientSize.Width;
            int panelHeight = _calendarPanelHost != null ? _calendarPanelHost.ClientSize.Height : _calendarPanel.ClientSize.Height;

            // Powrót do rozmiaru UserControl jeśli panele nie są jeszcze wymiarowane
            if (panelWidth < 200)
            {
                panelWidth = this.ClientSize.Width - 200; // Uwzględnij prawy panel (~170px) + marginesy
            }
            if (panelHeight < 200)
            {
                panelHeight = this.ClientSize.Height - 140; // Uwzględnij nagłówek i dolny panel
            }

            if (panelWidth < 200 || panelHeight < 200)
            {
                return; // Pomiń renderowanie, zostanie wywołane ponownie, gdy rozmiar będzie odpowiedni
            }

            _calendarPanel.AutoScroll = false;
            _calendarPanel.HorizontalScroll.Enabled = false;
            _calendarPanel.HorizontalScroll.Visible = false;
            _calendarPanel.VerticalScroll.Enabled = false;
            _calendarPanel.VerticalScroll.Visible = false;

            if (_dayHeadersPanel != null) _dayHeadersPanel.Visible = true;

            var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);

            int firstDayOffset = ((int)_firstDayOfWeek + 6) % 7;
            var startingDayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 6 - firstDayOffset) % 7;

            var monthStart = firstDayOfMonth;
            var monthEnd = firstDayOfMonth.AddMonths(1);
            // Użyj GetEvents() dla szybszego ładowania widoku miesiąca
            var events = _eventService.GetEvents(monthStart, monthEnd);

            int totalDaysToShow = startingDayOfWeek + daysInMonth;
            int weeksNeeded = (int)Math.Ceiling(totalDaysToShow / 7.0);
            if (weeksNeeded < 5) weeksNeeded = 5; // Minimum 5 weeks
            if (weeksNeeded > 6) weeksNeeded = 6; // Maximum 6 weeks

            int weekNumberWidth = 40; // Zmniejszone z 50
            int headerHeight = 35; // Zmniejszone z 40

            int availableWidthForCells = panelWidth - weekNumberWidth - 10;
            int availableHeightForCells = panelHeight - headerHeight - 10;

            int cellWidth = availableWidthForCells / 7;
            int cellHeight = availableHeightForCells / weeksNeeded;

            bool needsReinitDayHeaders = _dayHeaderLabels == null ||
                                          _dayHeaderLabels[0] == null ||
                                          _dayHeadersPanel == null ||
                                          !_dayHeadersPanel.Controls.Contains(_dayHeaderLabels[0]);

            if (needsReinitDayHeaders)
            {
                _dayHeaderLabels = new Label[7];
                string[] dayNamesLocal = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
                for (int i = 0; i < 7; i++)
                {
                    _dayHeaderLabels[i] = new Label
                    {
                        Text = dayNamesLocal[i],
                        Location = new Point(i * cellWidth, 0), // Zacznij od Y=0 dla prawidłowego wyśrodkowania
                        Size = new Size(cellWidth, headerHeight), // Użyj pełnej wysokości
                        Font = new Font("Lucida Handwriting", 11F, FontStyle.Regular),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent,
                        ForeColor = Color.FromArgb(44, 62, 80),
                        Name = $"_dayHeaderLabel{i}"
                    };
                    _dayHeadersPanel?.Controls.Add(_dayHeaderLabels[i]);
                }
            }

            bool needsReinitWeekLabels = _weekNumberLabels == null ||
                                          _weekNumberLabels[0] == null ||
                                          _calendarPanel == null ||
                                          !_calendarPanel.Controls.Contains(_weekNumberLabels[0]);

            if (needsReinitWeekLabels)
            {
                _weekNumberLabels = new Label[6];
                for (int i = 0; i < 6; i++)
                {
                    _weekNumberLabels[i] = new Label
                    {
                        Text = "",
                        Location = new Point(0, 50 + i * cellHeight),
                        Size = new Size(weekNumberWidth - 2, 20), // Zwiększona wysokość dla większej czcionki
                        Font = new Font("Segoe UI", 9, FontStyle.Regular), // Większa czcionka dla czytelności
                        TextAlign = ContentAlignment.MiddleRight, // Wyrównanie do prawej, aby nie nachodzić na kalendarz
                        ForeColor = Color.FromArgb(100, 100, 100), // Ciemniejszy dla widoczności
                        Name = $"_weekNumberLabel{i}",
                        Visible = false
                    };
                    _calendarPanel?.Controls.Add(_weekNumberLabels[i]);
                }
            }

            bool needsReinitDayCells = _dayCells == null ||
                                        _dayCells[0] == null ||
                                        _calendarPanel == null ||
                                        !_calendarPanel.Controls.Contains(_dayCells[0]);

            if (needsReinitDayCells)
            {
                _dayCells = new Panel[42];
                for (int i = 0; i < 42; i++)
                {
                    int cellRow = i / 7;
                    int cellCol = i % 7;
                    _dayCells[i] = new Panel
                    {
                        Location = new Point(cellCol * cellWidth + weekNumberWidth, cellRow * cellHeight + headerHeight),
                        Size = new Size(cellWidth - 2, cellHeight - 2),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White,
                        Cursor = Cursors.Hand,
                        Name = $"_dayCell{i}",
                        Visible = false
                    };
                    _calendarPanel?.Controls.Add(_dayCells[i]);
                }
            }
            else if (_dayCells != null)
            {
                for (int i = 0; i < 42; i++)
                {
                    if (_dayCells[i] != null)
                    {
                        int cellRow = i / 7;
                        int cellCol = i % 7;
                        _dayCells[i].Location = new Point(cellCol * cellWidth + weekNumberWidth, cellRow * cellHeight + headerHeight);
                        _dayCells[i].Size = new Size(cellWidth - 2, cellHeight - 2);
                    }
                }
            }

            if (_dayHeadersPanel != null)
            {
                _dayHeadersPanel.Visible = true;
                _dayHeadersPanel.Location = new Point(weekNumberWidth, 0);
                _dayHeadersPanel.Size = new Size(panelWidth - weekNumberWidth, headerHeight);
            }

            // Zaktualizuj nagłówki dni - przesuń nazwy zgodnie z początkiem tygodnia
            string[] dayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            int firstDayIndex = ((int)_firstDayOfWeek + 6) % 7; // Konwertuj na zakres 0-6 (Poniedziałek=0)

            if (_dayHeaderLabels != null)
            {
                for (int i = 0; i < 7; i++)
                {
                    if (_dayHeaderLabels[i] != null)
                    {
                        int dayIndex = (firstDayIndex + i) % 7; // Przesuń nazwy
                        _dayHeaderLabels[i].Text = dayNames[dayIndex];
                        _dayHeaderLabels[i].Visible = true;
                        _dayHeaderLabels[i].Location = new Point(i * cellWidth, 0); // Y=0 dla prawidłowego wyśrodkowania
                        _dayHeaderLabels[i].Size = new Size(cellWidth, headerHeight); // Pełna wysokość
                    }
                }
            }

            // Zaktualizuj tooltip i hover na pierwszym dniu (który teraz może być inny)
            if (_dayHeaderLabels != null && _dayHeaderLabels[0] != null)
            {
                // Użyj wspólnego tooltip (utwórz jeśli nie istnieje)
                if (_toolTip == null)
                {
                    _toolTip = new ToolTip();
                }

                try
                {
                    _toolTip.SetToolTip(_dayHeaderLabels[0], "Kliknij, aby zmienic poczatek tygodnia");
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Jeśli nie można utworzyć tooltip, po prostu pomiń
                    // (może być problem z handle'ami okien)
                }
            }

            string ToRomanNumeral(int number)
            {
                if (number < 1 || number > 52) return number.ToString();
                string[] roman = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
                if (number <= 10) return roman[number] + ".";
                if (number <= 20) return "X" + roman[number - 10] + ".";
                if (number <= 30) return "XX" + roman[number - 20] + ".";
                if (number <= 40) return "XXX" + roman[number - 30] + ".";
                if (number <= 50) return "XL" + roman[number - 40] + ".";
                return "L" + roman[number - 50] + ".";
            }

            int GetWeekNumber(DateTime date)
            {
                var startOfYear = new DateTime(date.Year, 1, 1);
                var daysDiff = (date - startOfYear).Days;
                int weekNum = (daysDiff / 7) + 1;
                return weekNum;
            }

            int row = 0;
            int col = startingDayOfWeek;
            int currentWeekNum = -1;

            if (_weekNumberLabels != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (_weekNumberLabels[i] != null)
                        _weekNumberLabels[i].Visible = false;
                }
            }

            if (_dayCells == null) return;

            for (int i = 0; i < 42; i++)
            {
                if (_dayCells[i] != null && !_dayCells[i].IsDisposed)
                {
                    try
                    {
                        _dayCells[i].Visible = false;
                        _dayCells[i].Controls.Clear(); // Usuń poprzednią zawartość
                    }
                    catch
                    {
                        int cellRow = i / 7;
                        int cellCol = i % 7;
                        _dayCells[i] = new Panel
                        {
                            Location = new Point(cellCol * cellWidth + weekNumberWidth, cellRow * cellHeight + headerHeight),
                            Size = new Size(cellWidth - 2, cellHeight - 2),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.White,
                            Cursor = Cursors.Hand,
                            Name = $"_dayCell{i}",
                            Visible = false
                        };
                        if (_calendarPanel != null && !_calendarPanel.Controls.Contains(_dayCells[i]))
                        {
                            _calendarPanel?.Controls.Add(_dayCells[i]);
                        }
                    }
                }
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(_currentDate.Year, _currentDate.Month, day);
                var dayEvents = events.Where(e => e.StartDateTime.HasValue && e.StartDateTime.Value.Date == date.Date).ToList();

                if (col == 0)
                {
                    int weekNum = GetWeekNumber(date);
                    if (weekNum != currentWeekNum)
                    {
                        currentWeekNum = weekNum;
                        if (row < 6 && _weekNumberLabels != null && _weekNumberLabels[row] != null)
                        {
                            _weekNumberLabels[row].Text = ToRomanNumeral(weekNum);
                            _weekNumberLabels[row].Location = new Point(0, row * cellHeight + headerHeight + cellHeight / 2 - 8);
                            _weekNumberLabels[row].Size = new Size(weekNumberWidth - 4, 16);
                            _weekNumberLabels[row].Visible = true;
                        }
                    }
                }

                int cellIndex = row * 7 + col;
                if (cellIndex < 42 && _dayCells[cellIndex] != null)
                {
                    var cell = _dayCells[cellIndex];

                    if (cell.IsDisposed)
                    {
                        int cellRow = cellIndex / 7;
                        int cellCol = cellIndex % 7;
                        cell = new Panel
                        {
                            Location = new Point(cellCol * cellWidth + weekNumberWidth, cellRow * cellHeight + headerHeight),
                            Size = new Size(cellWidth - 2, cellHeight - 2),
                            BorderStyle = BorderStyle.FixedSingle,
                            BackColor = Color.White,
                            Cursor = Cursors.Hand,
                            Name = $"_dayCell{cellIndex}",
                            Visible = false
                        };
                        _dayCells[cellIndex] = cell;
                        if (_calendarPanel != null && !_calendarPanel.Controls.Contains(cell))
                        {
                            _calendarPanel?.Controls.Add(cell);
                        }
                    }
                    else if (_calendarPanel != null && !_calendarPanel.Controls.Contains(cell))
                    {
                        _calendarPanel?.Controls.Add(cell);
                    }

                    cell.Location = new Point(col * cellWidth + weekNumberWidth, row * cellHeight + headerHeight);
                    if (_calendarPanel != null && !_calendarPanel.Controls.Contains(cell))
                    {
                        _calendarPanel.Controls.Add(cell);
                    }

                    cell.Location = new Point(col * cellWidth + weekNumberWidth, row * cellHeight + headerHeight);
                    cell.Size = new Size(cellWidth - 2, cellHeight - 2);

                    UpdateDayCell(cell, day, dayEvents, cellWidth, cellHeight, date);

                }

                col++;
                if (col == 7)
                {
                    col = 0;
                    row++;
                }
            }
        }

        private void UpdateDayCell(Panel panel, int day, List<Event> events, int width, int height, DateTime date)
        {
            bool isToday = date.Date == DateTime.Now.Date;

            panel.Size = new Size(width - 2, height - 2);
            panel.BackColor = isToday
                ? Color.FromArgb(230, 240, 255)
                : Color.White;

            // Poprawnie usuń i wyczyść istniejące kontrolki
            panel.SuspendLayout();
            foreach (Control ctrl in panel.Controls.OfType<Control>().ToList())
            {
                ctrl.Dispose();
            }
            panel.Controls.Clear();
            panel.ResumeLayout(false);

            // Etykieta z numerem dnia
            var lblDay = new Label
            {
                Text = day.ToString(),
                Location = new Point(5, 5),
                Size = new Size(30, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = isToday
                    ? Color.FromArgb(231, 76, 60) // Czerwony dla dzisiejszego numeru
                    : Color.FromArgb(44, 62, 80),
                Name = "dayLabel"
            };
            panel.Controls.Add(lblDay);

            // Czerwone serce dla dzisiejszego dnia (jak na twoim szkicu)
            if (isToday)
            {
                var heartLabel = new Label
                {
                    Text = "\u2764", // Symbol serca używający Unicode
                    Location = new Point(width - 35, 3),
                    Size = new Size(30, 25),
                    Font = new Font("Segoe UI", 14),
                    ForeColor = Color.FromArgb(231, 76, 60), // Czerwony kolor
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent
                };
                panel.Controls.Add(heartLabel);
            }

            int eventY = 30;
            foreach (var evt in events.Take(3)) // max 3 eventy
            {
                var eventLabel = new Label
                {
                    Text = evt.Title,
                    Location = new Point(5, eventY),
                    Size = new Size(width - 15, 20),
                    Font = new Font("Segoe UI", 8),
                    BackColor = ColorTranslator.FromHtml(evt.ColorCode),
                    ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(3, 2, 3, 2),
                    Cursor = Cursors.Hand,
                    AutoEllipsis = true
                };
                eventLabel.Click += (s, e) => ShowEventDetails(evt);
                panel.Controls.Add(eventLabel);
                eventY += 22;
            }

            if (events.Count > 3)
            {
                var moreLabel = new Label
                {
                    Text = $"+{events.Count - 3} more",
                    Location = new Point(5, eventY),
                    Size = new Size(width - 15, 20),
                    Font = new Font("Segoe UI", 7, FontStyle.Italic),
                    ForeColor = Color.Gray
                };
                panel.Controls.Add(moreLabel);
            }

            panel.DoubleClick += (s, e) => ShowAddEventDialog(date);
        }

        #endregion

        #region Widok tygodniowy (Week View)

        /// <summary>
        /// Renderuje widok tygodniowy kalendarza.
        /// 
        /// STRUKTURA:
        /// - 7 kolumn (dni tygodnia)
        /// - 24 wiersze (godziny 00-23)
        /// - Eventy jako kolorowe bloki
        /// - Opcjonalne bloki snu (ukośne paski)
        /// </summary>
        private void RenderWeekView()
        {
            if (_eventService == null) return;

            // Wymuś aktualizację układu
            _calendarPanel.PerformLayout();

            int panelWidth = _calendarPanelHost != null ? _calendarPanelHost.ClientSize.Width : _calendarPanel.ClientSize.Width;

            // Powrót do rozmiaru UserControl jeśli panele nie są jeszcze wymiarowane
            if (panelWidth < 200)
            {
                panelWidth = this.ClientSize.Width - 200; // Uwzględnij prawy panel + marginesy
            }

            // Włącz przewijanie w pionie (dla 24 godzin), wyłącz przewijanie w poziomie
            _calendarPanel.AutoScroll = true;
            _calendarPanel.HorizontalScroll.Enabled = false;
            _calendarPanel.HorizontalScroll.Visible = false;
            _calendarPanel.VerticalScroll.Enabled = true;
            _calendarPanel.VerticalScroll.Visible = true;

            // Ukryj kontrolki widoku miesięcznego kalendarza
            if (_dayHeadersPanel != null) _dayHeadersPanel.Visible = false;
            if (_btnWeekStartSelector != null) _btnWeekStartSelector.Visible = false;
            if (_dayCells != null)
            {
                foreach (var cell in _dayCells)
                {
                    if (cell != null) cell.Visible = false;
                }
            }
            if (_weekNumberLabels != null)
            {
                foreach (var label in _weekNumberLabels)
                {
                    if (label != null) label.Visible = false;
                }
            }

            int firstDayOffset = ((int)_firstDayOfWeek + 6) % 7;
            int currentDayOffset = ((int)_currentDate.DayOfWeek + 6) % 7;
            var weekStart = _currentDate.AddDays(-currentDayOffset + firstDayOffset);
            if (weekStart > _currentDate) weekStart = weekStart.AddDays(-7);

            var weekEnd = weekStart.AddDays(6);
            // Użyj GetEvents() dla szybszego ładowania widoku tygodnia
            var events = _eventService.GetEvents(weekStart.Date, weekEnd.Date.AddDays(1));

            var (sleepStart, sleepEnd) = GetSleepScheduleForCurrentContext();

            int timeLabelWidth = 50; // Zmniejszone z 60
            int hourHeight = 50; // Zmniejszone z 60 - bardziej kompaktowe
            int headerHeight = 35; // Zmniejszone z 40

            int scrollbarWidth = 20;
            int availableWidth = panelWidth - timeLabelWidth - scrollbarWidth;
            int dayColumnWidth = availableWidth / 7;

            Panel timeLabelsPanel = new Panel
            {
                Location = new Point(0, headerHeight),
                Size = new Size(timeLabelWidth, 24 * hourHeight),
                BackColor = Color.White,
                Name = "timeLabelsPanel"
            };
            _calendarPanel?.Controls.Add(timeLabelsPanel);

            for (int hour = 0; hour <= 24; hour++)
            {
                int labelY = hour * hourHeight;
                if (hour == 0) labelY -= 2;
                else if (hour == 24) labelY -= 15;
                else labelY -= 10;

                Label timeLabel = new Label
                {
                    Text = hour.ToString("00"),
                    Location = new Point(5, labelY),
                    Size = new Size(50, 20),
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.FromArgb(100, 100, 100),
                    TextAlign = ContentAlignment.MiddleRight,
                    BackColor = Color.Transparent,
                    Name = $"timeLabel{hour}"
                };
                timeLabelsPanel.Controls.Add(timeLabel);

                // Linia czasu (pozioma linia przez kalendarz) - umieszczona na początku każdej godziny
                if (hour < 24)
                {
                    Panel timeLine = new Panel
                    {
                        Location = new Point(timeLabelWidth, headerHeight + hour * hourHeight),
                        Size = new Size(availableWidth, 1),
                        BackColor = Color.FromArgb(230, 230, 230),
                        Name = $"timeLine{hour}"
                    };
                    _calendarPanel?.Controls.Add(timeLine);
                }
            }

            Panel daysContainer = new Panel
            {
                Location = new Point(timeLabelWidth, headerHeight),
                Size = new Size(availableWidth, 24 * hourHeight),
                BackColor = Color.White,
                Name = "daysContainer"
            };
            _calendarPanel?.Controls.Add(daysContainer);

            string[] dayNames = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            int firstDayIndex = ((int)_firstDayOfWeek + 6) % 7;

            for (int dayIndex = 0; dayIndex < 7; dayIndex++)
            {
                int actualDayIndex = (firstDayIndex + dayIndex) % 7;
                var dayDate = weekStart.AddDays(dayIndex);

                bool isToday = dayDate.Date == DateTime.Now.Date;

                Label dayHeader = new Label
                {
                    Text = $"{dayNames[actualDayIndex]}\n{dayDate.Day}",
                    Location = new Point(timeLabelWidth + dayIndex * dayColumnWidth, 0),
                    Size = new Size(dayColumnWidth, headerHeight),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = isToday ? Color.FromArgb(231, 76, 60) : Color.FromArgb(44, 62, 80),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = isToday ? Color.FromArgb(230, 240, 255) : Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Name = $"weekDayHeader{dayIndex}"
                };
                _calendarPanel?.Controls.Add(dayHeader);
                dayHeader.BringToFront();

                Panel dayColumn = new Panel
                {
                    Location = new Point(dayIndex * dayColumnWidth, 0),
                    Size = new Size(dayColumnWidth, 24 * hourHeight),
                    BackColor = isToday ? Color.FromArgb(230, 240, 255) : Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Name = $"dayColumn{dayIndex}"
                };
                daysContainer.Controls.Add(dayColumn);

                if (_chkShowSleepHours != null && _chkShowSleepHours.Checked)
                {
                    DrawSleepBlocksOnColumn(dayColumn, dayColumnWidth, hourHeight, sleepStart, sleepEnd);
                }

                var dayEvents = events.Where(e =>
                    e.StartDateTime.HasValue &&
                    (e.StartDateTime.Value.Date == dayDate.Date ||
                     (e.StartDateTime.Value.Date <= dayDate.Date && e.GetEndDateTime().Date >= dayDate.Date)))
                    .OrderBy(e => e.StartDateTime).ToList();
                foreach (var evt in dayEvents)
                {
                    if (!evt.StartDateTime.HasValue) continue;

                    // Dla wydarzeń wielodniowych, oblicz część widoczną w tym dniu
                    DateTime eventStart = evt.StartDateTime.Value;
                    DateTime eventEnd = evt.GetEndDateTime();
                    DateTime dayStart = dayDate.Date;
                    DateTime dayEnd = dayDate.Date.AddDays(1);

                    DateTime displayStart = eventStart > dayStart ? eventStart : dayStart;
                    DateTime displayEnd = eventEnd < dayEnd ? eventEnd : dayEnd;

                    int startHour = displayStart.Hour;
                    int startMinute = displayStart.Minute;
                    int endHour = displayEnd.Hour;
                    int endMinute = displayEnd.Minute;

                    if (displayEnd >= dayEnd)
                    {
                        endHour = 24;
                        endMinute = 0;
                    }

                    int startY = (int)((startHour * hourHeight) + (startMinute * hourHeight / 60.0));
                    int endY = (int)((endHour * hourHeight) + (endMinute * hourHeight / 60.0));
                    int eventHeight = Math.Max(1, endY - startY);

                    Panel eventPanel = new Panel
                    {
                        Location = new Point(5, startY),
                        Size = new Size(dayColumnWidth - 10, eventHeight),
                        BackColor = ColorTranslator.FromHtml(evt.ColorCode ?? "#3498DB"),
                        BorderStyle = BorderStyle.None,
                        Name = $"eventPanel{evt.EventID}"
                    };

                    Label eventLabel = new Label
                    {
                        Text = $"{evt.StartDateTime:HH:mm} {evt.Title}",
                        Location = new Point(5, 5),
                        Size = new Size(dayColumnWidth - 15, Math.Min(20, eventHeight - 10)),
                        Font = new Font("Segoe UI", 9, FontStyle.Regular),
                        ForeColor = Color.White,
                        BackColor = Color.Transparent,
                        AutoSize = false,
                        AutoEllipsis = true
                    };
                    eventPanel.Controls.Add(eventLabel);
                    eventPanel.Cursor = Cursors.Hand;
                    eventPanel.Click += (s, e) => ShowEventDetails(evt);
                    eventLabel.Click += (s, e) => ShowEventDetails(evt);
                    dayColumn.Controls.Add(eventPanel);
                }
            }
        }

        #endregion

        #region Widok dzienny (Day View)

        /// <summary>
        /// Renderuje widok dzienny kalendarza.
        /// 
        /// STRUKTURA:
        /// - 1 kolumna (aktualny dzień)
        /// - 24 wiersze (godziny 00-23)
        /// - Eventy jako kolorowe bloki
        /// - Opcjonalne bloki snu (ukośne paski)
        /// </summary>
        private void RenderDayView()
        {
            if (_eventService == null) return;

            // Wymuś aktualizację układu
            _calendarPanel.PerformLayout();

            int panelWidth = _calendarPanelHost != null ? _calendarPanelHost.ClientSize.Width : _calendarPanel.ClientSize.Width;

            // Powrót do rozmiaru UserControl jeśli panele nie są jeszcze wymiarowane
            if (panelWidth < 200)
            {
                panelWidth = this.ClientSize.Width - 200; // Uwzględnij prawy panel + marginesy
            }

            // Włącz przewijanie w pionie (dla 24 godzin), wyłącz przewijanie w poziomie
            _calendarPanel.AutoScroll = true;
            _calendarPanel.HorizontalScroll.Enabled = false;
            _calendarPanel.HorizontalScroll.Visible = false;
            _calendarPanel.VerticalScroll.Enabled = true;
            _calendarPanel.VerticalScroll.Visible = true;

            // Ukryj kontrolki widoku miesięcznego kalendarza
            if (_dayHeadersPanel != null) _dayHeadersPanel.Visible = false;
            if (_btnWeekStartSelector != null) _btnWeekStartSelector.Visible = false;
            if (_dayCells != null)
            {
                foreach (var cell in _dayCells)
                {
                    if (cell != null) cell.Visible = false;
                }
            }
            if (_weekNumberLabels != null)
            {
                foreach (var label in _weekNumberLabels)
                {
                    if (label != null) label.Visible = false;
                }
            }

            // Użyj GetEvents() dla szybszego ładowania widoku dnia
            var events = _eventService.GetEvents(_currentDate.Date, _currentDate.Date.AddDays(1));

            var (sleepStart, sleepEnd) = GetSleepScheduleForCurrentContext();

            // Mniejsza szerokość dla etykiet czasu w widoku dnia
            int timeLabelWidth = 40;
            int hourHeight = 50; // Zmniejszone z 60 - bardziej kompaktowe
            int headerHeight = 35; // Zmniejszone z 40

            int scrollbarWidth = 20;
            int availableWidth = panelWidth - timeLabelWidth - scrollbarWidth;
            int dayColumnWidth = availableWidth;

            Panel timeLabelsPanel = new Panel
            {
                Location = new Point(0, headerHeight),
                Size = new Size(timeLabelWidth, 24 * hourHeight),
                BackColor = Color.White,
                Name = "timeLabelsPanel"
            };
            _calendarPanel?.Controls.Add(timeLabelsPanel);

            for (int hour = 0; hour <= 24; hour++)
            {
                // Etykieta czasu - mniejsza i bliżej panelu dnia w widoku dnia
                int labelY = hour * hourHeight;
                if (hour == 0) labelY -= 5; // Przesuń pierwszą etykietę (00) w dół
                else if (hour == 24) labelY -= 16;
                else labelY -= 10;

                Label timeLabel = new Label
                {
                    Text = hour.ToString("00"),
                    Location = new Point(2, labelY),
                    Size = new Size(35, 20),
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.FromArgb(100, 100, 100),
                    TextAlign = ContentAlignment.MiddleRight,
                    BackColor = Color.Transparent,
                    Name = $"timeLabel{hour}"
                };
                timeLabelsPanel.Controls.Add(timeLabel);

                // Linia czasu (pozioma linia przez kalendarz) - umieszczona na początku każdej godziny
                if (hour < 24)
                {
                    Panel timeLine = new Panel
                    {
                        Location = new Point(timeLabelWidth, headerHeight + hour * hourHeight),
                        Size = new Size(availableWidth, 1),
                        BackColor = Color.FromArgb(230, 230, 230),
                        Name = $"timeLine{hour}"
                    };
                    _calendarPanel?.Controls.Add(timeLine);
                }
            }

            int dayColumnActualWidth = availableWidth / 2;
            int dayColumnX = timeLabelWidth + (availableWidth - dayColumnActualWidth) / 2;

            Panel dayColumn = new Panel
            {
                Location = new Point(dayColumnX, headerHeight),
                Size = new Size(dayColumnActualWidth, 24 * hourHeight),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Name = "dayColumn"
            };
            _calendarPanel?.Controls.Add(dayColumn);

            // Nagłówek dnia - również 1/2 szerokości i wyśrodkowany
            Label dayHeader = new Label
            {
                Text = _currentDate.ToString("dddd, MMMM dd"),
                Location = new Point(dayColumnX, 0),
                Size = new Size(dayColumnActualWidth, headerHeight),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Name = "dayHeader"
            };
            _calendarPanel?.Controls.Add(dayHeader);
            dayHeader.BringToFront();

            // Rysuj godziny snu, jeśli pole wyboru jest zaznaczone
            if (_chkShowSleepHours != null && _chkShowSleepHours.Checked)
            {
                DrawSleepBlocksOnColumn(dayColumn, dayColumnActualWidth, hourHeight, sleepStart, sleepEnd);
            }

            // Dodaj wydarzenia na ten dzień - uwzględnij wydarzenia rozpoczynające się tego dnia LUB trwające przez ten dzień
            var dayStart = _currentDate.Date;
            var dayEnd = _currentDate.Date.AddDays(1);
            var dayEvents = events.Where(e =>
                e.StartDateTime.HasValue &&
                (e.StartDateTime.Value.Date == dayStart ||
                 (e.StartDateTime.Value.Date <= dayStart && e.GetEndDateTime().Date >= dayStart)))
                .OrderBy(e => e.StartDateTime).ToList();
            foreach (var evt in dayEvents)
            {
                if (!evt.StartDateTime.HasValue) continue;

                // Dla wydarzeń wielodniowych, oblicz część widoczną w tym dniu
                DateTime eventStart = evt.StartDateTime.Value;
                DateTime eventEnd = evt.GetEndDateTime();

                DateTime displayStart = eventStart > dayStart ? eventStart : dayStart;
                DateTime displayEnd = eventEnd < dayEnd ? eventEnd : dayEnd;

                int startHour = displayStart.Hour;
                int startMinute = displayStart.Minute;
                int endHour = displayEnd.Hour;
                int endMinute = displayEnd.Minute;

                if (displayEnd >= dayEnd)
                {
                    endHour = 24;
                    endMinute = 0;
                }

                int startY = (int)((startHour * hourHeight) + (startMinute * hourHeight / 60.0));
                int endY = (int)((endHour * hourHeight) + (endMinute * hourHeight / 60.0));
                int eventHeight = Math.Max(1, endY - startY); // Minimum 1 pixel, tak jak w widoku tygodniowym

                Panel eventPanel = new Panel
                {
                    Location = new Point(5, startY),
                    Size = new Size(dayColumnActualWidth - 10, eventHeight),
                    BackColor = ColorTranslator.FromHtml(evt.ColorCode ?? "#3498DB"),
                    BorderStyle = BorderStyle.None,
                    Name = $"eventPanel{evt.EventID}"
                };

                DateTime eventEndDateTime = evt.GetEndDateTime();
                Label eventLabel = new Label
                {
                    Text = $"{evt.StartDateTime:HH:mm} - {eventEndDateTime:HH:mm} {evt.Title}",
                    Location = new Point(5, 5),
                    Size = new Size(dayColumnActualWidth - 15, Math.Max(1, Math.Min(20, eventHeight - 10))),
                    Font = new Font("Segoe UI", 9, FontStyle.Regular),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    AutoEllipsis = true
                };
                eventPanel.Controls.Add(eventLabel);
                eventPanel.Cursor = Cursors.Hand;
                eventPanel.Click += (s, e) => ShowEventDetails(evt);
                eventLabel.Click += (s, e) => ShowEventDetails(evt);
                dayColumn.Controls.Add(eventPanel);
            }
        }

        private void ShowEventDetails(Event evt)
        {
            if (_eventService == null) return;

            using (var detailsForm = new EventDetailsForm(evt, _eventService))
            {
                if (detailsForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCalendar();
                }
            }
        }

        private void ShowAddEventDialog(DateTime? date = null)
        {
            if (_eventService == null) return;

            using (var addForm = new AddEventForm(_eventService, date ?? _currentDate))
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    LoadCalendar();
                }
            }
        }

        #endregion

        #region Bloki snu (Sleep Blocks)

        /// <summary>
        /// Rysuje bloki godzin snu na kolumnie dnia (dla widoków Week i Day).
        /// Używa wzoru ukośnych pasków (HatchBrush).
        /// 
        /// OBSŁUGA PÓŁNOCY:
        /// - Jeśli sleepStart < sleepEnd: jeden blok (np. drzemka 13:00-15:00)
        /// - Jeśli sleepStart > sleepEnd: dwa bloki (23:00-24:00 i 00:00-07:00)
        /// </summary>
        private void DrawSleepBlocksOnColumn(Panel dayColumn, int columnWidth, int hourHeight, TimeSpan sleepStart, TimeSpan sleepEnd)
        {
            // Normalizuj do [0,24h)
            sleepStart = NormalizeDayTime(sleepStart);
            sleepEnd = NormalizeDayTime(sleepEnd);

            if (sleepStart < sleepEnd)
            {
                AddSleepBlock(dayColumn, columnWidth, hourHeight, sleepStart, sleepEnd, addLabel: true, name: "sleepBlock");
                return;
            }

            // Przypadek północy:
            // 1) od sleepStart -> 24:00 (noc)
            if (sleepStart.TotalMinutes < 24 * 60)
            {
                AddSleepBlock(dayColumn, columnWidth, hourHeight, sleepStart, TimeSpan.FromHours(24), addLabel: true, name: "sleepBlockNight");
            }

            // 2) od 00:00 -> sleepEnd (morning) (handle 00:xx correctly)
            if (sleepEnd.TotalMinutes > 0)
            {
                AddSleepBlock(dayColumn, columnWidth, hourHeight, TimeSpan.Zero, sleepEnd, addLabel: false, name: "sleepBlockMorning");
            }
        }

        private (TimeSpan sleepStart, TimeSpan sleepEnd) GetSleepScheduleForCurrentContext()
        {
            try
            {
                var userId = UserSession.ContextUserId;
                var user = _userService.GetUserById(userId);
                if (user?.SleepStart != null && user.SleepEnd != null)
                {
                    return (user.SleepStart.Value, user.SleepEnd.Value);
                }
            }
            catch
            {
                // ignoruj i przejdź do domyślnych
            }

            return (UserSession.SleepStart ?? TimeSpan.FromHours(23), UserSession.SleepEnd ?? TimeSpan.FromHours(7));
        }

        private static TimeSpan NormalizeDayTime(TimeSpan t)
        {
            // zachowaj tylko część czasu dnia; bezpiecznie obsłuż nieparzyste wartości
            var minutes = t.TotalMinutes % (24 * 60);
            if (minutes < 0) minutes += 24 * 60;
            return TimeSpan.FromMinutes(minutes);
        }

        private void AddSleepBlock(Panel dayColumn, int columnWidth, int hourHeight, TimeSpan start, TimeSpan end, bool addLabel, string name)
        {
            int startY = (int)(start.TotalHours * hourHeight);
            int endY = (int)(end.TotalHours * hourHeight);
            int blockHeight = endY - startY;
            if (blockHeight <= 0) return;

            Panel block = new Panel
            {
                Location = new Point(0, startY),
                Size = new Size(columnWidth, blockHeight),
                BackColor = Color.Transparent,
                Name = name
            };
            block.Paint += SleepBlock_Paint;

            if (addLabel)
            {
                Label sleepLabel = new Label
                {
                    Text = "Zzz Sleep",
                    Location = new Point(5, 5),
                    Size = new Size(columnWidth - 10, 20),
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.FromArgb(80, 80, 120),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.TopLeft
                };
                block.Controls.Add(sleepLabel);
            }

            dayColumn.Controls.Add(block);
            block.SendToBack(); // Umieść za wydarzeniami
        }

        /// <summary>
        /// Obsługa zdarzenia malowania dla bloków snu - rysuje wzór ukośnych pasków
        /// </summary>
        private void SleepBlock_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is not Panel panel) return;

            // Użyj HatchBrush dla wzoru ukośnych pasków
            using (var hatchBrush = new System.Drawing.Drawing2D.HatchBrush(
                System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal,
                Color.FromArgb(140, 120, 120, 160),  // Kolor paska (ciemniejszy fioletowo-szary)
                Color.FromArgb(40, 200, 200, 220)))  // Kolor tła (jasny z przezroczystością)
            {
                e.Graphics.FillRectangle(hatchBrush, panel.ClientRectangle);
            }

            // Rysuj obramowanie
            using (var borderPen = new Pen(Color.FromArgb(100, 100, 100, 140), 1))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, panel.Width - 1, panel.Height - 1);
            }
        }

        #endregion
    }
}






