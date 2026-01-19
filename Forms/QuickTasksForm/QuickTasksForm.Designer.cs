using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class QuickTasksForm
    {
        private IContainer components = null;

        private Label _lblTitle;
        private ListView _lstTasks;
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
            _lblTitle = new Label();
            _lstTasks = new ListView();
            _btnClose = new Button();
            SuspendLayout();
            // 
            // _lblTitle
            // 
            _lblTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _lblTitle.Location = new Point(12, 12);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(496, 40);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Low priority tasks (P1)";
            // 
            // _lstTasks
            // 
            _lstTasks.FullRowSelect = true;
            _lstTasks.GridLines = true;
            _lstTasks.Location = new Point(12, 58);
            _lstTasks.Name = "_lstTasks";
            _lstTasks.Size = new Size(496, 410);
            _lstTasks.TabIndex = 1;
            _lstTasks.UseCompatibleStateImageBehavior = false;
            _lstTasks.View = View.Details;
            // 
            // _btnClose
            // 
            _btnClose.DialogResult = DialogResult.OK;
            _btnClose.Location = new Point(418, 478);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new Size(90, 30);
            _btnClose.TabIndex = 2;
            _btnClose.Text = "Close";
            _btnClose.UseVisualStyleBackColor = true;
            // 
            // QuickTasksForm
            // 
            AcceptButton = _btnClose;
            CancelButton = _btnClose;
            ClientSize = new Size(520, 520);
            Controls.Add(_lblTitle);
            Controls.Add(_lstTasks);
            Controls.Add(_btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "QuickTasksForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Quick tasks (P1)";
            ResumeLayout(false);
        }
    }
}
