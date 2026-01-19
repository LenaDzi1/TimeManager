// ============================================================================
// AddEventForm.cs
// Formularz do tworzenia nowych eventów (zadań/aktywności) w kalendarzu.
// ============================================================================

using System;                   // Podstawowe typy .NET (DateTime, Exception, itp.)
using System.Collections.Generic; // Kolekcje generyczne (List<T>)
using System.ComponentModel;    // LicenseManager do wykrywania trybu design
using System.Windows.Forms;     // Windows Forms (Form, Button, itp.)
using TimeManager.Models;       // Model Event i inne modele danych
using TimeManager.Services;     // EventService do operacji na eventach

namespace TimeManager.Forms
{
    /// <summary>
    /// Formularz do dodawania nowego eventu (zadania/aktywności).
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Tworzenie eventów z tytułem, opisem i kategorią
    /// - Dwa tryby: SetEvent (stały czas) lub elastyczny (algorytm planuje)
    /// - Obsługa eventów cyklicznych (co X dni do daty końcowej)
    /// - Ustawianie deadline dla elastycznych eventów
    /// - Flagi: ważny, pilny, wymagany zasób, itp.
    /// 
    /// PRZEPŁYW DZIAŁANIA:
    /// 1. Użytkownik wypełnia formularz (tytuł, czas, opcje)
    /// 2. Kliknięcie "Save" waliduje dane
    /// 3. Dla eventów cyklicznych - generowane są klony na kolejne dni
    /// 4. Każdy event jest przekazywany do EventService.AddOrScheduleEvent()
    /// 5. Algorytm planowania znajduje slot czasowy (dla elastycznych)
    /// </summary>
    public partial class AddEventForm : Form
    {
        #region Pola prywatne

        /// <summary>
        /// Serwis do operacji na eventach (dodawanie, planowanie).
        /// Przekazywany przez konstruktor z MainForm/CalendarView.
        /// </summary>
        private readonly EventService _eventService;

        /// <summary>
        /// Data wybrana w kalendarzu podczas otwierania formularza.
        /// Używana jako domyślna data dla nowego eventu.
        /// </summary>
        private readonly DateTime _selectedDate;

        #endregion

        #region Konstruktory

        /// <summary>
        /// Konstruktor bezparametrowy - wymagany przez Visual Studio Designer.
        /// NIE używać w kodzie produkcyjnym - brak EventService!
        /// </summary>
        public AddEventForm()
            : this(null, DateTime.Now)
        {
        }

        /// <summary>
        /// Główny konstruktor formularza - używany w kodzie aplikacji.
        /// </summary>
        /// <param name="eventService">
        /// Serwis eventów do zapisywania i planowania.
        /// Może być null tylko w trybie designera.
        /// </param>
        /// <param name="selectedDate">
        /// Data wybrana w kalendarzu - będzie domyślną datą eventu.
        /// </param>
        public AddEventForm(EventService eventService, DateTime selectedDate)
        {
            // Wykryj czy jesteśmy w Visual Studio Designer
            // W trybie design nie inicjalizuj logiki biznesowej
            bool isDesign = LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            _eventService = eventService;
            _selectedDate = selectedDate;

            // Inicjalizuj kontrolki z Designer.cs
            InitializeComponent();

            // W trybie designera - stop, nie podpinaj eventów
            if (isDesign)
                return;

            // Podepnij handlery zdarzeń do kontrolek
            WireUpEvents();

            // Ustaw wartości domyślne dla wszystkich DateTimePicker-ów
            dtpStartDate.Value = _selectedDate;          // Data startu = wybrana data
            dtpEndDate.Value = _selectedDate;            // Data końca = wybrana data
            dtpDeadline.Value = _selectedDate;           // Deadline = wybrana data
            dtpStartTime.Value = DateTime.Now;           // Godzina startu = teraz
            dtpEndTime.Value = DateTime.Now.AddHours(1); // Godzina końca = za godzinę
            dtpDoInDaytimeUntil.Value = DateTime.Now.Date.AddHours(23); // Do 23:00
            dtpRecurringUntil.Value = _selectedDate.Date.AddDays(7);    // Rekurencja: +7 dni
            nudRecurringDays.Value = 1;                  // Co 1 dzień

            // Domyślny tryb: SetEvent (stały czas) - checkbox zaznaczony
            chkSetEvent.Checked = true;
            UpdateModePanels(); // Dostosuj widoczność paneli
        }

