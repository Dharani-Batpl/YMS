using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class SlotController : Controller
    {
        private readonly v1Client _apiClient;

        public SlotController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }
        // ===============================================================
        // 1️⃣ INDEX — Slot List Page
        // ===============================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            // Slot Status dropdown
            var slotStatusList = await _apiClient.SlotStatusAsync(); // create API client call
            ViewBag.SlotStatusList = Utils.Utility.PrepareSelectList(slotStatusList);
            try
            {
                ViewData["Title"] = "Slot Details";

                // Call the API method
                var result = await _apiClient.GetAllSlotDataAsync();

                // Serialize the result to JSON
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Pass JSON string to the view
                ViewData["SlotData"] = jsonResult;

                return View();
            }
            catch (Exception ex)
            {
                // Handle API error
                return StatusCode(500, ex.Message);
            }
        }

        // ===============================================================
        // 2️⃣ GET — Return all saved slots (API style)
        // ===============================================================
        [HttpGet]
        public IActionResult GetSlotList()
        {
            // Return empty list; integrate with DB/API as needed
            return Json(new List<SlotModel>());
        }

        // ===============================================================
        // 3️⃣ POST — Save multiple slots (from JS)
        // ===============================================================
        [HttpPost]
        public async Task<IActionResult> SaveSlots([FromBody] List<SlotModel> slots)
        {
            if (slots == null)
                return Json(new { success = false, message = "Slot list is null (no JSON body)." });
            if (slots.Count == 0)
                return Json(new { success = false, message = "Slot list is empty." });

            string currentUser = TempData["LoginUser"]?.ToString() ?? "System";

            try
            {
                var apiSlotList = slots.Select(s => new SlotModel
                {
                    Slot_id = s.Slot_id,
                    Lane_id = s.Lane_id,
                    Lane_name = s.Lane_name,
                    Slot_type = s.Slot_type,
                    Slot_name = s.Slot_name,
                    Is_occupied = s.Is_occupied,
                    Coordinates = s.Coordinates,
                    Description=s.Description,
                    Capacity_cnt = s.Capacity_cnt,
                    Slot_code = s.Slot_code ?? "",
                    Status_id = s.Status_id,
                    Status_name = s.Status_name,
                    Created_by = currentUser,
                    Created_at = DateTimeOffset.Now,
                    Version = 1
                }).ToList();

                await _apiClient.InsertSlotAsync(apiSlotList);

                return Json(new
                {
                    success = true,
                    message = $"{apiSlotList.Count} slots inserted successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Slot save failed: {ex.Message}" });
            }
        }

        // ===============================================================
        // UPDATE EXISTING SLOT
        // ===============================================================
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] SlotModel model)
        {
            if (model == null)
                return BadRequest("Slot data is null");

            string currentUser = TempData["LoginUser"]?.ToString() ?? "System";

            // Retrieve full form data from TempData if needed
            if (TempData["SlotFormData"] != null)
            {
                var json = TempData["SlotFormData"].ToString();
                var modelFromTempData = JsonConvert.DeserializeObject<SlotModel>(json);

                model.Slot_id = modelFromTempData.Slot_id;
                model.Lane_id = modelFromTempData.Lane_id;
                model.Lane_name = modelFromTempData.Lane_name;
                model.Slot_code = modelFromTempData.Slot_code;
                model.Slot_name = modelFromTempData.Slot_name;
                model.Coordinates = modelFromTempData.Coordinates;
                model.Capacity_cnt = modelFromTempData.Capacity_cnt;
                model.Version = modelFromTempData.Version;
            }

            try
            {
                var updateModel = new SlotUpdateModel
                {
                    Slot_id = model.Slot_id,
                    Lane_id = model.Lane_id,
                    Lane_name = model.Lane_name,
                    Slot_type = model.Slot_type,
                    Slot_name = model.Slot_name,
                    Slot_code = model.Slot_code ?? "",
                    Is_occupied = model.Is_occupied,
                    Status_id = model.Status_id,
                    Status_name = model.Status_name,
                    Description = model.Description,
                    Coordinates = model.Coordinates,
                    Capacity_cnt = model.Capacity_cnt,
                    Updated_by = currentUser,
                    Updated_at = DateTimeOffset.Now,
                    Version = model.Version + 1
                };

                // Call the API client to update slot
                await _apiClient.UpdateSlotDataAsync((long)model.Slot_id,updateModel);

                return Json(new { success = true, Slot_id = model.Slot_id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ===============================================================
        // 4️⃣ DELETE — Delete Slot
        // ===============================================================
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] YardManagementApplication.Models.SlotModel model)
        {
            try
            {
                // Set the user performing the deletion
                model.Updated_by = TempData["LoginUser"]?.ToString() ?? "System";

                // Call the API to delete the slot
                await _apiClient.DeleteSlotAsync(model.Slot_id);

                TempData["SuccessMessage"] = "Slot deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting slot: {ex.Message}");
                return View(model);
            }
        }








        //// ===============================================================
        //// 4️⃣ POST — Save individual slot (optional)
        //// ===============================================================
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SaveSlot(SlotModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View("Plot", model);

        //    // TODO: Replace with actual DB/API save
        //    model.Status_id = model.Status_id == 0 ? 1 : model.Status_id;
        //    model.Status_name ??= "Active";
        //    model.Created_by ??= "System";
        //    //model.Created_at ??= DateTimeOffset.Now;
        //    model.Updated_by ??= "System";
        //    model.Updated_at ??= DateTimeOffset.Now;
        //    model.Is_deleted = false;

        //    await Task.CompletedTask;

        //    TempData["Message"] = "Slot configuration saved successfully!";
        //    return RedirectToAction("Index");
        //}
    }
}
