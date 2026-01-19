using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace TimeManager.Forms
{
    partial class CalendarView
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_toolTip != null)
                {
                    _toolTip.Dispose();
                    _toolTip = null;
                }
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private Panel _headerPanel;
        private Panel _calendarPanel;
        private Panel _rightPanel;
        private Panel _bottomPanel;
        private Button _btnPrevious;
        private Button _btnNext;
        private Button _btnToday;
        private Label _lblCurrentDate;
        private Button _btnMonthView;
        private Button _btnWeekView;
        private Button _btnDayView;
        private Button _btnQuickTasks;
        private Button _btnAddEvent;
        private Label _lblView;
        private Panel _dayHeadersPanel;
        private Label[] _dayHeaderLabels;
        private Panel[] _dayCells;
        private Label[] _weekNumberLabels;
        private Button _btnWeekStartSelector;
        private ToolTip _toolTip;
        private CheckBox _chkShowSleepHours;

        private void InitializeComponent()
        {
            _headerPanel = new Panel();
            _lblCurrentDate = new Label();
            _btnPrevious = new Button();
            _btnNext = new Button();
            _btnToday = new Button();
            _btnAddEvent = new Button();
            _btnMonthView = new Button();
            _btnWeekView = new Button();
            _btnDayView = new Button();
            _btnQuickTasks = new Button();
            _lblView = new Label();
            _calendarPanel = new Panel();
            _btnWeekStartSelector = new Button();
            _dayHeadersPanel = new Panel();
            _rightPanel = new Panel();
            _chkShowSleepHours = new CheckBox();
            _bottomPanel = new Panel();
            _headerPanel.SuspendLayout();
            _calendarPanel.SuspendLayout();
            _rightPanel.SuspendLayout();
            _bottomPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _headerPanel
            // 
            _headerPanel.BackColor = Color.White;
            _headerPanel.Controls.Add(_lblCurrentDate);
            _headerPanel.Dock = DockStyle.Top;
            _headerPanel.Location = new Point(10, 10);
            _headerPanel.Name = "_headerPanel";
            _headerPanel.Size = new Size(1131, 50);
            _headerPanel.TabIndex = 0;
            // 
            // _lblCurrentDate
            // 
            _lblCurrentDate.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            _lblCurrentDate.ForeColor = Color.FromArgb(44, 62, 80);
            _lblCurrentDate.Location = new Point(426, 0);
            _lblCurrentDate.Name = "_lblCurrentDate";
            _lblCurrentDate.Size = new Size(300, 30);
            _lblCurrentDate.TabIndex = 3;
            _lblCurrentDate.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _btnPrevious
            // 
            _btnPrevious.BackColor = Color.White;
            _btnPrevious.Font = new Font("Segoe UI", 11F);
            _btnPrevious.ForeColor = Color.FromArgb(44, 62, 80);
            _btnPrevious.Location = new Point(40, 5);
            _btnPrevious.Name = "_btnPrevious";
            _btnPrevious.Size = new Size(180, 40);
            _btnPrevious.TabIndex = 0;
            _btnPrevious.Text = "◀ Prev";
            _btnPrevious.UseVisualStyleBackColor = false;
            // 
            // _btnNext
            // 
            _btnNext.BackColor = Color.White;
            _btnNext.Font = new Font("Segoe UI", 11F);
            _btnNext.ForeColor = Color.FromArgb(44, 62, 80);
            _btnNext.Location = new Point(260, 5);
            _btnNext.Name = "_btnNext";
            _btnNext.Size = new Size(180, 40);
            _btnNext.TabIndex = 2;
            _btnNext.Text = "Next ▶";
            _btnNext.UseVisualStyleBackColor = false;
            // 
            // _btnToday
            // 
            _btnToday.Location = new Point(10, 5);
            _btnToday.Name = "_btnToday";
            _btnToday.Size = new Size(100, 30);
            _btnToday.TabIndex = 1;
            _btnToday.Text = "Today";
            // 
            // _btnAddEvent
            // 
            _btnAddEvent.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnAddEvent.BackColor = Color.FromArgb(232, 245, 233);
            _btnAddEvent.Cursor = Cursors.Hand;
            _btnAddEvent.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnAddEvent.FlatAppearance.BorderSize = 2;
            _btnAddEvent.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnAddEvent.FlatStyle = FlatStyle.Flat;
            _btnAddEvent.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            _btnAddEvent.ForeColor = Color.FromArgb(39, 174, 96);
            _btnAddEvent.Location = new Point(970, 5);
            _btnAddEvent.Name = "_btnAddEvent";
            _btnAddEvent.Size = new Size(140, 40);
            _btnAddEvent.TabIndex = 7;
            _btnAddEvent.Text = "+ ADD";
            _btnAddEvent.UseVisualStyleBackColor = false;
            // 
            // _btnMonthView
            // 
            _btnMonthView.BackColor = Color.White;
            _btnMonthView.Cursor = Cursors.Hand;
            _btnMonthView.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnMonthView.FlatAppearance.BorderSize = 2;
            _btnMonthView.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 245, 255);
            _btnMonthView.FlatStyle = FlatStyle.Flat;
            _btnMonthView.Font = new Font("Segoe UI", 9F);
            _btnMonthView.ForeColor = Color.FromArgb(52, 152, 219);
            _btnMonthView.Location = new Point(20, 47);
            _btnMonthView.Name = "_btnMonthView";
            _btnMonthView.Size = new Size(110, 35);
            _btnMonthView.TabIndex = 4;
            _btnMonthView.Text = "Month";
            _btnMonthView.UseVisualStyleBackColor = false;
            // 
            // _btnWeekView
            // 
            _btnWeekView.BackColor = Color.White;
            _btnWeekView.Cursor = Cursors.Hand;
            _btnWeekView.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            _btnWeekView.FlatAppearance.BorderSize = 2;
            _btnWeekView.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            _btnWeekView.FlatStyle = FlatStyle.Flat;
            _btnWeekView.Font = new Font("Segoe UI", 9F);
            _btnWeekView.ForeColor = Color.FromArgb(127, 140, 141);
            _btnWeekView.Location = new Point(20, 92);
            _btnWeekView.Name = "_btnWeekView";
            _btnWeekView.Size = new Size(110, 35);
            _btnWeekView.TabIndex = 5;
            _btnWeekView.Text = "Week";
            _btnWeekView.UseVisualStyleBackColor = false;
            // 
            // _btnDayView
            // 
            _btnDayView.BackColor = Color.White;
            _btnDayView.Cursor = Cursors.Hand;
            _btnDayView.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            _btnDayView.FlatAppearance.BorderSize = 2;
            _btnDayView.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            _btnDayView.FlatStyle = FlatStyle.Flat;
            _btnDayView.Font = new Font("Segoe UI", 9F);
            _btnDayView.ForeColor = Color.FromArgb(127, 140, 141);
            _btnDayView.Location = new Point(20, 137);
            _btnDayView.Name = "_btnDayView";
            _btnDayView.Size = new Size(110, 35);
            _btnDayView.TabIndex = 6;
            _btnDayView.Text = "Day";
            _btnDayView.UseVisualStyleBackColor = false;
            // 
            // _btnQuickTasks
            // 
            _btnQuickTasks.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnQuickTasks.BackColor = Color.FromArgb(243, 229, 245);
            _btnQuickTasks.Cursor = Cursors.Hand;
            _btnQuickTasks.FlatAppearance.BorderColor = Color.FromArgb(155, 89, 182);
            _btnQuickTasks.FlatAppearance.BorderSize = 2;
            _btnQuickTasks.FlatAppearance.MouseOverBackColor = Color.FromArgb(225, 190, 231);
            _btnQuickTasks.FlatStyle = FlatStyle.Flat;
            _btnQuickTasks.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnQuickTasks.ForeColor = Color.FromArgb(142, 68, 173);
            _btnQuickTasks.Location = new Point(800, 5);
            _btnQuickTasks.Name = "_btnQuickTasks";
            _btnQuickTasks.Size = new Size(140, 40);
            _btnQuickTasks.TabIndex = 100;
            _btnQuickTasks.Text = "Quick tasks";
            _btnQuickTasks.UseVisualStyleBackColor = false;
            // 
            // _lblView
            // 
            _lblView.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _lblView.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            _lblView.ForeColor = Color.FromArgb(44, 62, 80);
            _lblView.Location = new Point(20, 12);
            _lblView.Name = "_lblView";
            _lblView.Size = new Size(110, 25);
            _lblView.TabIndex = 0;
            _lblView.Text = "View";
            _lblView.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _calendarPanel
            // 
            _calendarPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _calendarPanel.BackColor = Color.White;
            _calendarPanel.Controls.Add(_btnWeekStartSelector);
            _calendarPanel.Controls.Add(_dayHeadersPanel);
            _calendarPanel.Location = new Point(10, 60);
            _calendarPanel.Name = "_calendarPanel";
            _calendarPanel.Size = new Size(940, 625);
            _calendarPanel.TabIndex = 1;
            // 
            // _btnWeekStartSelector
            // 
            _btnWeekStartSelector.BackColor = Color.Transparent;
            _btnWeekStartSelector.Cursor = Cursors.Hand;
            _btnWeekStartSelector.FlatAppearance.BorderSize = 0;
            _btnWeekStartSelector.FlatAppearance.MouseDownBackColor = Color.FromArgb(230, 230, 230);
            _btnWeekStartSelector.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            _btnWeekStartSelector.FlatStyle = FlatStyle.Flat;
            _btnWeekStartSelector.Font = new Font("Segoe UI", 14F);
            _btnWeekStartSelector.Location = new Point(5, 5);
            _btnWeekStartSelector.Name = "_btnWeekStartSelector";
            _btnWeekStartSelector.Size = new Size(40, 35);
            _btnWeekStartSelector.TabIndex = 50;
            _btnWeekStartSelector.Text = ">>";
            _btnWeekStartSelector.UseVisualStyleBackColor = false;
            // 
            // _dayHeadersPanel
            // 
            _dayHeadersPanel.BackColor = Color.White;
            _dayHeadersPanel.Location = new Point(50, 0);
            _dayHeadersPanel.Name = "_dayHeadersPanel";
            _dayHeadersPanel.Size = new Size(921, 40);
            _dayHeadersPanel.TabIndex = 0;
            // 
            // _rightPanel
            // 
            _rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _rightPanel.BackColor = Color.FromArgb(235, 245, 255);
            _rightPanel.BorderStyle = BorderStyle.FixedSingle;
            _rightPanel.Controls.Add(_btnMonthView);
            _rightPanel.Controls.Add(_btnWeekView);
            _rightPanel.Controls.Add(_btnDayView);
            _rightPanel.Controls.Add(_chkShowSleepHours);
            _rightPanel.Controls.Add(_lblView);
            _rightPanel.Location = new Point(971, 133);
            _rightPanel.Margin = new Padding(0);
            _rightPanel.Name = "_rightPanel";
            _rightPanel.Padding = new Padding(10, 30, 10, 10);
            _rightPanel.Size = new Size(150, 240);
            _rightPanel.TabIndex = 2;
            // 
            // _chkShowSleepHours
            // 
            _chkShowSleepHours.AutoSize = true;
            _chkShowSleepHours.Font = new Font("Segoe UI", 8F);
            _chkShowSleepHours.ForeColor = Color.FromArgb(100, 100, 100);
            _chkShowSleepHours.Location = new Point(15, 185);
            _chkShowSleepHours.Name = "_chkShowSleepHours";
            _chkShowSleepHours.Size = new Size(118, 17);
            _chkShowSleepHours.TabIndex = 8;
            _chkShowSleepHours.Text = "Show sleep hours";
            // 
            // _bottomPanel
            // 
            _bottomPanel.BackColor = Color.White;
            _bottomPanel.Controls.Add(_btnPrevious);
            _bottomPanel.Controls.Add(_btnNext);
            _bottomPanel.Controls.Add(_btnAddEvent);
            _bottomPanel.Controls.Add(_btnQuickTasks);
            _bottomPanel.Dock = DockStyle.Bottom;
            _bottomPanel.Location = new Point(10, 691);
            _bottomPanel.Name = "_bottomPanel";
            _bottomPanel.Padding = new Padding(10);
            _bottomPanel.Size = new Size(1131, 70);
            _bottomPanel.TabIndex = 3;
            // 
            // CalendarView
            // 
            BackColor = Color.White;
            Controls.Add(_rightPanel);
            Controls.Add(_headerPanel);
            Controls.Add(_bottomPanel);
            Controls.Add(_calendarPanel);
            Name = "CalendarView";
            Padding = new Padding(10);
            Size = new Size(1151, 771);
            _headerPanel.ResumeLayout(false);
            _calendarPanel.ResumeLayout(false);
            _rightPanel.ResumeLayout(false);
            _rightPanel.PerformLayout();
            _bottomPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
