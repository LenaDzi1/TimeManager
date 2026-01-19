// ============================================================================
// TrackingView.cs
// Widok śledzenia nawyków, jedzenia, leków i roślin.
// ============================================================================

#region Importy
using System;                   // Podstawowe typy .NET (DateTime, Math)
using System.Collections.Generic; // Kolekcje (List, Dictionary)
using System.Drawing;           // Grafika (Color, Point, Size)
using System.Linq;              // LINQ (Where, OrderBy)
using System.Windows.Forms;     // Windows Forms (UserControl, Panel)
using TimeManager.Models;       // Modele (HabitCategory, FoodContainer, Plant)
using TimeManager.Services;     // Serwisy (TrackingService, FirstAidService)
#endregion

namespace TimeManager.Forms
{
    /// <summary>
    /// Widok śledzenia nawyków, jedzenia, leków i roślin.
    /// 
    /// ZAKŁADKI (TabControl):
    /// - Habits: nawyki z kategorii (Health, Work, itp.) z dziennym/nocnym śledzeniem
    /// - Food Tracking: lodówka, zamrażarka, spiżarnia, przepisy, lista zakupów
    /// - First Aid: apteczka, leki, harmonogramy dawkowania
    /// - Plants: rośliny do podlewania z przypomnieniami
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - CRUD dla każdego typu elementu
    /// - Filtrowanie po kategorii/kontenerze/dacie
    /// - Integracja z EventService (harmonogramy)
    /// - Ograniczenia ról (Kids nie widzą leków)
    /// </summary>
    public partial class TrackingView : UserControl
    {
        #region Pola prywatne - Serwisy

        /// <summary>Główny serwis trackingu (Habits, Food, Plants).</summary>
        private readonly TrackingService _trackingService;

        /// <summary>Serwis punktów (do nagradzania za nawyki).</summary>
        private readonly PointsService _pointsService = new();

        /// <summary>Serwis eventów (do harmonogramów).</summary>
        private EventService _eventService;

        /// <summary>Serwis First Aid (leki, apteczka).</summary>
        private FirstAidService _firstAidService;

        /// <summary>Serwis użytkowników.</summary>
        private readonly UserService _userService = new();

        #endregion

        #region Pola prywatne - Stan użytkownika

        /// <summary>ID aktualnie wyświetlanego użytkownika.</summary>
        private int? _currentUserId;

        #endregion

        #region Pola prywatne - Habits

        /// <summary>Lista kategorii nawyków.</summary>
        private List<HabitCategory> _habitCategories = new();

        /// <summary>Wszystkie kroki nawyków (dla filtrowania).</summary>
        private List<HabitStep> _allHabitSteps = new();

        /// <summary>Aktualnie wyświetlane kroki nawyków.</summary>
        private List<HabitStep> _habitSteps = new();

        /// <summary>Maksymalna szerokość kart nawyków.</summary>
        private const int HabitCardsMaxWidth = 280;

        /// <summary>Stan dzień/noc dla kart nawyków (key = stepId).</summary>
        private readonly Dictionary<int, DayNightState> _habitCardStates = new();

        /// <summary>Stan ukończenia nawyków dla wybranej daty.</summary>
        private Dictionary<int, HabitCompletionState> _habitCompletions = new();

        /// <summary>Flaga blokująca handlery dzień/noc (podczas aktualizacji UI).</summary>
        private bool _suppressDayNightHandlers;

        /// <summary>Wybrana data dla filtrowania nawyków.</summary>
        private DateTime _selectedHabitDate = DateTime.Today;

        #endregion

        #region Pola prywatne - Food Tracking

        /// <summary>Wybrany typ kontenera: "Fridge", "Freezer", "Pantry".</summary>
        private string _selectedContainerType = "Fridge";

        /// <summary>Aktualnie wybrany kontener jedzenia.</summary>
        private FoodContainer _selectedContainer;

        #endregion

        #region Pola prywatne - First Aid

        /// <summary>Aktualnie wybrany kontener leków.</summary>
        private MedicineContainer _selectedMedContainer;

        /// <summary>Wybrana data dla harmonogramu dawkowania.</summary>
        private DateTime _selectedMedDate = DateTime.Today;

        #endregion

        #region Konstruktor

        /// <summary>
        /// Konstruktor widoku śledzenia.
        /// 
        /// KOLEJNOŚĆ INICJALIZACJI:
        /// 1. Przypisz serwisy
        /// 2. InitializeComponent()
        /// 3. WireUpEvents() - podepnij handlery
        /// 4. Załaduj dane (kontenery, przepisy, rośliny, nawyki)
        /// 5. Zastosuj ograniczenia ról
        /// </summary>

        public TrackingView(TrackingService trackingService, EventService eventService = null, FirstAidService firstAidService = null)
        {
            _trackingService = trackingService ?? throw new ArgumentNullException(nameof(trackingService));
            _eventService = eventService ?? new EventService();
            _firstAidService = firstAidService ?? new FirstAidService();

            InitializeComponent();
            WireUpEvents();
            ConfigureStaticControls();

            HighlightSelectedTypeButton();
            LoadContainers();
            LoadRecipes();
            RefreshPlants();

            LoadMedContainers();
            InitializeMedCalendar();

            InitializeHabits();

            ApplyRoleRestrictions();

            if (_btnAddMedContainer != null) _btnAddMedContainer.Visible = true; 
            if (_btnMedShoppingList != null) _btnMedShoppingList.Visible = false;
        }

        private void ApplyRoleRestrictions()
        {
            // Dzieci mają ograniczony dostęp do funkcji śledzenia
            if (UserSession.IsKid)
            {
                if (_medTab != null)
                {
                    _tabControl.TabPages.Remove(_medTab);
                }

                if (_btnDeleteContainer != null)
                {
                    _btnDeleteContainer.Visible = false;
                    _btnDeleteContainer.Enabled = false;
                }
            }
        }

        public void NavigateToNotification(string notificationType, int? referenceId)
        {
            var type = (notificationType ?? string.Empty).ToLowerInvariant();
            switch (type)
            {
                case "foodtracking":
                case "food":
                case "fridge":
                    _tabControl.SelectedTab = _fridgeTab;
                    break;
                case "medicinetracking":
                case "medicine":
                case "medication":
                    _tabControl.SelectedTab = _medTab;
                    break;
                case "planttracker":
                case "plant":
                    _tabControl.SelectedTab = _plantsTab;
                    if (referenceId.HasValue)
                    {
                        RefreshPlants();
                        FocusPlantCard(referenceId.Value);
                    }
                    break;
                default:
                    _tabControl.SelectedTab = _fridgeTab;
                    break;
            }
        }

