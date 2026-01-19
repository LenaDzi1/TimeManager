// ============================================================================
// HomeBrowserForm.cs
// Przeglądarka układu domu - wizualizacja pięter, ścian i kontenerów.
// ============================================================================

#nullable enable

#region Importy
using System;                   // Podstawowe typy .NET (DateTime, Math)
using System.Collections.Generic; // Kolekcje (List, Dictionary)
using System.Data.SqlClient;    // Klient SQL Server
using System.Drawing;           // Grafika (Color, Point, Size)
using System.Linq;              // LINQ (Where, OrderBy)
using System.Windows.Forms;     // Windows Forms (UserControl, Panel)
using TimeManager.Database;     // Helper bazy danych
using TimeManager.Models;       // Modele (LayoutModel, FloorModel)
using TimeManager.Services;     // Serwisy (HomeLayoutService)
#endregion

namespace TimeManager.Forms
{
    /// <summary>
    /// Przeglądarka układu domu - pozwala wizualizować piętra, pokoje i kontenery.
    /// 
    /// DWA TRYBY (ViewMode):
    /// - Search: wyszukiwanie przedmiotów w kontenerach (read-only)
    /// - Design: projektowanie układu (piętra, ściany, kontenery)
    /// 
    /// GŁÓWNE FUNKCJE:
    /// - Canvas rysowania (ściany jako linie, kontenery jako prostokąty)
    /// - Hierarchia kontenerów (kontener w kontenerze)
    /// - CRUD przedmiotów i kontenerów
    /// - Wyszukiwanie po nazwie przedmiotu
    /// - Nawigacja między piętrami
    /// </summary>
    public partial class HomeBrowserForm : UserControl
    {
        #region Stałe

        /// <summary>Wysokość paska MainForm (dla pozycjonowania).</summary>
        private const int TOP_BAR_HEIGHT = 56;

        #endregion

        #region Pola prywatne - Serwis

        /// <summary>Serwis zarządzania layoutem domu.</summary>
        private readonly HomeLayoutService _layoutService;

        #endregion

        #region Pola prywatne - Model i stan

        /// <summary>Model aktualnego layoutu.</summary>
        private LayoutModel _layout = new();

        /// <summary>Indeks aktualnie wyświetlanego piętra.</summary>
        private int _currentFloorIndex;

        /// <summary>Aktualny tryb rysowania na canvas.</summary>
        private DrawMode _drawMode = DrawMode.None;

        /// <summary>Pierwszy punkt rysowanej linii/kontenera.</summary>
        private PointF? _firstPoint;

        /// <summary>ID podświetlonego kontenera (z wyszukiwania).</summary>
        private int? _highlightedContainerId;

        /// <summary>Aktualnie wybrany kontener.</summary>
        private ContainerModel? _selectedContainer;

        /// <summary>Aktualnie wybrana ściana.</summary>
        private WallModel? _selectedWall;

        #endregion

        #region Pola prywatne - Tymczasowe ID

        /// <summary>
        /// Tymczasowe ID dla nowych kontenerów (przed zapisem do DB).
        /// Muszą być ujemne żeby nie kolidować z ID z bazy.
        /// </summary>
        private int _nextTempContainerId = -1;

        /// <summary>
        /// Generuje następne tymczasowe ID dla kontenera.
        /// Zawsze ujemne żeby nie kolidować z ID z bazy.
        /// </summary>
        private int NextTempContainerId()
        {
            if (_nextTempContainerId >= 0) _nextTempContainerId = -1;
            return _nextTempContainerId--;
        }

        /// <summary>
        /// Upewnia się że kontener ma przypisane ID.
        /// </summary>
        private void EnsureContainerHasId(ContainerModel container)
        {
            if (container.ContainerId == 0)
                container.ContainerId = NextTempContainerId();
        }

        #endregion

        #region Pola prywatne - UI i widoki

        /// <summary>Aktualnie wybrany przedmiot na liście.</summary>
        private ItemInventoryModel? _selectedItemInList;

        /// <summary>Flaga do blokowania eventów TextChanged przy edycji nazwy przedmiotu.</summary>
        private bool _suppressItemNameEvents;

        /// <summary>Aktualny widok (Search/Design).</summary>
        private ViewMode _currentView = ViewMode.Search;

        /// <summary>Widok wyszukiwania.</summary>
        private HomeBrowserSearchView _searchView;

        /// <summary>Widok projektowania.</summary>
        private HomeBrowserDesignView _designView;

        #endregion

        #region Typy pomocnicze

        /// <summary>
        /// Tryb rysowania na canvas.
        /// </summary>
        private enum DrawMode
        {
            None,
            Wall,
            Container,
            Obstacle
        }

        /// <summary>
        /// Tryb widoku formularza.
        /// </summary>
        private enum ViewMode
        {
            Search,
            Design
        }

        #endregion

        #region Konstruktor

        public HomeBrowserForm()
        {
            _layoutService = new HomeLayoutService();

            InitializeComponent();

            // Inicjalizacja nowych widoków i podpięcie do hosta
            _searchView = new HomeBrowserSearchView { Dock = DockStyle.Fill };
            _designView = new HomeBrowserDesignView { Dock = DockStyle.Fill };

            // Domyślnie hostujemy widok wyszukiwania
            _currentView = ViewMode.Search;
            Controls.Clear();
            Controls.Add(_searchView);
            MapSearchViewControls();

            LoadLayout();
            WireEvents();
            RefreshFloorUi(); // Zamiast pełnego SwitchToSearchView które wyczyściłoby kontrolki ponownie
            
            // Zastosuj ograniczenia ról - Dzieci nie mogą edytować layoutu domu
            ApplyRoleRestrictions();
        }

        #endregion

        #region Inicjalizacja

        /// <summary>
        /// Ograniczenia ról - Kids nie mogą edytować layoutu.
        /// </summary>
        private void ApplyRoleRestrictions()
        {
            // Dzieci nie mają dostępu do przycisku Edytuj (mogą tylko wyszukiwać/przeglądać)
            if (UserSession.IsKid)
            {
                if (_btnEdit != null)
                {
                    _btnEdit.Visible = false;
                }
            }
        }

        /// <summary>
        /// Podmienia referencje kontrolek na te z widoku wyszukiwania.
        /// </summary>
        private void MapSearchViewControls()
        {
            _canvasPanel = _searchView.CanvasPanel;
            _txtSearch = _searchView.SearchTextBox;
            _lvResults = _searchView.ResultsListView;
            _lblResultInfo = _searchView.ResultInfoLabel;
            _lblFloor = _searchView.FloorLabel;
            _btnFloorUp = _searchView.FloorUpButton;
            _btnFloorDown = _searchView.FloorDownButton;
            _btnEdit = _searchView.EditButton;
            _containerExplorer = _searchView.ContainerExplorer;
            _lblLocationInfo = _searchView.LocationInfoLabel;
            _leftControlsPanel = _searchView.LeftControlsPanel;
            _txtFloorInput1 = _searchView.FloorInput1;
            _txtFloorInput2 = _searchView.FloorInput2;
        }

        /// <summary>
        /// Podmienia referencje kontrolek na te z widoku projektowania.
        /// Zachowujemy stary panel edycji (_rightPanel z dziećmi) w hostującym panelu design view.
        /// </summary>
        private void MapDesignViewControls()
        {
            _canvasPanel = _designView.CanvasPanel;
            _lblSize = _designView.SizeLabel;
            _numWidth = _designView.WidthInput;
            _numHeight = _designView.HeightInput;
            _btnFloorUp = _designView.FloorUpButton;
            _btnFloorDown = _designView.FloorDownButton;
            _btnAddFloor = _designView.AddFloorButton;
            _btnDeleteFloor = _designView.DeleteFloorButton;
            _btnAddWall = _designView.AddWallButton;
            _btnAddContainer = _designView.AddContainerButton;
            _btnSave = _designView.SaveButton;
            _txtFloorName = _designView.FloorNameTextBox;
            _rightPanel = _designView.RightPanel;

            // Prawy panel – mapowanie na nowe kontrolki widoku
            _lstContainers = _designView.ContainersList;
            _lblContainerSettings = _designView.ContainerSettingsLabel;
            _txtContainerName = _designView.ContainerNameTextBox;
            _txtRoomTag = _designView.RoomTagTextBox;
            _txtItemName = _designView.ItemNameTextBox;
            _numItemQty = _designView.ItemQtyNumeric;
            _cmbItemUnit = _designView.ItemUnitCombo;
            _btnAddItem = _designView.AddItemButton;
            _btnDeleteItems = _designView.DeleteItemsButton;
            _lvItems = _designView.ItemsListView;
            _lblAddingItem = _designView.AddingItemLabel;
            _lblPoints = _designView.PointsLabel;
            _lblPointA = _designView.PointALabel;
            _numPointAX = _designView.PointAXNumeric;
            _numPointAY = _designView.PointAYNumeric;
            _lblPointB = _designView.PointBLabel;
            _numPointBX = _designView.PointBXNumeric;
            _numPointBY = _designView.PointBYNumeric;
            _btnAddSubcontainer = _designView.AddSubcontainerButton;
            _btnDeleteContainer = _designView.DeleteContainerButton;
            _lblSubsections = _designView.SubsectionsLabel;
        }