        #endregion

        #region Podpinanie zdarzeń

        /// <summary>
        /// Podpina handlery zdarzeń do kontrolek.
        /// Wywoływane raz podczas inicjalizacji formularza.
        /// </summary>
        private void WireUpEvents()
        {
            // ===== Synchronizacja czasu i trwania =====
            // Gdy zmieni się duration - przelicz EndTime
            nudDuration.ValueChanged += nudDuration_ValueChanged;
            // Gdy zmieni się EndTime - przelicz duration
            dtpEndTime.ValueChanged += dtpEndTime_ValueChanged;

            // ===== Przełączniki UI =====
            // Checkbox "Cykliczny" - pokaż/ukryj opcje rekurencji
            chkRecurring.CheckedChanged += chkRecurring_CheckedChanged;
            // Checkbox "Do godziny" - włącz/wyłącz DateTimePicker
            chkDoInDaytimeUntil.CheckedChanged += chkDoInDaytimeUntil_CheckedChanged;
            // Checkbox "Deadline" - włącz/wyłącz DateTimePicker deadline
            chkDeadline.CheckedChanged += (s, e) => dtpDeadline.Enabled = chkDeadline.Checked;
            // Checkbox "Stały czas" - przełącz tryb SetEvent/elastyczny
            chkSetEvent.CheckedChanged += chkSetEvent_CheckedChanged;
            // Rekurencja: domyślnie wyłączona
            dtpRecurringUntil.Enabled = false;

            // ===== Przyciski =====
            btnSave.Click += btnSave_Click;     // Zapisz event
            btnCancel.Click += btnCancel_Click; // Anuluj
        }

        #endregion

        #region Handlery zdarzeń kontrolek

        /// <summary>
        /// Handler: zmiana checkboxa "Event cykliczny".
        /// Włącza/wyłącza kontrolki konfiguracji rekurencji.
        /// </summary>
        private void chkRecurring_CheckedChanged(object sender, EventArgs e)
        {
            bool isRecurring = chkRecurring.Checked;

            // Włącz/wyłącz wszystkie kontrolki rekurencji
            nudRecurringDays.Enabled = isRecurring;     // "Co ile dni"
            lblRecurringEach.Enabled = isRecurring;     // Label "Each"
            lblRecurringDays.Enabled = isRecurring;     // Label "days"
            lblRecurringUntil.Enabled = isRecurring;    // Label "until"
            dtpRecurringUntil.Enabled = isRecurring;    // Data końcowa

            // Aktualizuj widoczność (zależy też od trybu SetEvent)
            UpdateModePanels();
        }

        /// <summary>
        /// Handler: zmiana checkboxa "Wykonaj do godziny".
        /// Włącza/wyłącza DateTimePicker z godziną graniczną.
        /// </summary>
        private void chkDoInDaytimeUntil_CheckedChanged(object sender, EventArgs e)
        {
            dtpDoInDaytimeUntil.Enabled = chkDoInDaytimeUntil.Checked;
        }

        /// <summary>
        /// Handler: zmiana wartości "Czas trwania" (nudDuration).
        /// Automatycznie przelicza godzinę końcową na podstawie startu + duration.
        /// Działa tylko w trybie SetEvent (stały czas).
        /// </summary>
        private void nudDuration_ValueChanged(object sender, EventArgs e)
        {
            // W trybie elastycznym nie synchronizuj
            if (!chkSetEvent.Checked)
                return;

            // Oblicz nową godzinę końcową: start + duration
            var startDateTime = dtpStartDate.Value.Date.Add(dtpStartTime.Value.TimeOfDay);
            var endDateTime = startDateTime.AddMinutes((double)nudDuration.Value);

            // Zaktualizuj kontrolki końca
            dtpEndDate.Value = endDateTime.Date;
            dtpEndTime.Value = endDateTime;
        }

        /// <summary>
        /// Handler: zmiana godziny końcowej (dtpEndTime).
        /// Automatycznie przelicza duration na podstawie różnicy start-end.
        /// Działa tylko w trybie SetEvent (stały czas).
        /// </summary>
        private void dtpEndTime_ValueChanged(object sender, EventArgs e)
        {
            // W trybie elastycznym nie synchronizuj
            if (!chkSetEvent.Checked)
                return;

            // Oblicz różnicę między startem a końcem
            var startDateTime = dtpStartDate.Value.Date.Add(dtpStartTime.Value.TimeOfDay);
            var endDateTime = dtpEndDate.Value.Date.Add(dtpEndTime.Value.TimeOfDay);
            var duration = (int)(endDateTime - startDateTime).TotalMinutes;

            // Zaktualizuj duration (z ograniczeniem do min/max)
            if (duration > 0)
            {
                nudDuration.Value = Math.Max(nudDuration.Minimum,
                    Math.Min(nudDuration.Maximum, duration));
            }
        }

