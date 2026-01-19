// Import przestrzeni nazw
using System;                   // Podstawowe typy .NET

namespace TimeManager.Models
{


    /// <summary>
    /// Model nagrody wymienianej za punkty.
    /// </summary>
    public class Reward
    {
        public int RewardID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PointsCost { get; set; }
        public bool IsRedeemed { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Model wpisu w historii wymienionych nagród.
    /// </summary>
    public class RedeemedReward
    {
        public int HistoryID { get; set; }
        public string RewardName { get; set; }
        public int PointsSpent { get; set; }
        public DateTime RedeemedDate { get; set; }
    }
}
