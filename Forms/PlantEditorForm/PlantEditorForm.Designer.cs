using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class PlantEditorForm
    {
        private IContainer components = null;

        private Label _lblName;
        private TextBox _txtName;
        private Label _lblSpecies;
        private TextBox _txtSpecies;
        private Label _lblFrequency;
        private NumericUpDown _numFrequency;
        private Button _btnDelete;
        private Button _btnSave;
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
            _lblName = new Label();
            _txtName = new TextBox();
            _lblSpecies = new Label();
            _txtSpecies = new TextBox();
            _lblFrequency = new Label();
            _numFrequency = new NumericUpDown();
            _btnDelete = new Button();
            _btnSave = new Button();
            _btnCancel = new Button();
            ((ISupportInitialize)_numFrequency).BeginInit();
            SuspendLayout();
            // 
            // _lblName
            // 
            _lblName.AutoSize = true;
            _lblName.Location = new Point(15, 20);
            _lblName.Name = "_lblName";
            _lblName.Size = new Size(42, 15);
            _lblName.TabIndex = 0;
            _lblName.Text = "Name";
            // 
            // _txtName
            // 
            _txtName.Location = new Point(140, 16);
            _txtName.Name = "_txtName";
            _txtName.Size = new Size(170, 23);
            _txtName.TabIndex = 1;
            // 
            // _lblSpecies
            // 
            _lblSpecies.AutoSize = true;
            _lblSpecies.Location = new Point(15, 55);
            _lblSpecies.Name = "_lblSpecies";
            _lblSpecies.Size = new Size(124, 15);
            _lblSpecies.TabIndex = 2;
            _lblSpecies.Text = "Species (optional)";
            // 
            // _txtSpecies
            // 
            _txtSpecies.Location = new Point(140, 51);
            _txtSpecies.Name = "_txtSpecies";
            _txtSpecies.Size = new Size(170, 23);
            _txtSpecies.TabIndex = 3;
            // 
            // _lblFrequency
            // 
            _lblFrequency.AutoSize = true;
            _lblFrequency.Location = new Point(15, 90);
            _lblFrequency.Name = "_lblFrequency";
            _lblFrequency.Size = new Size(112, 15);
            _lblFrequency.TabIndex = 4;
            _lblFrequency.Text = "Frequency (days)";
            // 
            // _numFrequency
            // 
            _numFrequency.Location = new Point(140, 86);
            _numFrequency.Maximum = new decimal(new int[] { 365, 0, 0, 0 });
            _numFrequency.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numFrequency.Name = "_numFrequency";
            _numFrequency.Size = new Size(80, 23);
            _numFrequency.TabIndex = 5;
            _numFrequency.Value = new decimal(new int[] { 7, 0, 0, 0 });
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
            _btnDelete.Location = new Point(15, 140);
            _btnDelete.Name = "_btnDelete";
            _btnDelete.Size = new Size(80, 32);
            _btnDelete.TabIndex = 6;
            _btnDelete.Text = "Delete";
            _btnDelete.UseVisualStyleBackColor = false;
            _btnDelete.Visible = false;
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
            _btnSave.Location = new Point(140, 140);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(80, 32);
            _btnSave.TabIndex = 7;
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
            _btnCancel.Location = new Point(230, 140);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(80, 32);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // PlantEditorForm
            // 
            AcceptButton = _btnSave;
            BackColor = Color.FromArgb(248, 250, 252);
            CancelButton = _btnCancel;
            ClientSize = new Size(330, 187);
            Controls.Add(_lblName);
            Controls.Add(_txtName);
            Controls.Add(_lblSpecies);
            Controls.Add(_txtSpecies);
            Controls.Add(_lblFrequency);
            Controls.Add(_numFrequency);
            Controls.Add(_btnDelete);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PlantEditorForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Plant Editor";
            ((ISupportInitialize)_numFrequency).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
