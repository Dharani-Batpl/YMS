using Microsoft.AspNetCore.Mvc;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
    public class OperatorDashboardController : Controller
    {
      
        public IActionResult Index()
        {
            var model = GetDashboardModel();
            return View(model);
        }

        private OperatorDashboardModel GetDashboardModel()
        {
            // Sample data - Replace with real data retrieval logic
            return new OperatorDashboardModel
            {
                Shift = "Day Shift",
                CurrentDateTime = DateTime.Now,
                UserName = "John Doe",
                Tasks = new List<TaskItem>
                {
                    new TaskItem
                    {
                        VIN = "MA1ABC12345678901",
                        RouteFrom = "Yard-12",
                        RouteTo = "Inspection",
                        Priority = PriorityLevel.Critical,
                        SLADue = new TimeSpan(9, 0, 0),
                        Status = JobStatus.Completed,
                        ActionStatus = JobStatus.Completed
                    },
                    new TaskItem
                    {
                        VIN = "MA1ABC12345678903",
                        RouteFrom = "Audit",
                        RouteTo = "Rework",
                        Priority = PriorityLevel.Urgent,
                        SLADue = new TimeSpan(9, 40, 0),
                        Status = JobStatus.Completed,
                        ActionStatus = JobStatus.Completed
                    },
                    new TaskItem
                    {
                        VIN = "MA1ABC12345678905",
                        RouteFrom = "Storage-A",
                        RouteTo = "Loading Bay",
                        Priority = PriorityLevel.Normal,
                        SLADue = new TimeSpan(10, 15, 0),
                        Status = JobStatus.Completed,
                        ActionStatus = JobStatus.Completed
                    }
                }
            };
        }
    }
}


   


