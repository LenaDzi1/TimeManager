using System.Windows.Forms;

namespace TimeManager.Forms
{
    /// <summary>
    /// Widok projektowania w HomeBrowser – statyczny layout w Designerze.
    /// Host (HomeBrowserForm) podłącza logikę i dane.
    /// </summary>
    public partial class HomeBrowserDesignView : UserControl
    {
        public Panel CanvasPanel => _canvasPanel;
        public Label SizeLabel => _lblSize;
        public NumericUpDown WidthInput => _numWidth;
        public NumericUpDown HeightInput => _numHeight;
        public Button FloorUpButton => _btnFloorUp;
        public Button FloorDownButton => _btnFloorDown;
        public Button AddFloorButton => _btnAddFloor;
        public Button DeleteFloorButton => _btnDeleteFloor;
        public Button AddWallButton => _btnAddWall;
        public Button AddContainerButton => _btnAddContainer;
        public Button SaveButton => _btnSave;
        public TextBox FloorNameTextBox => _txtFloorName;
        public Panel RightPanel => _rightPanel;

        // Eksponujemy kontrolki edytora (prawy panel)
        public ListBox ContainersList => _lstContainers;
        public Label ContainerSettingsLabel => _lblContainerSettings;
        public TextBox ContainerNameTextBox => _txtContainerName;
        public TextBox RoomTagTextBox => _txtRoomTag;
        public TextBox ItemNameTextBox => _txtItemName;
        public NumericUpDown ItemQtyNumeric => _numItemQty;
        public ComboBox ItemUnitCombo => _cmbItemUnit;
        public Button AddItemButton => _btnAddItem;
        public Button DeleteItemsButton => _btnDeleteItems;
        public ListView ItemsListView => _lvItems;
        public Label AddingItemLabel => _lblAddingItem;
        public Label PointsLabel => _lblPoints;
        public Label PointALabel => _lblPointA;
        public NumericUpDown PointAXNumeric => _numPointAX;
        public NumericUpDown PointAYNumeric => _numPointAY;
        public Label PointBLabel => _lblPointB;
        public NumericUpDown PointBXNumeric => _numPointBX;
        public NumericUpDown PointBYNumeric => _numPointBY;
        public Button AddSubcontainerButton => _btnAddSubcontainer;
        public Button DeleteContainerButton => _btnDeleteContainer;
        public Label SubsectionsLabel => _lblSubsections;

        public HomeBrowserDesignView()
        {
            InitializeComponent();
        }
    }
}

