using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TimeManager.Models;

namespace TimeManager.Forms
{
    partial class AdminPanelForm
    {
        private IContainer components = null;

        private Label _lblTitle;
        private Label _lblInfo;
        private DataGridView _gridUsers;
        private Label _lblRole;
        private ComboBox _cmbNewRole;
        private Button _btnChangeRole;
        private Button _btnDeleteUser;
        private Button _btnChangePassword;
        private Button _btnRefresh;
        private Button _btnLogout;
        private Label _lblParentInfo;
        private ComboBox _cmbAssignParent;
        private Button _btnAssignParent;
        private Label _lblLegend;

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
            _lblTitle = new Label();
            _lblInfo = new Label();
            _gridUsers = new DataGridView();
            _lblRole = new Label();
            _cmbNewRole = new ComboBox();
            _btnChangeRole = new Button();
            _btnDeleteUser = new Button();
            _btnChangePassword = new Button();
            _btnRefresh = new Button();
            _btnLogout = new Button();
            _lblParentInfo = new Label();
            _cmbAssignParent = new ComboBox();
            _btnAssignParent = new Button();
            _lblLegend = new Label();
            ((ISupportInitialize)_gridUsers).BeginInit();
            SuspendLayout();
            // 
            // _lblTitle
            // 
            _lblTitle.AutoSize = true;
            _lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _lblTitle.Location = new Point(20, 15);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(241, 30);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "👤 User Management";
            // 
            // _lblInfo
            // 
            _lblInfo.AutoSize = true;
            _lblInfo.Font = new Font("Segoe UI", 9F);
            _lblInfo.ForeColor = Color.Gray;
            _lblInfo.Location = new Point(20, 50);
            _lblInfo.Name = "_lblInfo";
            _lblInfo.Size = new Size(261, 15);
            _lblInfo.TabIndex = 1;
            _lblInfo.Text = "Select a user to change their role or delete them.";
            // 
            // _gridUsers
            // 
            _gridUsers.AllowUserToAddRows = false;
            _gridUsers.AllowUserToDeleteRows = false;
            _gridUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _gridUsers.BackgroundColor = Color.White;
            _gridUsers.Location = new Point(20, 80);
            _gridUsers.MultiSelect = false;
            _gridUsers.Name = "_gridUsers";
            _gridUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _gridUsers.Size = new Size(640, 280);
            _gridUsers.TabIndex = 2;
            // 
            // _lblRole
            // 
            _lblRole.Font = new Font("Segoe UI", 9F);
            _lblRole.Location = new Point(20, 375);
            _lblRole.Name = "_lblRole";
            _lblRole.Size = new Size(70, 25);
            _lblRole.TabIndex = 3;
            _lblRole.Text = "New Role:";
            // 
            // _cmbNewRole
            // 
            _cmbNewRole.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbNewRole.Font = new Font("Segoe UI", 9F);
            _cmbNewRole.FormattingEnabled = true;
            _cmbNewRole.Location = new Point(95, 372);
            _cmbNewRole.Name = "_cmbNewRole";
            _cmbNewRole.Size = new Size(120, 23);
            _cmbNewRole.TabIndex = 4;
            // 
            // _btnChangeRole
            // 
            _btnChangeRole.BackColor = Color.FromArgb(52, 152, 219);
            _btnChangeRole.Cursor = Cursors.Hand;
            _btnChangeRole.FlatAppearance.BorderSize = 0;
            _btnChangeRole.FlatStyle = FlatStyle.Flat;
            _btnChangeRole.ForeColor = Color.White;
            _btnChangeRole.Location = new Point(230, 370);
            _btnChangeRole.Name = "_btnChangeRole";
            _btnChangeRole.Size = new Size(110, 30);
            _btnChangeRole.TabIndex = 5;
            _btnChangeRole.Text = "Change Role";
            _btnChangeRole.UseVisualStyleBackColor = false;
            // 
            // _btnDeleteUser
            // 
            _btnDeleteUser.BackColor = Color.FromArgb(231, 76, 60);
            _btnDeleteUser.Cursor = Cursors.Hand;
            _btnDeleteUser.FlatAppearance.BorderSize = 0;
            _btnDeleteUser.FlatStyle = FlatStyle.Flat;
            _btnDeleteUser.ForeColor = Color.White;
            _btnDeleteUser.Location = new Point(350, 370);
            _btnDeleteUser.Name = "_btnDeleteUser";
            _btnDeleteUser.Size = new Size(100, 30);
            _btnDeleteUser.TabIndex = 6;
            _btnDeleteUser.Text = "Delete User";
            _btnDeleteUser.UseVisualStyleBackColor = false;
            // 
            // _btnChangePassword
            // 
            _btnChangePassword.BackColor = Color.FromArgb(155, 89, 182);
            _btnChangePassword.Cursor = Cursors.Hand;
            _btnChangePassword.FlatAppearance.BorderSize = 0;
            _btnChangePassword.FlatStyle = FlatStyle.Flat;
            _btnChangePassword.ForeColor = Color.White;
            _btnChangePassword.Location = new Point(455, 370);
            _btnChangePassword.Name = "_btnChangePassword";
            _btnChangePassword.Size = new Size(115, 30);
            _btnChangePassword.TabIndex = 7;
            _btnChangePassword.Text = "Change Password";
            _btnChangePassword.UseVisualStyleBackColor = false;
            // 
            // _btnRefresh
            // 
            _btnRefresh.BackColor = Color.FromArgb(149, 165, 166);
            _btnRefresh.Cursor = Cursors.Hand;
            _btnRefresh.FlatAppearance.BorderSize = 0;
            _btnRefresh.FlatStyle = FlatStyle.Flat;
            _btnRefresh.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _btnRefresh.ForeColor = Color.White;
            _btnRefresh.Location = new Point(575, 370);
            _btnRefresh.Name = "_btnRefresh";
            _btnRefresh.Size = new Size(40, 30);
            _btnRefresh.TabIndex = 8;
            _btnRefresh.Text = "↻";
            _btnRefresh.UseVisualStyleBackColor = false;
            // 
            // _btnLogout
            // 
            _btnLogout.BackColor = Color.FromArgb(189, 195, 199);
            _btnLogout.Cursor = Cursors.Hand;
            _btnLogout.FlatAppearance.BorderSize = 0;
            _btnLogout.FlatStyle = FlatStyle.Flat;
            _btnLogout.ForeColor = Color.White;
            _btnLogout.Location = new Point(620, 370);
            _btnLogout.Name = "_btnLogout";
            _btnLogout.Size = new Size(60, 30);
            _btnLogout.TabIndex = 9;
            _btnLogout.Text = "Logout";
            _btnLogout.UseVisualStyleBackColor = false;
            // 
            // _lblParentInfo
            // 
            _lblParentInfo.Font = new Font("Segoe UI", 9F);
            _lblParentInfo.Location = new Point(20, 410);
            _lblParentInfo.Name = "_lblParentInfo";
            _lblParentInfo.Size = new Size(200, 20);
            _lblParentInfo.TabIndex = 10;
            _lblParentInfo.Text = "Assign parent to Kid:";
            // 
            // _cmbAssignParent
            // 
            _cmbAssignParent.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbAssignParent.Font = new Font("Segoe UI", 9F);
            _cmbAssignParent.FormattingEnabled = true;
            _cmbAssignParent.Location = new Point(20, 432);
            _cmbAssignParent.Name = "_cmbAssignParent";
            _cmbAssignParent.Size = new Size(220, 23);
            _cmbAssignParent.TabIndex = 11;
            // 
            // _btnAssignParent
            // 
            _btnAssignParent.BackColor = Color.FromArgb(52, 152, 219);
            _btnAssignParent.Cursor = Cursors.Hand;
            _btnAssignParent.FlatAppearance.BorderSize = 0;
            _btnAssignParent.FlatStyle = FlatStyle.Flat;
            _btnAssignParent.ForeColor = Color.White;
            _btnAssignParent.Location = new Point(250, 428);
            _btnAssignParent.Name = "_btnAssignParent";
            _btnAssignParent.Size = new Size(120, 28);
            _btnAssignParent.TabIndex = 12;
            _btnAssignParent.Text = "Assign Parent";
            _btnAssignParent.UseVisualStyleBackColor = false;
            // 
            // _lblLegend
            // 
            _lblLegend.AutoSize = true;
            _lblLegend.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            _lblLegend.ForeColor = Color.Gray;
            _lblLegend.Location = new Point(380, 437);
            _lblLegend.Name = "_lblLegend";
            _lblLegend.Size = new Size(276, 13);
            _lblLegend.TabIndex = 13;
            _lblLegend.Text = "Kid (limited) | User/Parent (full) | Administrator (manage)";
            // 
            // AdminPanelForm
            // 
            BackColor = Color.White;
            ClientSize = new Size(700, 470);
            Controls.Add(_lblTitle);
            Controls.Add(_lblInfo);
            Controls.Add(_gridUsers);
            Controls.Add(_lblRole);
            Controls.Add(_cmbNewRole);
            Controls.Add(_btnChangeRole);
            Controls.Add(_btnDeleteUser);
            Controls.Add(_btnChangePassword);
            Controls.Add(_btnRefresh);
            Controls.Add(_btnLogout);
            Controls.Add(_lblParentInfo);
            Controls.Add(_cmbAssignParent);
            Controls.Add(_btnAssignParent);
            Controls.Add(_lblLegend);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "AdminPanelForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Admin Panel - User Management";
            ((ISupportInitialize)_gridUsers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
