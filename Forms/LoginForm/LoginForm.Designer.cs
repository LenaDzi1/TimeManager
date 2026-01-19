using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class LoginForm
    {
        private IContainer components = null;
        
        private Panel _topBar;
        private Label _lblTitle;
        private Panel _mainPanel;
        private Label _lblUser;
        private Label _lblPassword;
        private Label _lblMode;
        private TextBox _txtUserName;
        private TextBox _txtPassword;
        private Button _btnLogin;
        private Button _btnSignupMode;

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
            _topBar = new Panel();
            _lblTitle = new Label();
            _mainPanel = new Panel();
            _lblMode = new Label();
            _lblUser = new Label();
            _txtUserName = new TextBox();
            _lblPassword = new Label();
            _txtPassword = new TextBox();
            _btnLogin = new Button();
            _btnSignupMode = new Button();
            _topBar.SuspendLayout();
            _mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _topBar
            // 
            _topBar.BackColor = Color.FromArgb(135, 206, 250);
            _topBar.Controls.Add(_lblTitle);
            _topBar.Dock = DockStyle.Top;
            _topBar.Location = new Point(0, 0);
            _topBar.Name = "_topBar";
            _topBar.Size = new Size(380, 60);
            _topBar.TabIndex = 0;
            // 
            // _lblTitle
            // 
            _lblTitle.Dock = DockStyle.Fill;
            _lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.Location = new Point(0, 0);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(380, 60);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "🕗Time Manager";
            _lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _mainPanel
            // 
            _mainPanel.BackColor = Color.White;
            _mainPanel.Controls.Add(_lblMode);
            _mainPanel.Controls.Add(_lblUser);
            _mainPanel.Controls.Add(_txtUserName);
            _mainPanel.Controls.Add(_lblPassword);
            _mainPanel.Controls.Add(_txtPassword);
            _mainPanel.Controls.Add(_btnLogin);
            _mainPanel.Controls.Add(_btnSignupMode);
            _mainPanel.Dock = DockStyle.Fill;
            _mainPanel.Location = new Point(0, 60);
            _mainPanel.Name = "_mainPanel";
            _mainPanel.Padding = new Padding(30, 20, 30, 20);
            _mainPanel.Size = new Size(380, 235);
            _mainPanel.TabIndex = 1;
            // 
            // _lblMode
            // 
            _lblMode.AutoSize = true;
            _lblMode.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            _lblMode.ForeColor = Color.FromArgb(44, 62, 80);
            _lblMode.Location = new Point(30, 20);
            _lblMode.Name = "_lblMode";
            _lblMode.Size = new Size(204, 25);
            _lblMode.TabIndex = 0;
            _lblMode.Text = "Log in to your account";
            // 
            // _lblUser
            // 
            _lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblUser.ForeColor = Color.FromArgb(52, 73, 94);
            _lblUser.Location = new Point(30, 55);
            _lblUser.Name = "_lblUser";
            _lblUser.Size = new Size(100, 18);
            _lblUser.TabIndex = 1;
            _lblUser.Text = "Username";
            // 
            // _txtUserName
            // 
            _txtUserName.BackColor = Color.White;
            _txtUserName.BorderStyle = BorderStyle.FixedSingle;
            _txtUserName.Font = new Font("Segoe UI", 11F);
            _txtUserName.Location = new Point(30, 75);
            _txtUserName.Name = "_txtUserName";
            _txtUserName.Size = new Size(320, 27);
            _txtUserName.TabIndex = 1;
            // 
            // _lblPassword
            // 
            _lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblPassword.ForeColor = Color.FromArgb(52, 73, 94);
            _lblPassword.Location = new Point(30, 110);
            _lblPassword.Name = "_lblPassword";
            _lblPassword.Size = new Size(100, 18);
            _lblPassword.TabIndex = 2;
            _lblPassword.Text = "Password";
            // 
            // _txtPassword
            // 
            _txtPassword.BackColor = Color.White;
            _txtPassword.BorderStyle = BorderStyle.FixedSingle;
            _txtPassword.Font = new Font("Segoe UI", 11F);
            _txtPassword.Location = new Point(30, 130);
            _txtPassword.Name = "_txtPassword";
            _txtPassword.Size = new Size(320, 27);
            _txtPassword.TabIndex = 2;
            _txtPassword.UseSystemPasswordChar = true;
            // 
            // _btnLogin
            // 
            _btnLogin.BackColor = Color.FromArgb(232, 245, 233);
            _btnLogin.Cursor = Cursors.Hand;
            _btnLogin.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnLogin.FlatAppearance.BorderSize = 2;
            _btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnLogin.FlatStyle = FlatStyle.Flat;
            _btnLogin.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            _btnLogin.ForeColor = Color.FromArgb(39, 174, 96);
            _btnLogin.Location = new Point(30, 177);
            _btnLogin.Name = "_btnLogin";
            _btnLogin.Size = new Size(150, 42);
            _btnLogin.TabIndex = 4;
            _btnLogin.Text = "Log in";
            _btnLogin.UseVisualStyleBackColor = false;
            // 
            // _btnSignupMode
            // 
            _btnSignupMode.BackColor = Color.FromArgb(248, 250, 252);
            _btnSignupMode.Cursor = Cursors.Hand;
            _btnSignupMode.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnSignupMode.FlatAppearance.BorderSize = 2;
            _btnSignupMode.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 245, 255);
            _btnSignupMode.FlatStyle = FlatStyle.Flat;
            _btnSignupMode.Font = new Font("Segoe UI", 10F);
            _btnSignupMode.ForeColor = Color.FromArgb(52, 152, 219);
            _btnSignupMode.Location = new Point(195, 177);
            _btnSignupMode.Name = "_btnSignupMode";
            _btnSignupMode.Size = new Size(155, 42);
            _btnSignupMode.TabIndex = 5;
            _btnSignupMode.Text = "Sign up instead";
            _btnSignupMode.UseVisualStyleBackColor = false;
            // 
            // LoginForm
            // 
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(380, 295);
            Controls.Add(_mainPanel);
            Controls.Add(_topBar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Time Manager";
            _topBar.ResumeLayout(false);
            _mainPanel.ResumeLayout(false);
            _mainPanel.PerformLayout();
            ResumeLayout(false);
        }
    }
}
