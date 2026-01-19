using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class AddRewardForm
    {
        private IContainer components = null;
        private TextBox _txtName;
        private TextBox _txtDescription;
        private NumericUpDown _numPointsCost;
        private Button _btnSave;
        private Button _btnCancel;
        private Label lblName;
        private Label lblDescription;
        private Label lblPoints;
        private Panel buttonsPanel;

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
            lblName = new Label();
            lblDescription = new Label();
            lblPoints = new Label();
            buttonsPanel = new Panel();
            ((ISupportInitialize)_numPointsCost).BeginInit();
            buttonsPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _txtName
            // 
            _txtName.Font = new Font("Segoe UI", 10F);
            _txtName.Location = new Point(20, 45);
            _txtName.Name = "_txtName";
            _txtName.Size = new Size(440, 25);
            _txtName.TabIndex = 1;
            // 
            // _txtDescription
            // 
            _txtDescription.Font = new Font("Segoe UI", 10F);
            _txtDescription.Location = new Point(20, 105);
            _txtDescription.Name = "_txtDescription";
            _txtDescription.Size = new Size(440, 25);
            _txtDescription.TabIndex = 3;
            // 
            // _numPointsCost
            // 
            _numPointsCost.Location = new Point(150, 140);
            _numPointsCost.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            _numPointsCost.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numPointsCost.Name = "_numPointsCost";
            _numPointsCost.Size = new Size(100, 23);
            _numPointsCost.TabIndex = 5;
            _numPointsCost.Value = new decimal(new int[] { 50, 0, 0, 0 });
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
            _btnSave.Location = new Point(360, 8);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(100, 30);
            _btnSave.TabIndex = 0;
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
            _btnCancel.Font = new Font("Segoe UI", 9F);
            _btnCancel.ForeColor = Color.FromArgb(100, 100, 100);
            _btnCancel.Location = new Point(23, 7);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(100, 30);
            _btnCancel.TabIndex = 1;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // lblName
            // 
            lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblName.Location = new Point(20, 20);
            lblName.Name = "lblName";
            lblName.Size = new Size(100, 20);
            lblName.TabIndex = 0;
            lblName.Text = "Name:";
            // 
            // lblDescription
            // 
            lblDescription.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblDescription.Location = new Point(20, 80);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(100, 20);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "Description:";
            // 
            // lblPoints
            // 
            lblPoints.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPoints.Location = new Point(20, 140);
            lblPoints.Name = "lblPoints";
            lblPoints.Size = new Size(120, 20);
            lblPoints.TabIndex = 4;
            lblPoints.Text = "Points Cost:";
            // 
            // buttonsPanel
            // 
            buttonsPanel.BackColor = Color.White;
            buttonsPanel.Controls.Add(_btnSave);
            buttonsPanel.Controls.Add(_btnCancel);
            buttonsPanel.Dock = DockStyle.Bottom;
            buttonsPanel.Location = new Point(0, 165);
            buttonsPanel.Name = "buttonsPanel";
            buttonsPanel.Padding = new Padding(20, 10, 20, 10);
            buttonsPanel.Size = new Size(483, 50);
            buttonsPanel.TabIndex = 6;
            // 
            // AddRewardForm
            // 
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(483, 215);
            Controls.Add(lblName);
            Controls.Add(_txtName);
            Controls.Add(lblDescription);
            Controls.Add(_txtDescription);
            Controls.Add(lblPoints);
            Controls.Add(_numPointsCost);
            Controls.Add(buttonsPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddRewardForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Reward";
            ((ISupportInitialize)_numPointsCost).EndInit();
            buttonsPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}