        /// <summary>
        /// Handler: zmiana checkboxa "Stały czas" (SetEvent).
        /// Przełącza między trybem stałego czasu a elastycznym.
        /// </summary>
        private void chkSetEvent_CheckedChanged(object sender, EventArgs e)
        {
            UpdateModePanels();
        }

        /// <summary>
        /// Aktualizuje widoczność paneli w zależności od trybu (SetEvent vs elastyczny).
        /// 
        /// TRYB SetEvent (stały czas):
        /// - Widoczny schedulePanel (data/godzina start/end)
        /// - Można ustawić rekurencję
        /// 
        /// TRYB elastyczny:
        /// - Widoczny flexiblePanel (deadline, opcje)
        /// - Algorytm sam znajdzie slot czasowy
        /// </summary>
        private void UpdateModePanels()
        {
            bool isSetEvent = chkSetEvent.Checked;

            // Przełącz widoczność paneli
            schedulePanel.Visible = isSetEvent;    // Panel z datą/godziną
            flexiblePanel.Visible = !isSetEvent;   // Panel z opcjami elastycznymi

            // Włącz/wyłącz kontrolki daty/godziny
            dtpStartDate.Enabled = isSetEvent;
            dtpStartTime.Enabled = isSetEvent;
            dtpEndDate.Enabled = isSetEvent;
            dtpEndTime.Enabled = isSetEvent;

            // Rekurencja dostępna tylko w trybie SetEvent
            chkRecurring.Visible = isSetEvent;
            lblRecurringEach.Visible = isSetEvent && chkRecurring.Checked;
            nudRecurringDays.Visible = isSetEvent && chkRecurring.Checked;
            lblRecurringDays.Visible = isSetEvent && chkRecurring.Checked;
            lblRecurringUntil.Visible = isSetEvent && chkRecurring.Checked;
            dtpRecurringUntil.Visible = isSetEvent && chkRecurring.Checked;

            // Wyłączając tryb SetEvent - automatycznie wyłącz rekurencję
            if (!isSetEvent)
            {
                chkRecurring.Checked = false;
            }
        }

        #endregion

        #region Zapisywanie eventu

        /// <summary>
        /// Handler: kliknięcie przycisku "Save".
        /// Waliduje dane, tworzy obiekt Event i zapisuje przez EventService.
        /// Dla eventów cyklicznych generuje kopie na kolejne dni.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            // ===== WALIDACJA =====
            // Tytuł jest wymagany
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter an event title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ===== OBLICZANIE DATY/GODZINY =====
            DateTime? startDateTime = null;
            DateTime? endDateTime = null;

