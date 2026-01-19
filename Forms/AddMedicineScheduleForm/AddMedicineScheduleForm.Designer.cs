using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class AddMedicineScheduleForm
    {
        private IContainer components = null;

        private Label _lblMedicine;
        private ComboBox _cmbMedicine;
        private Label _lblInterval;
        private NumericUpDown _numInterval;
        private Label _lblMorning;
        private NumericUpDown _numMorning;
        private Label _lblEvening;
        private NumericUpDown _numEvening;
        private Label _lblStart;
        private DateTimePicker _dtpStart;
        private CheckBox _chkEnd;
        private DateTimePicker _dtpEnd;
        private Button _btnSave;
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
            _lblMedicine = new Label();
            _cmbMedicine = new ComboBox();
            _lblInterval = new Label();
            _numInterval = new NumericUpDown();
            _lblMorning = new Label();
            _numMorning = new NumericUpDown();
            _lblEvening = new Label();
            _numEvening = new NumericUpDown();
            _lblStart = new Label();
            _dtpStart = new DateTimePicker();
            _chkEnd = new CheckBox();
            _dtpEnd = new DateTimePicker();
            _btnSave = new Button();
            _btnCancel = new Button();
            _btnDelete = new Button();
            ((ISupportInitialize)_numInterval).BeginInit();
            ((ISupportInitialize)_numMorning).BeginInit();
            ((ISupportInitialize)_numEvening).BeginInit();
            SuspendLayout();
            // 
            // _lblMedicine
            // 
            _lblMedicine.AutoSize = true;
            _lblMedicine.Location = new Point(15, 20);
            _lblMedicine.Name = "_lblMedicine";
            _lblMedicine.Size = new Size(24, 15);
            _lblMedicine.TabIndex = 0;
            _lblMedicine.Text = "Medicine";
            // 
            // _cmbMedicine
            // 
            _cmbMedicine.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbMedicine.FormattingEnabled = true;
            _cmbMedicine.Location = new Point(150, 16);
            _cmbMedicine.Name = "_cmbMedicine";
            _cmbMedicine.Size = new Size(240, 23);
            _cmbMedicine.TabIndex = 1;
            // 
            // _lblInterval
            // 
            _lblInterval.AutoSize = true;
            _lblInterval.Location = new Point(15, 55);
            _lblInterval.Name = "_lblInterval";
            _lblInterval.Size = new Size(55, 15);
            _lblInterval.TabIndex = 2;
            _lblInterval.Text = "Every (days)";
            // 
            // _numInterval
            // 
            _numInterval.Location = new Point(150, 51);
            _numInterval.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            _numInterval.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numInterval.Name = "_numInterval";
            _numInterval.Size = new Size(80, 23);
            _numInterval.TabIndex = 3;
            _numInterval.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _lblMorning
            // 
            _lblMorning.AutoSize = true;
            _lblMorning.Location = new Point(15, 90);
            _lblMorning.Name = "_lblMorning";
            _lblMorning.Size = new Size(69, 15);
            _lblMorning.TabIndex = 4;
            _lblMorning.Text = "Morning dose";
            // 
            // _numMorning
            // 
            _numMorning.DecimalPlaces = 1;
            _numMorning.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            _numMorning.Location = new Point(150, 86);
            _numMorning.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _numMorning.Name = "_numMorning";
            _numMorning.Size = new Size(80, 23);
            _numMorning.TabIndex = 5;
            // 
            // _lblEvening
            // 
            _lblEvening.AutoSize = true;
            _lblEvening.Location = new Point(15, 125);
            _lblEvening.Name = "_lblEvening";
            _lblEvening.Size = new Size(86, 15);
            _lblEvening.TabIndex = 6;
            _lblEvening.Text = "Evening dose";
            // 
            // _numEvening
            // 
            _numEvening.DecimalPlaces = 1;
            _numEvening.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            _numEvening.Location = new Point(150, 121);
            _numEvening.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _numEvening.Name = "_numEvening";
            _numEvening.Size = new Size(80, 23);
            _numEvening.TabIndex = 7;
            // 
            // _lblStart
            // 
            _lblStart.AutoSize = true;
            _lblStart.Location = new Point(15, 160);
            _lblStart.Name = "_lblStart";
            _lblStart.Size = new Size(65, 15);
            _lblStart.TabIndex = 8;
            _lblStart.Text = "Start date";
            // 
            // _dtpStart
            // 
            _dtpStart.Format = DateTimePickerFormat.Short;
            _dtpStart.Location = new Point(150, 156);
            _dtpStart.Name = "_dtpStart";
            _dtpStart.Size = new Size(180, 23);
            _dtpStart.TabIndex = 9;
            // 
            // _chkEnd
            // 
            _chkEnd.AutoSize = true;
            _chkEnd.Location = new Point(15, 190);
            _chkEnd.Name = "_chkEnd";
            _chkEnd.Size = new Size(85, 19);
            _chkEnd.TabIndex = 10;
            _chkEnd.Text = "End date";
            _chkEnd.UseVisualStyleBackColor = true;
            // 
            // _dtpEnd
            // 
            _dtpEnd.Enabled = false;
            _dtpEnd.Format = DateTimePickerFormat.Short;
            _dtpEnd.Location = new Point(150, 186);
            _dtpEnd.Name = "_dtpEnd";
            _dtpEnd.Size = new Size(180, 23);
            _dtpEnd.TabIndex = 11;
            // 
            // _btnSave
            // 
            _btnSave.BackColor = Color.FromArgb(232, 245, 233);
            _btnSave.Cursor = Cursors.Hand;
            _btnSave.DialogResult = DialogResult.OK;
            _btnSave.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnSave.FlatAppearance.BorderSize = 2;
            _btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnSave.FlatStyle = FlatStyle.Flat;
            _btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnSave.ForeColor = Color.FromArgb(39, 174, 96);
            _btnSave.Location = new Point(230, 215);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(80, 32);
            _btnSave.TabIndex = 12;
            _btnSave.Text = "Save";
            _btnSave.UseVisualStyleBackColor = false;
            // 
            // _btnCancel
            // 
            _btnCancel.BackColor = Color.FromArgb(248, 250, 252);
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            _btnCancel.FlatAppearance.BorderSize = 2;
            _btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.ForeColor = Color.FromArgb(100, 100, 100);
            _btnCancel.Location = new Point(320, 215);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(80, 32);
            _btnCancel.TabIndex = 13;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // _btnDelete
            // 
            _btnDelete.BackColor = Color.FromArgb(255, 245, 245);
            _btnDelete.Cursor = Cursors.Hand;
            _btnDelete.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDelete.FlatAppearance.BorderSize = 2;
            _btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 230, 230);
            _btnDelete.FlatStyle = FlatStyle.Flat;
            _btnDelete.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnDelete.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDelete.Location = new Point(15, 215);
            _btnDelete.Name = "_btnDelete";
            _btnDelete.Size = new Size(80, 32);
            _btnDelete.TabIndex = 14;
            _btnDelete.Text = "Delete";
            _btnDelete.UseVisualStyleBackColor = false;
            _btnDelete.Visible = false;
            // 
            // AddMedicineScheduleForm
            // 
            AcceptButton = _btnSave;
            BackColor = Color.FromArgb(248, 250, 252);
            CancelButton = _btnCancel;
            ClientSize = new Size(420, 260);
            Controls.Add(_lblMedicine);
            Controls.Add(_cmbMedicine);
            Controls.Add(_lblInterval);
            Controls.Add(_numInterval);
            Controls.Add(_lblMorning);
            Controls.Add(_numMorning);
            Controls.Add(_lblEvening);
            Controls.Add(_numEvening);
            Controls.Add(_lblStart);
            Controls.Add(_dtpStart);
            Controls.Add(_chkEnd);
            Controls.Add(_dtpEnd);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);
            Controls.Add(_btnDelete);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddMedicineScheduleForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Medicine Schedule";
            ((ISupportInitialize)_numInterval).EndInit();
            ((ISupportInitialize)_numMorning).EndInit();
            ((ISupportInitialize)_numEvening).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
