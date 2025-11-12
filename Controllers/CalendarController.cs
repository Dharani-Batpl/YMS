using Microsoft.AspNetCore.Mvc;

namespace YardManagementApplication.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetEvents()
        {
            var events = new CalendarEvent[]
            {
            new(Title : "Team Meeting", Start : "2025-11-10", End : "2025-11-10" ),
            new( Title : "Project Deadline", Start : "2025-11-13" , End: null)
        };
            return Json(events);
        }

        [HttpPost]
        public JsonResult AddEvent([FromBody] CalendarEvent newEvent)
        {
            // Save to database or memory here
            return Json(new { success = true, message = "Event added!" });
        }
    }

    public record CalendarEvent( string? Title ,string? Start,string? End);
     
}
