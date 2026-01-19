using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class TrackingView
    {
        private TabControl _tabControl;
        private TabPage _fridgeTab;
        private TabPage _habitsTab;
        private TabPage _medTab;
        private TabPage _plantsTab;

        private Panel _fridgeMainPanel;
        private Panel _fridgeContentPanel;
        private Panel _fridgeLeftPanel;
        private Panel _fridgeLeftContentPanel;
        private Panel _fridgeActionsPanel;
        private FlowLayoutPanel _fridgeActionsFlowPanel;
        private Button _btnAddFood;
        private Button _btnMoveFood;
        private Button _btnDeleteFood;
        private Label _lblFridgeHeading;
        private TabControl _containerTabs;
        private ListView _fridgeListView;

        private Panel _fridgeMiddlePanel;
        private Button _btnAddContainer;
        private Panel _typeButtonsPanel;
        private Panel _typeButtonsInnerPanel;
        private Button _btnFridge;
        private Button _btnFreezer;
        private Button _btnPantry;
        private Button _btnDeleteContainer;
        private Button _btnAddRecipe;

        private Panel _fridgeRightPanel;
        private Label _lblRecipesHeading;
        private ListView _recipeListView;
        private Button _btnShoppingList;

        private Panel _medPanel;
        private Panel _medContentPanel;
        private Panel _medLeftPanel;
        private Panel _medLeftContentPanel;
        private Panel _medActionsPanel;
        private FlowLayoutPanel _medActionsFlow;
        private Label _lblMedHeading;
        private ListView _medicineListView;
        private TabControl _medContainerTabs;
        private Panel _medMiddlePanel;
        private Panel _medMiddleFiller;
        private Button _btnAddMedContainer;
        private Button _btnDeleteMedContainer;
        private Button _btnMedShoppingList;
        private Button _btnAddMedicine;
        private Button _btnMoveMedicine;
        private Button _btnDeleteMedicine;
        private Panel _medRightPanel;
        private Label _lblIntakeHeader;
        private MonthCalendar _intakeCalendar;
        private FlowLayoutPanel _intakeItemsPanel;
        private Button _btnAddSchedule;
        
        private Panel _medToolbar;
        private Button _btnAddMedication;
        private ListView _medicationsListView;

        private Panel _plantsPanel;
        private Panel _plantsToolbar;
        private Button _btnAddPlant;
        private FlowLayoutPanel _plantsFlowPanel;
        private Panel _habitsPanel;
        private Panel _habitToolbar;
        private Button _btnAddHabitCategory;
        private Button _btnAddHabitStep;
        private TabControl _habitCategoryTabs;
        private Panel _habitContentPanel;
        private SplitContainer _habitSplitContainer;
        private ListView _habitStepsList;
        private Panel _habitRightPanel;
        private MonthCalendar _habitCalendar;
        private FlowLayoutPanel _habitCardsFlow;

        private void InitializeComponent()
        {
            _tabControl = new TabControl();
            _fridgeTab = new TabPage();
            _fridgeMainPanel = new Panel();
            _fridgeContentPanel = new Panel();
            _fridgeRightPanel = new Panel();
            _recipeListView = new ListView();
            _btnShoppingList = new Button();
            _lblRecipesHeading = new Label();
            _fridgeMiddlePanel = new Panel();
            _typeButtonsPanel = new Panel();
            _typeButtonsInnerPanel = new Panel();
            _btnPantry = new Button();
            _btnFreezer = new Button();
            _btnFridge = new Button();
            _btnDeleteContainer = new Button();
            _btnAddRecipe = new Button();
            _btnAddContainer = new Button();
            _fridgeLeftPanel = new Panel();
            _fridgeLeftContentPanel = new Panel();
            _fridgeListView = new ListView();
            _containerTabs = new TabControl();
            _fridgeActionsPanel = new Panel();
            _fridgeActionsFlowPanel = new FlowLayoutPanel();
            _btnAddFood = new Button();
            _btnMoveFood = new Button();
            _btnDeleteFood = new Button();
            _lblFridgeHeading = new Label();
            _habitsTab = new TabPage();
            _habitsPanel = new Panel();
            _habitContentPanel = new Panel();
            _habitSplitContainer = new SplitContainer();
            _habitStepsList = new ListView();
            _habitRightPanel = new Panel();
            _habitCardsFlow = new FlowLayoutPanel();
            _habitCalendar = new MonthCalendar();
            _habitCategoryTabs = new TabControl();
            _habitToolbar = new Panel();
            _btnAddHabitStep = new Button();
            _btnAddHabitCategory = new Button();
            _medTab = new TabPage();
            _medPanel = new Panel();
            _medContentPanel = new Panel();
            _medRightPanel = new Panel();
            _intakeItemsPanel = new FlowLayoutPanel();
            _btnAddSchedule = new Button();
            _intakeCalendar = new MonthCalendar();
            _lblIntakeHeader = new Label();
            _medMiddlePanel = new Panel();
            _medMiddleFiller = new Panel();
            _btnAddMedContainer = new Button();
            _btnDeleteMedContainer = new Button();
            _medLeftPanel = new Panel();
            _medLeftContentPanel = new Panel();
            _medicineListView = new ListView();
            _medContainerTabs = new TabControl();
            _medActionsPanel = new Panel();
            _medActionsFlow = new FlowLayoutPanel();
            _btnAddMedicine = new Button();
            _btnMoveMedicine = new Button();
            _btnDeleteMedicine = new Button();
            _lblMedHeading = new Label();
            _plantsTab = new TabPage();
            _plantsPanel = new Panel();
            _plantsFlowPanel = new FlowLayoutPanel();
            _plantsToolbar = new Panel();
            _btnAddPlant = new Button();
            _btnMedShoppingList = new Button();
            _medToolbar = new Panel();
            _medicationsListView = new ListView();
            _btnAddMedication = new Button();
            _tabControl.SuspendLayout();
            _fridgeTab.SuspendLayout();
            _fridgeMainPanel.SuspendLayout();
            _fridgeContentPanel.SuspendLayout();
            _fridgeRightPanel.SuspendLayout();
            _fridgeMiddlePanel.SuspendLayout();
            _typeButtonsPanel.SuspendLayout();
            _typeButtonsInnerPanel.SuspendLayout();
            _fridgeLeftPanel.SuspendLayout();
            _fridgeLeftContentPanel.SuspendLayout();
            _fridgeActionsPanel.SuspendLayout();
            _fridgeActionsFlowPanel.SuspendLayout();
            _habitsTab.SuspendLayout();
            _habitsPanel.SuspendLayout();
            _habitContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_habitSplitContainer).BeginInit();
            _habitSplitContainer.Panel1.SuspendLayout();
            _habitSplitContainer.Panel2.SuspendLayout();
            _habitSplitContainer.SuspendLayout();
            _habitRightPanel.SuspendLayout();
            _habitToolbar.SuspendLayout();
            _medTab.SuspendLayout();
            _medPanel.SuspendLayout();
            _medContentPanel.SuspendLayout();
            _medRightPanel.SuspendLayout();
            _medMiddlePanel.SuspendLayout();
            _medLeftPanel.SuspendLayout();
            _medLeftContentPanel.SuspendLayout();
            _medActionsPanel.SuspendLayout();
            _medActionsFlow.SuspendLayout();
            _plantsTab.SuspendLayout();
            _plantsPanel.SuspendLayout();
            _plantsToolbar.SuspendLayout();
            SuspendLayout();
            // 
            // _tabControl
            // 
            _tabControl.Controls.Add(_fridgeTab);
            _tabControl.Controls.Add(_habitsTab);
            _tabControl.Controls.Add(_medTab);
            _tabControl.Controls.Add(_plantsTab);
            _tabControl.Dock = DockStyle.Fill;
            _tabControl.Font = new Font("Segoe UI", 10F);
            _tabControl.Location = new Point(0, 0);
            _tabControl.Margin = new Padding(0);
            _tabControl.Name = "_tabControl";
            _tabControl.Padding = new Point(0, 0);
            _tabControl.SelectedIndex = 0;
            _tabControl.Size = new Size(1080, 720);
            _tabControl.TabIndex = 0;
            // 
            // _fridgeTab
            // 
            _fridgeTab.Controls.Add(_fridgeMainPanel);
            _fridgeTab.Location = new Point(4, 26);
            _fridgeTab.Margin = new Padding(0);
            _fridgeTab.Name = "_fridgeTab";
            _fridgeTab.Size = new Size(1072, 690);
            _fridgeTab.TabIndex = 0;
            _fridgeTab.Text = "🍎 Fridge & Pantry";
            _fridgeTab.UseVisualStyleBackColor = true;
            // 
            // _fridgeMainPanel
            // 
            _fridgeMainPanel.BackColor = Color.White;
            _fridgeMainPanel.Controls.Add(_fridgeContentPanel);
            _fridgeMainPanel.Dock = DockStyle.Fill;
            _fridgeMainPanel.Location = new Point(0, 0);
            _fridgeMainPanel.Name = "_fridgeMainPanel";
            _fridgeMainPanel.Padding = new Padding(10);
            _fridgeMainPanel.Size = new Size(1072, 690);
            _fridgeMainPanel.TabIndex = 0;
            // 
            // _fridgeContentPanel
            // 
            _fridgeContentPanel.BackColor = Color.White;
            _fridgeContentPanel.Controls.Add(_fridgeRightPanel);
            _fridgeContentPanel.Controls.Add(_fridgeMiddlePanel);
            _fridgeContentPanel.Controls.Add(_fridgeLeftPanel);
            _fridgeContentPanel.Dock = DockStyle.Fill;
            _fridgeContentPanel.Location = new Point(10, 10);
            _fridgeContentPanel.Name = "_fridgeContentPanel";
            _fridgeContentPanel.Padding = new Padding(10, 20, 10, 10);
            _fridgeContentPanel.Size = new Size(1052, 670);
            _fridgeContentPanel.TabIndex = 0;
            // 
            // _fridgeRightPanel
            // 
            _fridgeRightPanel.BackColor = Color.White;
            _fridgeRightPanel.Controls.Add(_recipeListView);
            _fridgeRightPanel.Controls.Add(_btnShoppingList);
            _fridgeRightPanel.Controls.Add(_lblRecipesHeading);
            _fridgeRightPanel.Dock = DockStyle.Fill;
            _fridgeRightPanel.Location = new Point(800, 20);
            _fridgeRightPanel.MinimumSize = new Size(200, 0);
            _fridgeRightPanel.Name = "_fridgeRightPanel";
            _fridgeRightPanel.Padding = new Padding(10);
            _fridgeRightPanel.Size = new Size(242, 640);
            _fridgeRightPanel.TabIndex = 0;
            // 
            // _recipeListView
            // 
            _recipeListView.BorderStyle = BorderStyle.FixedSingle;
            _recipeListView.Dock = DockStyle.Fill;
            _recipeListView.Font = new Font("Segoe UI", 9F);
            _recipeListView.FullRowSelect = true;
            _recipeListView.GridLines = true;
            _recipeListView.Location = new Point(10, 40);
            _recipeListView.Margin = new Padding(0);
            _recipeListView.MultiSelect = false;
            _recipeListView.Name = "_recipeListView";
            _recipeListView.Size = new Size(222, 554);
            _recipeListView.TabIndex = 0;
            _recipeListView.UseCompatibleStateImageBehavior = false;
            _recipeListView.View = View.Details;
            // 
            // _btnShoppingList
            // 
            _btnShoppingList.BackColor = Color.FromArgb(235, 245, 255);
            _btnShoppingList.Cursor = Cursors.Hand;
            _btnShoppingList.Dock = DockStyle.Bottom;
            _btnShoppingList.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnShoppingList.FlatAppearance.BorderSize = 2;
            _btnShoppingList.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnShoppingList.FlatStyle = FlatStyle.Flat;
            _btnShoppingList.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnShoppingList.ForeColor = Color.FromArgb(52, 152, 219);
            _btnShoppingList.Location = new Point(10, 594);
            _btnShoppingList.Margin = new Padding(0, 10, 0, 0);
            _btnShoppingList.Name = "_btnShoppingList";
            _btnShoppingList.Size = new Size(222, 36);
            _btnShoppingList.TabIndex = 1;
            _btnShoppingList.Text = "Shopping list";
            _btnShoppingList.UseVisualStyleBackColor = false;
            // 
            // _lblRecipesHeading
            // 
            _lblRecipesHeading.Dock = DockStyle.Top;
            _lblRecipesHeading.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _lblRecipesHeading.ForeColor = Color.FromArgb(44, 62, 80);
            _lblRecipesHeading.Location = new Point(10, 10);
            _lblRecipesHeading.Name = "_lblRecipesHeading";
            _lblRecipesHeading.Padding = new Padding(0, 5, 0, 0);
            _lblRecipesHeading.Size = new Size(222, 30);
            _lblRecipesHeading.TabIndex = 2;
            _lblRecipesHeading.Text = "Recepies";
            // 
            // _fridgeMiddlePanel
            // 
            _fridgeMiddlePanel.BackColor = Color.White;
            _fridgeMiddlePanel.Controls.Add(_typeButtonsPanel);
            _fridgeMiddlePanel.Controls.Add(_btnDeleteContainer);
            _fridgeMiddlePanel.Controls.Add(_btnAddRecipe);
            _fridgeMiddlePanel.Controls.Add(_btnAddContainer);
            _fridgeMiddlePanel.Dock = DockStyle.Left;
            _fridgeMiddlePanel.Location = new Point(570, 20);
            _fridgeMiddlePanel.Name = "_fridgeMiddlePanel";
            _fridgeMiddlePanel.Padding = new Padding(0, 10, 0, 10);
            _fridgeMiddlePanel.Size = new Size(230, 640);
            _fridgeMiddlePanel.TabIndex = 1;
            // 
            // _typeButtonsPanel
            // 
            _typeButtonsPanel.BackColor = Color.FromArgb(235, 245, 255);
            _typeButtonsPanel.Controls.Add(_typeButtonsInnerPanel);
            _typeButtonsPanel.Dock = DockStyle.Fill;
            _typeButtonsPanel.Location = new Point(0, 46);
            _typeButtonsPanel.Name = "_typeButtonsPanel";
            _typeButtonsPanel.Padding = new Padding(15, 20, 15, 15);
            _typeButtonsPanel.Size = new Size(230, 514);
            _typeButtonsPanel.TabIndex = 0;
            // 
            // _typeButtonsInnerPanel
            // 
            _typeButtonsInnerPanel.Anchor = AnchorStyles.None;
            _typeButtonsInnerPanel.Controls.Add(_btnPantry);
            _typeButtonsInnerPanel.Controls.Add(_btnFreezer);
            _typeButtonsInnerPanel.Controls.Add(_btnFridge);
            _typeButtonsInnerPanel.Location = new Point(15, 130);
            _typeButtonsInnerPanel.Name = "_typeButtonsInnerPanel";
            _typeButtonsInnerPanel.Size = new Size(200, 204);
            _typeButtonsInnerPanel.TabIndex = 0;
            // 
            // _btnPantry
            // 
            _btnPantry.BackColor = Color.White;
            _btnPantry.Cursor = Cursors.Hand;
            _btnPantry.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
            _btnPantry.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            _btnPantry.FlatStyle = FlatStyle.Flat;
            _btnPantry.Font = new Font("Segoe UI", 9F);
            _btnPantry.ForeColor = Color.FromArgb(80, 80, 80);
            _btnPantry.Location = new Point(0, 168);
            _btnPantry.Margin = new Padding(0, 4, 0, 0);
            _btnPantry.Name = "_btnPantry";
            _btnPantry.Size = new Size(200, 36);
            _btnPantry.TabIndex = 0;
            _btnPantry.Text = "Pantry";
            _btnPantry.UseVisualStyleBackColor = false;
            // 
            // _btnFreezer
            // 
            _btnFreezer.BackColor = Color.White;
            _btnFreezer.Cursor = Cursors.Hand;
            _btnFreezer.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
            _btnFreezer.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            _btnFreezer.FlatStyle = FlatStyle.Flat;
            _btnFreezer.Font = new Font("Segoe UI", 9F);
            _btnFreezer.ForeColor = Color.FromArgb(80, 80, 80);
            _btnFreezer.Location = new Point(0, 83);
            _btnFreezer.Margin = new Padding(0, 4, 0, 0);
            _btnFreezer.Name = "_btnFreezer";
            _btnFreezer.Size = new Size(200, 36);
            _btnFreezer.TabIndex = 1;
            _btnFreezer.Text = "Freezer";
            _btnFreezer.UseVisualStyleBackColor = false;
            // 
            // _btnFridge
            // 
            _btnFridge.BackColor = Color.White;
            _btnFridge.Cursor = Cursors.Hand;
            _btnFridge.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
            _btnFridge.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            _btnFridge.FlatStyle = FlatStyle.Flat;
            _btnFridge.Font = new Font("Segoe UI", 9F);
            _btnFridge.ForeColor = Color.FromArgb(80, 80, 80);
            _btnFridge.Location = new Point(0, 0);
            _btnFridge.Margin = new Padding(0, 0, 0, 4);
            _btnFridge.Name = "_btnFridge";
            _btnFridge.Size = new Size(200, 36);
            _btnFridge.TabIndex = 2;
            _btnFridge.Text = "Fridge";
            _btnFridge.UseVisualStyleBackColor = false;
            // 
            // _btnDeleteContainer
            // 
            _btnDeleteContainer.BackColor = Color.FromArgb(255, 240, 240);
            _btnDeleteContainer.Cursor = Cursors.Hand;
            _btnDeleteContainer.Dock = DockStyle.Bottom;
            _btnDeleteContainer.Enabled = false;
            _btnDeleteContainer.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDeleteContainer.FlatAppearance.BorderSize = 2;
            _btnDeleteContainer.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 220, 220);
            _btnDeleteContainer.FlatStyle = FlatStyle.Flat;
            _btnDeleteContainer.Font = new Font("Segoe UI", 9F);
            _btnDeleteContainer.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDeleteContainer.Location = new Point(0, 560);
            _btnDeleteContainer.Name = "_btnDeleteContainer";
            _btnDeleteContainer.Size = new Size(230, 32);
            _btnDeleteContainer.TabIndex = 1;
            _btnDeleteContainer.Text = "Delete container";
            _btnDeleteContainer.UseVisualStyleBackColor = false;
            _btnDeleteContainer.Visible = false;
            // 
            // _btnAddRecipe
            // 
            _btnAddRecipe.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddRecipe.Cursor = Cursors.Hand;
            _btnAddRecipe.Dock = DockStyle.Bottom;
            _btnAddRecipe.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddRecipe.FlatAppearance.BorderSize = 2;
            _btnAddRecipe.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnAddRecipe.FlatStyle = FlatStyle.Flat;
            _btnAddRecipe.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddRecipe.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddRecipe.Location = new Point(0, 592);
            _btnAddRecipe.Margin = new Padding(0, 10, 0, 0);
            _btnAddRecipe.Name = "_btnAddRecipe";
            _btnAddRecipe.Size = new Size(230, 38);
            _btnAddRecipe.TabIndex = 2;
            _btnAddRecipe.Text = "+ add a recipe";
            _btnAddRecipe.UseVisualStyleBackColor = false;
            // 
            // _btnAddContainer
            // 
            _btnAddContainer.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddContainer.Cursor = Cursors.Hand;
            _btnAddContainer.Dock = DockStyle.Top;
            _btnAddContainer.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddContainer.FlatAppearance.BorderSize = 2;
            _btnAddContainer.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnAddContainer.FlatStyle = FlatStyle.Flat;
            _btnAddContainer.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddContainer.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddContainer.Location = new Point(0, 10);
            _btnAddContainer.Margin = new Padding(0, 0, 0, 15);
            _btnAddContainer.Name = "_btnAddContainer";
            _btnAddContainer.Size = new Size(230, 36);
            _btnAddContainer.TabIndex = 3;
            _btnAddContainer.Text = "+ Add container";
            _btnAddContainer.UseVisualStyleBackColor = false;
            // 
            // _fridgeLeftPanel
            // 
            _fridgeLeftPanel.Controls.Add(_fridgeLeftContentPanel);
            _fridgeLeftPanel.Controls.Add(_fridgeActionsPanel);
            _fridgeLeftPanel.Controls.Add(_lblFridgeHeading);
            _fridgeLeftPanel.Dock = DockStyle.Left;
            _fridgeLeftPanel.Location = new Point(10, 20);
            _fridgeLeftPanel.Name = "_fridgeLeftPanel";
            _fridgeLeftPanel.Padding = new Padding(0, 10, 10, 10);
            _fridgeLeftPanel.Size = new Size(560, 640);
            _fridgeLeftPanel.TabIndex = 2;
            // 
            // _fridgeLeftContentPanel
            // 
            _fridgeLeftContentPanel.BackColor = Color.White;
            _fridgeLeftContentPanel.Controls.Add(_fridgeListView);
            _fridgeLeftContentPanel.Controls.Add(_containerTabs);
            _fridgeLeftContentPanel.Dock = DockStyle.Fill;
            _fridgeLeftContentPanel.Location = new Point(0, 40);
            _fridgeLeftContentPanel.Name = "_fridgeLeftContentPanel";
            _fridgeLeftContentPanel.Padding = new Padding(0, 10, 0, 10);
            _fridgeLeftContentPanel.Size = new Size(550, 540);
            _fridgeLeftContentPanel.TabIndex = 0;
            // 
            // _fridgeListView
            // 
            _fridgeListView.BorderStyle = BorderStyle.FixedSingle;
            _fridgeListView.Dock = DockStyle.Fill;
            _fridgeListView.Font = new Font("Segoe UI", 9F);
            _fridgeListView.FullRowSelect = true;
            _fridgeListView.GridLines = true;
            _fridgeListView.Location = new Point(0, 38);
            _fridgeListView.Margin = new Padding(0);
            _fridgeListView.MultiSelect = false;
            _fridgeListView.Name = "_fridgeListView";
            _fridgeListView.Size = new Size(550, 492);
            _fridgeListView.TabIndex = 0;
            _fridgeListView.UseCompatibleStateImageBehavior = false;
            _fridgeListView.View = View.Details;
            // 
            // _containerTabs
            // 
            _containerTabs.Appearance = TabAppearance.FlatButtons;
            _containerTabs.Dock = DockStyle.Top;
            _containerTabs.Font = new Font("Segoe UI", 9F);
            _containerTabs.Location = new Point(0, 10);
            _containerTabs.Margin = new Padding(0);
            _containerTabs.Name = "_containerTabs";
            _containerTabs.SelectedIndex = 0;
            _containerTabs.Size = new Size(550, 28);
            _containerTabs.TabIndex = 1;
            // 
            // _fridgeActionsPanel
            // 
            _fridgeActionsPanel.BackColor = Color.White;
            _fridgeActionsPanel.Controls.Add(_fridgeActionsFlowPanel);
            _fridgeActionsPanel.Dock = DockStyle.Bottom;
            _fridgeActionsPanel.Location = new Point(0, 580);
            _fridgeActionsPanel.Name = "_fridgeActionsPanel";
            _fridgeActionsPanel.Padding = new Padding(0, 5, 0, 0);
            _fridgeActionsPanel.Size = new Size(550, 50);
            _fridgeActionsPanel.TabIndex = 1;
            // 
            // _fridgeActionsFlowPanel
            // 
            _fridgeActionsFlowPanel.Controls.Add(_btnAddFood);
            _fridgeActionsFlowPanel.Controls.Add(_btnMoveFood);
            _fridgeActionsFlowPanel.Controls.Add(_btnDeleteFood);
            _fridgeActionsFlowPanel.Dock = DockStyle.Fill;
            _fridgeActionsFlowPanel.Location = new Point(0, 5);
            _fridgeActionsFlowPanel.Margin = new Padding(0);
            _fridgeActionsFlowPanel.Name = "_fridgeActionsFlowPanel";
            _fridgeActionsFlowPanel.Size = new Size(550, 45);
            _fridgeActionsFlowPanel.TabIndex = 0;
            _fridgeActionsFlowPanel.WrapContents = false;
            // 
            // _btnAddFood
            // 
            _btnAddFood.BackColor = Color.FromArgb(232, 245, 233);
            _btnAddFood.Cursor = Cursors.Hand;
            _btnAddFood.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnAddFood.FlatAppearance.BorderSize = 2;
            _btnAddFood.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnAddFood.FlatStyle = FlatStyle.Flat;
            _btnAddFood.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddFood.ForeColor = Color.FromArgb(39, 174, 96);
            _btnAddFood.Location = new Point(0, 0);
            _btnAddFood.Margin = new Padding(0, 0, 8, 0);
            _btnAddFood.Name = "_btnAddFood";
            _btnAddFood.Size = new Size(100, 34);
            _btnAddFood.TabIndex = 0;
            _btnAddFood.Text = "+ Add";
            _btnAddFood.UseVisualStyleBackColor = false;
            // 
            // _btnMoveFood
            // 
            _btnMoveFood.BackColor = Color.FromArgb(255, 249, 230);
            _btnMoveFood.Cursor = Cursors.Hand;
            _btnMoveFood.FlatAppearance.BorderColor = Color.FromArgb(241, 196, 15);
            _btnMoveFood.FlatAppearance.BorderSize = 2;
            _btnMoveFood.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 243, 200);
            _btnMoveFood.FlatStyle = FlatStyle.Flat;
            _btnMoveFood.Font = new Font("Segoe UI", 9F);
            _btnMoveFood.ForeColor = Color.FromArgb(180, 150, 10);
            _btnMoveFood.Location = new Point(108, 0);
            _btnMoveFood.Margin = new Padding(0, 0, 8, 0);
            _btnMoveFood.Name = "_btnMoveFood";
            _btnMoveFood.Size = new Size(100, 34);
            _btnMoveFood.TabIndex = 1;
            _btnMoveFood.Text = "Move to...";
            _btnMoveFood.UseVisualStyleBackColor = false;
            // 
            // _btnDeleteFood
            // 
            _btnDeleteFood.BackColor = Color.FromArgb(255, 240, 240);
            _btnDeleteFood.Cursor = Cursors.Hand;
            _btnDeleteFood.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDeleteFood.FlatAppearance.BorderSize = 2;
            _btnDeleteFood.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 220, 220);
            _btnDeleteFood.FlatStyle = FlatStyle.Flat;
            _btnDeleteFood.Font = new Font("Segoe UI", 9F);
            _btnDeleteFood.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDeleteFood.Location = new Point(216, 0);
            _btnDeleteFood.Margin = new Padding(0);
            _btnDeleteFood.Name = "_btnDeleteFood";
            _btnDeleteFood.Size = new Size(80, 34);
            _btnDeleteFood.TabIndex = 2;
            _btnDeleteFood.Text = "Delete";
            _btnDeleteFood.UseVisualStyleBackColor = false;
            // 
            // _lblFridgeHeading
            // 
            _lblFridgeHeading.Dock = DockStyle.Top;
            _lblFridgeHeading.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _lblFridgeHeading.ForeColor = Color.FromArgb(44, 62, 80);
            _lblFridgeHeading.Location = new Point(0, 10);
            _lblFridgeHeading.Name = "_lblFridgeHeading";
            _lblFridgeHeading.Padding = new Padding(0, 5, 0, 0);
            _lblFridgeHeading.Size = new Size(550, 30);
            _lblFridgeHeading.TabIndex = 2;
            _lblFridgeHeading.Text = "Kitchen food tracking";
            // 
            // _habitsTab
            // 
            _habitsTab.Controls.Add(_habitsPanel);
            _habitsTab.Location = new Point(4, 26);
            _habitsTab.Margin = new Padding(0);
            _habitsTab.Name = "_habitsTab";
            _habitsTab.Size = new Size(1072, 690);
            _habitsTab.TabIndex = 4;
            _habitsTab.Text = "✅ Habits";
            _habitsTab.UseVisualStyleBackColor = true;
            // 
            // _habitsPanel
            // 
            _habitsPanel.BackColor = Color.White;
            _habitsPanel.Controls.Add(_habitContentPanel);
            _habitsPanel.Controls.Add(_habitCategoryTabs);
            _habitsPanel.Controls.Add(_habitToolbar);
            _habitsPanel.Dock = DockStyle.Fill;
            _habitsPanel.Location = new Point(0, 0);
            _habitsPanel.Name = "_habitsPanel";
            _habitsPanel.Size = new Size(1072, 690);
            _habitsPanel.TabIndex = 0;
            // 
            // _habitContentPanel
            // 
            _habitContentPanel.Controls.Add(_habitSplitContainer);
            _habitContentPanel.Dock = DockStyle.Fill;
            _habitContentPanel.Location = new Point(0, 82);
            _habitContentPanel.Name = "_habitContentPanel";
            _habitContentPanel.Padding = new Padding(10);
            _habitContentPanel.Size = new Size(1072, 608);
            _habitContentPanel.TabIndex = 2;
            // 
            // _habitSplitContainer
            // 
            _habitSplitContainer.Dock = DockStyle.Fill;
            _habitSplitContainer.IsSplitterFixed = true;
            _habitSplitContainer.Location = new Point(10, 10);
            _habitSplitContainer.Name = "_habitSplitContainer";
            // 
            // _habitSplitContainer.Panel1
            // 
            _habitSplitContainer.Panel1.Controls.Add(_habitStepsList);
            // 
            // _habitSplitContainer.Panel2
            // 
            _habitSplitContainer.Panel2.Controls.Add(_habitRightPanel);
            _habitSplitContainer.Size = new Size(1052, 588);
            _habitSplitContainer.SplitterDistance = 424;
            _habitSplitContainer.TabIndex = 0;
            // 
            // _habitStepsList
            // 
            _habitStepsList.Dock = DockStyle.Fill;
            _habitStepsList.FullRowSelect = true;
            _habitStepsList.GridLines = true;
            _habitStepsList.Location = new Point(0, 0);
            _habitStepsList.MultiSelect = false;
            _habitStepsList.Name = "_habitStepsList";
            _habitStepsList.Size = new Size(424, 588);
            _habitStepsList.TabIndex = 0;
            _habitStepsList.UseCompatibleStateImageBehavior = false;
            _habitStepsList.View = View.Details;
            // 
            // _habitRightPanel
            // 
            _habitRightPanel.Controls.Add(_habitCardsFlow);
            _habitRightPanel.Controls.Add(_habitCalendar);
            _habitRightPanel.Dock = DockStyle.Fill;
            _habitRightPanel.Location = new Point(0, 0);
            _habitRightPanel.Name = "_habitRightPanel";
            _habitRightPanel.Padding = new Padding(8, 0, 0, 0);
            _habitRightPanel.Size = new Size(624, 588);
            _habitRightPanel.TabIndex = 0;
            // 
            // _habitCardsFlow
            // 
            _habitCardsFlow.AutoScroll = true;
            _habitCardsFlow.Dock = DockStyle.Fill;
            _habitCardsFlow.Location = new Point(8, 162);
            _habitCardsFlow.Name = "_habitCardsFlow";
            _habitCardsFlow.Padding = new Padding(5);
            _habitCardsFlow.Size = new Size(616, 426);
            _habitCardsFlow.TabIndex = 0;
            // 
            // _habitCalendar
            // 
            _habitCalendar.CalendarDimensions = new Size(2, 1);
            _habitCalendar.Dock = DockStyle.Top;
            _habitCalendar.Location = new Point(8, 0);
            _habitCalendar.MaxSelectionCount = 1;
            _habitCalendar.Name = "_habitCalendar";
            _habitCalendar.TabIndex = 1;
            // 
            // _habitCategoryTabs
            // 
            _habitCategoryTabs.Appearance = TabAppearance.FlatButtons;
            _habitCategoryTabs.Dock = DockStyle.Top;
            _habitCategoryTabs.Font = new Font("Segoe UI", 9F);
            _habitCategoryTabs.Location = new Point(0, 50);
            _habitCategoryTabs.Margin = new Padding(0);
            _habitCategoryTabs.Name = "_habitCategoryTabs";
            _habitCategoryTabs.Padding = new Point(8, 4);
            _habitCategoryTabs.SelectedIndex = 0;
            _habitCategoryTabs.Size = new Size(1072, 32);
            _habitCategoryTabs.TabIndex = 1;
            // 
            // _habitToolbar
            // 
            _habitToolbar.BackColor = Color.White;
            _habitToolbar.Controls.Add(_btnAddHabitStep);
            _habitToolbar.Controls.Add(_btnAddHabitCategory);
            _habitToolbar.Dock = DockStyle.Top;
            _habitToolbar.Location = new Point(0, 0);
            _habitToolbar.Name = "_habitToolbar";
            _habitToolbar.Padding = new Padding(10, 10, 10, 5);
            _habitToolbar.Size = new Size(1072, 50);
            _habitToolbar.TabIndex = 0;
            // 
            // _btnAddHabitStep
            // 
            _btnAddHabitStep.BackColor = Color.FromArgb(232, 245, 233);
            _btnAddHabitStep.Cursor = Cursors.Hand;
            _btnAddHabitStep.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnAddHabitStep.FlatAppearance.BorderSize = 2;
            _btnAddHabitStep.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnAddHabitStep.FlatStyle = FlatStyle.Flat;
            _btnAddHabitStep.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddHabitStep.ForeColor = Color.FromArgb(39, 174, 96);
            _btnAddHabitStep.Location = new Point(190, 10);
            _btnAddHabitStep.Name = "_btnAddHabitStep";
            _btnAddHabitStep.Size = new Size(130, 32);
            _btnAddHabitStep.TabIndex = 1;
            _btnAddHabitStep.Text = "+ Add step";
            _btnAddHabitStep.UseVisualStyleBackColor = false;
            // 
            // _btnAddHabitCategory
            // 
            _btnAddHabitCategory.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddHabitCategory.Cursor = Cursors.Hand;
            _btnAddHabitCategory.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddHabitCategory.FlatAppearance.BorderSize = 2;
            _btnAddHabitCategory.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnAddHabitCategory.FlatStyle = FlatStyle.Flat;
            _btnAddHabitCategory.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddHabitCategory.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddHabitCategory.Location = new Point(10, 10);
            _btnAddHabitCategory.Name = "_btnAddHabitCategory";
            _btnAddHabitCategory.Size = new Size(170, 32);
            _btnAddHabitCategory.TabIndex = 0;
            _btnAddHabitCategory.Text = "+ Add habit category";
            _btnAddHabitCategory.UseVisualStyleBackColor = false;
            // 
            // _medTab
            // 
            _medTab.Controls.Add(_medPanel);
            _medTab.Location = new Point(4, 26);
            _medTab.Margin = new Padding(0);
            _medTab.Name = "_medTab";
            _medTab.Size = new Size(1072, 690);
            _medTab.TabIndex = 2;
            _medTab.Text = "💊 Medications";
            _medTab.UseVisualStyleBackColor = true;
            // 
            // _medPanel
            // 
            _medPanel.BackColor = Color.White;
            _medPanel.Controls.Add(_medContentPanel);
            _medPanel.Dock = DockStyle.Fill;
            _medPanel.Location = new Point(0, 0);
            _medPanel.Name = "_medPanel";
            _medPanel.Padding = new Padding(10);
            _medPanel.Size = new Size(1072, 690);
            _medPanel.TabIndex = 0;
            // 
            // _medContentPanel
            // 
            _medContentPanel.BackColor = Color.White;
            _medContentPanel.Controls.Add(_medRightPanel);
            _medContentPanel.Controls.Add(_medMiddlePanel);
            _medContentPanel.Controls.Add(_medLeftPanel);
            _medContentPanel.Dock = DockStyle.Fill;
            _medContentPanel.Location = new Point(10, 10);
            _medContentPanel.Name = "_medContentPanel";
            _medContentPanel.Padding = new Padding(10, 20, 10, 10);
            _medContentPanel.Size = new Size(1052, 670);
            _medContentPanel.TabIndex = 0;
            // 
            // _medRightPanel
            // 
            _medRightPanel.BackColor = Color.White;
            _medRightPanel.Controls.Add(_intakeItemsPanel);
            _medRightPanel.Controls.Add(_btnAddSchedule);
            _medRightPanel.Controls.Add(_intakeCalendar);
            _medRightPanel.Controls.Add(_lblIntakeHeader);
            _medRightPanel.Dock = DockStyle.Fill;
            _medRightPanel.Location = new Point(800, 20);
            _medRightPanel.Name = "_medRightPanel";
            _medRightPanel.Padding = new Padding(10);
            _medRightPanel.Size = new Size(242, 640);
            _medRightPanel.TabIndex = 0;
            // 
            // _intakeItemsPanel
            // 
            _intakeItemsPanel.AutoScroll = true;
            _intakeItemsPanel.Dock = DockStyle.Fill;
            _intakeItemsPanel.FlowDirection = FlowDirection.TopDown;
            _intakeItemsPanel.Location = new Point(10, 202);
            _intakeItemsPanel.Name = "_intakeItemsPanel";
            _intakeItemsPanel.Padding = new Padding(4);
            _intakeItemsPanel.Size = new Size(222, 394);
            _intakeItemsPanel.TabIndex = 0;
            _intakeItemsPanel.WrapContents = false;
            // 
            // _btnAddSchedule
            // 
            _btnAddSchedule.BackColor = Color.FromArgb(232, 245, 233);
            _btnAddSchedule.Cursor = Cursors.Hand;
            _btnAddSchedule.Dock = DockStyle.Bottom;
            _btnAddSchedule.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnAddSchedule.FlatAppearance.BorderSize = 2;
            _btnAddSchedule.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnAddSchedule.FlatStyle = FlatStyle.Flat;
            _btnAddSchedule.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddSchedule.ForeColor = Color.FromArgb(39, 174, 96);
            _btnAddSchedule.Location = new Point(10, 596);
            _btnAddSchedule.Name = "_btnAddSchedule";
            _btnAddSchedule.Size = new Size(222, 34);
            _btnAddSchedule.TabIndex = 1;
            _btnAddSchedule.Text = "+ Add schedule";
            _btnAddSchedule.UseVisualStyleBackColor = false;
            // 
            // _intakeCalendar
            // 
            _intakeCalendar.Dock = DockStyle.Top;
            _intakeCalendar.Location = new Point(10, 40);
            _intakeCalendar.Margin = new Padding(0, 8, 0, 8);
            _intakeCalendar.MaxSelectionCount = 1;
            _intakeCalendar.Name = "_intakeCalendar";
            _intakeCalendar.TabIndex = 2;
            // 
            // _lblIntakeHeader
            // 
            _lblIntakeHeader.Dock = DockStyle.Top;
            _lblIntakeHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            _lblIntakeHeader.ForeColor = Color.FromArgb(44, 62, 80);
            _lblIntakeHeader.Location = new Point(10, 10);
            _lblIntakeHeader.Name = "_lblIntakeHeader";
            _lblIntakeHeader.Padding = new Padding(0, 5, 0, 0);
            _lblIntakeHeader.Size = new Size(222, 30);
            _lblIntakeHeader.TabIndex = 3;
            _lblIntakeHeader.Text = "Intake Monitoring";
            // 
            // _medMiddlePanel
            // 
            _medMiddlePanel.BackColor = Color.White;
            _medMiddlePanel.Controls.Add(_medMiddleFiller);
            _medMiddlePanel.Controls.Add(_btnAddMedContainer);
            _medMiddlePanel.Controls.Add(_btnDeleteMedContainer);
            _medMiddlePanel.Dock = DockStyle.Left;
            _medMiddlePanel.Location = new Point(570, 20);
            _medMiddlePanel.Name = "_medMiddlePanel";
            _medMiddlePanel.Padding = new Padding(0, 10, 0, 10);
            _medMiddlePanel.Size = new Size(230, 640);
            _medMiddlePanel.TabIndex = 1;
            // 
            // _medMiddleFiller
            // 
            _medMiddleFiller.BackColor = Color.FromArgb(235, 245, 255);
            _medMiddleFiller.Dock = DockStyle.Fill;
            _medMiddleFiller.Location = new Point(0, 46);
            _medMiddleFiller.Name = "_medMiddleFiller";
            _medMiddleFiller.Size = new Size(230, 548);
            _medMiddleFiller.TabIndex = 1;
            // 
            // _btnAddMedContainer
            // 
            _btnAddMedContainer.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddMedContainer.Cursor = Cursors.Hand;
            _btnAddMedContainer.Dock = DockStyle.Top;
            _btnAddMedContainer.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddMedContainer.FlatAppearance.BorderSize = 2;
            _btnAddMedContainer.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnAddMedContainer.FlatStyle = FlatStyle.Flat;
            _btnAddMedContainer.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddMedContainer.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddMedContainer.Location = new Point(0, 10);
            _btnAddMedContainer.Margin = new Padding(0, 0, 0, 15);
            _btnAddMedContainer.Name = "_btnAddMedContainer";
            _btnAddMedContainer.Size = new Size(230, 36);
            _btnAddMedContainer.TabIndex = 0;
            _btnAddMedContainer.Text = "+ Add container";
            _btnAddMedContainer.UseVisualStyleBackColor = false;
            _btnAddMedContainer.Click += _btnAddMedContainer_Click;
            // 
            // _btnDeleteMedContainer
            // 
            _btnDeleteMedContainer.BackColor = Color.FromArgb(255, 240, 240);
            _btnDeleteMedContainer.Cursor = Cursors.Hand;
            _btnDeleteMedContainer.Dock = DockStyle.Bottom;
            _btnDeleteMedContainer.Enabled = false;
            _btnDeleteMedContainer.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDeleteMedContainer.FlatAppearance.BorderSize = 2;
            _btnDeleteMedContainer.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 220, 220);
            _btnDeleteMedContainer.FlatStyle = FlatStyle.Flat;
            _btnDeleteMedContainer.Font = new Font("Segoe UI", 9F);
            _btnDeleteMedContainer.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDeleteMedContainer.Location = new Point(0, 594);
            _btnDeleteMedContainer.Margin = new Padding(0);
            _btnDeleteMedContainer.Name = "_btnDeleteMedContainer";
            _btnDeleteMedContainer.Size = new Size(230, 36);
            _btnDeleteMedContainer.TabIndex = 2;
            _btnDeleteMedContainer.Text = "Delete container";
            _btnDeleteMedContainer.UseVisualStyleBackColor = false;
            _btnDeleteMedContainer.Click += _btnDeleteMedContainer_Click;
            // 
            // _medLeftPanel
            // 
            _medLeftPanel.BackColor = Color.White;
            _medLeftPanel.Controls.Add(_medLeftContentPanel);
            _medLeftPanel.Controls.Add(_medActionsPanel);
            _medLeftPanel.Controls.Add(_lblMedHeading);
            _medLeftPanel.Dock = DockStyle.Left;
            _medLeftPanel.Location = new Point(10, 20);
            _medLeftPanel.Name = "_medLeftPanel";
            _medLeftPanel.Padding = new Padding(0, 10, 10, 10);
            _medLeftPanel.Size = new Size(560, 640);
            _medLeftPanel.TabIndex = 2;
            // 
            // _medLeftContentPanel
            // 
            _medLeftContentPanel.BackColor = Color.White;
            _medLeftContentPanel.Controls.Add(_medicineListView);
            _medLeftContentPanel.Controls.Add(_medContainerTabs);
            _medLeftContentPanel.Dock = DockStyle.Fill;
            _medLeftContentPanel.Location = new Point(0, 46);
            _medLeftContentPanel.Name = "_medLeftContentPanel";
            _medLeftContentPanel.Padding = new Padding(0, 10, 0, 10);
            _medLeftContentPanel.Size = new Size(550, 534);
            _medLeftContentPanel.TabIndex = 0;
            // 
            // _medicineListView
            // 
            _medicineListView.BorderStyle = BorderStyle.FixedSingle;
            _medicineListView.Dock = DockStyle.Fill;
            _medicineListView.Font = new Font("Segoe UI", 9F);
            _medicineListView.FullRowSelect = true;
            _medicineListView.GridLines = true;
            _medicineListView.Location = new Point(0, 38);
            _medicineListView.Margin = new Padding(0);
            _medicineListView.MultiSelect = false;
            _medicineListView.Name = "_medicineListView";
            _medicineListView.Size = new Size(550, 486);
            _medicineListView.TabIndex = 0;
            _medicineListView.UseCompatibleStateImageBehavior = false;
            _medicineListView.View = View.Details;
            // 
            // _medContainerTabs
            // 
            _medContainerTabs.Appearance = TabAppearance.FlatButtons;
            _medContainerTabs.Dock = DockStyle.Top;
            _medContainerTabs.Font = new Font("Segoe UI", 9F);
            _medContainerTabs.Location = new Point(0, 10);
            _medContainerTabs.Name = "_medContainerTabs";
            _medContainerTabs.SelectedIndex = 0;
            _medContainerTabs.Size = new Size(550, 28);
            _medContainerTabs.TabIndex = 1;
            // 
            // _medActionsPanel
            // 
            _medActionsPanel.BackColor = Color.White;
            _medActionsPanel.Controls.Add(_medActionsFlow);
            _medActionsPanel.Dock = DockStyle.Bottom;
            _medActionsPanel.Location = new Point(0, 580);
            _medActionsPanel.Name = "_medActionsPanel";
            _medActionsPanel.Padding = new Padding(0, 5, 0, 0);
            _medActionsPanel.Size = new Size(550, 50);
            _medActionsPanel.TabIndex = 1;
            // 
            // _medActionsFlow
            // 
            _medActionsFlow.Controls.Add(_btnAddMedicine);
            _medActionsFlow.Controls.Add(_btnMoveMedicine);
            _medActionsFlow.Controls.Add(_btnDeleteMedicine);
            _medActionsFlow.Dock = DockStyle.Fill;
            _medActionsFlow.Location = new Point(0, 5);
            _medActionsFlow.Name = "_medActionsFlow";
            _medActionsFlow.Size = new Size(550, 45);
            _medActionsFlow.TabIndex = 0;
            _medActionsFlow.WrapContents = false;
            // 
            // _btnAddMedicine
            // 
            _btnAddMedicine.BackColor = Color.FromArgb(232, 245, 233);
            _btnAddMedicine.Cursor = Cursors.Hand;
            _btnAddMedicine.FlatStyle = FlatStyle.Flat;
            _btnAddMedicine.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddMedicine.ForeColor = Color.FromArgb(39, 174, 96);
            _btnAddMedicine.Location = new Point(0, 0);
            _btnAddMedicine.Margin = new Padding(0, 0, 8, 0);
            _btnAddMedicine.Name = "_btnAddMedicine";
            _btnAddMedicine.Size = new Size(100, 34);
            _btnAddMedicine.TabIndex = 0;
            _btnAddMedicine.Text = "+ Add";
            _btnAddMedicine.UseVisualStyleBackColor = false;
            // 
            // _btnMoveMedicine
            // 
            _btnMoveMedicine.BackColor = Color.FromArgb(255, 249, 230);
            _btnMoveMedicine.Cursor = Cursors.Hand;
            _btnMoveMedicine.FlatAppearance.BorderColor = Color.FromArgb(241, 196, 15);
            _btnMoveMedicine.FlatAppearance.BorderSize = 2;
            _btnMoveMedicine.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 243, 200);
            _btnMoveMedicine.FlatStyle = FlatStyle.Flat;
            _btnMoveMedicine.Font = new Font("Segoe UI", 9F);
            _btnMoveMedicine.ForeColor = Color.FromArgb(180, 150, 10);
            _btnMoveMedicine.Location = new Point(108, 0);
            _btnMoveMedicine.Margin = new Padding(0, 0, 8, 0);
            _btnMoveMedicine.Name = "_btnMoveMedicine";
            _btnMoveMedicine.Size = new Size(100, 34);
            _btnMoveMedicine.TabIndex = 1;
            _btnMoveMedicine.Text = "Move to...";
            _btnMoveMedicine.UseVisualStyleBackColor = false;
            // 
            // _btnDeleteMedicine
            // 
            _btnDeleteMedicine.BackColor = Color.FromArgb(255, 240, 240);
            _btnDeleteMedicine.Cursor = Cursors.Hand;
            _btnDeleteMedicine.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDeleteMedicine.FlatAppearance.BorderSize = 2;
            _btnDeleteMedicine.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 220, 220);
            _btnDeleteMedicine.FlatStyle = FlatStyle.Flat;
            _btnDeleteMedicine.Font = new Font("Segoe UI", 9F);
            _btnDeleteMedicine.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDeleteMedicine.Location = new Point(216, 0);
            _btnDeleteMedicine.Margin = new Padding(0);
            _btnDeleteMedicine.Name = "_btnDeleteMedicine";
            _btnDeleteMedicine.Size = new Size(80, 34);
            _btnDeleteMedicine.TabIndex = 2;
            _btnDeleteMedicine.Text = "Delete";
            _btnDeleteMedicine.UseVisualStyleBackColor = false;
            // 
            // _lblMedHeading
            // 
            _lblMedHeading.Dock = DockStyle.Top;
            _lblMedHeading.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _lblMedHeading.ForeColor = Color.FromArgb(44, 62, 80);
            _lblMedHeading.Location = new Point(0, 10);
            _lblMedHeading.Name = "_lblMedHeading";
            _lblMedHeading.Padding = new Padding(0, 6, 0, 0);
            _lblMedHeading.Size = new Size(550, 36);
            _lblMedHeading.TabIndex = 2;
            _lblMedHeading.Text = "First Aid & Medicine Tracking";
            // 
            // _plantsTab
            // 
            _plantsTab.Controls.Add(_plantsPanel);
            _plantsTab.Location = new Point(4, 26);
            _plantsTab.Margin = new Padding(0);
            _plantsTab.Name = "_plantsTab";
            _plantsTab.Size = new Size(1072, 690);
            _plantsTab.TabIndex = 3;
            _plantsTab.Text = "🌱 Plants";
            _plantsTab.UseVisualStyleBackColor = true;
            // 
            // _plantsPanel
            // 
            _plantsPanel.BackColor = Color.White;
            _plantsPanel.Controls.Add(_plantsFlowPanel);
            _plantsPanel.Controls.Add(_plantsToolbar);
            _plantsPanel.Dock = DockStyle.Fill;
            _plantsPanel.Location = new Point(0, 0);
            _plantsPanel.Name = "_plantsPanel";
            _plantsPanel.Size = new Size(1072, 690);
            _plantsPanel.TabIndex = 0;
            // 
            // _plantsFlowPanel
            // 
            _plantsFlowPanel.AutoScroll = true;
            _plantsFlowPanel.Dock = DockStyle.Fill;
            _plantsFlowPanel.Location = new Point(0, 50);
            _plantsFlowPanel.Name = "_plantsFlowPanel";
            _plantsFlowPanel.Padding = new Padding(10);
            _plantsFlowPanel.Size = new Size(1072, 640);
            _plantsFlowPanel.TabIndex = 0;
            // 
            // _plantsToolbar
            // 
            _plantsToolbar.BackColor = Color.White;
            _plantsToolbar.Controls.Add(_btnAddPlant);
            _plantsToolbar.Dock = DockStyle.Top;
            _plantsToolbar.Location = new Point(0, 0);
            _plantsToolbar.Name = "_plantsToolbar";
            _plantsToolbar.Padding = new Padding(10, 10, 10, 5);
            _plantsToolbar.Size = new Size(1072, 50);
            _plantsToolbar.TabIndex = 1;
            // 
            // _btnAddPlant
            // 
            _btnAddPlant.BackColor = Color.FromArgb(232, 245, 233);
            _btnAddPlant.Cursor = Cursors.Hand;
            _btnAddPlant.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnAddPlant.FlatAppearance.BorderSize = 2;
            _btnAddPlant.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnAddPlant.FlatStyle = FlatStyle.Flat;
            _btnAddPlant.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddPlant.ForeColor = Color.FromArgb(39, 174, 96);
            _btnAddPlant.Location = new Point(10, 10);
            _btnAddPlant.Name = "_btnAddPlant";
            _btnAddPlant.Size = new Size(120, 32);
            _btnAddPlant.TabIndex = 0;
            _btnAddPlant.Text = "+ Add Plant";
            _btnAddPlant.UseVisualStyleBackColor = false;
            // 
            // _btnMedShoppingList
            // 
            _btnMedShoppingList.BackColor = Color.FromArgb(235, 245, 255);
            _btnMedShoppingList.Cursor = Cursors.Hand;
            _btnMedShoppingList.Dock = DockStyle.Bottom;
            _btnMedShoppingList.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnMedShoppingList.FlatAppearance.BorderSize = 2;
            _btnMedShoppingList.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnMedShoppingList.FlatStyle = FlatStyle.Flat;
            _btnMedShoppingList.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnMedShoppingList.ForeColor = Color.FromArgb(52, 152, 219);
            _btnMedShoppingList.Location = new Point(0, 580);
            _btnMedShoppingList.Margin = new Padding(0, 10, 0, 0);
            _btnMedShoppingList.Name = "_btnMedShoppingList";
            _btnMedShoppingList.Size = new Size(230, 40);
            _btnMedShoppingList.TabIndex = 3;
            _btnMedShoppingList.Text = "Shopping list";
            _btnMedShoppingList.UseVisualStyleBackColor = false;
            // 
            // _medToolbar
            // 
            _medToolbar.Location = new Point(0, 0);
            _medToolbar.Name = "_medToolbar";
            _medToolbar.Size = new Size(200, 100);
            _medToolbar.TabIndex = 0;
            // 
            // _medicationsListView
            // 
            _medicationsListView.Location = new Point(0, 0);
            _medicationsListView.Name = "_medicationsListView";
            _medicationsListView.Size = new Size(121, 97);
            _medicationsListView.TabIndex = 0;
            _medicationsListView.UseCompatibleStateImageBehavior = false;
            // 
            // _btnAddMedication
            // 
            _btnAddMedication.Location = new Point(0, 0);
            _btnAddMedication.Name = "_btnAddMedication";
            _btnAddMedication.Size = new Size(75, 23);
            _btnAddMedication.TabIndex = 0;
            // 
            // TrackingView
            // 
            BackColor = Color.White;
            Controls.Add(_tabControl);
            Margin = new Padding(0);
            Name = "TrackingView";
            Size = new Size(1080, 720);
            _tabControl.ResumeLayout(false);
            _fridgeTab.ResumeLayout(false);
            _fridgeMainPanel.ResumeLayout(false);
            _fridgeContentPanel.ResumeLayout(false);
            _fridgeRightPanel.ResumeLayout(false);
            _fridgeMiddlePanel.ResumeLayout(false);
            _typeButtonsPanel.ResumeLayout(false);
            _typeButtonsInnerPanel.ResumeLayout(false);
            _fridgeLeftPanel.ResumeLayout(false);
            _fridgeLeftContentPanel.ResumeLayout(false);
            _fridgeActionsPanel.ResumeLayout(false);
            _fridgeActionsFlowPanel.ResumeLayout(false);
            _habitsTab.ResumeLayout(false);
            _habitsPanel.ResumeLayout(false);
            _habitContentPanel.ResumeLayout(false);
            _habitSplitContainer.Panel1.ResumeLayout(false);
            _habitSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_habitSplitContainer).EndInit();
            _habitSplitContainer.ResumeLayout(false);
            _habitRightPanel.ResumeLayout(false);
            _habitToolbar.ResumeLayout(false);
            _medTab.ResumeLayout(false);
            _medPanel.ResumeLayout(false);
            _medContentPanel.ResumeLayout(false);
            _medRightPanel.ResumeLayout(false);
            _medMiddlePanel.ResumeLayout(false);
            _medLeftPanel.ResumeLayout(false);
            _medLeftContentPanel.ResumeLayout(false);
            _medActionsPanel.ResumeLayout(false);
            _medActionsFlow.ResumeLayout(false);
            _plantsTab.ResumeLayout(false);
            _plantsPanel.ResumeLayout(false);
            _plantsToolbar.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
