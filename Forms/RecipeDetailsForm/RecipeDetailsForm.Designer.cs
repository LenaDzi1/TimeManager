using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class RecipeDetailsForm
    {
        private IContainer components = null;
        private Label _lblNameLabel;
        private Label _lblName;
        private Label _lblDescriptionLabel;
        private Label _lblDescription;
        private Label _lblTimeLabel;
        private Label _lblTime;
        private Label _lblPortionsLabel;
        private NumericUpDown _numPortions;
        private Label _lblIngredients;
        private ListView _lstIngredients;
        private Button _btnDelete;
        private Button _btnPlanEating;
        private Button _btnEdit;
        private Button _btnClose;

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
            _lblNameLabel = new Label();
            _lblName = new Label();
            _lblDescriptionLabel = new Label();
            _lblDescription = new Label();
            _lblTimeLabel = new Label();
            _lblTime = new Label();
            _lblPortionsLabel = new Label();
            _numPortions = new NumericUpDown();
            _lblIngredients = new Label();
            _lstIngredients = new ListView();
            _btnDelete = new Button();
            _btnPlanEating = new Button();
            _btnEdit = new Button();
            _btnClose = new Button();
            ((ISupportInitialize)_numPortions).BeginInit();
            SuspendLayout();
            // 
            // _lblNameLabel
            // 
            _lblNameLabel.AutoSize = true;
            _lblNameLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblNameLabel.Location = new Point(20, 20);
            _lblNameLabel.Name = "_lblNameLabel";
            _lblNameLabel.Size = new Size(43, 15);
            _lblNameLabel.TabIndex = 0;
            _lblNameLabel.Text = "Name:";
            // 
            // _lblName
            // 
            _lblName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _lblName.ForeColor = Color.FromArgb(44, 62, 80);
            _lblName.Location = new Point(120, 20);
            _lblName.Name = "_lblName";
            _lblName.Size = new Size(450, 23);
            _lblName.TabIndex = 1;
            _lblName.Text = "Recipe Name";
            // 
            // _lblDescriptionLabel
            // 
            _lblDescriptionLabel.AutoSize = true;
            _lblDescriptionLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblDescriptionLabel.Location = new Point(20, 55);
            _lblDescriptionLabel.Name = "_lblDescriptionLabel";
            _lblDescriptionLabel.Size = new Size(74, 15);
            _lblDescriptionLabel.TabIndex = 2;
            _lblDescriptionLabel.Text = "Description:";
            // 
            // _lblDescription
            // 
            _lblDescription.Font = new Font("Segoe UI", 9F);
            _lblDescription.Location = new Point(120, 55);
            _lblDescription.Name = "_lblDescription";
            _lblDescription.Size = new Size(450, 60);
            _lblDescription.TabIndex = 3;
            _lblDescription.Text = "Description";
            // 
            // _lblTimeLabel
            // 
            _lblTimeLabel.AutoSize = true;
            _lblTimeLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblTimeLabel.Location = new Point(20, 125);
            _lblTimeLabel.Name = "_lblTimeLabel";
            _lblTimeLabel.Size = new Size(121, 15);
            _lblTimeLabel.TabIndex = 4;
            _lblTimeLabel.Text = "Time of preparation:";
            // 
            // _lblTime
            // 
            _lblTime.Font = new Font("Segoe UI", 9F);
            _lblTime.Location = new Point(145, 125);
            _lblTime.Name = "_lblTime";
            _lblTime.Size = new Size(94, 23);
            _lblTime.TabIndex = 5;
            _lblTime.Text = "30 min";
            // 
            // _lblPortionsLabel
            // 
            _lblPortionsLabel.AutoSize = true;
            _lblPortionsLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblPortionsLabel.Location = new Point(289, 125);
            _lblPortionsLabel.Name = "_lblPortionsLabel";
            _lblPortionsLabel.Size = new Size(56, 15);
            _lblPortionsLabel.TabIndex = 6;
            _lblPortionsLabel.Text = "Portions:";
            // 
            // _numPortions
            // 
            _numPortions.Location = new Point(351, 125);
            _numPortions.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numPortions.Name = "_numPortions";
            _numPortions.Size = new Size(150, 23);
            _numPortions.TabIndex = 7;
            _numPortions.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // _lblIngredients
            // 
            _lblIngredients.AutoSize = true;
            _lblIngredients.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblIngredients.Location = new Point(20, 158);
            _lblIngredients.Name = "_lblIngredients";
            _lblIngredients.Size = new Size(74, 15);
            _lblIngredients.TabIndex = 8;
            _lblIngredients.Text = "Ingredients:";
            // 
            // _lstIngredients
            // 
            _lstIngredients.Font = new Font("Segoe UI", 9F);
            _lstIngredients.FullRowSelect = true;
            _lstIngredients.GridLines = true;
            _lstIngredients.Location = new Point(20, 176);
            _lstIngredients.Name = "_lstIngredients";
            _lstIngredients.Size = new Size(550, 220);
            _lstIngredients.TabIndex = 9;
            _lstIngredients.UseCompatibleStateImageBehavior = false;
            _lstIngredients.View = View.Details;
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
            _btnDelete.Location = new Point(20, 413);
            _btnDelete.Name = "_btnDelete";
            _btnDelete.Size = new Size(120, 35);
            _btnDelete.TabIndex = 10;
            _btnDelete.Text = "Delete Recipe";
            _btnDelete.UseVisualStyleBackColor = false;
            // 
            // _btnPlanEating
            // 
            _btnPlanEating.BackColor = Color.FromArgb(200, 240, 210);
            _btnPlanEating.Cursor = Cursors.Hand;
            _btnPlanEating.FlatAppearance.BorderColor = Color.FromArgb(160, 220, 180);
            _btnPlanEating.FlatAppearance.BorderSize = 2;
            _btnPlanEating.FlatAppearance.MouseOverBackColor = Color.FromArgb(180, 230, 200);
            _btnPlanEating.FlatStyle = FlatStyle.Flat;
            _btnPlanEating.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnPlanEating.ForeColor = Color.FromArgb(40, 120, 60);
            _btnPlanEating.Location = new Point(146, 413);
            _btnPlanEating.Name = "_btnPlanEating";
            _btnPlanEating.Size = new Size(120, 35);
            _btnPlanEating.TabIndex = 11;
            _btnPlanEating.Text = "Plan Eating";
            _btnPlanEating.UseVisualStyleBackColor = false;
            // 
            // _btnEdit
            // 
            _btnEdit.BackColor = Color.FromArgb(235, 245, 255);
            _btnEdit.Cursor = Cursors.Hand;
            _btnEdit.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnEdit.FlatAppearance.BorderSize = 2;
            _btnEdit.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 235, 250);
            _btnEdit.FlatStyle = FlatStyle.Flat;
            _btnEdit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnEdit.ForeColor = Color.FromArgb(52, 152, 219);
            _btnEdit.Location = new Point(272, 413);
            _btnEdit.Name = "_btnEdit";
            _btnEdit.Size = new Size(120, 35);
            _btnEdit.TabIndex = 12;
            _btnEdit.Text = "Edit recipe";
            _btnEdit.UseVisualStyleBackColor = false;
            // 
            // _btnClose
            // 
            _btnClose.BackColor = Color.FromArgb(248, 250, 252);
            _btnClose.Cursor = Cursors.Hand;
            _btnClose.DialogResult = DialogResult.OK;
            _btnClose.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            _btnClose.FlatAppearance.BorderSize = 2;
            _btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.ForeColor = Color.FromArgb(100, 100, 100);
            _btnClose.Location = new Point(483, 413);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new Size(75, 35);
            _btnClose.TabIndex = 13;
            _btnClose.Text = "Close";
            _btnClose.UseVisualStyleBackColor = false;
            // 
            // RecipeDetailsForm
            // 
            AcceptButton = _btnClose;
            BackColor = Color.FromArgb(248, 250, 252);
            ClientSize = new Size(585, 465);
            Controls.Add(_lblNameLabel);
            Controls.Add(_lblName);
            Controls.Add(_lblDescriptionLabel);
            Controls.Add(_lblDescription);
            Controls.Add(_lblTimeLabel);
            Controls.Add(_lblTime);
            Controls.Add(_lblPortionsLabel);
            Controls.Add(_numPortions);
            Controls.Add(_lblIngredients);
            Controls.Add(_lstIngredients);
            Controls.Add(_btnDelete);
            Controls.Add(_btnPlanEating);
            Controls.Add(_btnEdit);
            Controls.Add(_btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RecipeDetailsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Recipe Details";
            ((ISupportInitialize)_numPortions).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
