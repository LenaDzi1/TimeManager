using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class FocusModeActiveForm
    {
        private IContainer components = null;

        private Panel _headerPanel;
        private Label _lblEventTitle;
        private Label _lblDuration;
        private Panel _progressPanel;
        private ProgressBar _progressBar;
        private Label _lblTimeRemaining;
        private Label _lblElapsed;
        private Label _lblPercentage;
        private Panel _buttonPanel;
        private Button _btnComplete;
        private Button _btnPause;
        private Button _btnCancel;

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
            _lblDuration = new Label();
            _lblEventTitle = new Label();
            _progressPanel = new Panel();
            _lblTimeRemaining = new Label();
            _progressBar = new ProgressBar();
            _lblElapsed = new Label();
            _lblPercentage = new Label();
            _buttonPanel = new Panel();
            _btnCancel = new Button();
            _btnPause = new Button();
            _btnComplete = new Button();
            _headerPanel.SuspendLayout();
            _progressPanel.SuspendLayout();
            _buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _headerPanel
            // 
            _headerPanel.BackColor = Color.FromArgb(135, 206, 250);
            _headerPanel.Controls.Add(_lblDuration);
            _headerPanel.Controls.Add(_lblEventTitle);
            _headerPanel.Dock = DockStyle.Top;
            _headerPanel.Location = new Point(0, 0);
            _headerPanel.Name = "_headerPanel";
            _headerPanel.Padding = new Padding(20, 15, 20, 15);
            _headerPanel.Size = new Size(500, 80);
            _headerPanel.TabIndex = 2;
            // 
            // _lblDuration
            // 
            _lblDuration.Dock = DockStyle.Top;
            _lblDuration.Font = new Font("Segoe UI", 10F);
            _lblDuration.ForeColor = Color.White;
            _lblDuration.Location = new Point(20, 50);
            _lblDuration.Name = "_lblDuration";
            _lblDuration.Size = new Size(460, 25);
            _lblDuration.TabIndex = 0;
            _lblDuration.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblEventTitle
            // 
            _lblEventTitle.Dock = DockStyle.Top;
            _lblEventTitle.Font = new Font("Segoe UI", 16F);
            _lblEventTitle.ForeColor = Color.White;
            _lblEventTitle.Location = new Point(20, 15);
            _lblEventTitle.Name = "_lblEventTitle";
            _lblEventTitle.Size = new Size(460, 35);
            _lblEventTitle.TabIndex = 1;
            _lblEventTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _progressPanel
            // 
            _progressPanel.BackColor = Color.FromArgb(248, 250, 252);
            _progressPanel.Controls.Add(_lblTimeRemaining);
            _progressPanel.Controls.Add(_progressBar);
            _progressPanel.Controls.Add(_lblElapsed);
            _progressPanel.Controls.Add(_lblPercentage);
            _progressPanel.Dock = DockStyle.Fill;
            _progressPanel.Location = new Point(0, 80);
            _progressPanel.Name = "_progressPanel";
            _progressPanel.Size = new Size(500, 200);
            _progressPanel.TabIndex = 0;
            // 
            // _lblTimeRemaining
            // 
            _lblTimeRemaining.AutoSize = true;
            _lblTimeRemaining.Font = new Font("Segoe UI", 48F, FontStyle.Bold);
            _lblTimeRemaining.ForeColor = Color.FromArgb(44, 62, 80);
            _lblTimeRemaining.Location = new Point(149, 3);
            _lblTimeRemaining.Name = "_lblTimeRemaining";
            _lblTimeRemaining.Size = new Size(202, 86);
            _lblTimeRemaining.TabIndex = 0;
            _lblTimeRemaining.Text = "00:00";
            _lblTimeRemaining.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _progressBar
            // 
            _progressBar.ForeColor = Color.FromArgb(46, 204, 113);
            _progressBar.Location = new Point(56, 105);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(400, 25);
            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.TabIndex = 1;
            // 
            // _lblElapsed
            // 
            _lblElapsed.AutoSize = true;
            _lblElapsed.Font = new Font("Segoe UI", 11F);
            _lblElapsed.ForeColor = Color.FromArgb(100, 100, 100);
            _lblElapsed.Location = new Point(207, 133);
            _lblElapsed.Name = "_lblElapsed";
            _lblElapsed.Size = new Size(103, 20);
            _lblElapsed.TabIndex = 2;
            _lblElapsed.Text = "Elapsed: 00:00";
            _lblElapsed.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _lblPercentage
            // 
            _lblPercentage.AutoSize = true;
            _lblPercentage.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            _lblPercentage.ForeColor = Color.FromArgb(46, 204, 113);
            _lblPercentage.Location = new Point(239, 153);
            _lblPercentage.Name = "_lblPercentage";
            _lblPercentage.Size = new Size(39, 25);
            _lblPercentage.TabIndex = 3;
            _lblPercentage.Text = "0%";
            _lblPercentage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _buttonPanel
            // 
            _buttonPanel.BackColor = Color.FromArgb(52, 152, 219);
            _buttonPanel.Controls.Add(_btnCancel);
            _buttonPanel.Controls.Add(_btnPause);
            _buttonPanel.Controls.Add(_btnComplete);
            _buttonPanel.Dock = DockStyle.Bottom;
            _buttonPanel.Location = new Point(0, 280);
            _buttonPanel.Name = "_buttonPanel";
            _buttonPanel.Padding = new Padding(30, 10, 30, 20);
            _buttonPanel.Size = new Size(500, 70);
            _buttonPanel.TabIndex = 1;
            // 
            // _btnCancel
            // 
            _btnCancel.BackColor = Color.FromArgb(231, 76, 60);
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.FlatAppearance.BorderSize = 0;
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnCancel.ForeColor = Color.White;
            _btnCancel.Location = new Point(350, 10);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(120, 40);
            _btnCancel.TabIndex = 0;
            _btnCancel.Text = "✕ Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // _btnPause
            // 
            _btnPause.BackColor = Color.FromArgb(241, 196, 15);
            _btnPause.Cursor = Cursors.Hand;
            _btnPause.FlatAppearance.BorderSize = 0;
            _btnPause.FlatStyle = FlatStyle.Flat;
            _btnPause.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnPause.ForeColor = Color.Black;
            _btnPause.Location = new Point(190, 10);
            _btnPause.Name = "_btnPause";
            _btnPause.Size = new Size(120, 40);
            _btnPause.TabIndex = 1;
            _btnPause.Text = "⏸ Pause";
            _btnPause.UseVisualStyleBackColor = false;
            // 
            // _btnComplete
            // 
            _btnComplete.BackColor = Color.FromArgb(46, 204, 113);
            _btnComplete.Cursor = Cursors.Hand;
            _btnComplete.FlatAppearance.BorderSize = 0;
            _btnComplete.FlatStyle = FlatStyle.Flat;
            _btnComplete.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnComplete.ForeColor = Color.White;
            _btnComplete.Location = new Point(30, 10);
            _btnComplete.Name = "_btnComplete";
            _btnComplete.Size = new Size(120, 40);
            _btnComplete.TabIndex = 2;
            _btnComplete.Text = "✓ Complete";
            _btnComplete.UseVisualStyleBackColor = false;
            // 
            // FocusModeActiveForm
            // 
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(500, 350);
            Controls.Add(_progressPanel);
            Controls.Add(_buttonPanel);
            Controls.Add(_headerPanel);
            MinimumSize = new Size(450, 450);
            Name = "FocusModeActiveForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Focus Mode";
            _headerPanel.ResumeLayout(false);
            _progressPanel.ResumeLayout(false);
            _progressPanel.PerformLayout();
            _buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