        private void LoadLayout()
        {
            _layout = _layoutService.LoadOrCreateLayout();
            _currentFloorIndex = 0;
            SyncHouseSizeControls();
            RefreshFloorUi();
            RefreshSearchResults();
        }

        private bool _searchViewEventsWired = false;
        private bool _designViewEventsWired = false;
        
        private void WireEvents()
        {
            if (_currentView == ViewMode.Search && !_searchViewEventsWired)
            {
                WireSearchViewEvents();
                _searchViewEventsWired = true;
            }
            else if (_currentView == ViewMode.Design && !_designViewEventsWired)
            {
                WireDesignViewEvents();
                _designViewEventsWired = true;
            }
        }

        #endregion

        #region SearchView - Wyszukiwanie

        /// <summary>
        /// Podpina eventy dla widoku wyszukiwania.
        /// </summary>
        private void WireSearchViewEvents()
        {
            _searchView.CanvasPanel.Paint += CanvasPanel_Paint;
            _searchView.CanvasPanel.MouseClick += CanvasPanel_MouseClick;
            _searchView.FloorUpButton.Click += BtnFloorUp_Click;
            _searchView.FloorDownButton.Click += BtnFloorDown_Click;
            _searchView.EditButton.Click += BtnEdit_Click;
            _searchView.SearchTextBox.TextChanged += TxtSearch_TextChanged;
            _searchView.ResultsListView.SelectedIndexChanged += LvResults_SelectedIndexChanged;
            _searchView.ResultsListView.ColumnWidthChanging += LvResultsColumnWidthChanging;
            _searchView.ContainerExplorer.AfterSelect += ContainerExplorerOnAfterSelect;
        }

        #endregion

        #region DesignView - Projektowanie

        /// <summary>
        /// Podpina eventy dla widoku projektowania.
        /// </summary>
        private void WireDesignViewEvents()
        {
            _designView.CanvasPanel.Paint += CanvasPanel_Paint;
            _designView.CanvasPanel.MouseClick += CanvasPanel_MouseClick;
            _designView.FloorUpButton.Click += BtnFloorUp_Click;
            _designView.FloorDownButton.Click += BtnFloorDown_Click;
            _designView.AddFloorButton.Click += AddFloorHandler;
            _designView.DeleteFloorButton.Click += BtnDeleteFloor_Click;
            _designView.AddWallButton.Click += BtnAddWall_Click;
            _designView.AddContainerButton.Click += BtnAddContainer_Click;
            _designView.SaveButton.Click += SaveAndBackHandler;
            _designView.WidthInput.ValueChanged += NumSize_ValueChanged;
            _designView.HeightInput.ValueChanged += NumSize_ValueChanged;
            
            if (_designView.FloorNameTextBox != null)
                _designView.FloorNameTextBox.TextChanged += TxtFloorName_TextChanged;
            
            _designView.ContainersList.SelectedIndexChanged += LstContainers_SelectedIndexChanged;
            _designView.AddItemButton.Click += BtnAddItem_Click;
            _designView.DeleteItemsButton.Click += BtnDeleteItems_Click;
            _designView.AddSubcontainerButton.Click += BtnAddSubcontainer_Click;
            _designView.DeleteContainerButton.Click += BtnDeleteContainer_Click;
            _designView.ContainerNameTextBox.TextChanged += TxtContainerName_TextChanged;
            _designView.RoomTagTextBox.TextChanged += TxtRoomTag_TextChanged;
            
            _designView.ItemsListView.SelectedIndexChanged += LvItems_SelectedIndexChanged;
            _designView.ItemNameTextBox.TextChanged += ItemNameTextBox_TextChanged;
            _designView.ItemQtyNumeric.ValueChanged += NumItemQtyOnValueChanged;
            _designView.ItemUnitCombo.SelectedIndexChanged += ItemUnitChanged;
            _designView.ItemUnitCombo.TextChanged += ItemUnitChanged;
            
            _designView.PointAXNumeric.ValueChanged += UpdateContainerPoints;
            _designView.PointAYNumeric.ValueChanged += UpdateContainerPoints;
            _designView.PointBXNumeric.ValueChanged += UpdateContainerPoints;
            _designView.PointBYNumeric.ValueChanged += UpdateContainerPoints;
        }

        private void ItemNameTextBox_TextChanged(object? sender, EventArgs e)
        {
            if (_suppressItemNameEvents) return;
            if (_currentView != ViewMode.Design) return;
            if (_selectedContainer == null) return;
            if (_selectedItemInList == null) return;
            if (_designView?.ItemsListView?.SelectedItems.Count == 0) return;

            var newName = _txtItemName.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(newName))
                return;

            _selectedItemInList.ItemName = newName;
            var selected = _designView?.ItemsListView?.SelectedItems[0];
            if (selected == null) return;
            if (ReferenceEquals(selected.Tag, _selectedItemInList))
            {
                selected.Text = newName;
            }

            RefreshSearchResults();
        }
        
        private void BtnFloorUp_Click(object? sender, EventArgs e) => ChangeFloor(1);
        private void BtnFloorDown_Click(object? sender, EventArgs e) => ChangeFloor(-1);
        private void BtnDeleteFloor_Click(object? sender, EventArgs e) => DeleteCurrentFloor();
        private void BtnEdit_Click(object? sender, EventArgs e) => SwitchToDesignView();
        private void BtnAddWall_Click(object? sender, EventArgs e) => SetDrawMode(DrawMode.Wall);
        private void BtnAddContainer_Click(object? sender, EventArgs e) => SetDrawMode(DrawMode.Container);
        private void NumSize_ValueChanged(object? sender, EventArgs e) => UpdateHouseSize();
        private void TxtFloorName_TextChanged(object? sender, EventArgs e) => UpdateFloorName();
        private void LstContainers_SelectedIndexChanged(object? sender, EventArgs e) => SelectContainerByList();
        private void BtnAddItem_Click(object? sender, EventArgs e) => AddItemToContainer();
        private void BtnDeleteItems_Click(object? sender, EventArgs e) => DeleteSelectedItems();
        private void BtnAddSubcontainer_Click(object? sender, EventArgs e) => AddSubcontainer();
        private void BtnDeleteContainer_Click(object? sender, EventArgs e) => DeleteContainer();
        private void TxtContainerName_TextChanged(object? sender, EventArgs e) => ApplyContainerNameRoomEdits();
        private void TxtRoomTag_TextChanged(object? sender, EventArgs e) => ApplyContainerNameRoomEdits();
        private void LvItems_SelectedIndexChanged(object? sender, EventArgs e) => SyncSelectedItemToEditors();
        private void TxtSearch_TextChanged(object? sender, EventArgs e) => RefreshSearchResults();
        private void LvResults_SelectedIndexChanged(object? sender, EventArgs e) => JumpToSearchResult();

        private void AddFloorHandler(object? sender, EventArgs e)
        {
            _layoutService.AddFloor(_layout);
            _currentFloorIndex = _layout.Floors.Count - 1;
            RefreshFloorUi();
        }

        private void SaveAndBackHandler(object? sender, EventArgs e)
        {
            SaveLayout();
            SwitchToSearchView();
        }

        private void LvResultsColumnWidthChanging(object? sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
        }
        private void ContainerExplorerOnAfterSelect(object? sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is ContainerModel container)
            {
                _highlightedContainerId = container.ContainerId;
            }
            else
            {
                _highlightedContainerId = null;
            }

