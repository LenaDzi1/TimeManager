using System;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class AddEventForm
    {
        private Panel headerPanel;
        private Label lblTitle;
        
        private Label labelTitle;
        private TextBox txtTitle;
        private Label labelDescription;
        private TextBox txtDescription;
        private Label labelDuration;
        private NumericUpDown nudDuration;
        private Label labelCategory;
        private ComboBox cmbCategory;
        private Label labelStartDate;
        private DateTimePicker dtpStartDate;
        private Label labelStartTime;
        private DateTimePicker dtpStartTime;
        private Label labelEndDate;
        private DateTimePicker dtpEndDate;
        private Label labelEndTime;
        private DateTimePicker dtpEndTime;
        private Label labelDeadline;
        private DateTimePicker dtpDeadline;
        private CheckBox chkDeadline;
        private CheckBox chkRecurring;
        private Label lblRecurringEach;
        private NumericUpDown nudRecurringDays;
        private Label lblRecurringDays;
        private Label lblRecurringUntil;
        private DateTimePicker dtpRecurringUntil;
        private CheckBox chkImportant;
        private CheckBox chkDontDoOn;
        private ComboBox cmbDayOfWeek;
        private CheckBox chkUrgent;
        private CheckBox chkPromise;
        private Label lblPromise;
        private TextBox txtPromisedTo;
        private CheckBox chkSetEvent;
        private CheckBox chkNoFixedTime;
        private CheckBox chkMargin;
        private NumericUpDown nudMargin;
        private CheckBox chkDoInDaytimeUntil;
        private DateTimePicker dtpDoInDaytimeUntil;
        private CheckBox chkRequireSupply;
        private Button btnSelectSupply;
        private CheckBox chkAtLeast;
        private NumericUpDown nudAtLeastCount;
        private Label lblAtLeastEach;
        private NumericUpDown nudAtLeastPeriod;
        private Label lblAtLeastDays;
        private Button btnClose;

        // Buttons
        private Button btnSave;
        private Button btnCancel;
        private Panel schedulePanel;
        private Panel flexiblePanel;
        
        private Panel contentPanel;

        private void InitializeComponent()
        {
            headerPanel = new Panel();
            lblTitle = new Label();
            btnClose = new Button();
            contentPanel = new Panel();
            labelTitle = new Label();
            txtTitle = new TextBox();
            labelDescription = new Label();
            txtDescription = new TextBox();
            labelCategory = new Label();
            cmbCategory = new ComboBox();
            labelDuration = new Label();
            nudDuration = new NumericUpDown();
            chkSetEvent = new CheckBox();
            schedulePanel = new Panel();
            labelStartDate = new Label();
            dtpStartDate = new DateTimePicker();
            labelStartTime = new Label();
            dtpStartTime = new DateTimePicker();
            labelEndDate = new Label();
            dtpEndDate = new DateTimePicker();
            labelEndTime = new Label();
            dtpEndTime = new DateTimePicker();
            chkRecurring = new CheckBox();
            lblRecurringEach = new Label();
            nudRecurringDays = new NumericUpDown();
            lblRecurringDays = new Label();
            lblRecurringUntil = new Label();
            dtpRecurringUntil = new DateTimePicker();
            flexiblePanel = new Panel();
            chkDeadline = new CheckBox();
            dtpDeadline = new DateTimePicker();
            chkImportant = new CheckBox();
            chkUrgent = new CheckBox();
            chkDoInDaytimeUntil = new CheckBox();
            dtpDoInDaytimeUntil = new DateTimePicker();
            chkPromise = new CheckBox();
            lblPromise = new Label();
            txtPromisedTo = new TextBox();
            chkRequireSupply = new CheckBox();
            btnSelectSupply = new Button();
            chkDontDoOn = new CheckBox();
            cmbDayOfWeek = new ComboBox();
            chkAtLeast = new CheckBox();
            nudAtLeastCount = new NumericUpDown();
            lblAtLeastEach = new Label();
            nudAtLeastPeriod = new NumericUpDown();
            lblAtLeastDays = new Label();
            labelDeadline = new Label();
            btnSave = new Button();
            btnCancel = new Button();
            chkNoFixedTime = new CheckBox();
            chkMargin = new CheckBox();
            nudMargin = new NumericUpDown();
            headerPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudDuration).BeginInit();
            schedulePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRecurringDays).BeginInit();
            flexiblePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudAtLeastCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAtLeastPeriod).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudMargin).BeginInit();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(135, 206, 250);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(520, 50);
            headerPanel.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(144, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Add New Event";
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.Transparent;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 76, 60);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(480, 8);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(32, 32);
            btnClose.TabIndex = 1;
            btnClose.Text = "×";
            btnClose.UseVisualStyleBackColor = false;
            // 
            // contentPanel
            // 
            contentPanel.BackColor = Color.FromArgb(250, 250, 252);
            contentPanel.Controls.Add(labelTitle);
            contentPanel.Controls.Add(txtTitle);
            contentPanel.Controls.Add(labelDescription);
            contentPanel.Controls.Add(txtDescription);
            contentPanel.Controls.Add(labelCategory);
            contentPanel.Controls.Add(cmbCategory);
            contentPanel.Controls.Add(labelDuration);
            contentPanel.Controls.Add(nudDuration);
            contentPanel.Controls.Add(chkSetEvent);
            contentPanel.Controls.Add(schedulePanel);
            contentPanel.Controls.Add(flexiblePanel);
            contentPanel.Controls.Add(btnSave);
            contentPanel.Controls.Add(btnCancel);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(0, 50);
            contentPanel.Name = "contentPanel";
            contentPanel.Padding = new Padding(20, 15, 20, 15);
            contentPanel.Size = new Size(520, 450);
            contentPanel.TabIndex = 1;
            // 
            // labelTitle
            // 
            labelTitle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            labelTitle.ForeColor = Color.FromArgb(44, 62, 80);
            labelTitle.Location = new Point(20, 15);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(200, 18);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "Event Title *";
            // 
            // txtTitle
            // 
            txtTitle.BackColor = Color.White;
            txtTitle.BorderStyle = BorderStyle.FixedSingle;
            txtTitle.Font = new Font("Segoe UI", 10F);
            txtTitle.Location = new Point(20, 35);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(478, 25);
            txtTitle.TabIndex = 1;
            // 
            // labelDescription
            // 
            labelDescription.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            labelDescription.ForeColor = Color.FromArgb(44, 62, 80);
            labelDescription.Location = new Point(20, 65);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(200, 18);
            labelDescription.TabIndex = 2;
            labelDescription.Text = "Description";
            // 
            // txtDescription
            // 
            txtDescription.BackColor = Color.White;
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.Font = new Font("Segoe UI", 10F);
            txtDescription.Location = new Point(20, 85);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(478, 55);
            txtDescription.TabIndex = 3;
            // 
            // labelCategory
            // 
            labelCategory.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            labelCategory.ForeColor = Color.FromArgb(44, 62, 80);
            labelCategory.Location = new Point(20, 148);
            labelCategory.Name = "labelCategory";
            labelCategory.Size = new Size(100, 18);
            labelCategory.TabIndex = 4;
            labelCategory.Text = "Category";
            // 
            // cmbCategory
            // 
            cmbCategory.BackColor = Color.White;
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.FlatStyle = FlatStyle.Flat;
            cmbCategory.Font = new Font("Segoe UI", 9F);
            cmbCategory.Items.AddRange(new object[] { "health", "family", "mentality", "finance", "work and career", "relax", "self development and education", "friends and people", "None" });
            cmbCategory.Location = new Point(20, 168);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(220, 23);
            cmbCategory.TabIndex = 5;
            // 
            // labelDuration
            // 
            labelDuration.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            labelDuration.ForeColor = Color.FromArgb(44, 62, 80);
            labelDuration.Location = new Point(260, 148);
            labelDuration.Name = "labelDuration";
            labelDuration.Size = new Size(130, 18);
            labelDuration.TabIndex = 6;
            labelDuration.Text = "Duration (minutes)";
            // 
            // nudDuration
            // 
            nudDuration.BackColor = Color.White;
            nudDuration.Font = new Font("Segoe UI", 9F);
            nudDuration.Location = new Point(260, 168);
            nudDuration.Maximum = new decimal(new int[] { 1440, 0, 0, 0 });
            nudDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudDuration.Name = "nudDuration";
            nudDuration.Size = new Size(90, 23);
            nudDuration.TabIndex = 7;
            nudDuration.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // chkSetEvent
            // 
            chkSetEvent.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            chkSetEvent.ForeColor = Color.FromArgb(41, 128, 185);
            chkSetEvent.Location = new Point(20, 200);
            chkSetEvent.Name = "chkSetEvent";
            chkSetEvent.Size = new Size(150, 22);
            chkSetEvent.TabIndex = 8;
            chkSetEvent.Text = "Fixed event";
            // 
            // schedulePanel
            // 
            schedulePanel.BackColor = Color.FromArgb(236, 240, 241);
            schedulePanel.Controls.Add(labelStartDate);
            schedulePanel.Controls.Add(dtpStartDate);
            schedulePanel.Controls.Add(labelStartTime);
            schedulePanel.Controls.Add(dtpStartTime);
            schedulePanel.Controls.Add(labelEndDate);
            schedulePanel.Controls.Add(dtpEndDate);
            schedulePanel.Controls.Add(labelEndTime);
            schedulePanel.Controls.Add(dtpEndTime);
            schedulePanel.Controls.Add(chkRecurring);
            schedulePanel.Controls.Add(lblRecurringEach);
            schedulePanel.Controls.Add(nudRecurringDays);
            schedulePanel.Controls.Add(lblRecurringDays);
            schedulePanel.Controls.Add(lblRecurringUntil);
            schedulePanel.Controls.Add(dtpRecurringUntil);
            schedulePanel.Location = new Point(20, 228);
            schedulePanel.Name = "schedulePanel";
            schedulePanel.Padding = new Padding(10);
            schedulePanel.Size = new Size(478, 140);
            schedulePanel.TabIndex = 9;
            // 
            // labelStartDate
            // 
            labelStartDate.Font = new Font("Segoe UI", 8F);
            labelStartDate.ForeColor = Color.FromArgb(100, 100, 100);
            labelStartDate.Location = new Point(10, 8);
            labelStartDate.Name = "labelStartDate";
            labelStartDate.Size = new Size(100, 16);
            labelStartDate.TabIndex = 0;
            labelStartDate.Text = "Start Date";
            // 
            // dtpStartDate
            // 
            dtpStartDate.Font = new Font("Segoe UI", 9F);
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.Location = new Point(10, 26);
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new Size(140, 23);
            dtpStartDate.TabIndex = 1;
            // 
            // labelStartTime
            // 
            labelStartTime.Font = new Font("Segoe UI", 8F);
            labelStartTime.ForeColor = Color.FromArgb(100, 100, 100);
            labelStartTime.Location = new Point(10, 55);
            labelStartTime.Name = "labelStartTime";
            labelStartTime.Size = new Size(100, 16);
            labelStartTime.TabIndex = 4;
            labelStartTime.Text = "Start Time";
            // 
            // dtpStartTime
            // 
            dtpStartTime.Font = new Font("Segoe UI", 9F);
            dtpStartTime.Format = DateTimePickerFormat.Time;
            dtpStartTime.Location = new Point(10, 73);
            dtpStartTime.Name = "dtpStartTime";
            dtpStartTime.ShowUpDown = true;
            dtpStartTime.Size = new Size(140, 23);
            dtpStartTime.TabIndex = 5;
            // 
            // labelEndDate
            // 
            labelEndDate.Font = new Font("Segoe UI", 8F);
            labelEndDate.ForeColor = Color.FromArgb(100, 100, 100);
            labelEndDate.Location = new Point(250, 8);
            labelEndDate.Name = "labelEndDate";
            labelEndDate.Size = new Size(100, 16);
            labelEndDate.TabIndex = 2;
            labelEndDate.Text = "End Date";
            // 
            // dtpEndDate
            // 
            dtpEndDate.Font = new Font("Segoe UI", 9F);
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Location = new Point(250, 26);
            dtpEndDate.Name = "dtpEndDate";
            dtpEndDate.Size = new Size(140, 23);
            dtpEndDate.TabIndex = 3;
            // 
            // labelEndTime
            // 
            labelEndTime.Font = new Font("Segoe UI", 8F);
            labelEndTime.ForeColor = Color.FromArgb(100, 100, 100);
            labelEndTime.Location = new Point(250, 55);
            labelEndTime.Name = "labelEndTime";
            labelEndTime.Size = new Size(100, 16);
            labelEndTime.TabIndex = 6;
            labelEndTime.Text = "End Time";
            // 
            // dtpEndTime
            // 
            dtpEndTime.Font = new Font("Segoe UI", 9F);
            dtpEndTime.Format = DateTimePickerFormat.Time;
            dtpEndTime.Location = new Point(250, 73);
            dtpEndTime.Name = "dtpEndTime";
            dtpEndTime.ShowUpDown = true;
            dtpEndTime.Size = new Size(140, 23);
            dtpEndTime.TabIndex = 7;
            // 
            // chkRecurring
            // 
            chkRecurring.Font = new Font("Segoe UI", 9F);
            chkRecurring.ForeColor = Color.FromArgb(44, 62, 80);
            chkRecurring.Location = new Point(10, 108);
            chkRecurring.Name = "chkRecurring";
            chkRecurring.Size = new Size(85, 20);
            chkRecurring.TabIndex = 8;
            chkRecurring.Text = "Repeat";
            // 
            // lblRecurringEach
            // 
            lblRecurringEach.Enabled = false;
            lblRecurringEach.Font = new Font("Segoe UI", 9F);
            lblRecurringEach.Location = new Point(100, 109);
            lblRecurringEach.Name = "lblRecurringEach";
            lblRecurringEach.Size = new Size(40, 18);
            lblRecurringEach.TabIndex = 9;
            lblRecurringEach.Text = "every";
            // 
            // nudRecurringDays
            // 
            nudRecurringDays.Enabled = false;
            nudRecurringDays.Font = new Font("Segoe UI", 9F);
            nudRecurringDays.Location = new Point(145, 106);
            nudRecurringDays.Maximum = new decimal(new int[] { 365, 0, 0, 0 });
            nudRecurringDays.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudRecurringDays.Name = "nudRecurringDays";
            nudRecurringDays.Size = new Size(55, 23);
            nudRecurringDays.TabIndex = 10;
            nudRecurringDays.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblRecurringDays
            // 
            lblRecurringDays.Enabled = false;
            lblRecurringDays.Font = new Font("Segoe UI", 9F);
            lblRecurringDays.Location = new Point(205, 109);
            lblRecurringDays.Name = "lblRecurringDays";
            lblRecurringDays.Size = new Size(40, 18);
            lblRecurringDays.TabIndex = 11;
            lblRecurringDays.Text = "days";
            // 
            // lblRecurringUntil
            // 
            lblRecurringUntil.Enabled = false;
            lblRecurringUntil.Font = new Font("Segoe UI", 9F);
            lblRecurringUntil.Location = new Point(250, 109);
            lblRecurringUntil.Name = "lblRecurringUntil";
            lblRecurringUntil.Size = new Size(35, 18);
            lblRecurringUntil.TabIndex = 12;
            lblRecurringUntil.Text = "until";
            // 
            // dtpRecurringUntil
            // 
            dtpRecurringUntil.Enabled = false;
            dtpRecurringUntil.Font = new Font("Segoe UI", 9F);
            dtpRecurringUntil.Format = DateTimePickerFormat.Short;
            dtpRecurringUntil.Location = new Point(290, 106);
            dtpRecurringUntil.Name = "dtpRecurringUntil";
            dtpRecurringUntil.Size = new Size(110, 23);
            dtpRecurringUntil.TabIndex = 13;
            // 
            // flexiblePanel
            // 
            flexiblePanel.BackColor = Color.FromArgb(236, 240, 241);
            flexiblePanel.Controls.Add(chkDeadline);
            flexiblePanel.Controls.Add(dtpDeadline);
            flexiblePanel.Controls.Add(chkImportant);
            flexiblePanel.Controls.Add(chkUrgent);
            flexiblePanel.Controls.Add(chkDoInDaytimeUntil);
            flexiblePanel.Controls.Add(dtpDoInDaytimeUntil);
            flexiblePanel.Controls.Add(chkPromise);
            flexiblePanel.Controls.Add(lblPromise);
            flexiblePanel.Controls.Add(txtPromisedTo);
            flexiblePanel.Controls.Add(chkRequireSupply);
            flexiblePanel.Controls.Add(btnSelectSupply);
            flexiblePanel.Controls.Add(chkDontDoOn);
            flexiblePanel.Controls.Add(cmbDayOfWeek);
            flexiblePanel.Controls.Add(chkAtLeast);
            flexiblePanel.Controls.Add(nudAtLeastCount);
            flexiblePanel.Controls.Add(lblAtLeastEach);
            flexiblePanel.Controls.Add(nudAtLeastPeriod);
            flexiblePanel.Controls.Add(lblAtLeastDays);
            flexiblePanel.Controls.Add(labelDeadline);
            flexiblePanel.Location = new Point(20, 228);
            flexiblePanel.Name = "flexiblePanel";
            flexiblePanel.Padding = new Padding(10);
            flexiblePanel.Size = new Size(478, 140);
            flexiblePanel.TabIndex = 10;
            flexiblePanel.Visible = false;
            // 
            // chkDeadline
            // 
            chkDeadline.Font = new Font("Segoe UI", 9F);
            chkDeadline.ForeColor = Color.FromArgb(44, 62, 80);
            chkDeadline.Location = new Point(10, 10);
            chkDeadline.Name = "chkDeadline";
            chkDeadline.Size = new Size(90, 20);
            chkDeadline.TabIndex = 0;
            chkDeadline.Text = "Deadline";
            // 
            // dtpDeadline
            // 
            dtpDeadline.Enabled = false;
            dtpDeadline.Font = new Font("Segoe UI", 9F);
            dtpDeadline.Format = DateTimePickerFormat.Short;
            dtpDeadline.Location = new Point(110, 8);
            dtpDeadline.Name = "dtpDeadline";
            dtpDeadline.Size = new Size(130, 23);
            dtpDeadline.TabIndex = 1;
            // 
            // chkImportant
            // 
            chkImportant.Font = new Font("Segoe UI", 9F);
            chkImportant.ForeColor = Color.FromArgb(44, 62, 80);
            chkImportant.Location = new Point(10, 40);
            chkImportant.Name = "chkImportant";
            chkImportant.Size = new Size(120, 20);
            chkImportant.TabIndex = 2;
            chkImportant.Text = "Important";
            // 
            // chkUrgent
            // 
            chkUrgent.Font = new Font("Segoe UI", 9F);
            chkUrgent.ForeColor = Color.FromArgb(44, 62, 80);
            chkUrgent.Location = new Point(140, 40);
            chkUrgent.Name = "chkUrgent";
            chkUrgent.Size = new Size(100, 20);
            chkUrgent.TabIndex = 3;
            chkUrgent.Text = "Urgent";
            // 
            // chkDoInDaytimeUntil
            // 
            chkDoInDaytimeUntil.Font = new Font("Segoe UI", 9F);
            chkDoInDaytimeUntil.ForeColor = Color.FromArgb(44, 62, 80);
            chkDoInDaytimeUntil.Location = new Point(10, 70);
            chkDoInDaytimeUntil.Name = "chkDoInDaytimeUntil";
            chkDoInDaytimeUntil.Size = new Size(150, 20);
            chkDoInDaytimeUntil.TabIndex = 4;
            chkDoInDaytimeUntil.Text = "Do until (daytime)";
            // 
            // dtpDoInDaytimeUntil
            // 
            dtpDoInDaytimeUntil.Enabled = false;
            dtpDoInDaytimeUntil.Font = new Font("Segoe UI", 9F);
            dtpDoInDaytimeUntil.Format = DateTimePickerFormat.Time;
            dtpDoInDaytimeUntil.Location = new Point(170, 68);
            dtpDoInDaytimeUntil.Name = "dtpDoInDaytimeUntil";
            dtpDoInDaytimeUntil.ShowUpDown = true;
            dtpDoInDaytimeUntil.Size = new Size(100, 23);
            dtpDoInDaytimeUntil.TabIndex = 5;
            // 
            // chkPromise
            // 
            chkPromise.Location = new Point(0, 0);
            chkPromise.Name = "chkPromise";
            chkPromise.Size = new Size(104, 24);
            chkPromise.TabIndex = 6;
            chkPromise.Visible = false;
            // 
            // lblPromise
            // 
            lblPromise.Location = new Point(0, 0);
            lblPromise.Name = "lblPromise";
            lblPromise.Size = new Size(100, 23);
            lblPromise.TabIndex = 7;
            lblPromise.Visible = false;
            // 
            // txtPromisedTo
            // 
            txtPromisedTo.Location = new Point(0, 0);
            txtPromisedTo.Name = "txtPromisedTo";
            txtPromisedTo.Size = new Size(100, 23);
            txtPromisedTo.TabIndex = 8;
            txtPromisedTo.Visible = false;
            // 
            // chkRequireSupply
            // 
            chkRequireSupply.Location = new Point(0, 0);
            chkRequireSupply.Name = "chkRequireSupply";
            chkRequireSupply.Size = new Size(104, 24);
            chkRequireSupply.TabIndex = 9;
            chkRequireSupply.Visible = false;
            // 
            // btnSelectSupply
            // 
            btnSelectSupply.Location = new Point(0, 0);
            btnSelectSupply.Name = "btnSelectSupply";
            btnSelectSupply.Size = new Size(75, 23);
            btnSelectSupply.TabIndex = 10;
            btnSelectSupply.Visible = false;
            // 
            // chkDontDoOn
            // 
            chkDontDoOn.Location = new Point(0, 0);
            chkDontDoOn.Name = "chkDontDoOn";
            chkDontDoOn.Size = new Size(104, 24);
            chkDontDoOn.TabIndex = 11;
            chkDontDoOn.Visible = false;
            // 
            // cmbDayOfWeek
            // 
            cmbDayOfWeek.Location = new Point(0, 0);
            cmbDayOfWeek.Name = "cmbDayOfWeek";
            cmbDayOfWeek.Size = new Size(121, 23);
            cmbDayOfWeek.TabIndex = 12;
            cmbDayOfWeek.Visible = false;
            // 
            // chkAtLeast
            // 
            chkAtLeast.Location = new Point(0, 0);
            chkAtLeast.Name = "chkAtLeast";
            chkAtLeast.Size = new Size(104, 24);
            chkAtLeast.TabIndex = 13;
            chkAtLeast.Visible = false;
            // 
            // nudAtLeastCount
            // 
            nudAtLeastCount.Location = new Point(0, 0);
            nudAtLeastCount.Name = "nudAtLeastCount";
            nudAtLeastCount.Size = new Size(120, 23);
            nudAtLeastCount.TabIndex = 14;
            nudAtLeastCount.Visible = false;
            // 
            // lblAtLeastEach
            // 
            lblAtLeastEach.Location = new Point(0, 0);
            lblAtLeastEach.Name = "lblAtLeastEach";
            lblAtLeastEach.Size = new Size(100, 23);
            lblAtLeastEach.TabIndex = 15;
            lblAtLeastEach.Visible = false;
            // 
            // nudAtLeastPeriod
            // 
            nudAtLeastPeriod.Location = new Point(0, 0);
            nudAtLeastPeriod.Name = "nudAtLeastPeriod";
            nudAtLeastPeriod.Size = new Size(120, 23);
            nudAtLeastPeriod.TabIndex = 16;
            nudAtLeastPeriod.Visible = false;
            // 
            // lblAtLeastDays
            // 
            lblAtLeastDays.Location = new Point(0, 0);
            lblAtLeastDays.Name = "lblAtLeastDays";
            lblAtLeastDays.Size = new Size(100, 23);
            lblAtLeastDays.TabIndex = 17;
            lblAtLeastDays.Visible = false;
            // 
            // labelDeadline
            // 
            labelDeadline.Location = new Point(0, 0);
            labelDeadline.Name = "labelDeadline";
            labelDeadline.Size = new Size(1, 1);
            labelDeadline.TabIndex = 18;
            labelDeadline.Visible = false;
            // 
            // btnSave - Outline style with light fill
            // 
            btnSave.BackColor = Color.FromArgb(232, 245, 233);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            btnSave.FlatAppearance.BorderSize = 2;
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnSave.ForeColor = Color.FromArgb(39, 174, 96);
            btnSave.Location = new Point(20, 380);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(130, 38);
            btnSave.TabIndex = 11;
            btnSave.Text = "Save Event";
            btnSave.UseVisualStyleBackColor = false;
            // 
            // btnCancel - Outline style (gray border, gray text)
            // 
            btnCancel.BackColor = Color.White;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            btnCancel.FlatAppearance.BorderSize = 2;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 10F);
            btnCancel.ForeColor = Color.FromArgb(127, 140, 141);
            btnCancel.Location = new Point(368, 380);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(130, 38);
            btnCancel.TabIndex = 12;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // chkNoFixedTime
            // 
            chkNoFixedTime.Location = new Point(0, 0);
            chkNoFixedTime.Name = "chkNoFixedTime";
            chkNoFixedTime.Size = new Size(104, 24);
            chkNoFixedTime.TabIndex = 0;
            chkNoFixedTime.Visible = false;
            // 
            // chkMargin
            // 
            chkMargin.Location = new Point(0, 0);
            chkMargin.Name = "chkMargin";
            chkMargin.Size = new Size(104, 24);
            chkMargin.TabIndex = 0;
            chkMargin.Visible = false;
            // 
            // nudMargin
            // 
            nudMargin.Location = new Point(0, 0);
            nudMargin.Name = "nudMargin";
            nudMargin.Size = new Size(120, 23);
            nudMargin.TabIndex = 0;
            nudMargin.Visible = false;
            // 
            // AddEventForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(250, 250, 252);
            ClientSize = new Size(520, 500);
            Controls.Add(contentPanel);
            Controls.Add(headerPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddEventForm";
            StartPosition = FormStartPosition.CenterParent;
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            contentPanel.ResumeLayout(false);
            contentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudDuration).EndInit();
            schedulePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)nudRecurringDays).EndInit();
            flexiblePanel.ResumeLayout(false);
            flexiblePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudAtLeastCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAtLeastPeriod).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudMargin).EndInit();
            ResumeLayout(false);
        }
    }
}
