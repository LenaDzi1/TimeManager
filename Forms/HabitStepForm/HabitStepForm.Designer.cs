using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class HabitStepForm
    {
        private IContainer components = null;

        private TableLayoutPanel _panel;
        private FlowLayoutPanel _buttonsPanel;
        private Label _lblName;
        private TextBox _txtName;
        private Label _lblStartDate;
        private DateTimePicker _dtStart;
        private CheckBox _chkHasEndDate;
        private DateTimePicker _dtEnd;
        private CheckBox _chkRepeat;
        private NumericUpDown _numRepeat;
        private Label _lblDescription;
        private TextBox _txtDescription;
        private CheckBox _chkDaytimeSplit;
        private Label _lblPoints;
        private NumericUpDown _numPoints;
        private Button _btnOk;
        private Button _btnCancel;
        private Button _btnDelete;

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
            _panel = new TableLayoutPanel();
            _lblName = new Label();
            _txtName = new TextBox();
            _lblStartDate = new Label();
            _dtStart = new DateTimePicker();
            _chkHasEndDate = new CheckBox();
            _dtEnd = new DateTimePicker();
            _chkRepeat = new CheckBox();
            _numRepeat = new NumericUpDown();
            _lblDescription = new Label();
            _txtDescription = new TextBox();
            _chkDaytimeSplit = new CheckBox();
            _lblPoints = new Label();
            _numPoints = new NumericUpDown();
            _buttonsPanel = new FlowLayoutPanel();
            _btnOk = new Button();
            _btnCancel = new Button();
            _btnDelete = new Button();
            _panel.SuspendLayout();
            ((ISupportInitialize)_numRepeat).BeginInit();
            ((ISupportInitialize)_numPoints).BeginInit();
            _buttonsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _panel
            // 
            _panel.AutoSize = true;
            _panel.ColumnCount = 2;
            _panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            _panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            _panel.Controls.Add(_lblName, 0, 0);
            _panel.Controls.Add(_txtName, 1, 0);
            _panel.Controls.Add(_lblStartDate, 0, 1);
            _panel.Controls.Add(_dtStart, 1, 1);
            _panel.Controls.Add(_chkHasEndDate, 0, 2);
            _panel.Controls.Add(_dtEnd, 1, 2);
            _panel.Controls.Add(_chkRepeat, 0, 3);
            _panel.Controls.Add(_numRepeat, 1, 3);
            _panel.Controls.Add(_lblDescription, 0, 4);
            _panel.Controls.Add(_txtDescription, 1, 4);
            _panel.Controls.Add(_chkDaytimeSplit, 0, 5);
            _panel.Controls.Add(_lblPoints, 0, 6);
            _panel.Controls.Add(_numPoints, 1, 6);
            _panel.Dock = DockStyle.Fill;
            _panel.Location = new Point(0, 0);
            _panel.Name = "_panel";
            _panel.Padding = new Padding(12);
            _panel.RowCount = 7;
            _panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            _panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            _panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            _panel.Size = new Size(480, 360);
            _panel.TabIndex = 0;
            // 
            // _lblName
            // 
            _lblName.Anchor = AnchorStyles.Left;
            _lblName.AutoSize = true;
            _lblName.Location = new Point(15, 16);
            _lblName.Name = "_lblName";
            _lblName.Padding = new Padding(0, 6, 0, 0);
            _lblName.Size = new Size(39, 21);
            _lblName.TabIndex = 0;
            _lblName.Text = "Name";
            // 
            // _txtName
            // 
            _txtName.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _txtName.Location = new Point(174, 15);
            _txtName.Name = "_txtName";
            _txtName.Size = new Size(291, 23);
            _txtName.TabIndex = 1;
            // 
            // _lblStartDate
            // 
            _lblStartDate.Anchor = AnchorStyles.Left;
            _lblStartDate.AutoSize = true;
            _lblStartDate.Location = new Point(15, 45);
            _lblStartDate.Name = "_lblStartDate";
            _lblStartDate.Padding = new Padding(0, 6, 0, 0);
            _lblStartDate.Size = new Size(57, 21);
            _lblStartDate.TabIndex = 2;
            _lblStartDate.Text = "Start date";
            // 
            // _dtStart
            // 
            _dtStart.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _dtStart.Format = DateTimePickerFormat.Short;
            _dtStart.Location = new Point(174, 44);
            _dtStart.Name = "_dtStart";
            _dtStart.Size = new Size(291, 23);
            _dtStart.TabIndex = 3;
            // 
            // _chkHasEndDate
            // 
            _chkHasEndDate.Anchor = AnchorStyles.Left;
            _chkHasEndDate.AutoSize = true;
            _chkHasEndDate.Location = new Point(15, 73);
            _chkHasEndDate.Name = "_chkHasEndDate";
            _chkHasEndDate.Size = new Size(100, 21);
            _chkHasEndDate.TabIndex = 4;
            _chkHasEndDate.Text = "End date";
            _chkHasEndDate.UseVisualStyleBackColor = true;
            // 
            // _dtEnd
            // 
            _dtEnd.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _dtEnd.Enabled = false;
            _dtEnd.Format = DateTimePickerFormat.Short;
            _dtEnd.Location = new Point(174, 73);
            _dtEnd.Name = "_dtEnd";
            _dtEnd.Size = new Size(291, 23);
            _dtEnd.TabIndex = 5;
            // 
            // _chkRepeat
            // 
            _chkRepeat.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _chkRepeat.AutoSize = true;
            _chkRepeat.Location = new Point(15, 104);
            _chkRepeat.Name = "_chkRepeat";
            _chkRepeat.Size = new Size(153, 19);
            _chkRepeat.TabIndex = 6;
            _chkRepeat.Text = "Repeat every (days)";
            _chkRepeat.UseVisualStyleBackColor = true;
            // 
            // _numRepeat
            // 
            _numRepeat.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _numRepeat.Enabled = false;
            _numRepeat.Location = new Point(174, 102);
            _numRepeat.Maximum = new decimal(new int[] { 365, 0, 0, 0 });
            _numRepeat.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numRepeat.Name = "_numRepeat";
            _numRepeat.Size = new Size(291, 23);
            _numRepeat.TabIndex = 8;
            _numRepeat.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _lblDescription
            // 
            _lblDescription.Anchor = AnchorStyles.Left;
            _lblDescription.AutoSize = true;
            _lblDescription.Location = new Point(15, 150);
            _lblDescription.Name = "_lblDescription";
            _lblDescription.Padding = new Padding(0, 6, 0, 0);
            _lblDescription.Size = new Size(67, 21);
            _lblDescription.TabIndex = 9;
            _lblDescription.Text = "Description";
            // 
            // _txtDescription
            // 
            _txtDescription.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _txtDescription.Location = new Point(174, 131);
            _txtDescription.Multiline = true;
            _txtDescription.Name = "_txtDescription";
            _txtDescription.ScrollBars = ScrollBars.Vertical;
            _txtDescription.Size = new Size(291, 60);
            _txtDescription.TabIndex = 10;
            // 
            // _chkDaytimeSplit
            // 
            _chkDaytimeSplit.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _chkDaytimeSplit.AutoSize = true;
            _chkDaytimeSplit.Location = new Point(15, 197);
            _chkDaytimeSplit.Name = "_chkDaytimeSplit";
            _chkDaytimeSplit.Size = new Size(153, 19);
            _chkDaytimeSplit.TabIndex = 11;
            _chkDaytimeSplit.Text = "Split into day/night";
            _chkDaytimeSplit.UseVisualStyleBackColor = true;
            // 
            // _lblPoints
            // 
            _lblPoints.Anchor = AnchorStyles.Left;
            _lblPoints.AutoSize = true;
            _lblPoints.Location = new Point(15, 273);
            _lblPoints.Name = "_lblPoints";
            _lblPoints.Padding = new Padding(0, 6, 0, 0);
            _lblPoints.Size = new Size(79, 21);
            _lblPoints.TabIndex = 12;
            _lblPoints.Text = "Points reward";
            // 
            // _numPoints
            // 
            _numPoints.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _numPoints.Location = new Point(174, 272);
            _numPoints.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            _numPoints.Name = "_numPoints";
            _numPoints.Size = new Size(291, 23);
            _numPoints.TabIndex = 13;
            // 
            // _buttonsPanel
            // 
            _buttonsPanel.Controls.Add(_btnOk);
            _buttonsPanel.Controls.Add(_btnCancel);
            _buttonsPanel.Controls.Add(_btnDelete);
            _buttonsPanel.Dock = DockStyle.Bottom;
            _buttonsPanel.FlowDirection = FlowDirection.RightToLeft;
            _buttonsPanel.Location = new Point(0, 360);
            _buttonsPanel.Name = "_buttonsPanel";
            _buttonsPanel.Padding = new Padding(12);
            _buttonsPanel.Size = new Size(480, 60);
            _buttonsPanel.TabIndex = 1;
            // 
            // _btnOk
            // 
            _btnOk.BackColor = Color.FromArgb(232, 245, 233);
            _btnOk.Cursor = Cursors.Hand;
            _btnOk.DialogResult = DialogResult.OK;
            _btnOk.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnOk.FlatAppearance.BorderSize = 2;
            _btnOk.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnOk.FlatStyle = FlatStyle.Flat;
            _btnOk.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnOk.ForeColor = Color.FromArgb(39, 174, 96);
            _btnOk.Location = new Point(363, 15);
            _btnOk.Name = "_btnOk";
            _btnOk.Size = new Size(90, 32);
            _btnOk.TabIndex = 0;
            _btnOk.Text = "Save";
            _btnOk.UseVisualStyleBackColor = false;
            // 
            // _btnCancel
            // 
            _btnCancel.BackColor = Color.FromArgb(248, 250, 252);
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            _btnCancel.FlatAppearance.BorderSize = 2;
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.ForeColor = Color.FromArgb(100, 100, 100);
            _btnCancel.Location = new Point(267, 15);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(90, 32);
            _btnCancel.TabIndex = 1;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // _btnDelete
            // 
            _btnDelete.BackColor = Color.FromArgb(255, 245, 245);
            _btnDelete.Cursor = Cursors.Hand;
            _btnDelete.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDelete.FlatAppearance.BorderSize = 2;
            _btnDelete.FlatStyle = FlatStyle.Flat;
            _btnDelete.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDelete.Location = new Point(171, 15);
            _btnDelete.Name = "_btnDelete";
            _btnDelete.Size = new Size(90, 32);
            _btnDelete.TabIndex = 2;
            _btnDelete.Text = "Delete";
            _btnDelete.UseVisualStyleBackColor = false;
            _btnDelete.Visible = false;
            // 
            // HabitStepForm
            // 
            AcceptButton = _btnOk;
            BackColor = Color.FromArgb(248, 250, 252);
            CancelButton = _btnCancel;
            ClientSize = new Size(480, 420);
            Controls.Add(_panel);
            Controls.Add(_buttonsPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "HabitStepForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Habit Step";
            _panel.ResumeLayout(false);
            _panel.PerformLayout();
            ((ISupportInitialize)_numRepeat).EndInit();
            ((ISupportInitialize)_numPoints).EndInit();
            _buttonsPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
