using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.IO;
using System.Reflection.Emit;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class YardController : Controller
    {
        private readonly v1Client _apiClient;

        public YardController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }

        // ===============================================================
        // 1️⃣ INDEX — Yard List Page (unchanged)
        // ===============================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var plantList = await _apiClient.PlantListCoordinatesAsync();
            ViewBag.PlantNameList = Utils.Utility.PrepareSelectList(plantList);
            

            var yardStatusList = await _apiClient.YardStatusAsync();
            ViewBag.YardStatusList = Utils.Utility.PrepareSelectList(yardStatusList);

            try
            {
                ViewData["Title"] = "Yard Master";
                var result = await _apiClient.GetAllYardAsync();
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);
                ViewData["YardData"] = jsonResult;

                return View();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===============================================================
        // 2️⃣ PLOT — Add / Edit / View Yard Page
        // ===============================================================
        [HttpGet]
        public IActionResult Plot(
            string mode = "add",
            long yard_id = 0,
            string yard_name = "",
            string yard_code = "",
            string yard_type="",
            long plant_id = 1,
            string plant_name = "",
            long status_id = 0,
            string status_name = "",
            string description = "",
            string created_by=""
           
         )
        {
            var model = new YardManagementApplication.Models.YardModel
            {
                Yard_id = yard_id,
                Yard_name = yard_name,
                Yard_code = yard_code,
                Yard_type = yard_type,
                Plant_id = plant_id,
                Plant_name = plant_name,
                Status_id = status_id,
                Status_name = status_name,
                Description = description,
                Created_by = created_by
                
            };
            ViewBag.Mode = mode.ToLower();
            TempData["YardFormData"] = JsonConvert.SerializeObject(model);
            return View(model);
        }

        // ===============================================================
        // 3️⃣ INSERT NEW YARD — Called from frontend Plot form
        // ===============================================================
        [HttpPost]
        public async Task<IActionResult> InsertYard([FromBody] YardModel model)
        {
            if (model == null) return BadRequest("Yard data is null");

            string currentUser = TempData["LoginUser"]?.ToString() ?? "System";

            if (TempData["YardFormData"] != null)
            {
                var json = TempData["YardFormData"].ToString();
                var formModel = JsonConvert.DeserializeObject<YardModel>(json);
                model.Plant_id = formModel.Plant_id;
                model.Status_id = formModel.Status_id;
                model.Plant_name = formModel.Plant_name;
                model.Description = formModel.Description;
                model.Status_name = formModel.Status_name;  
                model.Yard_type=formModel.Yard_type;
                model.Created_by = currentUser;
                model.Created_at=formModel.Created_at;
            }

            try
            {
                var apiModel = new YardManagementApplication.Models.YardModel
                {
                    Yard_id = (long)model.Yard_id,
                    Yard_name = model.Yard_name,
                    Yard_code = model.Yard_code,
                    Yard_type = model.Yard_type,
                    Plant_id = model.Plant_id,
                    Plant_name = model.Plant_name,
                    Status_id = model.Status_id,
                    Status_name = model.Status_name,
                    Coordinates = model.Coordinates,
                    Area_acres = (long)model.Area_acres,
                    Created_by = currentUser,
                    Created_at = DateTimeOffset.Now,
                    Version = 1,
                    Is_deleted = false
                };

                
                var newYardId = await _apiClient.InsertYardAsync(model);

                return Json(new { success = true, Yard_id = newYardId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        // ===============================================================
        // 4️⃣ UPDATE EXISTING YARD — Called from frontend Plot form
        // ===============================================================
        [HttpPost]
        public async Task<IActionResult> UpdateYard([FromBody] YardModel model)
        {
            if (model == null)
            {
                return BadRequest("Yard data is null");
            }

            string currentUser = TempData["LoginUser"]?.ToString() ?? "System";

            // Retrieve full form data from TempData (like Plant does)
            if (TempData["YardFormData"] != null)
            {
                var json = TempData["YardFormData"].ToString();
                var modelFromTempData = JsonConvert.DeserializeObject<YardModel>(json);

                model.Plant_id = modelFromTempData.Plant_id;
                model.Status_id = modelFromTempData.Status_id;
                model.Yard_id = modelFromTempData.Yard_id;
            }

            try
            {
                // ✅ Build the YardUpdateModel (same structure as Plant)
                var updateModel = new YardUpdateModel
                {
                    Yard_id = (long)model.Yard_id,
                    Yard_code = model.Yard_code,
                    Yard_name = model.Yard_name,
                    Plant_id = model.Plant_id,
                    Status_id = model.Status_id,
                    Coordinates = model.Coordinates,
                    Area_acres = model.Area_acres,
                    Description = model.Description,
                    Updated_by = currentUser,
                    Updated_at = DateTimeOffset.Now,
                    Version = model.Version + 1
                };

                // ✅ Call API update
                await _apiClient.UpdateYardAsync((long)model.Yard_id, updateModel);

                return Json(new { success = true, Yard_id = model.Yard_id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ===============================================================
        // 5️⃣ SAVE PLOT — Optional form post (unchanged)
        // ===============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePlot(
            YardModel model,
            string CoordinatesJson = "[]",
            double Area_acres = 0
        )
        {
            if (!ModelState.IsValid)
                return View("Plot", model);

            model.Coordinates = CoordinatesJson;
            model.Area_acres = Area_acres;

            TempData["Message"] = "Yard saved successfully!";
            return RedirectToAction("Index");
        }

        // ===============================================================
        // 6️⃣ DELETE — Delete Yard by ID (unchanged)
        // ===============================================================
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] YardModel model)
        {
            try
            {
                model.Updated_by = TempData["LoginUser"]?.ToString() ?? "System";
                await _apiClient.DeleteYardAsync(model.Yard_id);

                return Json(new { success = true, message = "Yard record deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPlantById(long Plant_id)
        {
            try
            {
                var response = await _apiClient.GetPlantByIdAsync(Plant_id);
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPlantCoordinates(long plantId)
        {
            if (plantId <= 0) return BadRequest("Invalid Plant ID");

            try
            {
                var plant = await _apiClient.GetPlantByIdAsync(plantId);

                if (plant == null) return NotFound("Plant not found");

                return Json(new { success = true, coordinates = plant.Coordinates ?? "[]" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetYardList()
        {
            try
            {
                var allYards = await _apiClient.GetAllYardAsync();
                return Json(allYards); // Return as JSON for JS to filter
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
