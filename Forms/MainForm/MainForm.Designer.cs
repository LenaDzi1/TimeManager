using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class MainForm
     {
        private Panel _mainPanel;
		private Panel _topBar;
		private Button _btnToggleMenu;
		private Button _btnTopCalendar;
		private Button _btnFridgeKitchen;
		private Button _btnStatistics;
		private Button _btnNotifications;
		private Button _btnAccount;
		private Panel _notificationDot;
		private Label _lblViewAs;
		private ComboBox _cmbViewAs;

        private void InitializeComponent()
        {
            _mainPanel = new Panel();
            _topBar = new Panel();
            _btnToggleMenu = new Button();
            _btnTopCalendar = new Button();
            _btnFridgeKitchen = new Button();
            _btnStatistics = new Button();
            _lblViewAs = new Label();
            _cmbViewAs = new ComboBox();
            _btnNotifications = new Button();
            _notificationDot = new Panel();
            _btnAccount = new Button();
            _topBar.SuspendLayout();
            SuspendLayout();
            // 
            // _mainPanel
            // 
            _mainPanel.BackColor = Color.FromArgb(245, 247, 250);
            _mainPanel.Dock = DockStyle.Fill;
            _mainPanel.Location = new Point(0, 56);
            _mainPanel.Name = "_mainPanel";
            _mainPanel.Padding = new Padding(20);
            _mainPanel.Size = new Size(1374, 673);
            _mainPanel.TabIndex = 2;
            // 
            // _topBar
            // 
            _topBar.BackColor = Color.FromArgb(135, 206, 250);
            _topBar.Controls.Add(_btnToggleMenu);
            _topBar.Controls.Add(_btnTopCalendar);
            _topBar.Controls.Add(_btnFridgeKitchen);
            _topBar.Controls.Add(_btnStatistics);
            _topBar.Controls.Add(_lblViewAs);
            _topBar.Controls.Add(_cmbViewAs);
            _topBar.Controls.Add(_btnNotifications);
            _btnNotifications.Controls.Add(_notificationDot);
            _topBar.Controls.Add(_btnAccount);
            _topBar.Dock = DockStyle.Top;
            _topBar.Location = new Point(0, 0);
            _topBar.Name = "_topBar";
            _topBar.Padding = new Padding(12, 8, 12, 8);
            _topBar.Size = new Size(1374, 56);
            _topBar.TabIndex = 1;
            // 
            // _btnToggleMenu
            // 
            _btnToggleMenu.Location = new Point(12, 10);
            _btnToggleMenu.Name = "_btnToggleMenu";
            _btnToggleMenu.Size = new Size(110, 36);
            _btnToggleMenu.TabIndex = 0;
            // 
            // _btnTopCalendar
            // 
            _btnTopCalendar.Location = new Point(130, 10);
            _btnTopCalendar.Name = "_btnTopCalendar";
            _btnTopCalendar.Size = new Size(90, 36);
            _btnTopCalendar.TabIndex = 1;
            // 
            // _btnFridgeKitchen
            // 
            _btnFridgeKitchen.Location = new Point(228, 10);
            _btnFridgeKitchen.Name = "_btnFridgeKitchen";
            _btnFridgeKitchen.Size = new Size(90, 36);
            _btnFridgeKitchen.TabIndex = 2;
            // 
            // _btnStatistics
            // 
            _btnStatistics.Location = new Point(326, 10);
            _btnStatistics.Name = "_btnStatistics";
            _btnStatistics.Size = new Size(90, 36);
            _btnStatistics.TabIndex = 4;
            // 
            // _lblViewAs
            // 
            _lblViewAs.AutoSize = true;
            _lblViewAs.Font = new Font("Segoe UI", 9F);
            _lblViewAs.ForeColor = Color.FromArgb(44, 62, 80);
            _lblViewAs.Location = new Point(433, 5);
            _lblViewAs.Name = "_lblViewAs";
            _lblViewAs.Size = new Size(49, 15);
            _lblViewAs.TabIndex = 5;
            _lblViewAs.Text = "View as:";
            _lblViewAs.Visible = false;
            // 
            // _cmbViewAs
            // 
            _cmbViewAs.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbViewAs.Font = new Font("Segoe UI", 9F);
            _cmbViewAs.Location = new Point(433, 23);
            _cmbViewAs.Name = "_cmbViewAs";
            _cmbViewAs.Size = new Size(180, 23);
            _cmbViewAs.TabIndex = 7;
            _cmbViewAs.Visible = false;
            // 
            // _btnNotifications
            // 
            _btnNotifications.Location = new Point(1208, 10);
            _btnNotifications.Name = "_btnNotifications";
            _btnNotifications.Size = new Size(44, 36);
            _btnNotifications.TabIndex = 5;
            // 
            // _notificationDot
            // 
            _notificationDot.BackColor = Color.Red;
            _notificationDot.Location = new Point(32, 2);
            _notificationDot.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _notificationDot.Name = "_notificationDot";
            _notificationDot.Size = new Size(10, 10);
            _notificationDot.TabIndex = 8;
            _notificationDot.Visible = false;
            // 
            // _btnAccount
            // 
            _btnAccount.Location = new Point(1262, 10);
            _btnAccount.Name = "_btnAccount";
            _btnAccount.Size = new Size(44, 36);
            _btnAccount.TabIndex = 6;
            // 
            // MainForm
            // 
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1374, 729);
            Controls.Add(_mainPanel);
            Controls.Add(_topBar);
            MinimumSize = new Size(1250, 700);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Time Manager - Smart Calendar & Organizer";
            _topBar.ResumeLayout(false);
            _topBar.PerformLayout();
            ResumeLayout(false);
        }
    }
}