        private void WireUpEvents()
        {
            _btnAddFood.Click += (_, __) => ShowAddFridgeItemDialog();
            _btnMoveFood.Click += (_, __) => OnMoveSelectedItem();
            _btnDeleteFood.Click += (_, __) => OnDeleteSelectedItem();

            _btnAddContainer.Click += (_, __) => OnAddContainer();
            _btnDeleteContainer.Click += (_, __) => OnDeleteSelectedContainer();

            _btnFridge.Click += (_, __) => OnContainerTypeChanged("Fridge");
            _btnFreezer.Click += (_, __) => OnContainerTypeChanged("Freezer");
            _btnPantry.Click += (_, __) => OnContainerTypeChanged("Pantry");
            _containerTabs.SelectedIndexChanged += (_, __) => OnContainerTabChanged();

            _fridgeListView.DoubleClick += (_, __) => OnEditSelectedItem();

            _btnAddRecipe.Click += (_, __) => ShowAddRecipeDialog();
            _btnShoppingList.Click += (_, __) => ShowShoppingList();
            _recipeListView.DoubleClick += (_, __) => OnRecipeDoubleClick();

            _btnAddMedicine.Click += (_, __) => ShowAddMedicineDialog();
            _btnMoveMedicine.Click += (_, __) => OnMoveMedicineItem();
            _btnDeleteMedicine.Click += (_, __) => OnDeleteMedicineItem();
            _btnAddMedContainer.Click += (_, __) => OnAddMedContainer();
            _btnMedShoppingList.Click += (_, __) => ShowMedShoppingList();
            _medContainerTabs.SelectedIndexChanged += (_, __) => OnMedContainerTabChanged();
            _medicineListView.DoubleClick += (_, __) => OnEditMedicineItem();
            _intakeCalendar.DateSelected += (_, e) => OnMedCalendarDateChanged(e.Start);
            _btnAddSchedule.Click += (_, __) => ShowAddScheduleDialog();

            _btnAddPlant.Click += (_, __) => ShowAddPlantDialog();

            _typeButtonsPanel.Resize += (_, __) => CenterTypeButtonsPanel();

            _btnAddHabitCategory.Click += (_, __) => OnAddHabitCategory();
            _btnAddHabitStep.Click += (_, __) => OnAddHabitStep();
            _habitCategoryTabs.SelectedIndexChanged += (_, __) => OnHabitCategoryChanged();
            _habitStepsList.DoubleClick += (_, __) => OnHabitStepDoubleClick();
            _habitStepsList.SelectedIndexChanged += (_, __) => OnHabitStepSelectionChanged();
            _habitCalendar.DateSelected += (_, args) =>
            {
                _selectedHabitDate = args.Start.Date;
                ApplyHabitFilter();
            };
        }

        private void ConfigureStaticControls()
        {
            _fridgeListView.Columns.Clear();
            _fridgeListView.Columns.Add("Name", 150);
            _fridgeListView.Columns.Add("Amount", 140);
            _fridgeListView.Columns.Add("Expire Date", 140);
            _fridgeListView.Columns.Add("State", 140);

            _recipeListView.Columns.Clear();
            _recipeListView.Columns.Add("Name", 150);
            _recipeListView.Columns.Add("Time of Preparation", 100);
            _recipeListView.Columns.Add("Scheduled", 70);
        }

        private void CenterTypeButtonsPanel()
        {
            if (_typeButtonsPanel == null || _typeButtonsInnerPanel == null) return;
            
            var parentHeight = _typeButtonsPanel.ClientSize.Height - _typeButtonsPanel.Padding.Vertical;
            var panelHeight = _typeButtonsInnerPanel.Height;
            var newY = Math.Max(0, (parentHeight - panelHeight) / 2);
            _typeButtonsInnerPanel.Location = new Point(_typeButtonsInnerPanel.Location.X, newY);
        }

