using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class EditTrackingItemForm
    {
        private IContainer components = null;
        private Label _lblProductName;
        private Label _lblAmount;
        private NumericUpDown _numAmount;
        private Label _lblUnit;
        private ComboBox _cmbUnit;
        private CheckBox _chkHasExpire;
        private DateTimePicker _dtpExpire;
        private Button _btnOk;
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
            _lblProductName = new Label();
            _lblAmount = new Label();
            _numAmount = new NumericUpDown();
            _lblUnit = new Label();
            _cmbUnit = new ComboBox();
            _chkHasExpire = new CheckBox();
            _dtpExpire = new DateTimePicker();
            _btnOk = new Button();
            _btnCancel = new Button();
            ((ISupportInitialize)_numAmount).BeginInit();
            SuspendLayout();
            // 
            // _lblProductName
            // 
            _lblProductName.AutoSize = true;
            _lblProductName.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            _lblProductName.ForeColor = Color.FromArgb(44, 62, 80);
            _lblProductName.Location = new Point(20, 20);
            _lblProductName.Name = "_lblProductName";
            _lblProductName.Size = new Size(62, 19);
            _lblProductName.TabIndex = 0;
            _lblProductName.Text = "Product";
            // 
            // _lblAmount
            // 
            _lblAmount.AutoSize = true;
            _lblAmount.Location = new Point(20, 60);
            _lblAmount.Name = "_lblAmount";
            _lblAmount.Size = new Size(54, 15);
            _lblAmount.TabIndex = 1;
            _lblAmount.Text = "Amount:";
            // 
            // _numAmount
            // 
            _numAmount.DecimalPlaces = 2;
            _numAmount.Location = new Point(91, 58);
            _numAmount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            _numAmount.Name = "_numAmount";
            _numAmount.Size = new Size(100, 23);
            _numAmount.TabIndex = 2;
            // 
            // _lblUnit
            // 
            _lblUnit.AutoSize = true;
            _lblUnit.Location = new Point(214, 60);
            _lblUnit.Name = "_lblUnit";
            _lblUnit.Size = new Size(32, 15);
            _lblUnit.TabIndex = 3;
            _lblUnit.Text = "Unit:";
            // 
            // _cmbUnit
            // 
            _cmbUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbUnit.FormattingEnabled = true;
            _cmbUnit.Items.AddRange(new object[] { "pcs", "ml", "g", "tablets", "capsules" });
            _cmbUnit.Location = new Point(270, 57);
            _cmbUnit.Name = "_cmbUnit";
            _cmbUnit.Size = new Size(90, 23);
            _cmbUnit.TabIndex = 4;
            // 
            // _chkHasExpire
            // 
            _chkHasExpire.AutoSize = true;
            _chkHasExpire.Location = new Point(20, 100);
            _chkHasExpire.Name = "_chkHasExpire";
            _chkHasExpire.Size = new Size(127, 19);
            _chkHasExpire.TabIndex = 5;
            _chkHasExpire.Text = "Has expiration date";
            _chkHasExpire.UseVisualStyleBackColor = true;
            // 
            // _dtpExpire
            // 
            _dtpExpire.Format = DateTimePickerFormat.Short;
            _dtpExpire.Location = new Point(180, 98);
            _dtpExpire.Name = "_dtpExpire";
            _dtpExpire.Size = new Size(180, 23);
            _dtpExpire.TabIndex = 6;
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
            _btnOk.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnOk.ForeColor = Color.FromArgb(39, 174, 96);
            _btnOk.Location = new Point(285, 138);
            _btnOk.Name = "_btnOk";
            _btnOk.Size = new Size(75, 30);
            _btnOk.TabIndex = 7;
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
            _btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.ForeColor = Color.FromArgb(100, 100, 100);
            _btnCancel.Location = new Point(20, 138);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(75, 30);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // EditMedicineItemForm
            // 
            AcceptButton = _btnOk;
            BackColor = Color.FromArgb(248, 250, 252);
            CancelButton = _btnCancel;
            ClientSize = new Size(382, 177);
            Controls.Add(_lblProductName);
            Controls.Add(_lblAmount);
            Controls.Add(_numAmount);
            Controls.Add(_lblUnit);
            Controls.Add(_cmbUnit);
            Controls.Add(_chkHasExpire);
            Controls.Add(_dtpExpire);
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditMedicineItemForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit Item";
            ((ISupportInitialize)_numAmount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
