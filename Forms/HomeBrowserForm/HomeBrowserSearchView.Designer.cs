using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class HomeBrowserSearchView
    {
        private TextBox _txtSearch;
        private ListView _lvResults;
        private Label _lblResultInfo;
        private Panel _canvasPanel;
        private Label _lblFloor;
        private Button _btnFloorUp;
        private Button _btnFloorDown;
        private Button _btnEdit;
        private TreeView _containerExplorer;
        private Label _lblLocationInfo;
        private Panel _leftControlsPanel;
        private TextBox _txtFloorInput1;
        private TextBox _txtFloorInput2;
        private Panel _leftPanel;
        private Panel _middlePanel;
        private Panel _rightPanel;
        private Panel _buttonsPanel;
        private Panel _locationPanel;

        private void InitializeComponent()
        {
            _txtSearch = new TextBox();
            _lvResults = new ListView();
            _lblResultInfo = new Label();
            _canvasPanel = new Panel();
            _lblFloor = new Label();
            _btnFloorUp = new Button();
            _btnFloorDown = new Button();
            _btnEdit = new Button();
            _containerExplorer = new TreeView();
            _lblLocationInfo = new Label();
            _leftControlsPanel = new Panel();
            _txtFloorInput1 = new TextBox();
            _txtFloorInput2 = new TextBox();
            _leftPanel = new Panel();
            _middlePanel = new Panel();
            _rightPanel = new Panel();
            _buttonsPanel = new Panel();
            _locationPanel = new Panel();
            var _lblSearchHeader = new Label();
            var _lblExplorerHeader = new Label();
            SuspendLayout();
            // 
            // _leftPanel
            // 
            _leftPanel.Dock = DockStyle.Left;
            _leftPanel.Width = 280;
            _leftPanel.Padding = new Padding(15, 10, 15, 10);
            _leftPanel.BackColor = Color.White;
            // 
            // _lblSearchHeader
            // 
            _lblSearchHeader.Dock = DockStyle.Top;
            _lblSearchHeader.Height = 35;
            _lblSearchHeader.Text = "🔍  Search Items";
            _lblSearchHeader.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            _lblSearchHeader.ForeColor = Color.FromArgb(44, 62, 80);
            _lblSearchHeader.TextAlign = ContentAlignment.MiddleLeft;
            _lblSearchHeader.Padding = new Padding(0, 5, 0, 5);
            // 
            // _txtSearch
            // 
            _txtSearch.Dock = DockStyle.Top;
            _txtSearch.Margin = new Padding(0, 0, 0, 8);
            _txtSearch.Height = 32;
            _txtSearch.Font = new Font("Segoe UI", 10F);
            _txtSearch.PlaceholderText = "Type to search...";
            // 
            // _lvResults
            // 
            _lvResults.Dock = DockStyle.Fill;
            _lvResults.FullRowSelect = true;
            _lvResults.GridLines = true;
            _lvResults.View = View.Details;
            _lvResults.Font = new Font("Segoe UI", 9F);
            _lvResults.Columns.Add("Item", 110);
            _lvResults.Columns.Add("Qty", 50);
            _lvResults.Columns.Add("Container", 90);
            // 
            // _lblResultInfo
            // 
            _lblResultInfo.Dock = DockStyle.Bottom;
            _lblResultInfo.Height = 22;
            _lblResultInfo.TextAlign = ContentAlignment.MiddleLeft;
            _lblResultInfo.Font = new Font("Segoe UI", 9F);
            _lblResultInfo.ForeColor = Color.FromArgb(100, 100, 100);
            _lblResultInfo.Text = "";
            _lblResultInfo.Visible = false;
            // 
            // _leftPanel.Controls
            //
            _leftPanel.Controls.Add(_lvResults);
            _leftPanel.Controls.Add(_txtSearch);
            _leftPanel.Controls.Add(_lblSearchHeader);
            _leftPanel.Controls.Add(_lblResultInfo);
            // 
            // _middlePanel
            // 
            _middlePanel.Dock = DockStyle.Left;
            _middlePanel.Width = 320;
            _middlePanel.Padding = new Padding(15, 10, 15, 10);
            _middlePanel.BackColor = Color.FromArgb(250, 251, 252);
            // 
            // _lblExplorerHeader
            // 
            _lblExplorerHeader.Dock = DockStyle.Top;
            _lblExplorerHeader.Height = 35;
            _lblExplorerHeader.Text = "🏠  Container Explorer";
            _lblExplorerHeader.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            _lblExplorerHeader.ForeColor = Color.FromArgb(44, 62, 80);
            _lblExplorerHeader.TextAlign = ContentAlignment.MiddleLeft;
            _lblExplorerHeader.Padding = new Padding(0, 5, 0, 5);
            // 
            // _lblFloor
            // 
            _lblFloor.Dock = DockStyle.Top;
            _lblFloor.Height = 28;
            _lblFloor.TextAlign = ContentAlignment.MiddleCenter;
            _lblFloor.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            _lblFloor.ForeColor = Color.FromArgb(52, 152, 219);
            _lblFloor.Text = string.Empty;
            // 
            // _containerExplorer
            // 
            _containerExplorer.Dock = DockStyle.Fill;
            _containerExplorer.Font = new Font("Segoe UI", 9F);
            _containerExplorer.BorderStyle = BorderStyle.FixedSingle;
            // 
            // _middlePanel.Controls
            //
            _middlePanel.Controls.Add(_containerExplorer);
            _middlePanel.Controls.Add(_lblFloor);
            _middlePanel.Controls.Add(_lblExplorerHeader);
            // 
            // _rightPanel
            // 
            _rightPanel.Dock = DockStyle.Fill;
            _rightPanel.Padding = new Padding(15, 10, 15, 10);
            _rightPanel.BackColor = Color.White;
            // 
            // _canvasPanel
            // 
            _canvasPanel.Dock = DockStyle.Fill;
            _canvasPanel.BackColor = Color.FromArgb(252, 253, 255);
            _canvasPanel.BorderStyle = BorderStyle.FixedSingle;
            _rightPanel.Controls.Add(_canvasPanel);
            // 
            // _buttonsPanel
            // 
            _buttonsPanel.Dock = DockStyle.Bottom;
            _buttonsPanel.Height = 60;
            _buttonsPanel.Padding = new Padding(15, 10, 15, 10);
            _buttonsPanel.BackColor = Color.FromArgb(248, 250, 252);
            // 
            // _btnFloorUp
            // 
            _btnFloorUp.Text = "▲ Up";
            _btnFloorUp.Size = new Size(120, 40);
            _btnFloorUp.Location = new Point(15, 10);
            _btnFloorUp.BackColor = Color.White;
            _btnFloorUp.Font = new Font("Segoe UI", 11F);
            _btnFloorUp.ForeColor = Color.FromArgb(44, 62, 80);
            _btnFloorUp.UseVisualStyleBackColor = false;
            _btnFloorUp.Cursor = Cursors.Hand;
            // 
            // _btnFloorDown
            // 
            _btnFloorDown.Text = "▼ Down";
            _btnFloorDown.Size = new Size(120, 40);
            _btnFloorDown.Location = new Point(145, 10);
            _btnFloorDown.BackColor = Color.White;
            _btnFloorDown.Font = new Font("Segoe UI", 11F);
            _btnFloorDown.ForeColor = Color.FromArgb(44, 62, 80);
            _btnFloorDown.UseVisualStyleBackColor = false;
            _btnFloorDown.Cursor = Cursors.Hand;
            // 
            // _btnEdit
            // 
            _btnEdit.Text = "Edit Layout";
            _btnEdit.Size = new Size(120, 40);
            _btnEdit.Location = new Point(275, 10);
            _btnEdit.BackColor = Color.White;
            _btnEdit.Font = new Font("Segoe UI", 11F);
            _btnEdit.ForeColor = Color.FromArgb(44, 62, 80);
            _btnEdit.UseVisualStyleBackColor = false;
            _btnEdit.Cursor = Cursors.Hand;
            // 
            // buttonsPanel.Controls
            //
            _buttonsPanel.Controls.Add(_btnFloorUp);
            _buttonsPanel.Controls.Add(_btnFloorDown);
            _buttonsPanel.Controls.Add(_btnEdit);
            // 
            // _lblLocationInfo
            // 
            _lblLocationInfo.Dock = DockStyle.Fill;
            _lblLocationInfo.Text = "Select an item from list to show location";
            _lblLocationInfo.TextAlign = ContentAlignment.MiddleLeft;
            _lblLocationInfo.Padding = new Padding(15, 0, 0, 0);
            _lblLocationInfo.Font = new Font("Segoe UI", 9F);
            _lblLocationInfo.ForeColor = Color.FromArgb(30, 80, 120);
            _lblLocationInfo.BackColor = Color.FromArgb(200, 230, 245);
            // 
            // _leftControlsPanel
            // 
            _leftControlsPanel.Visible = false;
            _txtFloorInput1.Visible = false;
            _txtFloorInput2.Visible = false;
            // 
            // HomeBrowserSearchView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(248, 250, 252);
            Padding = new Padding(10, 15, 10, 10);
            _locationPanel.Dock = DockStyle.Bottom;
            _locationPanel.Height = 40;
            _locationPanel.Padding = new Padding(0, 0, 0, 0);
            _locationPanel.BackColor = Color.FromArgb(200, 230, 245);
            _locationPanel.Controls.Add(_lblLocationInfo);
            Controls.Add(_rightPanel);
            Controls.Add(_middlePanel);
            Controls.Add(_leftPanel);
            Controls.Add(_locationPanel);
            Controls.Add(_buttonsPanel);
            Name = "HomeBrowserSearchView";
            Size = new Size(1260, 720);
            ResumeLayout(false);
        }
    }
}