        private void OnDeleteSelectedContainer()
        {
            if (_selectedContainer == null)
            {
                MessageBox.Show(
                    "Select a container to delete first.",
                    "No container",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the container \"{_selectedContainer.Name}\" " +
                "TOGETHER with all products inside?",
                "Delete container",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                _trackingService.DeleteContainerWithItems(_selectedContainer.ContainerID);

                _selectedContainer = null;
                LoadContainers();
                UpdateDeleteContainerButtonState();
                LoadFridgeItemsForSelectedContainer();
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

        private void OnContainerTypeChanged(string type)
        {
            _selectedContainerType = type;
            HighlightSelectedTypeButton();
            LoadContainers();
        }

        private void HighlightSelectedTypeButton()
        {
            if (_selectedContainerType == "Fridge")
            {
                _btnFridge.BackColor = Color.FromArgb(235, 245, 255);
                _btnFridge.ForeColor = Color.FromArgb(52, 152, 219);
                _btnFridge.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
                _btnFridge.FlatAppearance.BorderSize = 2;
            }
            else
            {
                _btnFridge.BackColor = Color.White;
                _btnFridge.ForeColor = Color.FromArgb(80, 80, 80);
                _btnFridge.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
                _btnFridge.FlatAppearance.BorderSize = 1;
            }

            if (_selectedContainerType == "Freezer")
            {
                _btnFreezer.BackColor = Color.FromArgb(235, 245, 255);
                _btnFreezer.ForeColor = Color.FromArgb(52, 152, 219);
                _btnFreezer.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
                _btnFreezer.FlatAppearance.BorderSize = 2;
            }
            else
            {
                _btnFreezer.BackColor = Color.White;
                _btnFreezer.ForeColor = Color.FromArgb(80, 80, 80);
                _btnFreezer.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
                _btnFreezer.FlatAppearance.BorderSize = 1;
            }

            if (_selectedContainerType == "Pantry")
            {
                _btnPantry.BackColor = Color.FromArgb(235, 245, 255);
                _btnPantry.ForeColor = Color.FromArgb(52, 152, 219);
                _btnPantry.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
                _btnPantry.FlatAppearance.BorderSize = 2;
            }
            else
            {
                _btnPantry.BackColor = Color.White;
                _btnPantry.ForeColor = Color.FromArgb(80, 80, 80);
                _btnPantry.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
                _btnPantry.FlatAppearance.BorderSize = 1;
            }
        }

        #endregion

        #region Habits Tracking

        /// <summary>
        /// Inicjalizuje sekcję nawyków: lista, kolumny, kategorie.
        /// </summary>
        private void InitializeHabits()
        {
            _btnAddHabitStep.Enabled = false;
            SetupHabitList();
            SetupMedicationList();
            EnsureHabitUserId();
            LoadHabitCategories();
        }

        private void EnsureHabitUserId()
        {
            if (_currentUserId.HasValue)
                return;

            var userName = UserSession.UserName;
            var user = string.IsNullOrWhiteSpace(userName) ? null : _userService.GetUser(userName);

            _currentUserId = user?.UserId ?? 1;
        }

        private void SetupHabitList()
        {
            _habitStepsList.Columns.Clear();
            _habitStepsList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _habitStepsList.Columns.Add("Name", 180);
            _habitStepsList.Columns.Add("From", 90);
            _habitStepsList.Columns.Add("To", 90);
            _habitStepsList.Columns.Add("Repeat (days)", 100);
            _habitStepsList.Columns.Add("Day/Night", 80);
        }

        private void SetupMedicationList()
        {
            _medicineListView.Columns.Clear();
            _medicineListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            _medicineListView.Columns.Add("name", 180);
            _medicineListView.Columns.Add("amount", 100);
            _medicineListView.Columns.Add("expire date", 120);
            _medicineListView.Columns.Add("state", 120);
        }

        private void LoadHabitCategories()
        {
            EnsureHabitUserId();
            if (_currentUserId == null)
                return;

            _habitCategories = _trackingService.GetHabitCategories(_currentUserId.Value);
            RenderHabitCategoryTabs();
            LoadHabitStepsForSelectedCategory();
        }

        private void RenderHabitCategoryTabs()
        {
            _habitCategoryTabs.TabPages.Clear();
            _habitCategoryTabs.MouseClick -= OnHabitCategoryTabClicked; 
            _habitCategoryTabs.MouseClick += OnHabitCategoryTabClicked;

            foreach (var cat in _habitCategories)
            {
                var tab = new TabPage(cat.Name) { Tag = cat };
                _habitCategoryTabs.TabPages.Add(tab);
            }

            _btnAddHabitStep.Enabled = _habitCategoryTabs.TabPages.Count > 0;
        }

        private void OnHabitCategoryTabClicked(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;

            for (int i = 0; i < _habitCategoryTabs.TabPages.Count; i++)
            {
                Rectangle r = _habitCategoryTabs.GetTabRect(i);
                if (r.Contains(e.Location))
                {
                    var tab = _habitCategoryTabs.TabPages[i];
                    
                    if (e.Button == MouseButtons.Right || (e.Button == MouseButtons.Left && _habitCategoryTabs.SelectedIndex == i))
                    {
                        var cat = tab.Tag as HabitCategory;
                        if (cat == null) return;

                        var ctx = new ContextMenuStrip();
                        ctx.Items.Add("Rename", null, (s, args) => RenameHabitCategory(cat));
                        ctx.Items.Add("Delete", null, (s, args) => DeleteHabitCategory(cat));
                        ctx.Show(_habitCategoryTabs, e.Location);
                    }
                    else
                    {
                        _habitCategoryTabs.SelectedIndex = i;
                    }
                    return;
                }
            }
        }

        private void RenameHabitCategory(HabitCategory cat)
        {
            string newName = AddContainerForm.Show("New name:", "Rename Category", cat.Name);
            if (string.IsNullOrWhiteSpace(newName) || newName == cat.Name) return;

            _trackingService.UpdateHabitCategory(cat.HabitCategoryId, newName.Trim());
            LoadHabitCategories();
            foreach(TabPage t in _habitCategoryTabs.TabPages)
            {
                if ((t.Tag as HabitCategory)?.HabitCategoryId == cat.HabitCategoryId)
                {
                    _habitCategoryTabs.SelectedTab = t;
                    break;
                }
            }
        }

        private void DeleteHabitCategory(HabitCategory cat)
        {
            if (MessageBox.Show($"Delete category '{cat.Name}' and all its habits?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _trackingService.DeleteHabitCategory(cat.HabitCategoryId);
                LoadHabitCategories();
            }
        }

        private HabitCategory GetSelectedHabitCategory()
        {
            if (_habitCategoryTabs.SelectedTab?.Tag is HabitCategory cat)
                return cat;
            return null;
        }

        private void LoadHabitStepsForSelectedCategory()
        {
            var cat = GetSelectedHabitCategory();
            if (cat == null || _currentUserId == null)
            {
                _allHabitSteps = new List<HabitStep>();
                _habitSteps = new List<HabitStep>();
                PopulateHabitList(_habitSteps);
                _btnAddHabitStep.Enabled = false;
                return;
            }

            _allHabitSteps = _trackingService.GetHabitSteps(cat.HabitCategoryId, _currentUserId.Value);
            _habitCardStates.Clear();
            _btnAddHabitStep.Enabled = true;
            ApplyHabitFilter();
        }

        private void OnAddHabitCategory()
        {
            EnsureHabitUserId();
            if (_currentUserId == null)
                return;

            string name = AddContainerForm.Show(
                "Category name:", "Add habit category", "Habit 1");

            if (string.IsNullOrWhiteSpace(name))
                return;

            _trackingService.AddHabitCategory(_currentUserId.Value, name.Trim());
            LoadHabitCategories();
        }

        private void OnAddHabitStep()
        {
            var cat = GetSelectedHabitCategory();
            EnsureHabitUserId();
            if (cat == null || _currentUserId == null)
            {
                MessageBox.Show("Add and select a category first.", "Habits", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new HabitStepForm(_trackingService, _currentUserId.Value);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                var step = dlg.Result;
                step.HabitCategoryId = cat.HabitCategoryId;
                step.UserId = _currentUserId.Value;
                _trackingService.AddHabitStep(step);
                LoadHabitStepsForSelectedCategory();
            }
        }

        private void OnHabitCategoryChanged()
        {
            LoadHabitStepsForSelectedCategory();
        }

        private void OnHabitStepDoubleClick()
        {
            var step = GetSelectedHabitStep();
            EnsureHabitUserId();
            if (step == null || _currentUserId == null)
                return;

            using var dlg = new HabitStepForm(_trackingService, _currentUserId.Value, step);
            var result = dlg.ShowDialog(this);
            if (result == DialogResult.Abort)
            {
                LoadHabitStepsForSelectedCategory();
                return;
            }
            if (result == DialogResult.OK)
            {
                var updated = dlg.Result;
                updated.HabitCategoryId = step.HabitCategoryId;
                updated.UserId = _currentUserId.Value;
                updated.HabitStepId = step.HabitStepId;
                
                if (_habitCardStates.ContainsKey(step.HabitStepId))
                    _habitCardStates.Remove(step.HabitStepId);

                _trackingService.UpdateHabitStep(updated);
                LoadHabitStepsForSelectedCategory();
            }
        }

        private HabitStep GetSelectedHabitStep()
        {
            if (_habitStepsList.SelectedItems.Count == 0)
                return null;

            return _habitStepsList.SelectedItems[0].Tag as HabitStep;
        }

        private void OnHabitStepSelectionChanged()
        {
        }

        private void RenderHabitCards(List<HabitStep> steps = null)
        {
            var src = steps ?? _habitSteps;

            _habitCardsFlow.Controls.Clear();
            if (src == null || !src.Any())
            {
                _habitCardsFlow.Controls.Add(new Label
                {
                    Text = "No steps yet",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Italic),
                    ForeColor = Color.Gray
                });
                return;
            }

            foreach (var step in src)
            {
                _habitCardsFlow.Controls.Add(CreateHabitCard(step));
            }
        }

        private Control CreateHabitCard(HabitStep step)
        {
            var today = _selectedHabitDate.Date;
            var comp = _habitCompletions.ContainsKey(step.HabitStepId)
                ? _habitCompletions[step.HabitStepId]
                : new HabitCompletionState();
            bool lockedToday = comp.Full || (!step.UseDaytimeSplit ? comp.Full : (comp.Day && comp.Night));
            var card = new Panel
            {
                Width = HabitCardsMaxWidth,
                Height = 120,
                BackColor = Color.FromArgb(245, 247, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Margin = new Padding(5)
            };

            var state = _habitCardStates.ContainsKey(step.HabitStepId)
                ? _habitCardStates[step.HabitStepId]
                : new DayNightState();
            _habitCardStates[step.HabitStepId] = state;

            var chkDone = new CheckBox
            {
                Location = new Point(8, 10),
                Size = new Size(16, 16),
                Checked = lockedToday || comp.Full || step.RemainingOccurrences.GetValueOrDefault(1) <= 0,
                Enabled = !lockedToday && step.RemainingOccurrences.GetValueOrDefault(1) > 0
            };
            chkDone.CheckedChanged += (_, _) =>
            {
                if (!chkDone.Checked)
                    return;
                chkDone.Enabled = false;
                var confirm = MessageBox.Show("Mark one occurrence as done?", "Habits", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes)
                {
                    _suppressDayNightHandlers = true;
                    chkDone.Checked = false;
                    _suppressDayNightHandlers = false;
                    chkDone.Enabled = !lockedToday && step.RemainingOccurrences.GetValueOrDefault(1) > 0;
                    return;
                }
                if (step.HabitStepId > 0 && _currentUserId.HasValue)
                {
                    _trackingService.MarkHabitCompletion(step.HabitStepId, _currentUserId.Value, today, null, step.UseDaytimeSplit);
                    LoadHabitStepsForSelectedCategory();
                }
            };
            card.Controls.Add(chkDone);

            var lblTitle = new Label
            {
                Text = step.Name,
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(30, 8)
            };
            card.Controls.Add(lblTitle);

            var when = new Label
            {
                Text = $"{FormatDate(step.StartDate)} → {FormatDate(step.EndDate)}", 
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(30, 30)
            };
            card.Controls.Add(when);

            int y = 50;
            if (!string.IsNullOrWhiteSpace(step.Description))
            {
                card.Controls.Add(new Label
                {
                    Text = Truncate(step.Description, 120),
                    AutoSize = true,
                    MaximumSize = new Size(HabitCardsMaxWidth - 20, 0),
                    Location = new Point(30, y),
                    Font = new Font("Segoe UI", 9),
                    ForeColor = Color.FromArgb(44, 62, 80)
                });
                y += 24;
            }

            if (step.UseDaytimeSplit)
            {
                int chkX = HabitCardsMaxWidth - 60;
                int iconX = chkX + 18;
                var chkDay = new CheckBox
                {
                    Location = new Point(chkX, 10),
                    Size = new Size(16, 16),
                    Checked = lockedToday ? true : (comp.Day || state.DayDone),
                    Enabled = !lockedToday && step.RemainingOccurrences.GetValueOrDefault(1) > 0
                };
                var lblSun = new Label
                {
                    Text = "☀",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(iconX, 8)
                };
                var chkNight = new CheckBox
                {
                    Location = new Point(chkX, 32),
                    Size = new Size(16, 16),
                    Checked = lockedToday ? true : (comp.Night || state.NightDone),
                    Enabled = !lockedToday && step.RemainingOccurrences.GetValueOrDefault(1) > 0
                };
                var lblMoon = new Label
                {
                    Text = "☾",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    Location = new Point(iconX, 30)
                };

                void tryComplete()
                {
                    if (state.DayDone && state.NightDone)
                    {
                        var confirm = MessageBox.Show("Mark the step as fully done?", "Habits", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirm != DialogResult.Yes)
                        {
                            _suppressDayNightHandlers = true;
                            state.DayDone = false;
                            state.NightDone = false;
                            chkDay.Checked = false;
                            chkNight.Checked = false;
                            _suppressDayNightHandlers = false;
                            return;
                        }
                        _suppressDayNightHandlers = true;
                        chkDone.Checked = true;
                        chkDone.Enabled = false;
                        chkDay.Enabled = false;
                        chkNight.Enabled = false;
                        _suppressDayNightHandlers = false;
                        if (step.HabitStepId > 0 && _currentUserId.HasValue)
                        {
                            _trackingService.MarkHabitCompletion(step.HabitStepId, _currentUserId.Value, today, null, step.UseDaytimeSplit);
                            LoadHabitStepsForSelectedCategory();
                        }
                    }
                }

                chkDay.CheckedChanged += (_, _) =>
                {
                    if (_suppressDayNightHandlers) return;
                    state.DayDone = chkDay.Checked;
                    if (step.HabitStepId > 0 && _currentUserId.HasValue && chkDay.Checked)
                    {
                        _trackingService.MarkHabitCompletion(step.HabitStepId, _currentUserId.Value, today, "Day", step.UseDaytimeSplit);
                        LoadHabitStepsForSelectedCategory();
                    }
                    else
                    {
                        tryComplete();
                    }
                };
                chkNight.CheckedChanged += (_, _) =>
                {
                    if (_suppressDayNightHandlers) return;
                    state.NightDone = chkNight.Checked;
                    if (step.HabitStepId > 0 && _currentUserId.HasValue && chkNight.Checked)
                    {
                        _trackingService.MarkHabitCompletion(step.HabitStepId, _currentUserId.Value, today, "Night", step.UseDaytimeSplit);
                        LoadHabitStepsForSelectedCategory();
                    }
                    else
                    {
                        tryComplete();
                    }
                };

                card.Controls.Add(lblSun);
                card.Controls.Add(chkDay);
                card.Controls.Add(lblMoon);
                card.Controls.Add(chkNight);
            }

            return card;
        }

        private static string Truncate(string text, int max)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Length <= max ? text : text.Substring(0, max) + "...";
        }

        private sealed class DayNightState
        {
            public bool DayDone { get; set; }
            public bool NightDone { get; set; }
        }

        private sealed class HabitCompletionState
        {
            public bool Full { get; set; }
            public bool Day { get; set; }
            public bool Night { get; set; }
        }

        private static string FormatDate(DateTime? date)
        {
            return date?.ToString("yyyy-MM-dd") ?? "-";
        }

        private void ApplyHabitFilter()
        {
            var filtered = _allHabitSteps
                .Where(step => OccursOnDate(step, _selectedHabitDate))
                .ToList();

            _habitSteps = _allHabitSteps;
            PopulateHabitList(_allHabitSteps);

            LoadCompletionsForDate(_selectedHabitDate);
            RenderHabitCards(filtered);
        }

        private void PopulateHabitList(IEnumerable<HabitStep> steps)
        {
            _habitStepsList.BeginUpdate();
            _habitStepsList.Items.Clear();

            if (steps != null)
            {
                foreach (var step in steps)
                {
                    var item = new ListViewItem(step.Name ?? string.Empty)
                    {
                        Tag = step
                    };
                    item.SubItems.Add(step.StartDate?.ToString("yyyy-MM-dd") ?? "-");
                    item.SubItems.Add(step.EndDate?.ToString("yyyy-MM-dd") ?? "-");
                    item.SubItems.Add(step.RepeatEveryDays?.ToString() ?? "-");
                    item.SubItems.Add(step.UseDaytimeSplit ? "Yes" : "No");
                    _habitStepsList.Items.Add(item);
                }
            }

            _habitStepsList.EndUpdate();
        }

        private void LoadCompletionsForDate(DateTime date)
        {
            if (_currentUserId == null)
                return;

            var list = _trackingService.GetHabitCompletions(date, _currentUserId.Value);
            _habitCompletions = list
                .GroupBy(x => x.HabitStepId)
                .ToDictionary(
                    g => g.Key,
                    g => new HabitCompletionState
                    {
                        Full = g.Any(x => string.IsNullOrEmpty(x.Part)),
                        Day = g.Any(x => string.Equals(x.Part, "Day", StringComparison.OrdinalIgnoreCase)),
                        Night = g.Any(x => string.Equals(x.Part, "Night", StringComparison.OrdinalIgnoreCase))
                    });
        }

        private static bool OccursOnDate(HabitStep step, DateTime day)
        {
            var d = day.Date;
            var start = (step.StartDate ?? d).Date;
            var end = (step.EndDate ?? start).Date;

            if (d < start || d > end)
                return false;

            if (step.RepeatEveryDays.HasValue && step.RepeatEveryDays.Value > 0)
            {
                var diff = (int)(d - start).TotalDays;
                return diff % step.RepeatEveryDays.Value == 0;
            }

            return true;
        }

        #endregion

        #region Food Tracking

        /// <summary>
        /// Ładuje kontenery jedzenia dla wybranego typu (Fridge/Freezer/Pantry).
        /// </summary>
        private void LoadContainers()
        {
            _containerTabs.TabPages.Clear();
            _selectedContainer = null;
            _fridgeListView.Items.Clear();

            var containers = _trackingService.GetContainers(_selectedContainerType);
            foreach (var c in containers)
            {
                var tab = new TabPage(c.Name) { Tag = c };
                _containerTabs.TabPages.Add(tab);
            }

            if (_containerTabs.TabPages.Count > 0)
            {
                _containerTabs.SelectedIndex = 0;
                OnContainerTabChanged();
            }
            else
            {
                _selectedContainer = null;
                _fridgeListView.Items.Clear();
            }

            UpdateDeleteContainerButtonState();
        }

        private void OnContainerTabChanged()
        {
            if (_containerTabs.SelectedTab == null)
            {
                _selectedContainer = null;
                return;
            }

            _selectedContainer = _containerTabs.SelectedTab.Tag as FoodContainer;
            LoadFridgeItemsForSelectedContainer();
            UpdateDeleteContainerButtonState();
        }

        private void LoadFridgeItemsForSelectedContainer()
        {
            _fridgeListView.Items.Clear();

            if (_selectedContainer == null)
                return;

            var items = _trackingService.GetFridgeItemsByContainer(_selectedContainer.ContainerID);

            foreach (var item in items)
            {
                var lvi = new ListViewItem(item.Name);
                lvi.SubItems.Add(item.Quantity.ToString("0.00") + " " + item.Unit);
                lvi.SubItems.Add(item.ExpirationDate?.ToString("yyyy-MM-dd") ?? "-");
                lvi.SubItems.Add(item.ExpireState);

                if (item.IsExpired())
                {
                    lvi.BackColor = Color.FromArgb(255, 200, 200);
                }
                else if (item.IsExpiringSoon())
                {
                    lvi.BackColor = Color.FromArgb(255, 250, 200);
                }

                lvi.Tag = item;
                _fridgeListView.Items.Add(lvi);
            }
        }

        private void OnAddContainer()
        {
            if (string.IsNullOrWhiteSpace(_selectedContainerType))
                return;

            string name = AddContainerForm.Show(
                $"New {_selectedContainerType} name:", 
                "Add container", 
                $"{_selectedContainerType} 1");

            if (string.IsNullOrWhiteSpace(name))
                return;

            var container = new FoodContainer { Name = name, Type = _selectedContainerType };
            _trackingService.AddContainer(container);
            LoadContainers();
        }

        private void OnDeleteSelectedItem()
        {
            if (_fridgeListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item to delete.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var lvi = _fridgeListView.SelectedItems[0];
            if (lvi.Tag is not FridgeItem item)
                return;

            if (MessageBox.Show($"Delete {item.Name}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _trackingService.DeleteFridgeItem(item.ItemID);
                LoadFridgeItemsForSelectedContainer();
            }
        }

        private void OnMoveSelectedItem()
        {
            if (_fridgeListView.SelectedItems.Count == 0 || _selectedContainer == null)
            {
                MessageBox.Show("Please select an item to move.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var lvi = _fridgeListView.SelectedItems[0];
            if (lvi.Tag is not FridgeItem item)
                return;

            var allContainers = new List<FoodContainer>();
            allContainers.AddRange(_trackingService.GetContainers("Fridge"));
            allContainers.AddRange(_trackingService.GetContainers("Freezer"));
            allContainers.AddRange(_trackingService.GetContainers("Pantry"));

            var containers = allContainers
                .Select(c => new MoveTrackingItemDialog.ContainerDisplayModel { Id = c.ContainerID, Name = c.Name })
                .ToList();

           
            using (var dlg = new MoveTrackingItemDialog(
                item.Name, 
                item.Quantity, 
                item.Unit, 
                containers, 
                _selectedContainer.ContainerID))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var target = dlg.TargetContainer;
                    var amount = dlg.AmountToMove;

                    if (target == null) return;

                    _trackingService.MoveFridgeItem(item.ItemID, target.Id, amount);
                    LoadFridgeItemsForSelectedContainer();
                }
            }
        }

        private void OnEditSelectedItem()
        {
            if (_fridgeListView.SelectedItems.Count == 0)
                return;

            var lvi = _fridgeListView.SelectedItems[0];
            if (lvi.Tag is not FridgeItem item)
                return;

            var freshItem = _trackingService.GetFridgeItemById(item.ItemID);
            if (freshItem == null)
                return;

            using (var dlg = new EditTrackingItemForm(
                freshItem.Name,
                freshItem.Quantity,
                freshItem.Unit,
                freshItem.ExpirationDate,
                (qty, unit, expire) =>
                {
                    freshItem.Quantity = qty;
                    freshItem.Unit = unit;
                    freshItem.ExpirationDate = expire;
                    freshItem.LastModified = DateTime.Now;

                    if (qty <= 0)
                        _trackingService.DeleteFridgeItem(freshItem.ItemID);
                    else
                        _trackingService.UpdateFridgeItem(freshItem);
                },
                EditTrackingItemForm.TrackingType.Food))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadFridgeItemsForSelectedContainer();
                }
            }
        }

        private void ShowAddFridgeItemDialog()
        {
            if (_selectedContainer == null)
            {
                MessageBox.Show("Select a container first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new AddTrackingItemForm(
                _selectedContainer.Name,
                () => _trackingService.GetAllProducts().Select(p => new AddTrackingItemForm.ProductDisplayModel { Id = p.ProductID, Name = p.Name, DefaultUnit = p.DefaultUnit }),
                (name, unit) => _trackingService.AddProduct(new FoodProduct { Name = name, DefaultUnit = unit }),
                (id) => _trackingService.DeleteProduct(id),
                (product, qty, unit, expire) =>
                {
                    var item = new FridgeItem
                    {
                        Name = product.Name,
                        ProductID = product.Id,
                        Quantity = qty,
                        Unit = unit,
                        ExpirationDate = expire,
              
                        ContainerID = _selectedContainer.ContainerID,
                        AddedDate = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    _trackingService.AddFridgeItem(item);
                },
                AddTrackingItemForm.TrackingType.Food
            ))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadFridgeItemsForSelectedContainer();
                }
            }
        }

        private void LoadRecipes()
        {
            _recipeListView.Items.Clear();

            var recipes = _trackingService.GetAllRecipes();
            foreach (var recipe in recipes)
            {
                var lvi = new ListViewItem(recipe.Name);
                lvi.SubItems.Add(recipe.PreparationTimeMinutes + " min");
                lvi.SubItems.Add(recipe.IsScheduled ? "Yes" : "No");
                if (recipe.IsScheduled)
                {
                    lvi.BackColor = Color.FromArgb(200, 255, 200); 
                }
                lvi.Tag = recipe;
                _recipeListView.Items.Add(lvi);
            }
        }

        private void OnRecipeDoubleClick()
        {
            if (_recipeListView.SelectedItems.Count == 0)
                return;

            var selectedItem = _recipeListView.SelectedItems[0];
            if (selectedItem.Tag is Recipe recipe)
            {
                var freshRecipe = _trackingService.GetRecipeById(recipe.RecipeID);
                if (freshRecipe != null)
                {
                    using (var dlg = new RecipeDetailsForm(_trackingService, freshRecipe))
                    {
                        if (dlg.ShowDialog(this) == DialogResult.OK)
                        {
                            LoadRecipes();
                        }
                    }
                }
            }
        }

        private void UpdateDeleteContainerButtonState()
        {
            bool hasContainer = _selectedContainer != null;
            _btnDeleteContainer.Visible = hasContainer;
            _btnDeleteContainer.Enabled = hasContainer;
        }

        private void ShowAddRecipeDialog()
        {
            using (var dlg = new AddRecipeForm(_trackingService))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadRecipes();
                }
            }
        }

        private void ShowShoppingList()
        {
            if (_eventService == null)
                _eventService = new EventService();
            if (_firstAidService == null)
                _firstAidService = new FirstAidService();

            var shoppingListService = new ShoppingListService(_trackingService, _firstAidService, _eventService);
            using (var form = new ShoppingListForm(shoppingListService))
            {
                form.ShowDialog(this);
            }
        }



        #endregion

        #region Plants Tracking

        /// <summary>
        /// Odświeża listę roślin i tworzy karty UI.
        /// </summary>
        private void RefreshPlants()
        {
            _plantsFlowPanel.Controls.Clear();

            var plants = _trackingService.GetAllPlants();
            if (!plants.Any())
            {
                _plantsFlowPanel.Controls.Add(new Label
                {
                    Text = "No plants tracked",
                    Font = new Font("Segoe UI", 12),
                    ForeColor = Color.Gray,
                    Size = new Size(300, 30)
                });
                return;
            }

            foreach (var plant in plants)
            {
                _plantsFlowPanel.Controls.Add(CreatePlantCard(plant));
            }
        }

        private Panel CreatePlantCard(Plant plant)
        {
            var needsWater = plant.NeedsWatering();

            var card = new Panel
            {
                Size = new Size(200, 220),
                BackColor = needsWater
                    ? Color.FromArgb(255, 235, 235)  
                    : Color.FromArgb(232, 245, 233), 
                Padding = new Padding(12),
                Margin = new Padding(6),
                Tag = plant.PlantID
            };

            card.Paint += (s, e) =>
            {
                var borderColor = needsWater
                    ? Color.FromArgb(231, 76, 60)   
                    : Color.FromArgb(46, 204, 113); 
                using var pen = new Pen(borderColor, 2);
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            var lblIcon = new Label
            {
                Text = "🌿",
                Location = new Point(135, 8),
                Size = new Size(50, 45),
                Font = new Font("Segoe UI", 28),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblIcon);

            var lblName = new Label
            {
                Text = plant.Name,
                Location = new Point(12, 12),
                Size = new Size(130, 24),
                Font = new Font("Segoe UI Semibold", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblName);

            var lblSpecies = new Label
            {
                Text = "(" + (plant.Species ?? "Plant") + ")",
                Location = new Point(12, 36),
                Size = new Size(130, 18),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(127, 140, 141),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblSpecies);

            var lblWatering = new Label
            {
                Text = needsWater
                    ? "⚠ Needs water!"
                    : $"Next: {(plant.NextWateringDate.HasValue ? plant.NextWateringDate.Value.ToString("MMM dd") : "-")}",
                Location = new Point(12, 60),
                Size = new Size(175, 20),
                Font = new Font("Segoe UI", 9),
                ForeColor = needsWater ? Color.FromArgb(231, 76, 60) : Color.FromArgb(52, 73, 94),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblWatering);

            var lblFrequency = new Label
            {
                Text = $"Every {plant.WateringFrequency} days",
                Location = new Point(12, 80),
                Size = new Size(175, 16),
                Font = new Font("Segoe UI", 8, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblFrequency);

            var btnWater = new Button
            {
                Text = "💧 Water",
                Location = new Point(12, 105),
                Size = new Size(176, 32),
                BackColor = Color.FromArgb(235, 245, 255),
                ForeColor = Color.FromArgb(52, 152, 219),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnWater.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            btnWater.FlatAppearance.BorderSize = 2;
            btnWater.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            btnWater.Click += (s, e) =>
            {
                _trackingService.WaterPlant(plant.PlantID);
                MessageBox.Show($"{plant.Name} watered! 💧", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshPlants();
            };
            card.Controls.Add(btnWater);

            // Dzieci nie mogą edytować roślin
            if (!UserSession.IsKid)
            {
                var btnEdit = new Button
                {
                    Text = "✏ Edit",
                    Location = new Point(12, 142),
                    Size = new Size(176, 32),
                    BackColor = Color.FromArgb(255, 249, 230),
                    ForeColor = Color.FromArgb(180, 150, 10),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9),
                    Cursor = Cursors.Hand
                };
                btnEdit.FlatAppearance.BorderColor = Color.FromArgb(241, 196, 15);
                btnEdit.FlatAppearance.BorderSize = 2;
                btnEdit.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 243, 200);
                btnEdit.Click += (_, __) => ShowEditPlantDialog(plant);
                card.Controls.Add(btnEdit);

                card.DoubleClick += (_, __) => ShowEditPlantDialog(plant);
            }

            return card;
        }

        private void FocusPlantCard(int plantId)
        {
            foreach (Control ctrl in _plantsFlowPanel.Controls)
            {
                if (ctrl.Tag is int id && id == plantId)
                {
                    _plantsFlowPanel.ScrollControlIntoView(ctrl);
                    ctrl.Focus();
                    break;
                }
            }
        }

        private void ShowAddPlantDialog()
        {
            using (var dlg = new PlantEditorForm(_trackingService))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshPlants();
                }
            }
        }

        private void ShowEditPlantDialog(Plant plant)
        {
            if (plant == null)
                return;

            var fresh = _trackingService.GetPlantById(plant.PlantID);
            if (fresh == null)
                return;

            using (var dlg = new PlantEditorForm(_trackingService, fresh))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshPlants();
                }
            }
        }

