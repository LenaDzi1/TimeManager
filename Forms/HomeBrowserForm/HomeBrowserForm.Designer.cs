using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class HomeBrowserForm
    {
        private TextBox _txtSearch;
        private ListView _lvResults;
        private Label _lblResultInfo;
        private Panel _canvasPanel;
        private Label _lblFloor;
        private Button _btnFloorUp;
        private Button _btnFloorDown;
        private Button _btnAddFloor;
        private Button _btnDeleteFloor;
        private Button _btnAddWall;
        private Button _btnAddContainer;
        private Button _btnSave;
        private NumericUpDown _numWidth;
        private NumericUpDown _numHeight;
        private Label _lblSize;
        private Panel _rightPanel;
        private ListBox _lstContainers;
        private TextBox _txtContainerName;
        private TextBox _txtRoomTag;
        private TextBox _txtItemName;
        private NumericUpDown _numItemQty;
        private ComboBox _cmbItemUnit;
        private Button _btnAddItem;
        private Button _btnDeleteItems;
        private ListView _lvItems;
        private Label _lblContainerSettings;
        private Label _lblAddingItem;
        private Label _lblPoints;
        private NumericUpDown _numPointAX;
        private NumericUpDown _numPointAY;
        private NumericUpDown _numPointBX;
        private NumericUpDown _numPointBY;
        private Label _lblPointA;
        private Label _lblPointB;
        private Button _btnAddSubcontainer;
        private Button _btnDeleteContainer;
        private Label _lblSubsections;
        private Button _btnEdit;
        private TreeView _containerExplorer;
        private Label _lblLocationInfo;
        private Panel _leftControlsPanel;
        private TextBox _txtFloorInput1;
        private TextBox _txtFloorInput2;
        private TextBox _txtFloorName;

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // HomeBrowserForm
            // 
            // Uwaga: Wszystkie kontrolki są tworzone przez subwidoki (HomeBrowserSearchView/HomeBrowserDesignView)
            // i mapowane przez MapSearchViewControls()/MapDesignViewControls() podczas runtime.
            // Ten InitializeComponent tylko konfiguruje podstawowe właściwości UserControl.
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            Name = "HomeBrowserForm";
            Size = new Size(1400, 820);
            ResumeLayout(false);
        }
    }
}
