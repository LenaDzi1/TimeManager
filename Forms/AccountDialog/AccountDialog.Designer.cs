using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TimeManager.Controls;

namespace TimeManager.Forms
{
    partial class AccountDialog
    {
        private IContainer components = null;
        
        private TableLayoutPanel _container;
        private FlowLayoutPanel _leftPanel;
        private FlowLayoutPanel _rightPanel;
        private Label _lblGreeting;
        private Label _lblRole;
        private Label _lblChangePassword;
        private Label _lblNewPassword;
        private TextBox _txtPassword;
        private Label _lblRepeatPassword;
        private TextBox _txtPasswordRepeat;
        private Button _btnSave;
        private Button _btnLogout;
        private Label _lblSleepTitle;
        private SleepCircleControl _sleepCircle;
        private Label _lblSleepSummary;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            _container = new TableLayoutPanel();
            _leftPanel = new FlowLayoutPanel();
            _lblGreeting = new Label();
            _lblRole = new Label();
            _lblChangePassword = new Label();
            _lblNewPassword = new Label();
            _txtPassword = new TextBox();
            _lblRepeatPassword = new Label();
            _txtPasswordRepeat = new TextBox();
            _btnSave = new Button();
            _btnLogout = new Button();
            _rightPanel = new FlowLayoutPanel();
            _lblSleepTitle = new Label();
            _sleepCircle = new SleepCircleControl();
            _lblSleepSummary = new Label();
            _container.SuspendLayout();
            _leftPanel.SuspendLayout();
            _rightPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _container
            // 
            _container.ColumnCount = 2;
            _container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            _container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            _container.Controls.Add(_leftPanel, 0, 0);
            _container.Controls.Add(_rightPanel, 1, 0);
            _container.Location = new Point(0, 0);
            _container.Name = "_container";
            _container.RowCount = 1;
            _container.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _container.Size = new Size(720, 374);
            _container.TabIndex = 0;
            // 
            // _leftPanel
            // 
            _leftPanel.BackColor = Color.FromArgb(248, 250, 252);
            _leftPanel.Controls.Add(_lblGreeting);
            _leftPanel.Controls.Add(_lblRole);
            _leftPanel.Controls.Add(_lblChangePassword);
            _leftPanel.Controls.Add(_lblNewPassword);
            _leftPanel.Controls.Add(_txtPassword);
            _leftPanel.Controls.Add(_lblRepeatPassword);
            _leftPanel.Controls.Add(_txtPasswordRepeat);
            _leftPanel.Controls.Add(_btnSave);
            _leftPanel.Controls.Add(_btnLogout);
            _leftPanel.Dock = DockStyle.Fill;
            _leftPanel.FlowDirection = FlowDirection.TopDown;
            _leftPanel.Location = new Point(3, 3);
            _leftPanel.Name = "_leftPanel";
            _leftPanel.Padding = new Padding(20);
            _leftPanel.Size = new Size(318, 368);
            _leftPanel.TabIndex = 0;
            // 
            // _lblGreeting
            // 
            _lblGreeting.AutoSize = true;
            _lblGreeting.Dock = DockStyle.Top;
            _lblGreeting.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            _lblGreeting.ForeColor = Color.FromArgb(44, 62, 80);
            _lblGreeting.Location = new Point(23, 20);
            _lblGreeting.Name = "_lblGreeting";
            _lblGreeting.Padding = new Padding(0, 0, 0, 10);
            _lblGreeting.Size = new Size(260, 47);
            _lblGreeting.TabIndex = 0;
            _lblGreeting.Text = "Hi!";
            // 
            // _lblRole
            // 
            _lblRole.AutoSize = true;
            _lblRole.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            _lblRole.ForeColor = Color.Gray;
            _lblRole.Location = new Point(23, 67);
            _lblRole.Name = "_lblRole";
            _lblRole.Padding = new Padding(0, 0, 0, 10);
            _lblRole.Size = new Size(29, 25);
            _lblRole.TabIndex = 1;
            _lblRole.Text = "Role";
            // 
            // _lblChangePassword
            // 
            _lblChangePassword.AutoSize = true;
            _lblChangePassword.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            _lblChangePassword.ForeColor = Color.FromArgb(52, 73, 94);
            _lblChangePassword.Location = new Point(23, 92);
            _lblChangePassword.Name = "_lblChangePassword";
            _lblChangePassword.Padding = new Padding(0, 10, 0, 0);
            _lblChangePassword.Size = new Size(132, 30);
            _lblChangePassword.TabIndex = 2;
            _lblChangePassword.Text = "Change password";
            // 
            // _lblNewPassword
            // 
            _lblNewPassword.AutoSize = true;
            _lblNewPassword.Location = new Point(23, 122);
            _lblNewPassword.Name = "_lblNewPassword";
            _lblNewPassword.Padding = new Padding(0, 6, 0, 0);
            _lblNewPassword.Size = new Size(84, 21);
            _lblNewPassword.TabIndex = 3;
            _lblNewPassword.Text = "New password";
            // 
            // _txtPassword
            // 
            _txtPassword.Location = new Point(23, 146);
            _txtPassword.Name = "_txtPassword";
            _txtPassword.Size = new Size(260, 23);
            _txtPassword.TabIndex = 4;
            _txtPassword.UseSystemPasswordChar = true;
            // 
            // _lblRepeatPassword
            // 
            _lblRepeatPassword.AutoSize = true;
            _lblRepeatPassword.Location = new Point(23, 172);
            _lblRepeatPassword.Name = "_lblRepeatPassword";
            _lblRepeatPassword.Padding = new Padding(0, 6, 0, 0);
            _lblRepeatPassword.Size = new Size(96, 21);
            _lblRepeatPassword.TabIndex = 5;
            _lblRepeatPassword.Text = "Repeat password";
            // 
            // _txtPasswordRepeat
            // 
            _txtPasswordRepeat.Location = new Point(23, 196);
            _txtPasswordRepeat.Name = "_txtPasswordRepeat";
            _txtPasswordRepeat.Size = new Size(260, 23);
            _txtPasswordRepeat.TabIndex = 6;
            _txtPasswordRepeat.UseSystemPasswordChar = true;
            // 
            // _btnSave
            // 
            _btnSave.BackColor = Color.FromArgb(232, 245, 233);
            _btnSave.Cursor = Cursors.Hand;
            _btnSave.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnSave.FlatAppearance.BorderSize = 2;
            _btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnSave.FlatStyle = FlatStyle.Flat;
            _btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnSave.ForeColor = Color.FromArgb(39, 174, 96);
            _btnSave.Location = new Point(23, 237);
            _btnSave.Margin = new Padding(3, 15, 3, 3);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(180, 38);
            _btnSave.TabIndex = 7;
            _btnSave.Text = "Save changes";
            _btnSave.UseVisualStyleBackColor = false;
            // 
            // _btnLogout
            // 
            _btnLogout.BackColor = Color.FromArgb(235, 245, 255);
            _btnLogout.Cursor = Cursors.Hand;
            _btnLogout.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnLogout.FlatAppearance.BorderSize = 2;
            _btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 235, 250);
            _btnLogout.FlatStyle = FlatStyle.Flat;
            _btnLogout.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnLogout.ForeColor = Color.FromArgb(52, 152, 219);
            _btnLogout.Location = new Point(23, 286);
            _btnLogout.Margin = new Padding(3, 8, 3, 3);
            _btnLogout.Name = "_btnLogout";
            _btnLogout.Size = new Size(180, 42);
            _btnLogout.TabIndex = 9;
            _btnLogout.Text = "Log out";
            _btnLogout.UseVisualStyleBackColor = false;
            // 
            // _rightPanel
            // 
            _rightPanel.Controls.Add(_lblSleepTitle);
            _rightPanel.Controls.Add(_sleepCircle);
            _rightPanel.Controls.Add(_lblSleepSummary);
            _rightPanel.FlowDirection = FlowDirection.TopDown;
            _rightPanel.Location = new Point(327, 3);
            _rightPanel.Name = "_rightPanel";
            _rightPanel.Padding = new Padding(10);
            _rightPanel.Size = new Size(321, 368);
            _rightPanel.TabIndex = 1;
            // 
            // _lblSleepTitle
            // 
            _lblSleepTitle.AutoSize = true;
            _lblSleepTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _lblSleepTitle.Location = new Point(13, 10);
            _lblSleepTitle.Name = "_lblSleepTitle";
            _lblSleepTitle.Size = new Size(276, 21);
            _lblSleepTitle.TabIndex = 0;
            _lblSleepTitle.Text = "Your sleeping circle (drag the dots)";
            // 
            // _sleepCircle
            // 
            _sleepCircle.BackColor = Color.White;
            _sleepCircle.Location = new Point(10, 41);
            _sleepCircle.Margin = new Padding(0, 10, 0, 10);
            _sleepCircle.Name = "_sleepCircle";
            _sleepCircle.Size = new Size(280, 280);
            _sleepCircle.SleepEnd = System.TimeSpan.Parse("07:00:00");
            _sleepCircle.SleepStart = System.TimeSpan.Parse("23:00:00");
            _sleepCircle.TabIndex = 1;
            // 
            // _lblSleepSummary
            // 
            _lblSleepSummary.AutoSize = true;
            _lblSleepSummary.Font = new Font("Segoe UI", 14F);
            _lblSleepSummary.Location = new Point(13, 331);
            _lblSleepSummary.Name = "_lblSleepSummary";
            _lblSleepSummary.Size = new Size(118, 25);
            _lblSleepSummary.TabIndex = 2;
            _lblSleepSummary.Text = "You sleep 0h";
            // 
            // AccountDialog
            // 
            BackColor = Color.White;
            ClientSize = new Size(649, 374);
            Controls.Add(_container);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AccountDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Account";
            _container.ResumeLayout(false);
            _leftPanel.ResumeLayout(false);
            _leftPanel.PerformLayout();
            _rightPanel.ResumeLayout(false);
            _rightPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}