        #region FirstAid Methods

        private void LoadMedContainers()
        {
            _medContainerTabs.TabPages.Clear();
            _selectedMedContainer = null;
            _medicineListView.Items.Clear();

            var containers = _firstAidService.GetContainers();
            foreach (var c in containers)
            {
                var tab = new TabPage(c.Name) { Tag = c };
                _medContainerTabs.TabPages.Add(tab);
            }

            if (_medContainerTabs.TabPages.Count > 0)
            {
                _medContainerTabs.SelectedIndex = 0;
                _selectedMedContainer = _medContainerTabs.TabPages[0].Tag as MedicineContainer;
                LoadMedicineItemsForSelectedContainer();
            }
            else
            {
                _selectedMedContainer = null;
                _medicineListView.Items.Clear();
            }

            UpdateMedDeleteContainerButtonState();
        }

        private void InitializeMedCalendar()
        {
            if (_intakeCalendar != null)
            {
                _intakeCalendar.SetDate(DateTime.Today);
            }
            RenderIntakeForDate(DateTime.Today);
        }

        private void OnMedContainerTabChanged()
        {
            if (_medContainerTabs.SelectedTab == null)
            {
                _selectedMedContainer = null;
                return;
            }

            _selectedMedContainer = _medContainerTabs.SelectedTab.Tag as MedicineContainer;
            LoadMedicineItemsForSelectedContainer();
            UpdateMedDeleteContainerButtonState();
        }

