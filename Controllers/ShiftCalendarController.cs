// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : ShiftCalendarController.cs
// Module      : Masters
// Author      : Dharani T
// Created On  : 2025-11-18
// Description : Controller for managing Shift calendar
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author       | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-11-18 | Dharani T   | Initial creation based on Shift Calendar Model structure.
// =================================================================================================

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;



namespace YardManagementApplication
{

    [Route("[controller]/[action]")]

    public class ShiftCalendarController : Controller
    {  
        // -----------------------------------------------------
        // Dependencies
        // -----------------------------------------------------
        private readonly v1Client _apiClient;
        
        private readonly ILogger<ShiftCalendarController> _logger;

        public ShiftCalendarController(v1Client apiClient, ILogger<ShiftCalendarController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        // =====================================================
        //  Render main page 
        // =====================================================
        public async Task<IActionResult> Index()
        {
            // Log action start
            string controller = nameof(ShiftCalendarController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {

                var templates = await _apiClient.GetAllTemplatesAsync();
                var plants = await _apiClient.GetAllPlantAsync();
                var holidayType = await _apiClient.HolidayTypeAsync();
                var holidayList = await _apiClient.GetAllHolidayAsync();
                var shiftData = await _apiClient.GetAllShiftAsync();
                var shiftCalendarData = await _apiClient.GetAllShiftCalendarAsync();


                Console.WriteLine("shiftCalendarData: " + JsonConvert.SerializeObject(shiftCalendarData));

                var vm = new ShiftCalendarViewModel
                {
                    Templates = templates?
                        .Select(t => new TemplateModel
                        {
                            Template_id = t.Template_id,
                            Template_name = t.Template_name,
                            Shift_id = t.Shift_id,
                            Template_description = t.Template_description,
                            Is_deleted = t.Is_deleted
                        })
                        .ToList()
                        ?? new List<TemplateModel>(),

                    Plants = plants?.ToList() ?? new List<PlantMasterModel>(),

                    Holidays = holidayType?.ToList() ?? new List<DropdownModel>(),

                    HolidayList = holidayList?.ToList() ?? new List<HolidayModel>(),

                    ShiftData = shiftData?.ToList() ?? new List<ShiftMasterModel>(),

                    ShiftCalendarData = shiftCalendarData?.ToList() ?? new List<ShiftCalendarModel>()
                };


                return View(vm);
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                return StatusCode(500, ex.Message);
            }
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
        public async Task<IActionResult> Create([FromBody] ShiftCalendarModel model)
        {
            // Log action start
            string controller = nameof(ShiftCalendarController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


            try
            {
                if (model == null)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "Invalid Request",
                        message = "Request body cannot be empty."
                    });
                }

                model.Created_by = HttpContext.Session.GetString("LoginUser");

                Console.WriteLine("Received ShiftCalendarModel:", JsonConvert.SerializeObject(model));

                // Insert new Shift record

                var result = await _apiClient.InsertShiftCalendarAsync(model);


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | Result {result}",
                    controller, action, result
                );

                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record created successfully"
                });
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                if (ex is ApiException<ResponseModel> apiEx && apiEx.Result != null)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "Error",
                        message = apiEx.Result.Detail
                    });
                }

                return BadRequest(new
                {
                    status = "error",
                    title = "Error",
                    message = ex.Message
                });
            }

        }
    }

    public record CalendarEvent( string? Title ,string? Start,string? End);
     
}