            if (chkSetEvent.Checked)
            {
                // TRYB SetEvent: użyj wybranych dat i godzin
                startDateTime = dtpStartDate.Value.Date.Add(dtpStartTime.Value.TimeOfDay);
                endDateTime = dtpEndDate.Value.Date.Add(dtpEndTime.Value.TimeOfDay);

                // Nie pozwalaj na ustawianie w przeszłości
                if (startDateTime.Value < DateTime.Now)
                {
                    MessageBox.Show("Start time cannot be in the past.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                // TRYB elastyczny: algorytm zaplanuje automatycznie
                // Podpowiedź: nie planuj wcześniej niż teraz
                var hint = dtpStartDate.Value.Date.Add(dtpStartTime.Value.TimeOfDay);
                if (hint < DateTime.Now)
                    hint = DateTime.Now;
                startDateTime = hint;
            }

            // ===== MAPOWANIE KATEGORII =====
            var categoryName = cmbCategory.SelectedItem?.ToString() ?? "None";
            var categoryCode = MapCategoryCode(categoryName);

            // ===== TWORZENIE OBIEKTU EVENT =====
            var newEvent = new Event
            {
                // Podstawowe dane
                Title = txtTitle.Text,
                Description = txtDescription.Text,

                // Data/godzina - dla elastycznych może być null, algorytm ustali
                StartDateTime = startDateTime,
                EndDate = chkSetEvent.Checked ? dtpEndDate.Value.Date : null,
                EndTime = chkSetEvent.Checked ? dtpEndTime.Value : null,
                EndDateTime = endDateTime,
                Duration = (int)nudDuration.Value,

                // Kategoria
                CategoryCode = categoryCode,
                Category = categoryCode.ToString(),

                // Deadline - tylko dla elastycznych eventów
                // Ustawiamy na koniec dnia (23:59:59)
                Deadline = (!chkSetEvent.Checked && chkDeadline.Checked)
                    ? dtpDeadline.Value.Date.AddDays(1).AddSeconds(-1)
                    : (DateTime?)null,

                // Flagi booleowskie
                HasMargin = false,          // Margines czasowy (nieużywany)
                MarginMinutes = null,
                IsImportant = chkImportant.Checked,    // Ważny
                IsUrgent = chkUrgent.Checked,          // Pilny
                IsSetEvent = chkSetEvent.Checked,      // Tryb stałego czasu
            
                DoInDaytimeUntil = chkDoInDaytimeUntil.Checked,
                DoInDaytimeUntilTime = chkDoInDaytimeUntil.Checked ? dtpDoInDaytimeUntil.Value : null,
         

                // Wykluczony dzień tygodnia
              
                // Rekurencja - flagi (faktyczne klony generowane poniżej)
                IsRecurring = chkRecurring.Checked,
                RecurrenceDays = chkRecurring.Checked ? (int?)nudRecurringDays.Value : null,

                // Reguła "minimum X razy w Y dni"
              

                // Zasoby (nieużywane aktualnie)
            
            };

            // ===== OBLICZANIE PRIORYTETU I KOLORU =====
            newEvent.CalculatePriority();

            try
            {
                // ===== OBSŁUGA REKURENCJI =====
                // Lista eventów do dodania (główny + klony)
                var eventsToAdd = new List<Event> { newEvent };

                // Wyczyść flagi rekurencji na głównym evencie
                // (zapobiega ponownemu generowaniu przez algorytm)
                newEvent.IsRecurring = false;
                newEvent.RecurrenceDays = null;

                // Jeśli włączona rekurencja - generuj klony
                if (chkRecurring.Checked && nudRecurringDays.Value > 0 && dtpRecurringUntil.Enabled)
                {
                    DateTime baseDate = dtpStartDate.Value.Date;      // Data bazowa
                    DateTime untilDate = dtpRecurringUntil.Value.Date; // Data końcowa
                    int step = (int)nudRecurringDays.Value;           // Co ile dni

                    // Generuj klony od następnego wystąpienia do daty końcowej
                    DateTime current = baseDate.AddDays(step);
                    while (current <= untilDate)
                    {
                        // Utwórz klon eventu
                        var clone = CloneEvent(newEvent);

                        // Przesuń daty na nowy dzień (zachowaj godziny)
                        if (chkSetEvent.Checked && startDateTime.HasValue && endDateTime.HasValue)
                        {
                            var timeSpanStart = startDateTime.Value.TimeOfDay;
                            var timeSpanEnd = endDateTime.Value.TimeOfDay;
                            clone.StartDateTime = current.Add(timeSpanStart);
                            clone.EndDateTime = current.Add(timeSpanEnd);
                            clone.EndDate = clone.EndDateTime?.Date;
                            clone.EndTime = clone.EndDateTime;
                        }
                        else
                        {
                            // Elastyczny event - daty null
                            clone.StartDateTime = null;
                            clone.EndDateTime = null;
                            clone.EndDate = null;
                            clone.EndTime = null;
                        }

                        eventsToAdd.Add(clone);
                        current = current.AddDays(step);
                    }
                }

                // ===== ZAPISYWANIE EVENTÓW =====
                foreach (var ev in eventsToAdd)
                {
                    // Oblicz okno czasowe do szukania slotu
                    var (from, to) = CalculateScheduleWindow(ev, startDateTime, endDateTime);
                    // Dodaj i zaplanuj event
                    _eventService.AddOrScheduleEvent(ev, from, to);
                }

                // Sukces - pokaż komunikat i zamknij formularz
                MessageBox.Show("Event added and scheduled successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (InvalidOperationException ex)
            {
                // Błąd logiki (np. brak wolnego slotu)
                MessageBox.Show(ex.Message, "Cannot add event",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           
        }

        /// <summary>
        /// Oblicza okno czasowe (from-to) w którym algorytm ma szukać wolnego slotu.
        /// 
        /// DLA SetEvent:
        /// - Od: wybrana data
        /// - Do: wybrana data + 1 dzień
        /// 
        /// DLA elastycznego:
        /// - Od: dzisiaj (nie od przeglądanej daty!)
        /// - Do: deadline lub +30 dni
        /// </summary>
        /// <param name="ev">Event do zaplanowania</param>
        /// <param name="startDateTime">Data/godzina startu (dla SetEvent)</param>
        /// <param name="endDateTime">Data/godzina końca (dla SetEvent)</param>
        /// <returns>Tupla (scheduleFrom, scheduleTo)</returns>
        private (DateTime scheduleFrom, DateTime scheduleTo) CalculateScheduleWindow(
            Event ev, DateTime? startDateTime, DateTime? endDateTime)
        {
            DateTime scheduleFrom;

            if (ev.IsSetEvent && ev.StartDateTime.HasValue)
            {
                // SetEvent - szukaj od wybranej daty
                scheduleFrom = ev.StartDateTime.Value.Date;
            }
            else
            {
                // Elastyczny - ZAWSZE od dziś (nie od przeglądanej daty)
                scheduleFrom = DateTime.Today;
            }

            DateTime scheduleTo;

            if (ev.IsSetEvent && endDateTime.HasValue)
            {
                // SetEvent - do następnego dnia po końcu
                scheduleTo = endDateTime.Value.Date.AddDays(1);
            }
            else if (ev.Deadline.HasValue)
            {
                // Jest deadline - szukaj do deadline
                scheduleTo = ev.Deadline.Value.Date.AddDays(1);
            }
            else
            {
                // Brak deadline - domyślnie szukaj 30 dni do przodu
                scheduleTo = scheduleFrom.AddDays(30);
            }

            return (scheduleFrom, scheduleTo);
        }

        #endregion

        #region Metody pomocnicze

        /// <summary>
        /// Tworzy głęboką kopię eventu (dla rekurencji).
        /// Nowy event ma te same właściwości ale bez ID i bez flag rekurencji.
        /// </summary>
        /// <param name="source">Event źródłowy do skopiowania</param>
        /// <returns>Nowy obiekt Event z skopiowanymi właściwościami</returns>
        private Event CloneEvent(Event source)
        {
            return new Event
            {
                // Kopiuj wszystkie właściwości
                Title = source.Title,
                Description = source.Description,
                StartDateTime = source.StartDateTime,
                EndDateTime = source.EndDateTime,
                EndDate = source.EndDate,
                EndTime = source.EndTime,
                Duration = source.Duration,
                CategoryCode = source.CategoryCode,
                Category = source.Category,
                Deadline = source.Deadline,
                HasMargin = source.HasMargin,
                MarginMinutes = source.MarginMinutes,
                IsImportant = source.IsImportant,
                IsUrgent = source.IsUrgent,
                IsSetEvent = source.IsSetEvent,
     
                Priority = source.Priority,

                // NIE kopiuj flag rekurencji (zapobiega nieskończonemu generowaniu)
                IsRecurring = false,
                RecurrenceDays = null,
           
            };
        }

        /// <summary>
        /// Mapuje nazwę kategorii (string) na kod kategorii (int).
        /// Kody są używane w bazie danych i do kolorowania eventów.
        /// </summary>
        /// <param name="name">Nazwa kategorii (np. "Health", "Work and Career")</param>
        /// <returns>
        /// Kod kategorii:
        /// 0 = None/Unknown
        /// 1 = Health
        /// 2 = Family
        /// 3 = Mentality
        /// 4 = Finance
        /// 5 = Work and Career
        /// 6 = Relax
        /// 7 = Self Development and Education
        /// 8 = Friends and People
        /// </returns>
        private int MapCategoryCode(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return 0;

            switch (name.Trim().ToLowerInvariant())
            {
                case "health": return 1;
                case "family": return 2;
                case "mentality": return 3;
                case "finance": return 4;
                case "work and career": return 5;
                case "relax": return 6;
                case "self development and education": return 7;
                case "friends and people": return 8;
                default: return 0;
            }
        }

        /// <summary>
        /// Handler: kliknięcie przycisku "Cancel".
        /// Zamyka formularz bez zapisywania.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}