        private void LoadMedicineItemsForSelectedContainer()
        {
            _medicineListView.Items.Clear();

            if (_selectedMedContainer == null)
                return;

            var items = _firstAidService.GetMedicineItemsByContainer(_selectedMedContainer.ContainerID);

            foreach (var item in items)
            {
                var lvi = new ListViewItem(item.Name);
                lvi.SubItems.Add(item.Quantity.ToString("0.00") + " " + item.Unit);
                lvi.SubItems.Add(item.ExpirationDate?.ToString("yyyy-MM-dd") ?? "-");
                lvi.SubItems.Add(item.ExpireState);

                if (item.IsExpired())
                {
                    lvi.BackColor = Color.FromArgb(255, 200, 200);
                }
                else if (item.IsExpiringSoon())
                {
                    lvi.BackColor = Color.FromArgb(255, 250, 200);
                }

                lvi.Tag = item;
                _medicineListView.Items.Add(lvi);
            }
        }

        private void OnAddMedContainer()
        {
            string name = AddContainerForm.Show(
                "New container name:", 
                "Add container", 
                "Container 1");

            if (string.IsNullOrWhiteSpace(name))
                return;

            var container = new MedicineContainer { Name = name };
            _firstAidService.AddContainer(container);
            LoadMedContainers();
        }

