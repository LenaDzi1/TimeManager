using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class AddRecipeForm
    {
        private IContainer components = null;
        private Label _lblName;
        private TextBox _txtName;
        private Label _lblDescription;
        private TextBox _txtDescription;
        private Label _lblTime;
        private NumericUpDown _numTime;
        private Label _lblPortions;
        private NumericUpDown _numPortions;
        private Label _lblIngredients;
        private Label _lblProduct;
        private ComboBox _cmbProduct;
        private Label _lblAmount;
        private NumericUpDown _numAmount;
        private Label _lblUnit;
        private ComboBox _cmbUnit;
        private Button _btnAddIngredient;
        private ListView _lstIngredients;
        private Button _btnRemoveIngredient;
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
            _lblDescription = new Label();
            _txtDescription = new TextBox();
            _lblTime = new Label();
            _numTime = new NumericUpDown();
            _lblPortions = new Label();
            _numPortions = new NumericUpDown();
            _lblIngredients = new Label();
            _lblProduct = new Label();
            _cmbProduct = new ComboBox();
            _lblAmount = new Label();
            _numAmount = new NumericUpDown();
            _lblUnit = new Label();
            _cmbUnit = new ComboBox();
            _btnAddIngredient = new Button();
            _lstIngredients = new ListView();
            _btnRemoveIngredient = new Button();
            _btnSave = new Button();
            _btnCancel = new Button();
            ((ISupportInitialize)_numTime).BeginInit();
            ((ISupportInitialize)_numPortions).BeginInit();
            ((ISupportInitialize)_numAmount).BeginInit();
            SuspendLayout();
            // 
            // _lblName
            // 
            _lblName.AutoSize = true;
            _lblName.Location = new Point(20, 20);
            _lblName.Name = "_lblName";
            _lblName.Size = new Size(42, 15);
            _lblName.TabIndex = 0;
            _lblName.Text = "Name:";
            // 
            // _txtName
            // 
            _txtName.Location = new Point(120, 18);
            _txtName.Name = "_txtName";
            _txtName.Size = new Size(450, 23);
            _txtName.TabIndex = 1;
            // 
            // _lblDescription
            // 
            _lblDescription.AutoSize = true;
            _lblDescription.Location = new Point(20, 55);
            _lblDescription.Name = "_lblDescription";
            _lblDescription.Size = new Size(70, 15);
            _lblDescription.TabIndex = 2;
            _lblDescription.Text = "Description:";
            // 
            // _txtDescription
            // 
            _txtDescription.Location = new Point(120, 53);
            _txtDescription.Multiline = true;
            _txtDescription.Name = "_txtDescription";
            _txtDescription.ScrollBars = ScrollBars.Vertical;
            _txtDescription.Size = new Size(450, 80);
            _txtDescription.TabIndex = 3;
            // 
            // _lblTime
            // 
            _lblTime.AutoSize = true;
            _lblTime.Location = new Point(20, 145);
            _lblTime.Name = "_lblTime";
            _lblTime.Size = new Size(89, 15);
            _lblTime.TabIndex = 4;
            _lblTime.Text = "Time (minutes):";
            // 
            // _numTime
            // 
            _numTime.Location = new Point(120, 143);
            _numTime.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            _numTime.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _numTime.Name = "_numTime";
            _numTime.Size = new Size(150, 23);
            _numTime.TabIndex = 5;
            _numTime.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // _lblPortions
            // 
            _lblPortions.AutoSize = true;
            _lblPortions.Location = new Point(20, 180);
            _lblPortions.Name = "_lblPortions";
            _lblPortions.Size = new Size(53, 15);
            _lblPortions.TabIndex = 6;
            _lblPortions.Text = "Portions:";
            // 
            // _numPortions
            // 
            _numPortions.Location = new Point(120, 178);
            _numPortions.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
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
            _lblIngredients.Location = new Point(20, 220);
            _lblIngredients.Name = "_lblIngredients";
            _lblIngredients.Size = new Size(73, 15);
            _lblIngredients.TabIndex = 8;
            _lblIngredients.Text = "Ingredients:";
            // 
            // _lblProduct
            // 
            _lblProduct.AutoSize = true;
            _lblProduct.Location = new Point(20, 245);
            _lblProduct.Name = "_lblProduct";
            _lblProduct.Size = new Size(52, 15);
            _lblProduct.TabIndex = 9;
            _lblProduct.Text = "Product:";
            // 
            // _cmbProduct
            // 
            _cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbProduct.FormattingEnabled = true;
            _cmbProduct.Location = new Point(120, 243);
            _cmbProduct.Name = "_cmbProduct";
            _cmbProduct.Size = new Size(200, 23);
            _cmbProduct.TabIndex = 10;
            // 
            // _lblAmount
            // 
            _lblAmount.AutoSize = true;
            _lblAmount.Location = new Point(20, 275);
            _lblAmount.Name = "_lblAmount";
            _lblAmount.Size = new Size(51, 15);
            _lblAmount.TabIndex = 11;
            _lblAmount.Text = "Amount:";
            // 
            // _numAmount
            // 
            _numAmount.DecimalPlaces = 2;
            _numAmount.Location = new Point(120, 273);
            _numAmount.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            _numAmount.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            _numAmount.Name = "_numAmount";
            _numAmount.Size = new Size(100, 23);
            _numAmount.TabIndex = 12;
            _numAmount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _lblUnit
            // 
            _lblUnit.AutoSize = true;
            _lblUnit.Location = new Point(230, 275);
            _lblUnit.Name = "_lblUnit";
            _lblUnit.Size = new Size(30, 15);
            _lblUnit.TabIndex = 13;
            _lblUnit.Text = "Unit:";
            // 
            // _cmbUnit
            // 
            _cmbUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbUnit.FormattingEnabled = true;
            _cmbUnit.Items.AddRange(new object[] { "g", "kg", "ml", "l", "pcs" });
            _cmbUnit.Location = new Point(270, 273);
            _cmbUnit.Name = "_cmbUnit";
            _cmbUnit.Size = new Size(90, 23);
            _cmbUnit.TabIndex = 14;
            // 
            // _btnAddIngredient
            // 
            _btnAddIngredient.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddIngredient.Cursor = Cursors.Hand;
            _btnAddIngredient.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddIngredient.FlatAppearance.BorderSize = 2;
            _btnAddIngredient.FlatStyle = FlatStyle.Flat;
            _btnAddIngredient.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddIngredient.Location = new Point(370, 271);
            _btnAddIngredient.Name = "_btnAddIngredient";
            _btnAddIngredient.Size = new Size(120, 25);
            _btnAddIngredient.TabIndex = 15;
            _btnAddIngredient.Text = "Add Ingredient";
            _btnAddIngredient.UseVisualStyleBackColor = false;
            // 
            // _lstIngredients
            // 
            _lstIngredients.Font = new Font("Segoe UI", 9F);
            _lstIngredients.FullRowSelect = true;
            _lstIngredients.GridLines = true;
            _lstIngredients.Location = new Point(20, 310);
            _lstIngredients.Name = "_lstIngredients";
            _lstIngredients.Size = new Size(550, 150);
            _lstIngredients.TabIndex = 16;
            _lstIngredients.UseCompatibleStateImageBehavior = false;
            _lstIngredients.View = View.Details;
            // 
            // _btnRemoveIngredient
            // 
            _btnRemoveIngredient.BackColor = Color.FromArgb(255, 245, 245);
            _btnRemoveIngredient.Cursor = Cursors.Hand;
            _btnRemoveIngredient.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnRemoveIngredient.FlatAppearance.BorderSize = 2;
            _btnRemoveIngredient.FlatStyle = FlatStyle.Flat;
            _btnRemoveIngredient.ForeColor = Color.FromArgb(231, 76, 60);
            _btnRemoveIngredient.Location = new Point(20, 470);
            _btnRemoveIngredient.Name = "_btnRemoveIngredient";
            _btnRemoveIngredient.Size = new Size(120, 30);
            _btnRemoveIngredient.TabIndex = 17;
            _btnRemoveIngredient.Text = "Remove Selected";
            _btnRemoveIngredient.UseVisualStyleBackColor = false;
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
            _btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnSave.ForeColor = Color.FromArgb(39, 174, 96);
            _btnSave.Location = new Point(400, 515);
            _btnSave.Name = "_btnSave";
            _btnSave.Size = new Size(75, 30);
            _btnSave.TabIndex = 18;
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
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.ForeColor = Color.FromArgb(100, 100, 100);
            _btnCancel.Location = new Point(485, 515);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(75, 30);
            _btnCancel.TabIndex = 19;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // AddRecipeForm
            // 
            AcceptButton = _btnSave;
            CancelButton = _btnCancel;
            ClientSize = new Size(600, 560);
            Controls.Add(_lblName);
            Controls.Add(_txtName);
            Controls.Add(_lblDescription);
            Controls.Add(_txtDescription);
            Controls.Add(_lblTime);
            Controls.Add(_numTime);
            Controls.Add(_lblPortions);
            Controls.Add(_numPortions);
            Controls.Add(_lblIngredients);
            Controls.Add(_lblProduct);
            Controls.Add(_cmbProduct);
            Controls.Add(_lblAmount);
            Controls.Add(_numAmount);
            Controls.Add(_lblUnit);
            Controls.Add(_cmbUnit);
            Controls.Add(_btnAddIngredient);
            Controls.Add(_lstIngredients);
            Controls.Add(_btnRemoveIngredient);
            Controls.Add(_btnSave);
            Controls.Add(_btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddRecipeForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Add Recipe";
            ((ISupportInitialize)_numTime).EndInit();
            ((ISupportInitialize)_numPortions).EndInit();
            ((ISupportInitialize)_numAmount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
