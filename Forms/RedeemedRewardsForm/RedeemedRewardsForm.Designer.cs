using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class RedeemedRewardsForm
    {
        private IContainer components = null;

        private Label _lblTitle;
        private ListView _lstRewards;
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
            _lstRewards = new ListView();
            _btnClose = new Button();
            SuspendLayout();
            // 
            // _lblTitle
            // 
            _lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            _lblTitle.ForeColor = Color.FromArgb(44, 62, 80);
            _lblTitle.Location = new Point(12, 12);
            _lblTitle.Name = "_lblTitle";
            _lblTitle.Size = new Size(496, 40);
            _lblTitle.TabIndex = 0;
            _lblTitle.Text = "Redeemed Rewards History";
            _lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _lstRewards
            // 
            _lstRewards.FullRowSelect = true;
            _lstRewards.GridLines = true;
            _lstRewards.Location = new Point(12, 60);
            _lstRewards.Name = "_lstRewards";
            _lstRewards.Size = new Size(496, 400);
            _lstRewards.TabIndex = 1;
            _lstRewards.UseCompatibleStateImageBehavior = false;
            _lstRewards.View = View.Details;
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
            // RedeemedRewardsForm
            // 
            AcceptButton = _btnClose;
            CancelButton = _btnClose;
            ClientSize = new Size(520, 520);
            Controls.Add(_lblTitle);
            Controls.Add(_lstRewards);
            Controls.Add(_btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RedeemedRewardsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Redeemed Rewards";
            ResumeLayout(false);
        }
    }
}