            _canvasPanel.Invalidate();
        }

        private void SaveLayout()
        {
            try
            {
                _layoutService.SaveLayout(_layout);
                MessageBox.Show("Layout saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Saving failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshSearchResults()
        {
            var term = _txtSearch.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(term))
            {
                _lvResults.Items.Clear();
                return;
            }

            _lvResults.Columns.Clear();
            _lvResults.Columns.Add("Item", 80);      
            _lvResults.Columns.Add("Quantity", 60);   
            _lvResults.Columns.Add("Container", 92);  
            while (_lvResults.Columns.Count > 3)
            {
                _lvResults.Columns.RemoveAt(_lvResults.Columns.Count - 1);
            }

            try
            {
                var results = _layoutService.SearchItems(term);
                _lvResults.Items.Clear();
                foreach (var result in results)
                {
                    var item = new ListViewItem(result.ItemName)
                    {
                        Tag = result
                    };
                    item.SubItems.Add($"{result.Quantity} {result.Unit}");
                    item.SubItems.Add(result.ContainerName ?? "?");
                    _lvResults.Items.Add(item);
                }
            }
            catch (Exception)
            {
                // Ignoruj błędy wyszukiwania
            }
        }

        private void DeleteCurrentFloor()
        {
            if (!_layout.Floors.Any()) return;

            if (_layout.Floors.Count <= 1)
            {
                MessageBox.Show("Warning: There must be at least one floor in the design.",
                    "Cannot Delete Floor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete floor \"{_layout.Floors[_currentFloorIndex].Title}\"?\n\nThis will permanently delete all walls, containers, and items on this floor.",
                "Delete Floor",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                var floorToDelete = _layout.Floors[_currentFloorIndex];
                _layout.Floors.RemoveAt(_currentFloorIndex);

                if (_currentFloorIndex >= _layout.Floors.Count)
                {
                    _currentFloorIndex = _layout.Floors.Count - 1;
                }

                try
                {
                    using var connection = DatabaseHelper.GetConnection();
                    connection.Open();
                    using var cmd = new SqlCommand("DELETE FROM Floors WHERE FloorID=@id", connection);
                    cmd.Parameters.AddWithValue("@id", floorToDelete.FloorId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting floor: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                RefreshFloorUi();
            }
        }

        private void SwitchToSearchView()
        {
            _currentView = ViewMode.Search;
            _drawMode = DrawMode.None;
            _firstPoint = null;
            
            _selectedContainer = null;
            _selectedWall = null;
            _highlightedContainerId = null;
            
            Controls.Clear();
            Controls.Add(_searchView);
            MapSearchViewControls();
            WireEvents();

            RefreshFloorUi();
            RefreshContainerExplorer();
            RefreshSearchResults();
            _canvasPanel.Invalidate();
        }



        private void JumpToSearchResult()
        {
            if (_lvResults.SelectedItems.Count == 0) return;
            var data = _lvResults.SelectedItems[0].Tag as ItemInventoryModel;
            if (data == null) return;

            var floorIndex = _layout.Floors.FindIndex(f => f.FloorNumber == data.FloorNumber);
            if (floorIndex >= 0)
            {
                _currentFloorIndex = floorIndex;
                _highlightedContainerId = data.ContainerId;
                
                var floor = _layout.Floors[floorIndex];
                string floorName = string.IsNullOrWhiteSpace(floor.Title) 
                    ? $"Floor {floor.FloorNumber + 1}" 
                    : floor.Title;
                    
                _lblLocationInfo.Text = $"{data.ContainerName}; {floorName}; {data.RoomTag ?? "room"}; {data.Quantity} {data.Unit}";
                RefreshFloorUi();
                RefreshContainerExplorer();
                HighlightContainerInExplorer(data.ContainerId);
            }
        }

        private void RefreshContainerExplorer()
        {
            _containerExplorer.Nodes.Clear();
            if (!_layout.Floors.Any()) return;

            var floor = _layout.Floors[_currentFloorIndex];
            var rootContainers = floor.Containers.Where(c => !c.ParentContainerId.HasValue).ToList();

            foreach (var container in rootContainers)
            {
                var node = CreateContainerNode(container, floor);
                _containerExplorer.Nodes.Add(node);
            }
        }

        private TreeNode CreateContainerNode(ContainerModel container, FloorModel floor)
        {
            var node = new TreeNode(container.Name)
            {
                Tag = container
            };

            var subcontainers = floor.Containers
                .Where(c => c.ParentContainerId == container.ContainerId && c.ContainerId != container.ContainerId)
                .ToList();
            foreach (var subcontainer in subcontainers)
            {
                var subNode = CreateContainerNode(subcontainer, floor);
                node.Nodes.Add(subNode);
            }

            foreach (var item in container.Items)
            {
                var itemNode = new TreeNode($"{item.ItemName} ({item.Quantity} {item.Unit})")
                {
                    Tag = item,
                    ForeColor = Color.Gray
                };
                node.Nodes.Add(itemNode);
            }

            return node;
        }

        private void HighlightContainerInExplorer(int containerId)
        {
            TreeNode? foundNode = null;

            foreach (TreeNode root in _containerExplorer.Nodes)
            {
                foundNode = FindNodeByContainer(root, containerId);
                if (foundNode != null)
                    break;
            }

            if (foundNode == null)
                return;

            _containerExplorer.BeginUpdate();
            try
            {
                _containerExplorer.CollapseAll();

                TreeNode? current = foundNode;
                while (current != null)
                {
                    current.Expand();
                    current = current.Parent;
                }

                _containerExplorer.SelectedNode = foundNode;
                foundNode.EnsureVisible();
            }
            finally
            {
                _containerExplorer.EndUpdate();
            }
        }
        private TreeNode? FindNodeByContainer(TreeNode node, int containerId)
        {
            if (node.Tag is ContainerModel container && container.ContainerId == containerId)
                return node;

            foreach (TreeNode child in node.Nodes)
            {
                var result = FindNodeByContainer(child, containerId);
                if (result != null)
                    return result;
            }

            return null;
        }

        private bool FindAndSelectNode(TreeNode node, int containerId)
        {
            if (node.Tag is ContainerModel container && container.ContainerId == containerId)
            {
                _containerExplorer.SelectedNode = node;
                _containerExplorer.ExpandAll();
                return true;
            }

            foreach (TreeNode child in node.Nodes)
            {
                if (FindAndSelectNode(child, containerId))
                    return true;
            }

            return false;
        }

        private void SetDrawMode(DrawMode mode)
        {
            _drawMode = mode;
            _firstPoint = null;
            Cursor = mode == DrawMode.None ? Cursors.Default : Cursors.Cross;
        }

        private void ChangeFloor(int delta)
        {
            if (!_layout.Floors.Any()) return;
            _currentFloorIndex = Math.Clamp(_currentFloorIndex + delta, 0, _layout.Floors.Count - 1);
            RefreshFloorUi();
            if (_currentView == ViewMode.Search)
            {
                RefreshContainerExplorer();
            }
        }

        private void SyncHouseSizeControls()
        {
            if (_numWidth == null || _numHeight == null)
                return;
                
            _numWidth.Value = Math.Max(_numWidth.Minimum, Math.Min(_numWidth.Maximum, _layout.DefaultWidthMeters));
            _numHeight.Value = Math.Max(_numHeight.Minimum, Math.Min(_numHeight.Maximum, _layout.DefaultHeightMeters));
        }



        private void UpdateFloorName()
        {
            if (_currentView != ViewMode.Design) return;
            if (!_layout.Floors.Any()) return;
            if (_txtFloorName == null) return;

            var floor = _layout.Floors[_currentFloorIndex];
            floor.Title = _txtFloorName.Text?.Trim();
        }

        private void RefreshFloorUi()
        {
            if (!_layout.Floors.Any()) return;

            var floor = _layout.Floors[_currentFloorIndex];
            if (_currentView == ViewMode.Search)
            {
                _lblFloor.Visible = true;
                _lblFloor.Text = string.IsNullOrWhiteSpace(floor.Title) 
                    ? $"Floor {floor.FloorNumber + 1}" 
                    : floor.Title;
            }
            else
            {
                _lblFloor.Visible = false;
                if (_txtFloorName != null)
                {
                    _txtFloorName.Text = floor.Title ?? string.Empty;
                }
            }
            PopulateContainers();
            if (_currentView == ViewMode.Search)
            {
                RefreshContainerExplorer();
            }
            _canvasPanel.Invalidate();
        }

        private bool _suppressContainerEvents;

        private void PopulateContainers(bool clearSelection = true)
        {
            var floor = _layout.Floors[_currentFloorIndex];

            int? selectedId = _selectedContainer?.ContainerId;
            int? selectedParentId = _selectedContainer?.ParentContainerId;

            _suppressContainerEvents = true;

            if (_currentView == ViewMode.Design)
            {
                RefreshSubcontainersList(selectedId, clearSelection);
            }
            else if (_lstContainers != null)
            {
                _lstContainers.Items.Clear();
                foreach (var container in floor.Containers)
                {
                    var item = new ListItem<ContainerModel>(container.Name, container);
                    _lstContainers.Items.Add(item);
                    if (!clearSelection && selectedId.HasValue && container.ContainerId == selectedId.Value)
                    {
                        _lstContainers.SelectedItem = item;
                    }
                }
            }

            if (clearSelection)
            {
                _selectedContainer = null;
                if (_lvItems != null) _lvItems.Items.Clear();
                if (_txtContainerName != null) _txtContainerName.Text = string.Empty;
                if (_txtRoomTag != null) _txtRoomTag.Text = string.Empty;
                if (_lstContainers != null) _lstContainers.Items.Clear();
                if (_cmbItemUnit != null) _cmbItemUnit.Text = "pcs";
            }
            _suppressContainerEvents = false;
        }

        private void RefreshSubcontainersList(int? previouslySelectedId, bool clearSelection)
        {
            _lstContainers.Items.Clear();

            if (_selectedContainer == null)
                return;

            if (_selectedContainer.ContainerId == 0)
                return;

            var floor = _layout.Floors[_currentFloorIndex];
            var subs = floor.Containers
                .Where(c => c.ParentContainerId == _selectedContainer.ContainerId && 
                           c.ContainerId != _selectedContainer.ContainerId) 
                .ToList();

            foreach (var sub in subs)
            {
                var item = new ListItem<ContainerModel>(sub.Name, sub);
                _lstContainers.Items.Add(item);
            }
        }


        private void SelectContainerByList()
        {
            if (_lstContainers.SelectedItem is not ListItem<ContainerModel> item) return;
            _selectedContainer = item.Value;

            _txtContainerName.Text = _selectedContainer.Name ?? string.Empty;
            _txtRoomTag.Text = _selectedContainer.RoomTag ?? string.Empty;


            if (_currentView == ViewMode.Design)
            {
                SetPointEditorsFromContainer();
            }

            RefreshItemsList();

            if (_currentView == ViewMode.Design)
            {
                PopulateContainers(clearSelection: false);
            }

            UpdatePointsDescription();
            _canvasPanel.Invalidate();
        }


        private void UpdateContainerPoints(object? sender, EventArgs e)
        {
            if (_currentView != ViewMode.Design)
                return;

            if (_layout.Floors == null || _layout.Floors.Count == 0)
                return;

            var floor = _layout.Floors[_currentFloorIndex];

            decimal width = floor.WidthMeters > 0 ? floor.WidthMeters : 1m;
            decimal height = floor.HeightMeters > 0 ? floor.HeightMeters : 1m;

            decimal axMeters = _numPointAX.Value;
            decimal ayMeters = _numPointAY.Value;
            decimal bxMeters = _numPointBX.Value;
            decimal byMeters = _numPointBY.Value;

            axMeters = Math.Min(width, Math.Max(0m, axMeters));
            bxMeters = Math.Min(width, Math.Max(0m, bxMeters));
            ayMeters = Math.Min(height, Math.Max(0m, ayMeters));
            byMeters = Math.Min(height, Math.Max(0m, byMeters));

            if (_selectedWall != null)
            {
                // Konwertuj na znormalizowane [0..1]
                _selectedWall.StartX = (float)(axMeters / width);
                _selectedWall.StartY = (float)(ayMeters / height);
                _selectedWall.EndX = (float)(bxMeters / width);
                _selectedWall.EndY = (float)(byMeters / height);

                _canvasPanel.Invalidate();
                return;
            }

            if (_selectedContainer == null)
                return;

            // 3) Konwertuj na znormalizowane [0..1] (procent domu)
            float newAX = (float)(axMeters / width);
            float newBX = (float)(bxMeters / width);
            float newAY = (float)(ayMeters / height);
            float newBY = (float)(byMeters / height);

            newAX = Math.Max(0f, Math.Min(1f, newAX));
            newBX = Math.Max(0f, Math.Min(1f, newBX));
            newAY = Math.Max(0f, Math.Min(1f, newAY));
            newBY = Math.Max(0f, Math.Min(1f, newBY));

            if (_selectedContainer.ParentContainerId.HasValue)
            {
                var parent = floor.Containers
                    .FirstOrDefault(c => c.ContainerId == _selectedContainer.ParentContainerId.Value);

                if (parent != null)
                {
                    float parentMinX = Math.Min(parent.PointAX, parent.PointBX);
                    float parentMaxX = Math.Max(parent.PointAX, parent.PointBX);
                    float parentMinY = Math.Min(parent.PointAY, parent.PointBY);
                    float parentMaxY = Math.Max(parent.PointAY, parent.PointBY);

                    newAX = Math.Max(parentMinX, Math.Min(parentMaxX, newAX));
                    newBX = Math.Max(parentMinX, Math.Min(parentMaxX, newBX));
                    newAY = Math.Max(parentMinY, Math.Min(parentMaxY, newAY));
                    newBY = Math.Max(parentMinY, Math.Min(parentMaxY, newBY));
                }
            }

            float oldMinX = Math.Min(_selectedContainer.PointAX, _selectedContainer.PointBX);
            float oldMaxX = Math.Max(_selectedContainer.PointAX, _selectedContainer.PointBX);
            float oldMinY = Math.Min(_selectedContainer.PointAY, _selectedContainer.PointBY);
            float oldMaxY = Math.Max(_selectedContainer.PointAY, _selectedContainer.PointBY);
            _selectedContainer.PointAX = newAX;
            _selectedContainer.PointAY = newAY;
            _selectedContainer.PointBX = newBX;
            _selectedContainer.PointBY = newBY;

            float newMinX = Math.Min(newAX, newBX);
            float newMaxX = Math.Max(newAX, newBX);
            float newMinY = Math.Min(newAY, newBY);
            float newMaxY = Math.Max(newAY, newBY);
            RescaleSubcontainersToParent(_selectedContainer,
                oldMinX, oldMaxX, oldMinY, oldMaxY,
                newMinX, newMaxX, newMinY, newMaxY);

            SetPointEditorsFromContainer();

            UpdatePointsDescription();
            _canvasPanel.Invalidate();
        }


        private void UpdatePointsDescription()
        {
            if (_lblPoints == null)
            {
                return;
            }

            _lblPoints.Text = "Points:";
        }

        /// <summary>
        /// Rekursywnie przeskalowuje wszystkie podkontenery gdy ich rodzic jest zmieniany.
        /// Utrzymuje proporcjonalną pozycję i rozmiar względem rodzica.
        /// </summary>
        private void RescaleSubcontainersToParent(ContainerModel parent,
            float oldMinX, float oldMaxX, float oldMinY, float oldMaxY,
            float newMinX, float newMaxX, float newMinY, float newMaxY)
        {
            if (!_layout.Floors.Any()) return;
            var floor = _layout.Floors[_currentFloorIndex];

            var subcontainers = floor.Containers
                .Where(c => c.ParentContainerId == parent.ContainerId && c.ContainerId != parent.ContainerId)
                .ToList();

            float oldWidth = oldMaxX - oldMinX;
            float oldHeight = oldMaxY - oldMinY;
            float newWidth = newMaxX - newMinX;
            float newHeight = newMaxY - newMinY;

            if (oldWidth <= 0 || oldHeight <= 0) return;

            foreach (var sub in subcontainers)
            {
                float subOldMinX = Math.Min(sub.PointAX, sub.PointBX);
                float subOldMaxX = Math.Max(sub.PointAX, sub.PointBX);
                float subOldMinY = Math.Min(sub.PointAY, sub.PointBY);
                float subOldMaxY = Math.Max(sub.PointAY, sub.PointBY);

                float relAX = (sub.PointAX - oldMinX) / oldWidth;
                float relAY = (sub.PointAY - oldMinY) / oldHeight;
                float relBX = (sub.PointBX - oldMinX) / oldWidth;
                float relBY = (sub.PointBY - oldMinY) / oldHeight;

                sub.PointAX = newMinX + relAX * newWidth;
                sub.PointAY = newMinY + relAY * newHeight;
                sub.PointBX = newMinX + relBX * newWidth;
                sub.PointBY = newMinY + relBY * newHeight;
                float subNewMinX = Math.Min(sub.PointAX, sub.PointBX);
                float subNewMaxX = Math.Max(sub.PointAX, sub.PointBX);
                float subNewMinY = Math.Min(sub.PointAY, sub.PointBY);
                float subNewMaxY = Math.Max(sub.PointAY, sub.PointBY);

                RescaleSubcontainersToParent(sub,
                    subOldMinX, subOldMaxX, subOldMinY, subOldMaxY,
                    subNewMinX, subNewMaxX, subNewMinY, subNewMaxY);
            }
        }

        /// <summary>
        /// Zaktualizuj edytory numeryczne A/B aby pokazywały punkty kontenera w metrach
        /// (ułamek * rozmiar domu) i dostosuj ich zakresy.
        /// </summary>
        private void SetPointEditorsFromContainer()
        {
            if (_selectedContainer == null || _currentView != ViewMode.Design)
                return;

            if (_layout.Floors == null || _layout.Floors.Count == 0)
                return;

            var floor = _layout.Floors[_currentFloorIndex];

            decimal width = floor.WidthMeters > 0 ? floor.WidthMeters : 1m;
            decimal height = floor.HeightMeters > 0 ? floor.HeightMeters : 1m;

            _numPointAX.ValueChanged -= UpdateContainerPoints;
            _numPointAY.ValueChanged -= UpdateContainerPoints;
            _numPointBX.ValueChanged -= UpdateContainerPoints;
            _numPointBY.ValueChanged -= UpdateContainerPoints;

            _numPointAX.Minimum = 0;
            _numPointBX.Minimum = 0;
            _numPointAY.Minimum = 0;
            _numPointBY.Minimum = 0;

            _numPointAX.Maximum = width;
            _numPointBX.Maximum = width;
            _numPointAY.Maximum = height;
            _numPointBY.Maximum = height;

            decimal axMeters = (decimal)_selectedContainer.PointAX * width;
            decimal ayMeters = (decimal)_selectedContainer.PointAY * height;
            decimal bxMeters = (decimal)_selectedContainer.PointBX * width;
            decimal byMeters = (decimal)_selectedContainer.PointBY * height;

            axMeters = Math.Min(width, Math.Max(0m, axMeters));
            ayMeters = Math.Min(height, Math.Max(0m, ayMeters));
            bxMeters = Math.Min(width, Math.Max(0m, bxMeters));
            byMeters = Math.Min(height, Math.Max(0m, byMeters));

            _numPointAX.Value = axMeters;
            _numPointAY.Value = ayMeters;
            _numPointBX.Value = bxMeters;
            _numPointBY.Value = byMeters;

            _numPointAX.ValueChanged += UpdateContainerPoints;
            _numPointAY.ValueChanged += UpdateContainerPoints;
            _numPointBX.ValueChanged += UpdateContainerPoints;
            _numPointBY.ValueChanged += UpdateContainerPoints;
        }

        /// <summary>
        /// Zaktualizuj edytory numeryczne A/B aby pokazywały punkty ściany w metrach
        /// i dostosuj ich zakresy. Podobne do SetPointEditorsFromContainer.
        /// </summary>
        private void SetPointEditorsFromWall()
        {
            if (_selectedWall == null || _currentView != ViewMode.Design)
                return;

            if (_layout.Floors == null || _layout.Floors.Count == 0)
                return;

            var floor = _layout.Floors[_currentFloorIndex];
            decimal width = floor.WidthMeters > 0 ? floor.WidthMeters : 10m;
            decimal height = floor.HeightMeters > 0 ? floor.HeightMeters : 10m;

            _numPointAX.ValueChanged -= UpdateContainerPoints;
            _numPointAY.ValueChanged -= UpdateContainerPoints;
            _numPointBX.ValueChanged -= UpdateContainerPoints;
            _numPointBY.ValueChanged -= UpdateContainerPoints;

            _numPointAX.Minimum = 0;
            _numPointAY.Minimum = 0;
            _numPointBX.Minimum = 0;
            _numPointBY.Minimum = 0;
            _numPointAX.Maximum = width;
            _numPointAY.Maximum = height;
            _numPointBX.Maximum = width;
            _numPointBY.Maximum = height;

            _numPointAX.Value = Math.Min(width, Math.Max(0, (decimal)(_selectedWall.StartX * (float)width)));
            _numPointAY.Value = Math.Min(height, Math.Max(0, (decimal)(_selectedWall.StartY * (float)height)));
            _numPointBX.Value = Math.Min(width, Math.Max(0, (decimal)(_selectedWall.EndX * (float)width)));
            _numPointBY.Value = Math.Min(height, Math.Max(0, (decimal)(_selectedWall.EndY * (float)height)));

            _numPointAX.ValueChanged += UpdateContainerPoints;
            _numPointAY.ValueChanged += UpdateContainerPoints;
            _numPointBX.ValueChanged += UpdateContainerPoints;
            _numPointBY.ValueChanged += UpdateContainerPoints;
        }


        private void ApplyContainerNameRoomEdits()
        {
            if (_suppressContainerEvents) return;
            if (_selectedContainer == null) return;

            _selectedContainer.Name = _txtContainerName.Text?.Trim() ?? string.Empty;
            
            var newRoomTag = string.IsNullOrWhiteSpace(_txtRoomTag.Text)
                ? null
                : _txtRoomTag.Text.Trim();
            
            if (_selectedContainer.RoomTag != newRoomTag)
            {
                var root = _selectedContainer;
                var floor = _layout.Floors[_currentFloorIndex];
                
                while (root.ParentContainerId.HasValue)
                {
                    var parent = floor.Containers.FirstOrDefault(c => c.ContainerId == root.ParentContainerId.Value);
                    if (parent == null) break;
                    root = parent;
                }

                root.RoomTag = newRoomTag;
                PropagateRoomTagToSubcontainers(root, newRoomTag);
            }

            PopulateContainers(clearSelection: false);
            _canvasPanel.Invalidate();
        }

        private void SwitchToDesignView()
        {
            this.SuspendLayout(); 
            try 
            {
                _currentView = ViewMode.Design;
                _drawMode = DrawMode.None;
                _firstPoint = null;
                _selectedContainer = null;
                _selectedWall = null;
                _highlightedContainerId = null;
                
                Controls.Clear();
                
                if (_designView != null)
                {
                    _designView.Dispose();
                }
                _designView = new HomeBrowserDesignView();
                _designViewEventsWired = false;
                
                _designView.Dock = DockStyle.Fill;
                Controls.Add(_designView);
                MapDesignViewControls();
                
                _designView.PerformLayout();
                _canvasPanel.PerformLayout();
                
                SyncHouseSizeControls();
                WireEvents();

                SetupRightPanelDesignView();

                RefreshFloorUi();
                PopulateContainers(clearSelection: true);
                
                _canvasPanel.Invalidate();
            }
            finally
            {
                this.ResumeLayout(true); 
            }
        }

        private bool SelectContainerByPoint(PointF point)
        {
            var floor = _layout.Floors[_currentFloorIndex];

            var candidates = new List<ContainerModel>();
            const float tolerance = 0.001f;

            foreach (var container in floor.Containers)
            {
                if (container.ParentContainerId.HasValue)
                    continue;

                var minX = Math.Min(container.PointAX, container.PointBX);
                var maxX = Math.Max(container.PointAX, container.PointBX);
                var minY = Math.Min(container.PointAY, container.PointBY);
                var maxY = Math.Max(container.PointAY, container.PointBY);

                bool isInside = point.X >= (minX - tolerance) && point.X <= (maxX + tolerance) &&
                               point.Y >= (minY - tolerance) && point.Y <= (maxY + tolerance);

                if (isInside)
                {
                    candidates.Add(container);
                }
            }

            if (candidates.Count == 0) return false;

            // Sortuj po powierzchni (przybliżona szerokość * wysokość) aby wybrać najmniejszy kontener
            var bestMatch = candidates
                .OrderBy(c =>Math.Abs((c.PointBX - c.PointAX) * (c.PointBY - c.PointAY)))
                .ThenByDescending(c => c.ContainerId) 
                .First();

            if (!bestMatch.IsVisible)
            {
                bestMatch.IsVisible = true;
            }

            _selectedContainer = bestMatch;

            _suppressContainerEvents = true;
            try
            {
                _txtContainerName.Text = _selectedContainer.Name ?? string.Empty;
                _txtRoomTag.Text = _selectedContainer.RoomTag ?? string.Empty;
            }
            finally
            {
                _suppressContainerEvents = false;
            }

            if (_currentView == ViewMode.Design)
            {
                SetPointEditorsFromContainer();
            }

            PopulateContainers(clearSelection: false);
            RefreshItemsList();
            _canvasPanel.Invalidate();
            return true;
        }

        /// <summary>
        /// Rekursywnie ustawia RoomTag na wszystkich podkontenerach danego kontenera.
        /// </summary>
        private void PropagateRoomTagToSubcontainers(ContainerModel parent, string? roomTag)
        {
            if (!_layout.Floors.Any()) return;
            var floor = _layout.Floors[_currentFloorIndex];
            
            var subcontainers = floor.Containers
                .Where(c => c.ParentContainerId == parent.ContainerId && c.ContainerId != parent.ContainerId)
                .ToList();
            
            foreach (var sub in subcontainers)
            {
                sub.RoomTag = roomTag;
                PropagateRoomTagToSubcontainers(sub, roomTag);
            }
        }

        private void AddItemToContainer()
        {
            if (_selectedContainer == null) return;
            if (string.IsNullOrWhiteSpace(_txtItemName.Text)) return;

            var item = new ItemInventoryModel
            {
                ItemName = _txtItemName.Text.Trim(),
                Quantity = _numItemQty.Value,
                Unit = _cmbItemUnit.SelectedItem?.ToString() ?? "pcs",
                ContainerId = _selectedContainer.ContainerId
            };

            _selectedContainer.Items.Add(item);
            _txtItemName.Clear();
            _numItemQty.Value = 1;
            RefreshItemsList();
            RefreshSearchResults();
        }

        private void DeleteSelectedItems()
        {
            if (_selectedContainer == null) return;
            if (_lvItems.SelectedItems.Count == 0) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete {_lvItems.SelectedItems.Count} item(s)?",
                "Delete Items",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                var itemsToRemove = new List<ItemInventoryModel>();
                foreach (ListViewItem listItem in _lvItems.SelectedItems)
                {
                    if (listItem.Tag is ItemInventoryModel item)
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (var item in itemsToRemove)
                {
                    _selectedContainer.Items.Remove(item);
                }

                RefreshItemsList();
                RefreshSearchResults();
            }
        }
        private void RefreshItemsList()
        {
            _selectedItemInList = null;  
            _lvItems.Items.Clear();
            if (_selectedContainer == null) return;

            foreach (var item in _selectedContainer.Items)
            {
                var viewItem = new ListViewItem(item.ItemName)
                {
                    Tag = item
                };
                viewItem.SubItems.Add($"{item.Quantity} {item.Unit}");
                _lvItems.Items.Add(viewItem);
            }
        }
        private void SyncSelectedItemToEditors()
        {
            if (_currentView != ViewMode.Design)
                return;

            if (_lvItems.SelectedItems.Count == 0)
            {
                _selectedItemInList = null;
                return;
            }

            var listItem = _lvItems.SelectedItems[0];
            if (listItem.Tag is not ItemInventoryModel item)
            {
                _selectedItemInList = null;
                return;
            }

            _selectedItemInList = item;

            _numItemQty.ValueChanged -= NumItemQtyOnValueChanged;
            try
            {
                decimal qty = item.Quantity;
                if (qty < _numItemQty.Minimum) qty = _numItemQty.Minimum;
                if (qty > _numItemQty.Maximum) qty = _numItemQty.Maximum;
                _numItemQty.Value = qty;
            }
            finally
            {
                _numItemQty.ValueChanged += NumItemQtyOnValueChanged;
            }

            _suppressItemNameEvents = true;
            try
            {
                _txtItemName.Text = item.ItemName ?? string.Empty;
            }
            finally
            {
                _suppressItemNameEvents = false;
            }
            if (!string.IsNullOrEmpty(item.Unit))
            {
                int idx = _cmbItemUnit.Items.IndexOf(item.Unit);
                if (idx < 0)
                {
                    _cmbItemUnit.Items.Add(item.Unit);
                    idx = _cmbItemUnit.Items.IndexOf(item.Unit);
                }
                _cmbItemUnit.SelectedIndex = idx;
            }
        }
        private void NumItemQtyOnValueChanged(object? sender, EventArgs e)
        {
            if (_currentView != ViewMode.Design)
                return;

            if (_selectedContainer == null)
                return;

            if (_selectedItemInList == null)
                return;

            if (_lvItems.SelectedItems.Count == 0)
                return;

            _selectedItemInList.Quantity = _numItemQty.Value;

            RefreshItemsList();
            RefreshSearchResults();
        }
        private void ItemUnitChanged(object? sender, EventArgs e)
        {
            if (_currentView != ViewMode.Design)
                return;

            if (_selectedContainer == null)
                return;

            if (_selectedItemInList == null)
                return;

            if (_lvItems.SelectedItems.Count == 0)
                return;

            string unit = _cmbItemUnit.Text.Trim();

            _selectedItemInList.Unit = unit;

            // Zachowaj jednostkę na liście rozwijanej do przyszłego użycia
            if (!string.IsNullOrEmpty(unit) && !_cmbItemUnit.Items.Contains(unit))
            {
                _cmbItemUnit.Items.Add(unit);
            }

            RefreshItemsList();
            RefreshSearchResults();
        }

        private void SetupRightPanelDesignView()
        {
            _rightPanel.AutoScrollPosition = new Point(0, 0);
            
            int yPos = 12;
            int spacing = 8; 
            int fieldHeight = 40; 
            int buttonHeight = 32; 
            int panelWidth = 273;

            _lblContainerSettings.Visible = true;
            _lblContainerSettings.Dock = DockStyle.None;
            _lblContainerSettings.Location = new Point(12, yPos);
            _lblContainerSettings.Size = new Size(panelWidth, 25);
            _lblContainerSettings.Text = "Container settings";
            yPos += 25;

            _txtContainerName.Visible = true;
            _txtContainerName.Dock = DockStyle.None;
            _txtContainerName.Location = new Point(12, yPos);
            _txtContainerName.Size = new Size(panelWidth, fieldHeight);
            _txtContainerName.PlaceholderText = "Name";
            yPos += fieldHeight + spacing;

            _txtRoomTag.Visible = true;
            _txtRoomTag.Dock = DockStyle.None;
            _txtRoomTag.Location = new Point(12, yPos);
            _txtRoomTag.Size = new Size(panelWidth, fieldHeight);
            _txtRoomTag.PlaceholderText = "Room";
            yPos += fieldHeight + spacing;

            _lblPoints.Visible = true;
            _lblPoints.Location = new Point(12, yPos);
            yPos += 25;

            _lblPointA.Visible = true;
            _lblPointA.Location = new Point(12, yPos);
            _numPointAX.Visible = true;
            _numPointAX.Location = new Point(40, yPos - 2);
            _numPointAX.Size = new Size(100, 27);
            _numPointAY.Visible = true;
            _numPointAY.Location = new Point(150, yPos - 2);
            _numPointAY.Size = new Size(100, 27);
            yPos += 35;

            _lblPointB.Visible = true;
            _lblPointB.Location = new Point(12, yPos);
            _numPointBX.Visible = true;
            _numPointBX.Location = new Point(40, yPos - 2);
            _numPointBX.Size = new Size(100, 27);
            _numPointBY.Visible = true;
            _numPointBY.Location = new Point(150, yPos - 2);
            _numPointBY.Size = new Size(100, 27);
            yPos += 35;


            _btnAddSubcontainer.Visible = true;
            _btnAddSubcontainer.Location = new Point(12, yPos);
            _btnAddSubcontainer.Size = new Size(panelWidth, buttonHeight);
            _btnAddSubcontainer.FlatStyle = FlatStyle.Flat;
            _btnAddSubcontainer.FlatAppearance.BorderColor = Color.Black;
            _btnAddSubcontainer.FlatAppearance.BorderSize = 1;
            _btnAddSubcontainer.BackColor = Color.White;
            _btnAddSubcontainer.UseVisualStyleBackColor = false;
            yPos += buttonHeight + spacing;

            _btnDeleteContainer.Visible = true;
            _btnDeleteContainer.Location = new Point(12, yPos);
            _btnDeleteContainer.Size = new Size(panelWidth, buttonHeight);
            yPos += buttonHeight + spacing;

            _lblSubsections.Visible = true;
            _lblSubsections.Location = new Point(12, yPos);
            _lblSubsections.Text = "Subcontainers:";
            yPos += 25;
            _lstContainers.Visible = true;
            _lstContainers.Dock = DockStyle.None;
            _lstContainers.Location = new Point(12, yPos);
            _lstContainers.Size = new Size(panelWidth, 100);
            yPos += 100 + spacing * 2;
            _lblAddingItem.Visible = true;
            _lblAddingItem.Location = new Point(12, yPos);
            yPos += 25;

            _txtItemName.Visible = true;
            _txtItemName.Dock = DockStyle.None;
            _txtItemName.Location = new Point(12, yPos);
            _txtItemName.Size = new Size(panelWidth, fieldHeight);
            _txtItemName.PlaceholderText = "Item name";
            yPos += fieldHeight + spacing;

            _numItemQty.Visible = true;
            _numItemQty.Dock = DockStyle.None;
            _numItemQty.Location = new Point(12, yPos);
            _numItemQty.Size = new Size(panelWidth, fieldHeight);
            yPos += fieldHeight + spacing;

            _cmbItemUnit.Visible = true;
            _cmbItemUnit.Dock = DockStyle.None;
            _cmbItemUnit.Location = new Point(12, yPos);
            _cmbItemUnit.Size = new Size(panelWidth, fieldHeight);
            _cmbItemUnit.DropDownStyle = ComboBoxStyle.DropDown;
            yPos += fieldHeight + spacing;

            _btnAddItem.Visible = true;
            _btnAddItem.Dock = DockStyle.None;
            _btnAddItem.Location = new Point(12, yPos);
            _btnAddItem.Size = new Size(panelWidth, buttonHeight);
            _btnAddItem.Text = "Add item";
            yPos += buttonHeight + spacing;

            _lvItems.Visible = true;
            _lvItems.Dock = DockStyle.None;
            _lvItems.Location = new Point(12, yPos);
            _lvItems.Size = new Size(panelWidth, 120);
            _lvItems.FullRowSelect = true;
            _lvItems.GridLines = true;
            _lvItems.View = View.Details;
            _lvItems.Columns.Clear();
            _lvItems.Columns.Add("Item", 150);
            _lvItems.Columns.Add("Quantity", 100);
            yPos += 120 + spacing;

            _btnDeleteItems.Visible = true;
            _btnDeleteItems.Dock = DockStyle.None;
            _btnDeleteItems.Location = new Point(12, yPos);
            _btnDeleteItems.Size = new Size(panelWidth, buttonHeight);
            _btnDeleteItems.Text = "Delete items";
            yPos += buttonHeight + spacing;
        }

        private void ShowWallSettingsPanel()
        {
            if (_selectedWall == null) return;

            _lblContainerSettings.Text = "Wall settings";

            _txtContainerName.Visible = false;
            _txtRoomTag.Visible = false;
            int yPos = 45; 
            
            _lblPoints.Location = new Point(12, yPos);
            yPos += 25;

            _lblPointA.Location = new Point(12, yPos);
            _numPointAX.Location = new Point(40, yPos - 2);
            _numPointAY.Location = new Point(150, yPos - 2);
            yPos += 35;

            _lblPointB.Location = new Point(12, yPos);
            _numPointBX.Location = new Point(40, yPos - 2);
            _numPointBY.Location = new Point(150, yPos - 2);
            yPos += 35; 

            _btnDeleteContainer.Text = "Delete wall";
            _btnDeleteContainer.Visible = true;
            _btnDeleteContainer.Location = new Point(12, yPos);
            
            SetPointEditorsFromWall();

            _lblPoints.Text = "Points (start/end):";
            _btnAddSubcontainer.Visible = false;
            _lblSubsections.Visible = false;
            _lstContainers.Visible = false;

            // Ukryj sekcję przedmiotów
            _lblAddingItem.Visible = false;
            _txtItemName.Visible = false;
            _numItemQty.Visible = false;
            _cmbItemUnit.Visible = false;
            _btnAddItem.Visible = false;
            _lvItems.Visible = false;
            _btnDeleteItems.Visible = false;

            _canvasPanel.Invalidate();
        }

        private void ShowContainerSettingsPanel()
        {
            _lblContainerSettings.Text = "Container settings";

            _txtContainerName.Visible = true;
            _txtContainerName.Enabled = true;
            _txtRoomTag.Visible = true;
            _txtRoomTag.Enabled = true;
            
            
            _lblPoints.Location = new Point(12, 133);
            _lblPointA.Location = new Point(12, 158);
            _numPointAX.Location = new Point(40, 156);
            _numPointAY.Location = new Point(150, 156);
            
            _lblPointB.Location = new Point(12, 193);
            _numPointBX.Location = new Point(40, 191);
            _numPointBY.Location = new Point(150, 191);
            
            int yPos = 228;
            _btnAddSubcontainer.Location = new Point(12, yPos);
            yPos += 40; 
            
            _btnDeleteContainer.Text = "Delete container";
            _btnDeleteContainer.Visible = true;
            _btnDeleteContainer.Location = new Point(12, yPos); 
            
            _btnDeleteContainer.Location = new Point(12, 270); 

            _lblPoints.Text = "Points:";
            _btnAddSubcontainer.Visible = true;
            _lblSubsections.Visible = true;
            _lstContainers.Visible = true;

            _lblAddingItem.Visible = true;
            _txtItemName.Visible = true;
            _numItemQty.Visible = true;
            _cmbItemUnit.Visible = true;
            _btnAddItem.Visible = true;
            _lvItems.Visible = true;
            _btnDeleteItems.Visible = true;

            if (_selectedContainer == null)
            {
                _txtContainerName.Text = string.Empty;
                _txtRoomTag.Text = string.Empty;
                _numPointAX.Value = 0;
                _numPointAY.Value = 0;
                _numPointBX.Value = 0;
                _numPointBY.Value = 0;
            }
            else
            {
                _txtContainerName.Text = _selectedContainer.Name ?? string.Empty;
                _txtRoomTag.Text = _selectedContainer.RoomTag ?? string.Empty;
            }

            _canvasPanel.Invalidate();
        }

        private void AddSubcontainer()
        {
            if (_selectedContainer == null)
            {
                MessageBox.Show(
                    "Please select a container first.",
                    "No container selected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var floor = _layout.Floors[_currentFloorIndex];
            var parentContainer = _selectedContainer;
            EnsureContainerHasId(parentContainer);

            float parentMinX = Math.Min(parentContainer.PointAX, parentContainer.PointBX);
            float parentMaxX = Math.Max(parentContainer.PointAX, parentContainer.PointBX);
            float parentMinY = Math.Min(parentContainer.PointAY, parentContainer.PointBY);
            float parentMaxY = Math.Max(parentContainer.PointAY, parentContainer.PointBY);

            // Zrób podkontener nieco mniejszy od rodzica (80% rozmiaru, wyśrodkowany)
            float subWidth = (parentMaxX - parentMinX) * 0.8f;
            float subHeight = (parentMaxY - parentMinY) * 0.8f;
            float offsetX = (parentMaxX - parentMinX) * 0.1f;
            float offsetY = (parentMaxY - parentMinY) * 0.1f;

            var subcontainer = new ContainerModel
            {
                ContainerId = NextTempContainerId(),
                Name = $"Subcontainer {floor.Containers.Count + 1}",
                RoomTag = parentContainer.RoomTag,
                IsVisible = true,
                ParentContainerId = parentContainer.ContainerId,
                PointAX = parentMinX + offsetX,
                PointAY = parentMinY + offsetY,
                PointBX = parentMinX + offsetX + subWidth,
                PointBY = parentMinY + offsetY + subHeight
            };

            floor.Containers.Add(subcontainer);

            // Wybierz nowo utworzony podkontener
            _selectedContainer = subcontainer;
            _txtContainerName.Text = _selectedContainer.Name;
            _txtRoomTag.Text = _selectedContainer.RoomTag ?? string.Empty;

            // Użyj istniejącej metody aby poprawnie ustawić edytory punktów w metrach
            SetPointEditorsFromContainer();

            PopulateContainers(clearSelection: false);
            RefreshItemsList();
            _canvasPanel.Invalidate();
        }

        private void DeleteContainer()
        {
            if (_selectedWall != null)
            {
                var wallResult = MessageBox.Show(
                    "Are you sure you want to delete this wall?",
                    "Delete Wall",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (wallResult == DialogResult.Yes)
                {
                    var floor = _layout.Floors[_currentFloorIndex];
                    floor.Walls.Remove(_selectedWall);
                    _selectedWall = null;
                    ShowContainerSettingsPanel();
                    _canvasPanel.Invalidate();
                }
                return;
            }

            if (_selectedContainer == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete container \"{_selectedContainer.Name}\"?\n\nThis will permanently delete the container and all its items.",
                "Delete Container",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                var floor = _layout.Floors[_currentFloorIndex];
                floor.Containers.Remove(_selectedContainer);
                _selectedContainer = null;
                PopulateContainers();
                _canvasPanel.Invalidate();
            }
        }

        private void CanvasPanel_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.Clear(_canvasPanel.BackColor);

            if (!_layout.Floors.Any()) return;
            var floor = _layout.Floors[_currentFloorIndex];
            var bounds = GetCanvasBounds();

            // Rysuj siatkę - 1 komórka = 0.25 metra (4 komórki na metr)
            using var gridPen = new Pen(Color.FromArgb(220, 225, 230), 1);
            float widthMeters = (float)floor.WidthMeters;
            float heightMeters = (float)floor.HeightMeters;
            if (widthMeters <= 0) widthMeters = 10;
            if (heightMeters <= 0) heightMeters = 10;
            
            float pixelsPerMeterX = bounds.Width / widthMeters;
            float pixelsPerMeterY = bounds.Height / heightMeters;
            float gridStepX = pixelsPerMeterX * 0.25f; 
            float gridStepY = pixelsPerMeterY * 0.25f;
            
            if (gridStepX < 5) gridStepX = pixelsPerMeterX; 
            if (gridStepY < 5) gridStepY = pixelsPerMeterY;
            
            for (float x = bounds.Left; x <= bounds.Right; x += gridStepX)
            {
                e.Graphics.DrawLine(gridPen, x, bounds.Top, x, bounds.Bottom);
            }
            for (float y = bounds.Top; y <= bounds.Bottom; y += gridStepY)
            {
                e.Graphics.DrawLine(gridPen, bounds.Left, y, bounds.Right, y);
            }

            using var borderPen = new Pen(Color.Gray, 2);
            e.Graphics.DrawRectangle(borderPen, bounds);

            foreach (var wall in floor.Walls)
            {
                bool isSelectedWall = wall == _selectedWall;
                using var pen = new Pen(isSelectedWall ? Color.OrangeRed : Color.SteelBlue, isSelectedWall ? 5 : 3);
                e.Graphics.DrawLine(pen,
                    CanvasPoint(bounds, wall.StartX, wall.StartY),
                    CanvasPoint(bounds, wall.EndX, wall.EndY));
            }

            foreach (var container in floor.Containers)
            {
                bool isSubcontainer = container.ParentContainerId.HasValue;
                bool isSelected = container == _selectedContainer;
                bool isHighlightedFromSearch =
                    (_currentView == ViewMode.Search && container.ContainerId == _highlightedContainerId);

                // Podkontenery są ukryte chyba że:
                //  - zostały wybrane (widok projektu), LUB
                //  - są podświetlonym kontenerem z wyniku wyszukiwania (widok wyszukiwania)
                if (isSubcontainer && !isSelected && !isHighlightedFromSearch)
                {
                    continue;
                }

                if (!container.IsVisible) continue;
                var rect = ContainerRect(bounds, container);

                Color fillColor;
                if (container == _selectedContainer)
                {
                    fillColor = Color.FromArgb(100, 181, 246); 
                }
                else if (container.ContainerId == _highlightedContainerId)
                {
                    fillColor = Color.FromArgb(255, 204, 128); 
                }
                else
                {
                    fillColor = Color.FromArgb(135, 206, 250); 
                }

                using var brush = new SolidBrush(fillColor);
                Color borderColor = container == _selectedContainer ? Color.FromArgb(33, 150, 243) : Color.DodgerBlue;
                int borderWidth = container == _selectedContainer ? 4 : 1;
                using var pen = new Pen(borderColor, borderWidth);
                e.Graphics.FillRectangle(brush, rect);
                e.Graphics.DrawRectangle(pen, rect);
                using var textBrush = new SolidBrush(Color.Black);
                e.Graphics.DrawString(container.Name, Font, textBrush, rect.Location);
            }

            if (_selectedContainer != null && _currentView == ViewMode.Design)
            {
                var pointA = CanvasPoint(bounds, _selectedContainer.PointAX, _selectedContainer.PointAY);
                var pointB = CanvasPoint(bounds, _selectedContainer.PointBX, _selectedContainer.PointBY);

                using var linePen = new Pen(Color.FromArgb(76, 175, 80), 2);
                linePen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                e.Graphics.DrawLine(linePen, pointA, pointB);
                using var pointABrush = new SolidBrush(Color.FromArgb(244, 67, 54)); // Red
                using var pointAPen = new Pen(Color.DarkRed, 2);
                int pointSize = 8;
                e.Graphics.FillEllipse(pointABrush,
                    pointA.X - pointSize / 2,
                    pointA.Y - pointSize / 2,
                    pointSize,
                    pointSize);
                e.Graphics.DrawEllipse(pointAPen,
                    pointA.X - pointSize / 2,
                    pointA.Y - pointSize / 2,
                    pointSize,
                    pointSize);

                using var labelBrush = new SolidBrush(Color.DarkRed);
                using var labelFont = new Font(Font.FontFamily, 10, FontStyle.Bold);
                e.Graphics.DrawString("A", labelFont, labelBrush, pointA.X + pointSize / 2 + 2, pointA.Y - 8);

                using var pointBBrush = new SolidBrush(Color.FromArgb(33, 150, 243)); 
                using var pointBPen = new Pen(Color.DarkBlue, 2);
                e.Graphics.FillEllipse(pointBBrush,
                    pointB.X - pointSize / 2,
                    pointB.Y - pointSize / 2,
                    pointSize,
                    pointSize);
                e.Graphics.DrawEllipse(pointBPen,
                    pointB.X - pointSize / 2,
                    pointB.Y - pointSize / 2,
                    pointSize,
                    pointSize);

                using var labelBBrush = new SolidBrush(Color.DarkBlue);
                e.Graphics.DrawString("B", labelFont, labelBBrush, pointB.X + pointSize / 2 + 2, pointB.Y - 8);
            }
        }

        private Rectangle GetCanvasBounds()
        {
            const int margin = 30;
            // Maksymalna powierzchnia którą możemy użyć wewnątrz panelu
            var available = new Rectangle(
                margin,
                margin,
                _canvasPanel.Width - margin * 2,
                _canvasPanel.Height - margin * 2);

            if (!_layout.Floors.Any())
                return available;

            var floor = _layout.Floors[_currentFloorIndex];

            float wMeters = (float)floor.WidthMeters;
            float hMeters = (float)floor.HeightMeters;

            if (wMeters <= 0 || hMeters <= 0)
                return available;

            float houseAspect = wMeters / hMeters;
            float canvasAspect = available.Width / (float)available.Height;

            int width, height, x, y;

            if (houseAspect >= canvasAspect)
            {
                width = available.Width;
                height = (int)(width / houseAspect);
                x = available.Left;
                y = available.Top + (available.Height - height) / 2;
            }
            else
            {
                height = available.Height;
                width = (int)(height * houseAspect);
                x = available.Left + (available.Width - width) / 2;
                y = available.Top;
            }

            return new Rectangle(x, y, width, height);
        }

        private PointF CanvasPoint(Rectangle bounds, float normX, float normY)
        {
            return new PointF(
                bounds.Left + normX * bounds.Width,
                bounds.Top + normY * bounds.Height);
        }

        private Rectangle ContainerRect(Rectangle bounds, ContainerModel container)
        {
            var topLeft = CanvasPoint(bounds,
                Math.Min(container.PointAX, container.PointBX),
                Math.Min(container.PointAY, container.PointBY));
            var bottomRight = CanvasPoint(bounds,
                Math.Max(container.PointAX, container.PointBX),
                Math.Max(container.PointAY, container.PointBY));
            return Rectangle.FromLTRB(
                (int)topLeft.X,
                (int)topLeft.Y,
                (int)bottomRight.X,
                (int)bottomRight.Y);
        }

        private void CanvasPanel_MouseClick(object? sender, MouseEventArgs e)
        {
            if (_currentView != ViewMode.Design) return;

            if (!_layout.Floors.Any()) return;
            var bounds = GetCanvasBounds();
            if (!bounds.Contains(e.Location))
            {
                _drawMode = DrawMode.None;
                Cursor = Cursors.Default;
                return;
            }

            var normPoint = new PointF(
                (e.X - bounds.Left) / (float)bounds.Width,
                (e.Y - bounds.Top) / (float)bounds.Height);

            if (_drawMode == DrawMode.None)
            {
                var clickedWall = FindWallAtPoint(normPoint);
                if (clickedWall != null)
                {
                    // Wybierz ścianę i pokaż panel ustawień ściany
                    _selectedWall = clickedWall;
                    _selectedContainer = null;
                    ShowWallSettingsPanel();
                    _canvasPanel.Invalidate();
                    return;
                }

                if (_selectedWall != null)
                {
                    _selectedWall = null;
                    ShowContainerSettingsPanel();
                }

                bool containerFound = SelectContainerByPoint(normPoint);
                return;
            }

            if (_firstPoint == null)
            {
                _firstPoint = normPoint;
                return;
            }

            var floor = _layout.Floors[_currentFloorIndex];

            switch (_drawMode)
            {
                case DrawMode.Wall:
                    floor.Walls.Add(new WallModel
                    {
                        StartX = _firstPoint.Value.X,
                        StartY = _firstPoint.Value.Y,
                        EndX = normPoint.X,
                        EndY = normPoint.Y
                    });
                    break;
                case DrawMode.Container:
                case DrawMode.Obstacle:
                    var name = _drawMode == DrawMode.Obstacle ? "Obstacle" : $"Container {floor.Containers.Count + 1}";
                    var container = new ContainerModel
                    {
                        ContainerId = NextTempContainerId(),
                        Name = name,
                        RoomTag = _drawMode == DrawMode.Obstacle ? "Obstacle" : null,
                        IsVisible = _drawMode != DrawMode.Obstacle,
                        PointAX = _firstPoint.Value.X,
                        PointAY = _firstPoint.Value.Y,
                        PointBX = normPoint.X,
                        PointBY = normPoint.Y
                    };
                    floor.Containers.Add(container);

                    if (_currentView == ViewMode.Design)
                    {
                        _selectedContainer = container;

                        _txtContainerName.Text = _selectedContainer.Name ?? string.Empty;
                        _txtRoomTag.Text = _selectedContainer.RoomTag ?? string.Empty;

                        SetPointEditorsFromContainer();


                        UpdatePointsDescription();
                        RefreshItemsList();
                        _canvasPanel.Invalidate();
                    }
                    break;
            }

            _firstPoint = null;
            SetDrawMode(DrawMode.None);
            PopulateContainers(clearSelection: false);
            _canvasPanel.Invalidate();
        }

        private WallModel? FindWallAtPoint(PointF point)
        {
            if (!_layout.Floors.Any()) return null;

            var floor = _layout.Floors[_currentFloorIndex];
            const float tolerance = 0.02f; 

            foreach (var wall in floor.Walls)
            {
                float distance = DistanceToLineSegment(point, 
                    new PointF(wall.StartX, wall.StartY), 
                    new PointF(wall.EndX, wall.EndY));

                if (distance <= tolerance)
                {
                    return wall;
                }
            }

            return null;
        }

        private float DistanceToLineSegment(PointF point, PointF lineStart, PointF lineEnd)
        {
            float dx = lineEnd.X - lineStart.X;
            float dy = lineEnd.Y - lineStart.Y;

            if (dx == 0 && dy == 0)
            {
                return (float)Math.Sqrt(
                    Math.Pow(point.X - lineStart.X, 2) + 
                    Math.Pow(point.Y - lineStart.Y, 2));
            }

            float t = Math.Max(0, Math.Min(1, 
                ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / 
                (dx * dx + dy * dy)));

            float closestX = lineStart.X + t * dx;
            float closestY = lineStart.Y + t * dy;

            return (float)Math.Sqrt(
                Math.Pow(point.X - closestX, 2) + 
                Math.Pow(point.Y - closestY, 2));
        }

        private void UpdateHouseSize()
        {
            _layout.DefaultWidthMeters = _numWidth.Value;
            _layout.DefaultHeightMeters = _numHeight.Value;
            foreach (var floor in _layout.Floors)
            {
                floor.WidthMeters = _layout.DefaultWidthMeters;
                floor.HeightMeters = _layout.DefaultHeightMeters;
            }

            if (_currentView == ViewMode.Design)
            {
                if (_selectedContainer != null)
                {
                    SetPointEditorsFromContainer();
                }
                else if (_selectedWall != null)
                {
                    SetPointEditorsFromWall();
                }
            }

            UpdatePointsDescription();
            _canvasPanel.Invalidate();
        }



        private class ListItem<T>
        {
            public string Text { get; }
            public T Value { get; }

            public ListItem(string text, T value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString() => Text;
        }

        #endregion
    }
}
