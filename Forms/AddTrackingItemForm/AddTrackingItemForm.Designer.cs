using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class AddTrackingItemForm
    {
        private IContainer components = null;
        private Label _lblProduct;
        private ComboBox _cmbProduct;
        private Button _btnNewProduct;
        private Button _btnDeleteProduct;
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
            _lblProduct = new Label();
            _cmbProduct = new ComboBox();
            _btnNewProduct = new Button();
            _btnDeleteProduct = new Button();
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
            // _lblProduct
            // 
            _lblProduct.AutoSize = true;
            _lblProduct.Location = new Point(20, 20);
            _lblProduct.Name = "_lblProduct";
            _lblProduct.Size = new Size(52, 15);
            _lblProduct.TabIndex = 0;
            _lblProduct.Text = "Product:";
            // 
            // _cmbProduct
            // 
            _cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbProduct.FormattingEnabled = true;
            _cmbProduct.Location = new Point(78, 17);
            _cmbProduct.Name = "_cmbProduct";
            _cmbProduct.Size = new Size(200, 23);
            _cmbProduct.TabIndex = 1;
            // 
            // _btnNewProduct
            // 
            _btnNewProduct.BackColor = Color.FromArgb(235, 245, 255);
            _btnNewProduct.Cursor = Cursors.Hand;
            _btnNewProduct.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnNewProduct.FlatAppearance.BorderSize = 2;
            _btnNewProduct.FlatStyle = FlatStyle.Flat;
            _btnNewProduct.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnNewProduct.ForeColor = Color.FromArgb(52, 152, 219);
            _btnNewProduct.Location = new Point(294, 17);
            _btnNewProduct.Name = "_btnNewProduct";
            _btnNewProduct.Size = new Size(30, 25);
            _btnNewProduct.TabIndex = 2;
            _btnNewProduct.Text = "+";
            _btnNewProduct.UseVisualStyleBackColor = false;
            // 
            // _btnDeleteProduct
            // 
            _btnDeleteProduct.BackColor = Color.FromArgb(255, 245, 245);
            _btnDeleteProduct.Cursor = Cursors.Hand;
            _btnDeleteProduct.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnDeleteProduct.FlatAppearance.BorderSize = 2;
            _btnDeleteProduct.FlatStyle = FlatStyle.Flat;
            _btnDeleteProduct.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnDeleteProduct.ForeColor = Color.FromArgb(231, 76, 60);
            _btnDeleteProduct.Location = new Point(330, 17);
            _btnDeleteProduct.Name = "_btnDeleteProduct";
            _btnDeleteProduct.Size = new Size(30, 25);
            _btnDeleteProduct.TabIndex = 3;
            _btnDeleteProduct.Text = "-";
            _btnDeleteProduct.UseVisualStyleBackColor = false;
            // 
            // _lblAmount
            // 
            _lblAmount.AutoSize = true;
            _lblAmount.Location = new Point(20, 60);
            _lblAmount.Name = "_lblAmount";
            _lblAmount.Size = new Size(54, 15);
            _lblAmount.TabIndex = 4;
            _lblAmount.Text = "Amount:";
            // 
            // _numAmount
            // 
            _numAmount.DecimalPlaces = 2;
            _numAmount.Location = new Point(90, 58);
            _numAmount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            _numAmount.Name = "_numAmount";
            _numAmount.Size = new Size(100, 23);
            _numAmount.TabIndex = 5;
            // 
            // _lblUnit
            // 
            _lblUnit.AutoSize = true;
            _lblUnit.Location = new Point(213, 61);
            _lblUnit.Name = "_lblUnit";
            _lblUnit.Size = new Size(32, 15);
            _lblUnit.TabIndex = 6;
            _lblUnit.Text = "Unit:";
            // 
            // _cmbUnit
            // 
            _cmbUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbUnit.FormattingEnabled = true;
            _cmbUnit.Items.AddRange(new object[] { "pcs", "ml", "g", "tablets", "capsules" });
            _cmbUnit.Location = new Point(251, 58);
            _cmbUnit.Name = "_cmbUnit";
            _cmbUnit.Size = new Size(90, 23);
            _cmbUnit.TabIndex = 7;
            // 
            // _chkHasExpire
            // 
            _chkHasExpire.AutoSize = true;
            _chkHasExpire.Checked = true;
            _chkHasExpire.CheckState = CheckState.Checked;
            _chkHasExpire.Location = new Point(20, 100);
            _chkHasExpire.Name = "_chkHasExpire";
            _chkHasExpire.Size = new Size(127, 19);
            _chkHasExpire.TabIndex = 8;
            _chkHasExpire.Text = "Has expiration date";
            _chkHasExpire.UseVisualStyleBackColor = true;
            // 
            // _dtpExpire
            // 
            _dtpExpire.Format = DateTimePickerFormat.Short;
            _dtpExpire.Location = new Point(180, 98);
            _dtpExpire.Name = "_dtpExpire";
            _dtpExpire.Size = new Size(180, 23);
            _dtpExpire.TabIndex = 9;
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
            _btnOk.Location = new Point(285, 143);
            _btnOk.Name = "_btnOk";
            _btnOk.Size = new Size(75, 30);
            _btnOk.TabIndex = 10;
            _btnOk.Text = "Add";
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
            _btnCancel.Location = new Point(20, 143);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(75, 30);
            _btnCancel.TabIndex = 11;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // AddTrackingItemForm
            // 
            AcceptButton = _btnOk;
            BackColor = Color.FromArgb(248, 250, 252);
            CancelButton = _btnCancel;
            ClientSize = new Size(380, 189);
            Controls.Add(_lblProduct);
            Controls.Add(_cmbProduct);
            Controls.Add(_btnNewProduct);
            Controls.Add(_btnDeleteProduct);
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
            Name = "AddTrackingItemForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Item";
            ((ISupportInitialize)_numAmount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
