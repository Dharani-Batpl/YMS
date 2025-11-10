using System;
using System.Collections.Generic;

namespace YardManagementApplication.Models
{
    public class OperatorDashboardModel
    {
        public string Shift { get; set; } = "Day Shift";
        public DateTime CurrentDateTime { get; set; } = DateTime.Now;
        public string UserName { get; set; } = "John Doe";

        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        // Aggregate counts
        public int TotalTasks => Tasks?.Count ?? 0;
        public int AssignedTasks => Tasks?.FindAll(t => t.Status == JobStatus.Assigned).Count ?? 0;
        public int CompletedTasks => Tasks?.FindAll(t => t.Status == JobStatus.Completed).Count ?? 0;
    }

    public class TaskItem
    {
        public string VIN { get; set; } = string.Empty;
        public string RouteFrom { get; set; } = string.Empty;
        public string RouteTo { get; set; } = string.Empty;

        public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;

        public TimeSpan SLADue { get; set; }

        public JobStatus Status { get; set; } = JobStatus.Assigned;

        // Match the UI, Action is same as status for now
        public JobStatus ActionStatus { get; set; } = JobStatus.Assigned;
    }

    public enum PriorityLevel
    {
        Normal,
        Urgent,
        Critical
    }

    public enum JobStatus
    {
        Assigned,
        Completed
    }
}



