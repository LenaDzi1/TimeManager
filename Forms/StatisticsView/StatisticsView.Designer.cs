using System.Drawing;
using System.Windows.Forms;

namespace TimeManager.Forms
{
    partial class StatisticsView
    {
        private Panel _radarChartPanel;
        private Label _lblTotalPoints;
        private Label _lblBestCategory;
        private Label _lblWorstCategory;
        private ListView _rewardsListView;
        private Button _btnAddReward;
        private Button _btnRedeemReward;
        private Button _btnHistory;
        private Button _btnClearStatistics;
        private Panel leftPanel;
        private Panel pointsPanel;
        private Panel rightPanel;
        private Panel rewardButtonsPanel;
        private Panel rewardHeaderPanel;
        private Label rewardIcon;
        private Label lblRewardCatalog;
        private TableLayoutPanel mainLayout;

        private void InitializeComponent()
        {
            mainLayout = new TableLayoutPanel();
            leftPanel = new Panel();
            _radarChartPanel = new Panel();
            pointsPanel = new Panel();
            _btnClearStatistics = new Button();
            _lblTotalPoints = new Label();
            _lblBestCategory = new Label();
            _lblWorstCategory = new Label();
            rightPanel = new Panel();
            _rewardsListView = new ListView();
            rewardButtonsPanel = new Panel();
            _btnAddReward = new Button();
            _btnRedeemReward = new Button();
            _btnHistory = new Button();
            rewardHeaderPanel = new Panel();
            rewardIcon = new Label();
            lblRewardCatalog = new Label();
            mainLayout.SuspendLayout();
            leftPanel.SuspendLayout();
            pointsPanel.SuspendLayout();
            rightPanel.SuspendLayout();
            rewardButtonsPanel.SuspendLayout();
            rewardHeaderPanel.SuspendLayout();
            SuspendLayout();
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightPanel, 1, 0);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(20, 20);
            mainLayout.Margin = new Padding(0);
            mainLayout.Name = "mainLayout";
            mainLayout.RowCount = 1;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.Size = new Size(1360, 760);
            mainLayout.TabIndex = 0;
            // 
            // leftPanel
            // 
            leftPanel.BackColor = Color.White;
            leftPanel.Controls.Add(_radarChartPanel);
            leftPanel.Controls.Add(pointsPanel);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Location = new Point(3, 3);
            leftPanel.Name = "leftPanel";
            leftPanel.Padding = new Padding(0, 0, 10, 0);
            leftPanel.Size = new Size(782, 754);
            leftPanel.TabIndex = 0;
            // 
            // _radarChartPanel
            // 
            _radarChartPanel.BackColor = Color.White;
            _radarChartPanel.Dock = DockStyle.Fill;
            _radarChartPanel.Location = new Point(0, 0);
            _radarChartPanel.Margin = new Padding(0, 0, 0, 10);
            _radarChartPanel.Name = "_radarChartPanel";
            _radarChartPanel.Size = new Size(772, 624);
            _radarChartPanel.TabIndex = 0;
            _radarChartPanel.Paint += RadarChartPanel_Paint;
            // 
            // pointsPanel
            // 
            pointsPanel.BackColor = Color.FromArgb(248, 250, 252);
            pointsPanel.Controls.Add(_btnClearStatistics);
            pointsPanel.Controls.Add(_lblTotalPoints);
            pointsPanel.Controls.Add(_lblBestCategory);
            pointsPanel.Controls.Add(_lblWorstCategory);
            pointsPanel.Dock = DockStyle.Bottom;
            pointsPanel.Location = new Point(0, 624);
            pointsPanel.Name = "pointsPanel";
            pointsPanel.Padding = new Padding(15);
            pointsPanel.Size = new Size(772, 130);
            pointsPanel.TabIndex = 1;
            // 
            // _lblTotalPoints
            // 
            _lblTotalPoints.BackColor = Color.FromArgb(255, 248, 230);
            _lblTotalPoints.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            _lblTotalPoints.ForeColor = Color.FromArgb(180, 130, 20);
            _lblTotalPoints.Location = new Point(15, 15);
            _lblTotalPoints.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _lblTotalPoints.Name = "_lblTotalPoints";
            _lblTotalPoints.Size = new Size(220, 55);
            _lblTotalPoints.TabIndex = 1;
            _lblTotalPoints.Text = "You have 0 points";
            _lblTotalPoints.TextAlign = ContentAlignment.MiddleLeft;
            _lblTotalPoints.Padding = new Padding(12, 0, 12, 0);
            // 
            // _lblBestCategory
            // 
            _lblBestCategory.BackColor = Color.FromArgb(245, 250, 245);
            _lblBestCategory.Font = new Font("Segoe UI", 10F);
            _lblBestCategory.ForeColor = Color.FromArgb(39, 174, 96);
            _lblBestCategory.Location = new Point(245, 15);
            _lblBestCategory.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _lblBestCategory.Name = "_lblBestCategory";
            _lblBestCategory.Size = new Size(220, 55);
            _lblBestCategory.TabIndex = 2;
            _lblBestCategory.Text = "You are great at: -";
            _lblBestCategory.TextAlign = ContentAlignment.MiddleLeft;
            _lblBestCategory.Padding = new Padding(12, 0, 12, 0);
            _lblBestCategory.Tag = Color.FromArgb(39, 174, 96);
            // 
            // _lblWorstCategory
            // 
            _lblWorstCategory.BackColor = Color.FromArgb(245, 248, 255);
            _lblWorstCategory.Font = new Font("Segoe UI", 10F);
            _lblWorstCategory.ForeColor = Color.FromArgb(52, 152, 219);
            _lblWorstCategory.Location = new Point(475, 15);
            _lblWorstCategory.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _lblWorstCategory.Name = "_lblWorstCategory";
            _lblWorstCategory.Size = new Size(220, 55);
            _lblWorstCategory.TabIndex = 3;
            _lblWorstCategory.Text = "You should work on: -";
            _lblWorstCategory.TextAlign = ContentAlignment.MiddleLeft;
            _lblWorstCategory.Padding = new Padding(12, 0, 12, 0);
            _lblWorstCategory.Tag = Color.FromArgb(52, 152, 219);
            // 
            // _btnClearStatistics
            // 
            _btnClearStatistics.BackColor = Color.FromArgb(255, 240, 240);
            _btnClearStatistics.Cursor = Cursors.Hand;
            _btnClearStatistics.FlatAppearance.BorderColor = Color.FromArgb(231, 76, 60);
            _btnClearStatistics.FlatAppearance.BorderSize = 2;
            _btnClearStatistics.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 220, 220);
            _btnClearStatistics.FlatStyle = FlatStyle.Flat;
            _btnClearStatistics.Font = new Font("Segoe UI", 9F);
            _btnClearStatistics.ForeColor = Color.FromArgb(231, 76, 60);
            _btnClearStatistics.Location = new Point(15, 80);
            _btnClearStatistics.Name = "_btnClearStatistics";
            _btnClearStatistics.Size = new Size(150, 34);
            _btnClearStatistics.TabIndex = 0;
            _btnClearStatistics.Text = "Clear statistics";
            _btnClearStatistics.UseVisualStyleBackColor = false;
            // 
            // rightPanel
            // 
            rightPanel.BackColor = Color.White;
            rightPanel.Controls.Add(_rewardsListView);
            rightPanel.Controls.Add(rewardButtonsPanel);
            rightPanel.Controls.Add(rewardHeaderPanel);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Location = new Point(791, 3);
            rightPanel.Name = "rightPanel";
            rightPanel.Padding = new Padding(10, 0, 0, 0);
            rightPanel.Size = new Size(566, 754);
            rightPanel.TabIndex = 1;
            // 
            // _rewardsListView
            // 
            _rewardsListView.Dock = DockStyle.Fill;
            _rewardsListView.Font = new Font("Segoe UI", 10F);
            _rewardsListView.FullRowSelect = true;
            _rewardsListView.GridLines = true;
            _rewardsListView.Location = new Point(10, 60);
            _rewardsListView.Margin = new Padding(0);
            _rewardsListView.Name = "_rewardsListView";
            _rewardsListView.Size = new Size(556, 624);
            _rewardsListView.TabIndex = 0;
            _rewardsListView.UseCompatibleStateImageBehavior = false;
            _rewardsListView.View = View.Details;
            _rewardsListView.Columns.Add("Name", 350);
            _rewardsListView.Columns.Add("Points", 100);
            // 
            // rewardButtonsPanel
            // 
            rewardButtonsPanel.BackColor = Color.White;
            rewardButtonsPanel.Controls.Add(_btnAddReward);
            rewardButtonsPanel.Controls.Add(_btnRedeemReward);
            rewardButtonsPanel.Controls.Add(_btnHistory);
            rewardButtonsPanel.Dock = DockStyle.Bottom;
            rewardButtonsPanel.Location = new Point(10, 684);
            rewardButtonsPanel.Name = "rewardButtonsPanel";
            rewardButtonsPanel.Padding = new Padding(0, 10, 0, 10);
            rewardButtonsPanel.Size = new Size(556, 70);
            rewardButtonsPanel.TabIndex = 1;
            // 
            // _btnAddReward
            // 
            _btnAddReward.BackColor = Color.FromArgb(235, 245, 255);
            _btnAddReward.Cursor = Cursors.Hand;
            _btnAddReward.FlatAppearance.BorderColor = Color.FromArgb(52, 152, 219);
            _btnAddReward.FlatAppearance.BorderSize = 2;
            _btnAddReward.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 225, 250);
            _btnAddReward.FlatStyle = FlatStyle.Flat;
            _btnAddReward.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnAddReward.ForeColor = Color.FromArgb(52, 152, 219);
            _btnAddReward.Location = new Point(10, 15);
            _btnAddReward.Margin = new Padding(0);
            _btnAddReward.Name = "_btnAddReward";
            _btnAddReward.Size = new Size(150, 36);
            _btnAddReward.TabIndex = 0;
            _btnAddReward.Text = "+ Add Reward";
            _btnAddReward.UseVisualStyleBackColor = false;
            // 
            // _btnRedeemReward
            // 
            _btnRedeemReward.BackColor = Color.FromArgb(232, 245, 233);
            _btnRedeemReward.Cursor = Cursors.Hand;
            _btnRedeemReward.FlatAppearance.BorderColor = Color.FromArgb(46, 204, 113);
            _btnRedeemReward.FlatAppearance.BorderSize = 2;
            _btnRedeemReward.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 230, 201);
            _btnRedeemReward.FlatStyle = FlatStyle.Flat;
            _btnRedeemReward.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnRedeemReward.ForeColor = Color.FromArgb(39, 174, 96);
            _btnRedeemReward.Location = new Point(170, 15);
            _btnRedeemReward.Margin = new Padding(0);
            _btnRedeemReward.Name = "_btnRedeemReward";
            _btnRedeemReward.Size = new Size(150, 36);
            _btnRedeemReward.TabIndex = 1;
            _btnRedeemReward.Text = "Redeem Points";
            _btnRedeemReward.UseVisualStyleBackColor = false;
            // 
            // _btnHistory
            // 
            _btnHistory.BackColor = Color.FromArgb(245, 238, 248);
            _btnHistory.Cursor = Cursors.Hand;
            _btnHistory.FlatAppearance.BorderColor = Color.FromArgb(155, 89, 182);
            _btnHistory.FlatAppearance.BorderSize = 2;
            _btnHistory.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 225, 245);
            _btnHistory.FlatStyle = FlatStyle.Flat;
            _btnHistory.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            _btnHistory.ForeColor = Color.FromArgb(155, 89, 182);
            _btnHistory.Location = new Point(330, 15);
            _btnHistory.Margin = new Padding(0);
            _btnHistory.Name = "_btnHistory";
            _btnHistory.Size = new Size(100, 36);
            _btnHistory.TabIndex = 2;
            _btnHistory.Text = "History";
            _btnHistory.UseVisualStyleBackColor = false;
            // 
            // rewardHeaderPanel
            // 
            rewardHeaderPanel.BackColor = Color.FromArgb(245, 248, 250);
            rewardHeaderPanel.Controls.Add(rewardIcon);
            rewardHeaderPanel.Controls.Add(lblRewardCatalog);
            rewardHeaderPanel.Dock = DockStyle.Top;
            rewardHeaderPanel.Location = new Point(10, 0);
            rewardHeaderPanel.Name = "rewardHeaderPanel";
            rewardHeaderPanel.Padding = new Padding(10, 0, 10, 0);
            rewardHeaderPanel.Size = new Size(556, 60);
            rewardHeaderPanel.TabIndex = 2;
            // 
            // rewardIcon
            // 
            rewardIcon.Font = new Font("Segoe UI", 22F);
            rewardIcon.Location = new Point(10, 8);
            rewardIcon.Name = "rewardIcon";
            rewardIcon.Size = new Size(45, 45);
            rewardIcon.TabIndex = 0;
            rewardIcon.Text = "🏆";
            // 
            // lblRewardCatalog
            // 
            lblRewardCatalog.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            lblRewardCatalog.ForeColor = Color.FromArgb(44, 62, 80);
            lblRewardCatalog.Location = new Point(55, 15);
            lblRewardCatalog.Name = "lblRewardCatalog";
            lblRewardCatalog.Size = new Size(200, 30);
            lblRewardCatalog.TabIndex = 1;
            lblRewardCatalog.Text = "Reward Catalog";
            // 
            // StatisticsView
            // 
            AutoScroll = true;
            BackColor = Color.White;
            Controls.Add(mainLayout);
            Name = "StatisticsView";
            Padding = new Padding(20);
            Size = new Size(1400, 800);
            mainLayout.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            pointsPanel.ResumeLayout(false);
            rightPanel.ResumeLayout(false);
            rewardButtonsPanel.ResumeLayout(false);
            rewardHeaderPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
