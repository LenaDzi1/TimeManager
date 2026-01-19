using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class EditRewardForm
    {
        private IContainer components = null;
        private TextBox _txtName;
        private TextBox _txtDescription;
        private NumericUpDown _numPointsCost;
        private Button _btnSave;
        private Button _btnCancel;
        private Button _btnDelete;
        private Label lblName;
        private Label lblDescription;
        private Label lblPoints;

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
            _txtName = new TextBox();
            _txtDescription = new TextBox();
            _numPointsCost = new NumericUpDown();
            _btnSave = new Button();
            _btnCancel = new Button();
            _btnDelete = new Button();
            lblName = new Label();
            lblDescription = new Label();
            lblPoints = new Label();
            ((ISupportInitialize)_numPointsCost).BeginInit();
            SuspendLayout();
            // 
            // _txtName
            // 
            _txtName.Location = new Point(18, 40);
            _txtName.Name = "_txtName";
            _txtName.Size = new Size(480, 23);
            _txtName.TabIndex = 1;
            // 
            // _txtDescription
            // 
            _txtDescription.Location = new Point(18, 98);
            _txtDescription.Multiline = true;
            _txtDescription.Name = "_txtDescription";
            _txtDescription.ScrollBars = ScrollBars.Vertical;
            _txtDescription.Size = new Size(480, 120);
            _txtDescription.TabIndex = 3;
            // 
            // _numPointsCost
            // 
            _numPointsCost.Location = new Point(91, 232);
            _numPointsCost.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            _numPointsCost.Name = "_numPointsCost";
            _numPointsCost.Size = new Size(120, 23);
            _numPointsCost.TabIndex = 5;
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
            _btnSave.Location = new Point(408, 278);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(90, 30);
            _btnSave.TabIndex = 8;
            _btnSave.Text = "Save";
            _btnSave.UseVisualStyleBackColor = false;
            _btnSave.Click += OnSaveClick;
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
            _btnCancel.Location = new Point(18, 278);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(90, 30);
            _btnCancel.TabIndex = 7;
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
            _btnDelete.Location = new Point(290, 278);
            _btnDelete.Name = "_btnDelete";
            _btnDelete.Size = new Size(90, 30);
            _btnDelete.TabIndex = 6;
            _btnDelete.Text = "Delete";
            _btnDelete.UseVisualStyleBackColor = false;
            _btnDelete.Click += OnDeleteClick;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblName.Location = new Point(18, 18);
            lblName.Name = "lblName";
            lblName.Size = new Size(40, 15);
            lblName.TabIndex = 0;
            lblName.Text = "Name";
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDescription.Location = new Point(18, 76);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(71, 15);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "Description";
            // 
            // lblPoints
            // 
            lblPoints.AutoSize = true;
            lblPoints.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPoints.Location = new Point(18, 234);
            lblPoints.Name = "lblPoints";
            lblPoints.Size = new Size(67, 15);
            lblPoints.TabIndex = 4;
            lblPoints.Text = "Points cost";
            // 
            // EditRewardForm
            // 
            AcceptButton = _btnSave;
            BackColor = Color.FromArgb(248, 250, 252);
            CancelButton = _btnCancel;
            ClientSize = new Size(520, 320);
            Controls.Add(lblName);
            Controls.Add(_txtName);
            Controls.Add(lblDescription);
            Controls.Add(_txtDescription);
            Controls.Add(lblPoints);
            Controls.Add(_numPointsCost);
            Controls.Add(_btnDelete);
            Controls.Add(_btnCancel);
            Controls.Add(_btnSave);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditRewardForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Edit Reward";
            ((ISupportInitialize)_numPointsCost).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}


