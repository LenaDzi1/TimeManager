using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TimeManager.Models;

namespace TimeManager.Forms
{
    public partial class RedeemedRewardsForm : Form
    {
        private readonly List<RedeemedReward> _rewards;

        public RedeemedRewardsForm(List<RedeemedReward> rewards)
        {
            _rewards = rewards ?? new List<RedeemedReward>();
            InitializeComponent();
            SetupListView();
        }

        private void SetupListView()
        {
            _lstRewards.Columns.Add("Reward Name", 200);
            _lstRewards.Columns.Add("Redeemed Date", 150);
            _lstRewards.Columns.Add("Points", 100);

            foreach (var r in _rewards)
            {
                var item = new ListViewItem(r.RewardName);
                item.SubItems.Add(r.RedeemedDate.ToString("yyyy-MM-dd HH:mm"));
                item.SubItems.Add(r.PointsSpent.ToString());
                _lstRewards.Items.Add(item);
            }
        }
    }
}
