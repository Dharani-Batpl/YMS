using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
   
    public class TaskController : Controller
    {
        private readonly ILogger<TaskController> _logger;
        private readonly v1Client _apiClient;
        internal class DropdownViewModel
        {

            public List<SelectListItem> Operators { get; set; }
         
        }
        public TaskController(ILogger<TaskController> logger, v1Client apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }
        // GET: TaskController
        public ActionResult Index()
        {
            
            var operators=_apiClient.GetOperatorsAsync().Result;
            var model = new DropdownViewModel
            {
                Operators = operators.Select(o => new SelectListItem
                {
                    Value = o.Employee_id.ToString(),
                    Text = o.Employee_code
                }).ToList()
            };
            var lv = new LivePicklistViewModel();
            lv= GetLivePicklistViewModel();
            var multimodels= new Tuple<DropdownViewModel, LivePicklistViewModel>(model, lv);
            return View(multimodels);
        }

        // GET: TaskController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TaskController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TaskController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TaskController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: TaskController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TaskController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult GetTasks()
        {


            // Placeholder for getting tasks logic
            return Ok(new { message = "GetTasks endpoint hit" });
        }

        [HttpGet("GetEolData")]
        public IActionResult GetEolData()
        {

         var ret=  _apiClient.GetVehicleMovementAsync().Result;

            // Return vehicle movement data as JSON
            return Ok(ret);
        }

        [HttpPost]
        public IActionResult AssignVehicle1(string _operator, string _operatorname, string notes, string selectedData, string autoassign = "false")
        {

            var dict = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(selectedData);
            var multiparams = new List<usp_vehicle_assignment>();
            foreach (var item in dict)
            {
                var parameters = new usp_vehicle_assignment
                {
                    i_vin_serial_no = item["vin"] as string,
                    i_operator_id = int.Parse(_operator),
                    i_operator_name = _operatorname,
                    i_supervisor_name = User.Identity.Name ?? "test",
                    i_origin = item["origin"] as string,
                    i_origin_slot = null,
                    i_destination = item["destination"] as string,
                    i_destination_slot = null,
                    i_priority = item["priority"] as string
                };
                multiparams.Add(parameters);
            }

            var mparams = JsonConvert.SerializeObject(multiparams);
            var result = _apiClient.AddVehicleMovementAsync(autoassign, mparams).Result;

            return Ok();
        }


        [HttpGet("GetLivePicklistDashboardData")]
        public IActionResult GetLivePicklistDashboardData()
        {
           


            // Return vehicle movement data as JSON
            return Ok(GetLivePicklistViewModel());

        }

        [HttpGet("GetLivePicklistData")]
        public IActionResult GetLivePicklistData()
        { 
        return Ok(_apiClient.GetPickListDataAsync().Result);

        }

        public LivePicklistViewModel GetLivePicklistViewModel() {
            var summary = _apiClient.GetSummaryCardAsync().Result;
            var operators = _apiClient.GetOperatorStatusOverviewAsync().Result;
            var picklist = _apiClient.GetPickListDataAsync().Result;
            var lpm = new LivePicklistViewModel();
            
            foreach (var sum in summary)
            {
                if (sum.Status_group == "Assigned")
                    lpm.Assigned = sum.Total_vehicles;
                else if (sum.Status_group == "In Progress")
                    lpm.InProgress = sum.Total_vehicles;
                else if (sum.Status_group == "Completed")
                    lpm.Completed = sum.Total_vehicles;
                else if (sum.Status_group == "At Risk")
                    lpm.AtRisk = sum.Total_vehicles;
            }
            var operatorsList = new List<OperatorStatus>();
            foreach (var op in operators)
            {
                operatorsList.Add(new OperatorStatus
                {
                    Name = op.Driver_name,
                    ActivePicks = op.Active_picks,
                    CompletedToday = op.Completed_today,
                    Location = op.Last_location,
                    LastActivity = op.Last_activity_time?.ToString("g") ?? "N/A"
                });
            }
            lpm.Operators = operatorsList;
            lpm.PicklistOrders =new List<Models.LivePicklistOrder>();
            foreach (var pk in picklist)
            {
                var plo = new Models.LivePicklistOrder
                {
                    picklist_id = pk.Picklist_id,
                    vin = pk.Vin,
                    route = pk.Route,
                    status = pk.Status,
                    sla_due = pk.Sla_due?.DateTime,
                    elapsed_minutes = pk.Elapsed_minutes,
                    oprator = pk.Oprator,
                    detailed_status = pk.Detailed_status,
                    completion_at = pk.Completion_at?.DateTime
                };
                lpm.PicklistOrders.Add(plo);
            }
            
            return lpm;

        }

        [HttpPost("CancelVehicleMovement")]
        public ActionResult<int> CancelVehicleMovement(string vin)
        {
            var result = _apiClient.CancelVehicleMovementAsync(vin).Result;
            return Ok(result);
        }


    }
}
