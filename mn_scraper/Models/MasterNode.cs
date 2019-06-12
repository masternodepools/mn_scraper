using System;

namespace MnScraper.Models
{
    public class MasterNode
    {
        public MasterNode(string rewardPerDay, string numberOfMasterNodes, string requiredAmount)
        {
            this.RewardPerDay = rewardPerDay;
            this.NumberOfMasterNodes = numberOfMasterNodes;
            this.RequiredAmount = requiredAmount;
        }
        public string RewardPerDay { get; set; }
        public string NumberOfMasterNodes { get; set; }
        public string RequiredAmount { get; set; }
    }
}