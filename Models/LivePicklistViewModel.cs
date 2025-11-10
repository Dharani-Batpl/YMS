using System.Collections.Generic;

namespace YardManagementApplication.Models
{
    public class LivePicklistViewModel
    {
        public int Assigned { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int AtRisk { get; set; }

        public List<OperatorStatus> Operators { get; set; }

        public List<LivePicklistOrder> PicklistOrders { get; set; }
    }
    public class OperatorStatus
    {
        public string Name { get; set; }
        public int ActivePicks { get; set; }
        public int CompletedToday { get; set; }
        public string Location { get; set; }
        public string LastActivity { get; set; }
    }
}