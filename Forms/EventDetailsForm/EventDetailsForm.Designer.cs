using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class EventDetailsForm
    {
        private IContainer components = null;
        private Panel _headerPanel;
        private Label _lblTitle;
        private Panel _contentPanel;
        private Label _lblDescription;
        private Panel _infoCard;
        private Label _lblDateTime;
        private Label _lblDuration;
        private Label _lblCategory;
        private Label _lblWeight;
        private Panel _buttonPanel;
        private Button _btnStartFocus;
        private Button _btnComplete;
        private Button _btnEdit;
        private Button _btnDelete;
        private Button _btnClose;

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
            _headerPanel = new Panel();
            _lblTitle = new Label();
            _contentPanel = new Panel();
            _lblDescription = new Label();
            _infoCard = new Panel();
            _lblDateTime = new Label();
            _lblDuration = new Label();
            _lblCategory = new Label();
            _lblWeight = new Label();
            _buttonPanel = new Panel();
            _btnStartFocus = new Button();
            _btnComplete = new Button();
            _btnEdit = new Button();
            _btnDelete = new Button();
            _btnClose = new Button();
            _headerPanel.SuspendLayout();
            _contentPanel.SuspendLayout();
            _infoCard.SuspendLayout();
            _buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _headerPanel
            // 
            _headerPanel.BackColor = Color.FromArgb(135, 206, 250);
            _headerPanel.Controls.Add(_lblTitle);
            _headerPanel.Dock = DockStyle.Top;
            _headerPanel.Height = 50;
            _headerPanel.Padding = new Padding(20, 0, 20, 0);
            // 
            // _lblTitle
            // 
            _lblTitle.AutoEllipsis = true;
            _lblTitle.Dock = DockStyle.Fill;
            _lblTitle.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.Text = "Event Title";
            _lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _contentPanel
            // 
            _contentPanel.BackColor = Color.FromArgb(248, 250, 252);
            _contentPanel.Controls.Add(_infoCard);
            _contentPanel.Controls.Add(_lblDescription);
            _contentPanel.Dock = DockStyle.Fill;
            _contentPanel.Padding = new Padding(20);
            // 
            // _lblDescription
            // 
            _lblDescription.BackColor = Color.White;
            _lblDescription.BorderStyle = BorderStyle.FixedSingle;
            _lblDescription.Font = new Font("Segoe UI", 10F);
            _lblDescription.ForeColor = Color.FromArgb(52, 73, 94);
            _lblDescription.Location = new Point(20, 20);
            _lblDescription.Padding = new Padding(10);
            _lblDescription.Size = new Size(440, 80);
            _lblDescription.Text = "Description";
            // 
            // _infoCard
            // 
            _infoCard.BackColor = Color.White;
            _infoCard.BorderStyle = BorderStyle.FixedSingle;
            _infoCard.Controls.Add(_lblDateTime);
            _infoCard.Controls.Add(_lblDuration);
            _infoCard.Controls.Add(_lblCategory);
            _infoCard.Controls.Add(_lblWeight);
            _infoCard.Location = new Point(20, 110);
            _infoCard.Padding = new Padding(15);
            _infoCard.Size = new Size(440, 130);
            // 
            // _lblDateTime
            // 
            _lblDateTime.AutoSize = true;
            _lblDateTime.Font = new Font("Segoe UI", 10F);
            _lblDateTime.ForeColor = Color.FromArgb(52, 73, 94);
            _lblDateTime.Location = new Point(15, 15);
            _lblDateTime.Text = "Date/Time: -";
            // 
            // _lblDuration
            // 
            _lblDuration.AutoSize = true;
            _lblDuration.Font = new Font("Segoe UI", 10F);
            _lblDuration.ForeColor = Color.FromArgb(52, 73, 94);
            _lblDuration.Location = new Point(15, 40);
            _lblDuration.Text = "Duration: -";
            // 
            // _lblCategory
            // 
            _lblCategory.AutoSize = true;
            _lblCategory.Font = new Font("Segoe UI", 10F);
            _lblCategory.ForeColor = Color.FromArgb(52, 73, 94);
            _lblCategory.Location = new Point(15, 65);
            _lblCategory.Text = "Category: -";
            // 
            // _lblWeight
            // 
            _lblWeight.AutoSize = true;
            _lblWeight.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _lblWeight.ForeColor = Color.FromArgb(39, 174, 96);
            _lblWeight.Location = new Point(15, 95);
            _lblWeight.Text = "Weight: 0 points";
            _lblWeight.Cursor = Cursors.Hand;
            // 
            // _buttonPanel
            _buttonPanel.BackColor = Color.FromArgb(248, 250, 252);
            _buttonPanel.Controls.Add(_btnClose);
            _buttonPanel.Controls.Add(_btnDelete);
            _buttonPanel.Controls.Add(_btnEdit);
            _buttonPanel.Controls.Add(_btnComplete);
            _buttonPanel.Controls.Add(_btnStartFocus);
            _buttonPanel.Dock = DockStyle.Bottom;
            _buttonPanel.Height = 70;
            _buttonPanel.Padding = new Padding(20, 15, 20, 15);
            // 
            // _btnStartFocus
            // 
            _btnStartFocus.BackColor = Color.FromArgb(232, 245, 233);
            _btnStartFocus.Cursor = Cursors.Hand;
            _btnStartFocus.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnStartFocus.FlatAppearance.BorderSize = 2;
            _btnStartFocus.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnStartFocus.FlatStyle = FlatStyle.Flat;
            _btnStartFocus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnStartFocus.ForeColor = Color.FromArgb(39, 174, 96);
            _btnStartFocus.Location = new Point(20, 15);
            _btnStartFocus.Size = new Size(100, 38);
            _btnStartFocus.Text = "Focus";
            // 
            // _btnComplete
            // 
            _btnComplete.BackColor = Color.FromArgb(232, 245, 233);
            _btnComplete.Cursor = Cursors.Hand;
            _btnComplete.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnComplete.FlatAppearance.BorderSize = 2;
            _btnComplete.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnComplete.FlatStyle = FlatStyle.Flat;
            _btnComplete.Font = new Font("Segoe UI", 9F);
            _btnComplete.ForeColor = Color.FromArgb(39, 174, 96);
            _btnComplete.Location = new Point(130, 15);
            _btnComplete.Size = new Size(100, 38);
            _btnComplete.Text = "Complete";
            // 
            // _btnEdit
            // 
            _btnEdit.BackColor = Color.FromArgb(248, 250, 252);
            _btnEdit.Cursor = Cursors.Hand;
            _btnEdit.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnEdit.FlatAppearance.BorderSize = 2;
            _btnEdit.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 245, 255);
            _btnEdit.FlatStyle = FlatStyle.Flat;
            _btnEdit.Font = new Font("Segoe UI", 9F);
            _btnEdit.ForeColor = Color.FromArgb(52, 152, 219);
            _btnEdit.Location = new Point(240, 15);
            _btnEdit.Size = new Size(80, 38);
            _btnEdit.Text = "Edit";
            // 
            // _btnDelete
            // 
            _btnDelete.BackColor = Color.FromArgb(255, 245, 245);
            _btnDelete.Cursor = Cursors.Hand;
            _btnDelete.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDelete.FlatAppearance.BorderSize = 2;
            _btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 230, 230);
            _btnDelete.FlatStyle = FlatStyle.Flat;
            _btnDelete.Font = new Font("Segoe UI", 9F);
            _btnDelete.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDelete.Location = new Point(330, 15);
            _btnDelete.Size = new Size(80, 38);
            _btnDelete.Text = "Delete";
            // 
            // _btnClose
            // 
            _btnClose.BackColor = Color.FromArgb(248, 250, 252);
            _btnClose.Cursor = Cursors.Hand;
            _btnClose.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            _btnClose.FlatAppearance.BorderSize = 2;
            _btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.Font = new Font("Segoe UI", 9F);
            _btnClose.ForeColor = Color.FromArgb(100, 100, 100);
            _btnClose.Location = new Point(420, 15);
            _btnClose.Size = new Size(80, 38);
            _btnClose.Text = "Close";
            // 
            // EventDetailsForm
            // 
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(520, 380);
            Controls.Add(_contentPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_headerPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EventDetailsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Event Details";
            _headerPanel.ResumeLayout(false);
            _contentPanel.ResumeLayout(false);
            _infoCard.ResumeLayout(false);
            _infoCard.PerformLayout();
            _buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
