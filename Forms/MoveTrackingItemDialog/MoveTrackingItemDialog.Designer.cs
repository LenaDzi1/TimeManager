using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class MoveTrackingItemDialog
    {
        private IContainer components = null;
        private Label _lblTarget;
        private ComboBox _cmbTargetContainer;
        private Label _lblAmount;
        private NumericUpDown _numAmount;
        private Label _lblUnit;
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
            _lblTarget = new Label();
            _cmbTargetContainer = new ComboBox();
            _lblAmount = new Label();
            _numAmount = new NumericUpDown();
            _lblUnit = new Label();
            _btnOk = new Button();
            _btnCancel = new Button();
            ((ISupportInitialize)_numAmount).BeginInit();
            SuspendLayout();
            // 
            // _lblTarget
            // 
            _lblTarget.AutoSize = true;
            _lblTarget.Location = new Point(15, 20);
            _lblTarget.Name = "_lblTarget";
            _lblTarget.Size = new Size(96, 15);
            _lblTarget.TabIndex = 0;
            _lblTarget.Text = "Target container:";
            // 
            // _cmbTargetContainer
            // 
            _cmbTargetContainer.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbTargetContainer.FormattingEnabled = true;
            _cmbTargetContainer.Location = new Point(140, 16);
            _cmbTargetContainer.Name = "_cmbTargetContainer";
            _cmbTargetContainer.Size = new Size(190, 23);
            _cmbTargetContainer.TabIndex = 1;
            // 
            // _lblAmount
            // 
            _lblAmount.AutoSize = true;
            _lblAmount.Location = new Point(15, 60);
            _lblAmount.Name = "_lblAmount";
            _lblAmount.Size = new Size(101, 15);
            _lblAmount.TabIndex = 2;
            _lblAmount.Text = "Amount to move:";
            // 
            // _numAmount
            // 
            _numAmount.DecimalPlaces = 2;
            _numAmount.Increment = new decimal(new int[] { 10, 0, 0, 131072 });
            _numAmount.Location = new Point(140, 56);
            _numAmount.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            _numAmount.Name = "_numAmount";
            _numAmount.Size = new Size(80, 23);
            _numAmount.TabIndex = 3;
            _numAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _lblUnit
            // 
            _lblUnit.AutoSize = true;
            _lblUnit.Location = new Point(226, 60);
            _lblUnit.Name = "_lblUnit";
            _lblUnit.Size = new Size(29, 15);
            _lblUnit.TabIndex = 4;
            _lblUnit.Text = "unit";
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
            _btnOk.Location = new Point(260, 110);
            _btnOk.Name = "_btnOk";
            _btnOk.Size = new Size(80, 32);
            _btnOk.TabIndex = 5;
            _btnOk.Text = "Move";
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
            _btnCancel.Location = new Point(15, 110);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(80, 32);
            _btnCancel.TabIndex = 6;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // MoveMedicineItemDialog
            // 
            AcceptButton = _btnOk;
            BackColor = Color.FromArgb(248, 250, 252);
            CancelButton = _btnCancel;
            ClientSize = new Size(360, 160);
            Controls.Add(_lblTarget);
            Controls.Add(_cmbTargetContainer);
            Controls.Add(_lblAmount);
            Controls.Add(_numAmount);
            Controls.Add(_lblUnit);
            Controls.Add(_btnOk);
            Controls.Add(_btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MoveMedicineItemDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Move Item";
            ((ISupportInitialize)_numAmount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