        private void OnDeleteMedContainer()
        {
            if (_selectedMedContainer == null)
            {
                MessageBox.Show(
                    "Please select a container to delete.",
                    "No container",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the container \"{_selectedMedContainer.Name}\" " +
                "TOGETHER with all items inside?",
                "Delete container",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                _firstAidService.DeleteContainerWithItems(_selectedMedContainer.ContainerID);
                _selectedMedContainer = null;
                LoadMedContainers();
                UpdateMedDeleteContainerButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while deleting the container:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnDeleteMedicineItem()
        {
            if (_medicineListView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item to delete.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var lvi = _medicineListView.SelectedItems[0];
            if (lvi.Tag is not MedicineItem item)
                return;

            if (MessageBox.Show($"Delete {item.Name}?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _firstAidService.DeleteMedicineItem(item.ItemID);
                LoadMedicineItemsForSelectedContainer();
            }
        }

        private void OnMoveMedicineItem()
        {
            if (_medicineListView.SelectedItems.Count == 0 || _selectedMedContainer == null)
            {
                MessageBox.Show("Please select an item to move.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var lvi = _medicineListView.SelectedItems[0];
            if (lvi.Tag is not MedicineItem item)
                return;

            var containers = _firstAidService.GetContainers()
                .Select(c => new MoveTrackingItemDialog.ContainerDisplayModel { Id = c.ContainerID, Name = c.Name });

            using (var dlg = new MoveTrackingItemDialog(
                item.Name,
                (decimal)item.Quantity,
                item.Unit,
                containers,
                _selectedMedContainer.ContainerID))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var target = dlg.TargetContainer;
                    var amount = dlg.AmountToMove;

                    if (target == null) return;

                    _firstAidService.MoveMedicineItem(item.ItemID, target.Id, amount);
                    LoadMedicineItemsForSelectedContainer();
                }
            }
        }

        private void OnEditMedicineItem()
        {
            if (_medicineListView.SelectedItems.Count == 0)
                return;

            var lvi = _medicineListView.SelectedItems[0];
            if (lvi.Tag is not MedicineItem item)
                return;

            var freshItem = _firstAidService.GetMedicineItemById(item.ItemID);
            if (freshItem == null)
                return;

            using (var dlg = new EditTrackingItemForm(
                freshItem.Name,
                freshItem.Quantity,
                freshItem.Unit,
                freshItem.ExpirationDate,
                (qty, unit, expire) =>
                {
                    freshItem.Quantity = qty;
                    freshItem.Unit = unit;
                    freshItem.ExpirationDate = expire;
                    freshItem.LastModified = DateTime.Now;

                    if (qty <= 0)
                        _firstAidService.DeleteMedicineItem(freshItem.ItemID);
                    else
                        _firstAidService.UpdateMedicineItem(freshItem);
                }))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadMedicineItemsForSelectedContainer();
                }
            }
        }

        private void ShowAddMedicineDialog()
        {
            if (_selectedMedContainer == null)
            {
                MessageBox.Show("Select a container first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dlg = new AddTrackingItemForm(
                _selectedMedContainer.Name,
                () => _firstAidService.GetAllProducts().Select(p => new AddTrackingItemForm.ProductDisplayModel { Id = p.ProductID, Name = p.Name, DefaultUnit = p.DefaultUnit }),
                (name, unit) => _firstAidService.AddProduct(new MedicineProduct { Name = name, DefaultUnit = unit }),
                (id) => _firstAidService.DeleteProduct(id),
                (product, qty, unit, expire) =>
                {
                    var item = new MedicineItem
                    {
                        Name = product.Name,
                        ProductID = product.Id,
                        Quantity = qty,
                        Unit = unit,
                        ExpirationDate = expire,
                  
                        ContainerID = _selectedMedContainer.ContainerID,
                        AddedDate = DateTime.Now,
                        LastModified = DateTime.Now
                    };
                    _firstAidService.AddMedicineItem(item);
                }))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadMedicineItemsForSelectedContainer();
                }
            }
        }

        private void ShowMedShoppingList()
        {
            var shoppingListService = new ShoppingListService(_trackingService, _firstAidService, _eventService);
            using (var form = new ShoppingListForm(shoppingListService))
            {
                form.ShowDialog(this);
            }
        }

        private void OnMedCalendarDateChanged(DateTime date)
        {
            _selectedMedDate = date.Date;
            RenderIntakeForDate(_selectedMedDate);
        }

        private void RenderIntakeForDate(DateTime date)
        {
            if (_intakeItemsPanel == null)
                return;

            _intakeItemsPanel.Controls.Clear();

            var schedules = _firstAidService.GetActiveSchedules();
            bool hasItems = false;

            foreach (var schedule in schedules)
            {
                if (!IsScheduleDueOnDate(schedule, date))
                    continue;

                hasItems = true;
                var medicine = _firstAidService.GetMedicineItemById(schedule.MedicineItemID);
                var status = _firstAidService.GetIntakeStatus(schedule.ScheduleID, date);
                _intakeItemsPanel.Controls.Add(CreateIntakeCard(schedule, status, medicine));
            }

            if (!hasItems)
            {
                _intakeItemsPanel.Controls.Add(new Label
                {
                    Text = "No scheduled doses for selected day",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9, FontStyle.Italic),
                    ForeColor = Color.Gray,
                    Padding = new Padding(5)
                });
            }
        }

