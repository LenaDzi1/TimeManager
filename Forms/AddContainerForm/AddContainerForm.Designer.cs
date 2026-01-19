using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class AddContainerForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox _inputBox;
        private Button _btnOk;
        private Button _btnCancel;
        private Label _lblPrompt;

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
            _lblPrompt = new Label();
            _inputBox = new TextBox();
            _btnOk = new Button();
            _btnCancel = new Button();
            SuspendLayout();
            // 
            // _lblPrompt
            // 
            _lblPrompt.AutoSize = true;
            _lblPrompt.Font = new Font("Segoe UI", 10F);
            _lblPrompt.ForeColor = SystemColors.ControlText;
            _lblPrompt.Location = new Point(12, 15);
            _lblPrompt.Name = "_lblPrompt";
            _lblPrompt.Size = new Size(82, 19);
            _lblPrompt.TabIndex = 1;
            _lblPrompt.Text = "Enter name:";
            // 
            // _inputBox
            // 
            _inputBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _inputBox.Font = new Font("Segoe UI", 10F);
            _inputBox.Location = new Point(15, 38);
            _inputBox.Name = "_inputBox";
            _inputBox.Size = new Size(350, 25);
            _inputBox.TabIndex = 0;
            // 
            // _btnOk
            // 
            _btnOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnOk.BackColor = Color.FromArgb(232, 245, 233);
            _btnOk.Cursor = Cursors.Hand;
            _btnOk.DialogResult = DialogResult.OK;
            _btnOk.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnOk.FlatAppearance.BorderSize = 2;
            _btnOk.FlatStyle = FlatStyle.Flat;
            _btnOk.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            _btnOk.ForeColor = Color.FromArgb(39, 174, 96);
            _btnOk.Location = new Point(265, 92);
            _btnOk.Name = "_btnOk";
            _btnOk.Size = new Size(100, 34);
            _btnOk.TabIndex = 0;
            _btnOk.Text = "Save";
            _btnOk.UseVisualStyleBackColor = false;
            // 
            // _btnCancel
            // 
            _btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnCancel.BackColor = Color.FromArgb(248, 250, 252);
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.FlatAppearance.BorderColor = Color.FromArgb(149, 165, 166);
            _btnCancel.FlatAppearance.BorderSize = 2;
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.Font = new Font("Segoe UI", 9.5F);
            _btnCancel.ForeColor = Color.FromArgb(100, 100, 100);
            _btnCancel.Location = new Point(142, 92);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(100, 34);
            _btnCancel.TabIndex = 1;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // AddContainerForm
            // 
            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
            ClientSize = new Size(384, 141);
            Controls.Add(_btnCancel);
            Controls.Add(_btnOk);
            Controls.Add(_inputBox);
            Controls.Add(_lblPrompt);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(300, 180);
            Name = "AddContainerForm";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
