// ============================================================================
// EventDetailsForm.cs
// Formularz szczegółów eventu - wyświetlanie, edycja, usuwanie, ukończenie.
// ============================================================================

#region Importy
using System;                   // Podstawowe typy .NET (DateTime, ArgumentNullException)
using System.Drawing;           // Grafika (Point, Size) do dynamicznych kontrolek
using System.Windows.Forms;     // Windows Forms (Form, Button, TextBox, itp.)
using TimeManager.Models;       // Model Event
using TimeManager.Services;     // EventService, PointsService, FocusModeService
#endregion

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz szczegółów eventu - wyświetla i edytuje dane wybranego eventu.
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Wyświetlanie szczegółów: tytuł, opis, data/godzina, kategoria, priorytet
    /// - Edycja podstawowych parametrów (przez dynamiczny dialog)
    /// - Usuwanie eventu z potwierdzeniem
    /// - Oznaczanie jako ukończony (dodaje punkty i usuwa event)
    /// - Uruchamianie trybu Focus Mode
    /// 
    /// PRZEPŁYW UŻYCIA:
    /// 1. Użytkownik klika event w kalendarzu
    /// 2. Otwiera się ten formularz z danymi eventu
    /// 3. Użytkownik może: edytować, usunąć, ukończyć lub rozpocząć focus
    /// 4. Po zamknięciu - CalendarView odświeża widok jeśli były zmiany
    /// </summary>
    public partial class EventDetailsForm : Form
    {
        #region Pola prywatne

        /// <summary>
        /// Event którego szczegóły wyświetlamy.
        /// Otrzymany przez konstruktor, modyfikowany podczas edycji.
        /// </summary>
        private readonly Event _event;

        /// <summary>
        /// Serwis eventów do operacji CRUD (Create, Read, Update, Delete).
        /// </summary>
        private readonly EventService _eventService;

        /// <summary>
        /// Lista predefiniowanych kategorii zgodnych z wymaganiami aplikacji.
        /// Używana w ComboBox podczas edycji kategorii eventu.
        /// </summary>
        private static readonly string[] FixedCategories =
        {
            "health",                           // Zdrowie
            "family",                           // Rodzina
            "mentality",                        // Mentalność/psychika
            "finance",                          // Finanse
            "work and career",                  // Praca i kariera
            "relax",                            // Odpoczynek
            "self development and education",   // Samorozwój i edukacja
            "friends and people",               // Przyjaciele i ludzie
            "None"                              // Brak kategorii
        };

        #endregion

        #region Konstruktor

        /// <summary>
        /// Konstruktor formularza szczegółów eventu.
        /// </summary>
        /// <param name="evt">Event do wyświetlenia (wymagany, nie może być null)</param>
        /// <param name="eventService">Serwis eventów (wymagany, nie może być null)</param>
        /// <exception cref="ArgumentNullException">Gdy evt lub eventService jest null</exception>
        public EventDetailsForm(Event evt, EventService eventService)
        {
            // Walidacja argumentów - oba wymagane
            _event = evt ?? throw new ArgumentNullException(nameof(evt));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));

            // Inicjalizuj kontrolki z Designer.cs
            InitializeComponent();

            // Podepnij handlery zdarzeń
            WireUpEvents();

            // Wypełnij etykiety danymi eventu
            LoadEventDetails();
        }

        #endregion

        #region Inicjalizacja

        /// <summary>
        /// Podpina handlery zdarzeń do przycisków.
        /// Używa null-check bo kontrolki mogą nie istnieć
        /// (np. w uproszczonym designerze).
        /// </summary>
        private void WireUpEvents()
        {
            // Przycisk "Edit" - otwiera dialog edycji
            if (_btnEdit != null)
                _btnEdit.Click += BtnEdit_Click;

            // Przycisk "Delete" - usuwa event
            if (_btnDelete != null)
                _btnDelete.Click += BtnDelete_Click;

            // Przycisk "Complete" - oznacza jako ukończony
            if (_btnComplete != null)
                _btnComplete.Click += BtnComplete_Click;

            // Przycisk "Start Focus" - uruchamia tryb focus
            if (_btnStartFocus != null)
                _btnStartFocus.Click += BtnStartFocus_Click;

            // Przycisk "Close" - zamyka formularz
            if (_btnClose != null)
                _btnClose.Click += BtnClose_Click;
        }

        /// <summary>
        /// Wypełnia etykiety na formularzu danymi z eventu.
        /// Wywoływane przy inicjalizacji i po każdej edycji.
        /// </summary>
        private void LoadEventDetails()
        {
            // ===== TYTUŁ =====
            _lblTitle.Text = string.IsNullOrWhiteSpace(_event.Title)
                ? "(No title)"
                : _event.Title;

            // ===== OPIS =====
            _lblDescription.Text = string.IsNullOrWhiteSpace(_event.Description)
                ? "No description."
                : _event.Description;

            // ===== DATA I GODZINA =====
            string when;
            if (_event.StartDateTime.HasValue)
            {
                var start = _event.StartDateTime.Value;
                DateTime end;

                try
                {
                    // Próbuj uzyskać datę końca z pomocniczej metody
                    end = _event.EndDateTime ?? _event.GetEndDateTime();
                }
                catch
                {
                    // Fallback: oblicz z duration
                    end = start.AddMinutes(_event.Duration > 0 ? _event.Duration : 60);
                }

                // Format: "poniedziałek, 2024-01-15 10:00 – 2024-01-15 11:00"
                when = $"{start:dddd, yyyy-MM-dd HH:mm}  –  {end:yyyy-MM-dd HH:mm}";

                // Dodaj znacznik jeśli ukończony
                if (_event.IsCompleted)
                    when += "   (COMPLETED)";
            }
            else
            {
                // Event jeszcze nie zaplanowany
                when = "Not scheduled yet";
                if (_event.IsCompleted)
                    when += "   (COMPLETED)";
            }

            _lblDateTime.Text = "When: " + when;

            // ===== CZAS TRWANIA =====
            var durationMinutes = _event.Duration;

            // Jeśli Duration = 0, oblicz z różnicy dat
            if (durationMinutes <= 0 && _event.StartDateTime.HasValue && _event.EndDateTime.HasValue)
            {
                durationMinutes = (int)(_event.EndDateTime.Value - _event.StartDateTime.Value).TotalMinutes;
            }

            _lblDuration.Text = $"Duration: {durationMinutes} min";

            // ===== KATEGORIA =====
            var categoryDisplay = "None";
            if (!string.IsNullOrWhiteSpace(_event.Category))
            {
                // W bazie Category jest zapisane jako kod (0-8) w formie stringa
                if (int.TryParse(_event.Category, out var code))
                    categoryDisplay = Event.GetCategoryName(code);
                else
                    categoryDisplay = _event.Category;
            }
            _lblCategory.Text = "Category: " + categoryDisplay;

            // ===== PRIORYTET I FLAGI =====
            var priorityText = $"Priority: {_event.Priority}";
            var flags = "";

            // Dodaj flagi w nawiasach kwadratowych
            if (_event.IsImportant) flags += " [Important]";
            if (_event.IsUrgent) flags += " [Urgent]";
            if (_event.IsSetEvent) flags += " [Fixed time]";

            _lblWeight.Text = $"{priorityText}{flags}";
        }

        #endregion

        #region Edycja eventu

        /// <summary>
        /// Handler przycisku "Edit" - otwiera dynamicznie tworzony dialog edycji.
        /// 
        /// DIALOG ZAWIERA:
        /// - Pole tekstowe dla tytułu
        /// - Pole tekstowe dla opisu (wieloliniowe)
        /// - DateTimePicker dla daty/godziny startu
        /// - DateTimePicker dla daty/godziny końca
        /// - ComboBox dla kategorii
        /// 
        /// PO ZAPISANIU:
        /// - Waliduje dane (tytuł niepusty, end > start)
        /// - Aktualizuje obiekt Event
        /// - Zapisuje do bazy przez EventService
        /// - Jeśli zmienił się czas - wywołuje reschedule
        /// </summary>
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // Tworzymy dialog programowo (bez oddzielnego Form)
            using (var dlg = new Form())
            {
                // ===== KONFIGURACJA OKNA DIALOGOWEGO =====
                dlg.Text = "Edit event";
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.ClientSize = new Size(520, 360);
                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;

                // Pozycje kontrolek (dla spójnego layoutu)
                int xLabel = 15;       // X etykiet
                int xControl = 140;    // X kontrolek wprowadzania
                int y = 20;            // Aktualna pozycja Y
                int dy = 30;           // Odstęp między wierszami

                // ===== TYTUŁ =====
                var lblTitle = new Label
                {
                    Text = "Title:",
                    Location = new Point(xLabel, y + 4),
                    AutoSize = true
                };
                var txtTitle = new TextBox
                {
                    Location = new Point(xControl, y),
                    Width = 340,
                    Text = _event.Title ?? string.Empty
                };
                y += dy;

                // ===== OPIS (wieloliniowy) =====
                var lblDesc = new Label
                {
                    Text = "Description:",
                    Location = new Point(xLabel, y + 4),
                    AutoSize = true
                };
                var txtDescription = new TextBox
                {
                    Location = new Point(xControl, y),
                    Width = 340,
                    Height = 80,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Text = _event.Description ?? string.Empty
                };
                y += 90; // Większy odstęp bo pole wieloliniowe

                // ===== DATA/GODZINA STARTU =====
                var lblStart = new Label
                {
                    Text = "Start:",
                    Location = new Point(xLabel, y + 4),
                    AutoSize = true
                };
                var dtpStart = new DateTimePicker
                {
                    Location = new Point(xControl, y),
                    Width = 200,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "yyyy-MM-dd HH:mm",
                    ShowUpDown = true,  // Strzałki zamiast kalendarza
                    Value = _event.StartDateTime ?? DateTime.Now
                };
                y += dy;

                // ===== DATA/GODZINA KOŃCA =====
                var lblEnd = new Label
                {
                    Text = "End:",
                    Location = new Point(xLabel, y + 4),
                    AutoSize = true
                };

                // Domyślna wartość końca: start + duration (lub +60 min)
                DateTime defaultEnd = (_event.StartDateTime ?? DateTime.Now)
                                      .AddMinutes(_event.Duration > 0 ? _event.Duration : 60);

                var dtpEnd = new DateTimePicker
                {
                    Location = new Point(xControl, y),
                    Width = 200,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "yyyy-MM-dd HH:mm",
                    ShowUpDown = true,
                    Value = _event.EndDateTime ?? defaultEnd
                };
                y += dy;

                // ===== KATEGORIA =====
                var lblCategory = new Label
                {
                    Text = "Category:",
                    Location = new Point(xLabel, y + 4),
                    AutoSize = true
                };
                var cmbCategory = new ComboBox
                {
                    Location = new Point(xControl, y),
                    Width = 200,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                // Wypełnij ComboBox kategoriami
                cmbCategory.Items.AddRange(FixedCategories);

                // Znajdź aktualną kategorię eventu
                var currentCat = "None";
                if (!string.IsNullOrWhiteSpace(_event.Category))
                {
                    // Category w DB jest kodem (0-8) jako string - konwertuj na nazwę
                    if (int.TryParse(_event.Category, out var code))
                        currentCat = Event.GetCategoryName(code);
                    else
                        currentCat = _event.Category;
                }

                // Wybierz odpowiednią pozycję w ComboBox
                int catIndex = 0;
                for (int i = 0; i < cmbCategory.Items.Count; i++)
                {
                    if (string.Equals(cmbCategory.Items[i].ToString(), currentCat,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        catIndex = i;
                        break;
                    }
                }
                cmbCategory.SelectedIndex = catIndex;
                y += dy + 10;

                // ===== PRZYCISKI =====
                var btnOk = new Button
                {
                    Text = "Save",
                    DialogResult = DialogResult.OK,
                    Location = new Point(dlg.ClientSize.Width - 190, dlg.ClientSize.Height - 45),
                    Size = new Size(80, 30)
                };
                var btnCancel = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(dlg.ClientSize.Width - 100, dlg.ClientSize.Height - 45),
                    Size = new Size(80, 30)
                };

                // Ustaw domyślne przyciski dla Enter/Escape
                dlg.AcceptButton = btnOk;
                dlg.CancelButton = btnCancel;

                // Dodaj wszystkie kontrolki do dialogu
                dlg.Controls.AddRange(new Control[]
                {
                    lblTitle, txtTitle,
                    lblDesc, txtDescription,
                    lblStart, dtpStart,
                    lblEnd, dtpEnd,
                    lblCategory, cmbCategory,
                    btnOk, btnCancel
                });

                // ===== POKAZANIE DIALOGU I OBSŁUGA WYNIKU =====
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // --- WALIDACJA ---

                    // Tytuł jest wymagany
                    if (string.IsNullOrWhiteSpace(txtTitle.Text))
                    {
                        MessageBox.Show("Title cannot be empty.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Koniec musi być po starcie
                    if (dtpEnd.Value <= dtpStart.Value)
                    {
                        MessageBox.Show("End time must be after start time.", "Validation",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // --- ZAPISZ STARE WARTOŚCI CZASU (do wykrycia zmian) ---
                    var oldStart = _event.StartDateTime ?? DateTime.MinValue;
                    var oldEnd = _event.EndDateTime ?? DateTime.MinValue;
                    var newStart = dtpStart.Value;
                    var newEnd = dtpEnd.Value;
                    bool timeChanged = oldStart != newStart || oldEnd != newEnd;

                    // --- ZASTOSUJ ZMIANY DO OBIEKTU EVENT ---
                    _event.Title = txtTitle.Text.Trim();
                    _event.Description = txtDescription.Text.Trim();
                    _event.StartDateTime = newStart;
                    _event.EndDateTime = newEnd;
                    _event.Duration = (int)(newEnd - newStart).TotalMinutes;

                    // --- MAPOWANIE KATEGORII (nazwa → kod) ---
                    // W bazie zapisujemy kod (0-8) jako string
                    var selectedCategoryName = cmbCategory.SelectedItem?.ToString() ?? "None";
                    var selectedCode = 0;
                    for (int code = 0; code <= 8; code++)
                    {
                        if (string.Equals(Event.GetCategoryName(code), selectedCategoryName, 
                            StringComparison.OrdinalIgnoreCase))
                        {
                            selectedCode = code;
                            break;
                        }
                    }
                    _event.CategoryCode = selectedCode;
                    _event.Category = selectedCode.ToString();
                    // ColorCode jest obliczany automatycznie

                    // --- PRZELICZ PRIORYTET ---
                    try
                    {
                        _event.CalculatePriority();
                    }
                    catch
                    {
                        // Ignoruj jeśli metoda nie istnieje lub rzuca wyjątek
                    }

                    // --- ZAPISZ DO BAZY ---
                    if (timeChanged)
                    {
                        // Czas się zmienił - użyj reschedule (może wpłynąć na inne eventy)
                        _eventService.UpdateEventWithReschedule(_event, oldStart, oldEnd, newStart, newEnd);
                    }
                    else
                    {
                        // Tylko dane tekstowe - prosty update
                        _eventService.UpdateEvent(_event);
                    }

                    // --- ODŚWIEŻ WIDOK ---
                    LoadEventDetails();

                    // Ustaw DialogResult żeby CalendarView wiedział o zmianach
                    this.DialogResult = DialogResult.OK;

                    MessageBox.Show("Event updated successfully.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #endregion

        #region Usuwanie eventu

        /// <summary>
        /// Handler przycisku "Delete" - usuwa event z potwierdzeniem.
        /// Po usunięciu zamyka formularz i informuje CalendarView o zmianach.
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // Pokaż dialog potwierdzenia
            var result = MessageBox.Show(
                "Are you sure you want to delete this event?",
                "Delete event",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Usuń event z bazy
                _eventService.DeleteEvent(_event.EventID);

                MessageBox.Show("Event deleted successfully.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Zamknij formularz z wynikiem OK (CalendarView odświeży widok)
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        #endregion

        #region Ukończenie eventu

        /// <summary>
        /// Handler przycisku "Complete" - oznacza event jako ukończony.
        /// 
        /// DZIAŁANIE:
        /// 1. Pokaż dialog potwierdzenia
        /// 2. Dodaj punkty do statystyk kategorii (10 pkt)
        /// 3. Dodaj punkty do sumy do wymiany (10 pkt)
        /// 4. Usuń event z kalendarza (ukończony = zrobiony)
        /// 5. Zamknij formularz i pokaż komunikat sukcesu
        /// </summary>
        private void BtnComplete_Click(object sender, EventArgs e)
        {
            // Pokaż dialog potwierdzenia
            var result = MessageBox.Show(
                "Mark this event as completed?",
                "Complete event",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // --- DODAJ PUNKTY ---
                var pointsService = new Services.PointsService();

                // Konwertuj kod kategorii na nazwę (dla statystyk)
                string category = "None";
                if (!string.IsNullOrWhiteSpace(_event.Category))
                {
                    if (int.TryParse(_event.Category, out var code))
                        category = Event.GetCategoryName(code);
                    else
                        category = _event.Category;
                }

                // 10 punktów do statystyk kategorii
                pointsService.AddCategoryPoints(category, 10);
                
                // Priority * 10 punktów do sumy do wymiany na nagrody
                // (P1=10, P2=20, P3=30, P4=40, P5=50)
                int totalPoints = _event.Priority * 10;
                pointsService.AddTotalPoints(totalPoints);

                // --- USUŃ EVENT (ukończony = zrobiony) ---
                _eventService.DeleteEvent(_event.EventID);

                // --- ZAMKNIJ FORMULARZ ---
                this.DialogResult = DialogResult.OK;
                this.Close();

                // Pokaż komunikat sukcesu (po zamknięciu)
                MessageBox.Show($"Event marked as completed!\n\n+10 pts in {category}\n+{totalPoints} pts total (P{_event.Priority} × 10)",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Focus Mode

        /// <summary>
        /// Handler przycisku "Start Focus" - uruchamia tryb skupienia.
        /// 
        /// DZIAŁANIE:
        /// 1. Utwórz sesję focus przez FocusModeService
        /// 2. Ukryj ten formularz
        /// 3. Pokaż FocusModeActiveForm (timer, blokada rozpraszaczy)
        /// 4. Po zakończeniu - pokaż ten formularz ponownie
        /// </summary>
        private void BtnStartFocus_Click(object sender, EventArgs e)
        {
            // Utwórz serwis i rozpocznij sesję
            var focusService = new Services.FocusModeService();
            focusService.StartFocusSession(_event.EventID, _event.Duration);

            // Otwórz formularz aktywnego focus mode
            var focusForm = new FocusModeActiveForm(_event, focusService);

            // Ukryj ten formularz na czas focus
            this.Hide();
            var result = focusForm.ShowDialog();
            this.Show();

            // Jeśli focus zakończony pomyślnie - można odświeżyć dane
            if (result == DialogResult.OK)
            {
                // Pobierz zaktualizowany event z bazy (mogły się zmienić dane)
                var refreshedEvent = _eventService.GetEventById(_event.EventID);
                if (refreshedEvent != null)
                {
                    // Event został zaktualizowany - można dodać logikę odświeżania
                }
            }
        }

        #endregion

        #region Zamykanie formularza

        /// <summary>
        /// Handler przycisku "Close" - zamyka formularz.
        /// Jeśli wcześniej była edycja, DialogResult jest już ustawiony na OK.
        /// CalendarView sprawdzi DialogResult i odświeży widok jeśli potrzeba.
        /// </summary>
        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