        private bool IsScheduleDueOnDate(MedicineSchedule schedule, DateTime date)
        {
            if (schedule == null || schedule.IntervalDays <= 0)
                return false;

            var day = date.Date;

            if (day < schedule.StartDate.Date)
                return false;

            if (schedule.EndDate.HasValue && day > schedule.EndDate.Value.Date)
                return false;

            var diff = (int)(day - schedule.StartDate.Date).TotalDays;
            return diff % schedule.IntervalDays == 0;
        }

        private Panel CreateIntakeCard(MedicineSchedule schedule, MedicineIntakeStatus status, MedicineItem medicine)
        {
            var card = new Panel
            {
                Width = 340,
                Height = 100,
                BackColor = Color.FromArgb(245, 247, 250),
                Margin = new Padding(0, 5, 0, 5),
                Padding = new Padding(8),
                BorderStyle = BorderStyle.FixedSingle,
                Tag = schedule
            };
            card.Cursor = Cursors.Default;

            var lblName = new Label
            {
                Text = schedule.MedicineName,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(8, 8),
                Size = new Size(300, 18)
            };
            card.Controls.Add(lblName);
            lblName.Cursor = Cursors.Hand;
            lblName.Click += (_, __) => EditSchedule(schedule);

            var infoText = $"every {schedule.IntervalDays} days from {schedule.StartDate:yyyy-MM-dd}";
            if (schedule.EndDate.HasValue)
            {
                infoText += $" to {schedule.EndDate.Value:yyyy-MM-dd}";
            }

            var lblInfo = new Label
            {
                Text = infoText,
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = Color.Gray,
                Location = new Point(8, 30),
                Size = new Size(320, 16)
            };
            card.Controls.Add(lblInfo);
            lblInfo.Cursor = Cursors.Hand;
            lblInfo.Click += (_, __) => EditSchedule(schedule);

            int y = 52;
            decimal availableQty = medicine?.Quantity ?? 0;

            if (schedule.MorningDose.HasValue)
            {
                bool canTakeMorning = availableQty >= schedule.MorningDose.Value && schedule.MorningDose.Value > 0;
                var chkMorning = new CheckBox
                {
                    Text = $"Morning: {schedule.MorningDose} doses",
                    Location = new Point(8, y),
                    AutoSize = true,
                    Checked = status?.MorningTaken ?? false,
                    Enabled = canTakeMorning
                };
                chkMorning.CheckedChanged += (_, __) =>
                {
                    var ok = _firstAidService.SetIntakeStatus(schedule.ScheduleID, _selectedMedDate, chkMorning.Checked, null);
                    if (!ok)
                    {
                        MessageBox.Show("Insufficient medicine quantity.", "No medicine", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    RenderIntakeForDate(_selectedMedDate);
                    LoadMedicineItemsForSelectedContainer();
                };
                card.Controls.Add(chkMorning);
                y += 24;
            }

            if (schedule.EveningDose.HasValue)
            {
                bool canTakeEvening = availableQty >= schedule.EveningDose.Value && schedule.EveningDose.Value > 0;
                var chkEvening = new CheckBox
                {
                    Text = $"Evening: {schedule.EveningDose} doses",
                    Location = new Point(8, y),
                    AutoSize = true,
                    Checked = status?.EveningTaken ?? false,
                    Enabled = canTakeEvening
                };
                chkEvening.CheckedChanged += (_, __) =>
                {
                    var ok = _firstAidService.SetIntakeStatus(schedule.ScheduleID, _selectedMedDate, null, chkEvening.Checked);
                    if (!ok)
                    {
                        MessageBox.Show("Insufficient medicine quantity.", "No medicine", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    RenderIntakeForDate(_selectedMedDate);
                    LoadMedicineItemsForSelectedContainer();
                };
                card.Controls.Add(chkEvening);
            }

            return card;
        }

        private void EditSchedule(MedicineSchedule schedule)
        {
            if (schedule == null)
                return;

            var fresh = _firstAidService.GetScheduleById(schedule.ScheduleID);
            if (fresh == null)
                return;

            using var dlg = new AddMedicineScheduleForm(_firstAidService, fresh);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                RenderIntakeForDate(_selectedMedDate);
                LoadMedicineItemsForSelectedContainer();
            }
        }

        private void ShowAddScheduleDialog()
        {
            using var dlg = new AddMedicineScheduleForm(_firstAidService);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                RenderIntakeForDate(_selectedMedDate);
            }
        }

        private void UpdateMedDeleteContainerButtonState()
        {
            if (_btnDeleteMedContainer == null)
                return;

            bool hasContainer = _selectedMedContainer != null;
            _btnDeleteMedContainer.Visible = hasContainer;
            _btnDeleteMedContainer.Enabled = hasContainer;
        }

        #endregion

        private void _btnAddMedContainer_Click(object sender, EventArgs e)
        {
        }

        private void _btnDeleteMedContainer_Click(object sender, EventArgs e)
        {
            OnDeleteMedContainer();
        }

        #endregion
    }
}
