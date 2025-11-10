using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class LaneController : Controller
    {
        private readonly v1Client _apiClient;

        public LaneController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }
        // ===============================================================
        // 1️⃣ INDEX — Lane List Page (for selected Yard)
        // ===============================================================
        //[HttpGet]
        //public async Task<IActionResult> Index(long yardId, string yardName)
        //{
        //    // Placeholder: Return a sample lane for the given yard
        //    var lanes = new List<laneModel>
        //    {
        //        new laneModel
        //        {
        //            Lane_id = 1,
        //            Yard_id = yardId,
        //            Yard_name = yardName,
        //            Lane_code = "LN001",
        //            Lane_name = "Runway Lane 1",
        //            Lane_type = "Runway",
        //            Description = "Main entry lane",
        //            Status_id = 1,
        //            Status_name = "Active",
        //            Created_by = "Admin",
        //            Created_at = DateTimeOffset.Now.AddDays(-10),
        //            Updated_by = "Admin",
        //            Updated_at = DateTimeOffset.Now,
        //            Version = 1,
        //            Total_slots = 0,
        //            Occupied_slots = 0
        //        }
        //    };

        //    ViewBag.YardId = yardId;
        //    ViewBag.YardName = yardName;

        //    await Task.CompletedTask;
        //    return View(lanes);
        //}

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            // Lane Status dropdown
            var laneStatusList = await _apiClient.LaneStatusAsync(); // create API client call
            ViewBag.LaneStatusList = Utils.Utility.PrepareSelectList(laneStatusList);
            try
            {
                ViewData["Title"] = "Lane Details";

                // Call the API method
                var result = await _apiClient.GetAllLaneDataAsync();

                // Serialize the result to JSON
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Pass JSON string to the view
                ViewData["LaneData"] = jsonResult;

                return View();
            }
            catch (Exception ex)
            {
                // Handle API error
                return StatusCode(500, ex.Message);
            }
        }
        // ===============================================================
        // 2️⃣ PLOT — Define / Edit Lanes on Map / Load Yard Polygon in Map
        // ===============================================================
        [HttpGet]
        public IActionResult Plot(long yardId, string yardName, string coordinates)
        {
            var model = new laneModel
            {
                Yard_id = yardId,
                Yard_name = yardName
            };

            ViewBag.YardCoordinates = coordinates ?? "[]";
            return View(model);
        }


        // ===============================================================
        // 3️⃣ SAVE — Save Lane Details (simulated for now)
        // ===============================================================
        [HttpPost]
        public async Task<IActionResult> SaveAutoLanes([FromBody] List<laneModel> lanes)
        {
            if (lanes == null || lanes.Count == 0)
                return BadRequest("No lane data received.");

            string currentUser = TempData["LoginUser"]?.ToString() ?? "System";

            try
            {
                // Prepare lanes for API
                var apiLaneList = lanes.Select(l => new YardManagementApplication.LaneModel
                {
                    Lane_id=l.Lane_id,
                    Yard_id = 1,
                    Yard_name = l.Yard_name,
                    Lane_type = l.Lane_type,
                    Lane_name=l.Lane_name,
                    Total_slots = l.Total_slots,
                    Occupied_slots = l.Occupied_slots,
                    Lane_code = l.Lane_code ?? "",
                    Status_id=l.Status_id,
                    Status_name=l.Status_name,
                    Created_by = currentUser,
                    Created_at = DateTimeOffset.Now,
                    Version = 1

                }).ToList();

                // Call API (inserts all lanes)
                await _apiClient.InsertLaneDataAsync(apiLaneList);

                // Success message now uses the count from the list
                return Json(new
                {
                    success = true,
                    message = $"{apiLaneList.Count} lanes inserted successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===============================================================
        // 5️⃣ UPDATE — Update existing Lane
        // ===============================================================
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] laneModel model)
        {
            if (model == null)
                return BadRequest("Lane data is null");

            string currentUser = TempData["LoginUser"]?.ToString() ?? "System";

            try
            {
                // Create update model (matching API expectation)
                var updateModel = new LaneUpdateModel
                {
                    Lane_id = model.Lane_id,
                    Yard_id = model.Yard_id,
                    Yard_name = model.Yard_name,
                    Lane_type = model.Lane_type,
                    Lane_name = model.Lane_name,
                    Total_slots = model.Total_slots,
                    Occupied_slots = model.Occupied_slots,
                    Lane_code = model.Lane_code ?? "",
                    Status_id = model.Status_id,
                    Status_name = model.Status_name,
                    Description = model.Description,
                    Updated_by = currentUser,
                    Updated_at = DateTimeOffset.Now,
                    Version = model.Version + 1
                };

                // Call API client to update lane
                await _apiClient.UpdateLaneDataAsync((long)model.Lane_id, updateModel);

                return Json(new { success = true, Lane_id = model.Lane_id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lane update failed: {ex.Message}" });
            }
        }


        // ===============================================================
        // 4️⃣ DELETE — Delete Lane
        // ===============================================================
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] YardManagementApplication.Models.laneModel model)
        {
            try
            {
                // Set the user performing the deletion
                model.Updated_by = TempData["LoginUser"]?.ToString() ?? "System";

                // Call the API to delete the lane
                await _apiClient.DeleteLaneAsync(model.Lane_id);

                TempData["SuccessMessage"] = "Lane deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting lane: {ex.Message}");
                return View(model);
            }
        }



    }
}
