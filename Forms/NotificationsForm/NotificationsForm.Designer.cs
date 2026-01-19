using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms.Notifications
{
    partial class NotificationsForm
    {
        private Label _titleLabel;
        private Panel _mainContainer;
        private FlowLayoutPanel _notificationsFlow;
        private Panel _footerPanel;
        private Panel _cardsWrapper;

        private void InitializeComponent()
        {
            _titleLabel = new Label();
            _mainContainer = new Panel();
            _cardsWrapper = new Panel();
            _notificationsFlow = new FlowLayoutPanel();
            _footerPanel = new Panel();
            _mainContainer.SuspendLayout();
            _cardsWrapper.SuspendLayout();
            _footerPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _titleLabel
            // 
            _titleLabel.AutoSize = true;
            _titleLabel.Font = new Font("Segoe UI", 20.25F, FontStyle.Regular, GraphicsUnit.Point);
            _titleLabel.ForeColor = Color.Black;
            _titleLabel.Location = new Point(24, 20);
            _titleLabel.Name = "_titleLabel";
            _titleLabel.Size = new Size(169, 37);
            _titleLabel.TabIndex = 0;
            _titleLabel.Text = "Notifications";
            // 
            // _mainContainer
            // 
            _mainContainer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _mainContainer.BackColor = Color.White;
            _mainContainer.Controls.Add(_cardsWrapper);
            _mainContainer.Controls.Add(_titleLabel);
            _mainContainer.Location = new Point(12, 12);
            _mainContainer.Name = "_mainContainer";
            _mainContainer.Padding = new Padding(16);
            _mainContainer.Size = new Size(860, 640);
            _mainContainer.TabIndex = 1;
            // 
            // _cardsWrapper
            // 
            _cardsWrapper.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _cardsWrapper.BackColor = Color.WhiteSmoke;
            _cardsWrapper.BorderStyle = BorderStyle.FixedSingle;
            _cardsWrapper.Controls.Add(_notificationsFlow);
            _cardsWrapper.Location = new Point(20, 70);
            _cardsWrapper.Name = "_cardsWrapper";
            _cardsWrapper.Padding = new Padding(10);
            _cardsWrapper.Size = new Size(820, 550);
            _cardsWrapper.TabIndex = 3;
            // 
            // _notificationsFlow
            // 
            _notificationsFlow.AutoScroll = true;
            _notificationsFlow.BackColor = Color.White;
            _notificationsFlow.Dock = DockStyle.Fill;
            _notificationsFlow.FlowDirection = FlowDirection.TopDown;
            _notificationsFlow.Location = new Point(10, 10);
            _notificationsFlow.Margin = new Padding(0);
            _notificationsFlow.Name = "_notificationsFlow";
            _notificationsFlow.Padding = new Padding(4);
            _notificationsFlow.Size = new Size(798, 448);
            _notificationsFlow.TabIndex = 0;
            _notificationsFlow.WrapContents = false;
            _notificationsFlow.SizeChanged += NotificationsFlow_SizeChanged;
            // 
            // 
            // NotificationsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gainsboro;
            ClientSize = new Size(884, 664);
            Controls.Add(_mainContainer);
            MinimumSize = new Size(720, 520);
            Name = "NotificationsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Notifications";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            Load += NotificationsForm_Load;
            _mainContainer.ResumeLayout(false);
            _mainContainer.PerformLayout();
            _cardsWrapper.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}

