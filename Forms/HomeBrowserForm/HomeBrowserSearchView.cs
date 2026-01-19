using System.Windows.Forms;

namespace TimeManager.Forms
{
    public partial class HomeBrowserSearchView : UserControl
    {
        public TextBox SearchTextBox => _txtSearch;
        public ListView ResultsListView => _lvResults;
        public Label ResultInfoLabel => _lblResultInfo;
        public Panel CanvasPanel => _canvasPanel;
        public Label FloorLabel => _lblFloor;
        public Button FloorUpButton => _btnFloorUp;
        public Button FloorDownButton => _btnFloorDown;
        public Button EditButton => _btnEdit;
        public TreeView ContainerExplorer => _containerExplorer;
        public Label LocationInfoLabel => _lblLocationInfo;
        public Panel LeftControlsPanel => _leftControlsPanel;
        public TextBox FloorInput1 => _txtFloorInput1;
        public TextBox FloorInput2 => _txtFloorInput2;

        public HomeBrowserSearchView()
        {
            InitializeComponent();
        }
    }
}









