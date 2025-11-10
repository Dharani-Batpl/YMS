using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using YardManagementApplication.Models;
using Newtonsoft;
using Newtonsoft.Json;

namespace YardManagementApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class PlantMasterController : Controller
    {

        private readonly v1Client _apiClient;

        public PlantMasterController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }
        // ===============================================================
        // 1️⃣ INDEX — Plant List Page
        // ===============================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            // Load Enterprise List for dropdown
            var enterpriseList = await _apiClient.EnterpriseListAsync();
            ViewBag.EnterpriseNameList = Utils.Utility.PrepareSelectList(enterpriseList);

            // Plant Status dropdown
            var plantStatusList = await _apiClient.PlantStatusAsync(); // create API client call
            ViewBag.PlantStatusList = Utils.Utility.PrepareSelectList(plantStatusList);
            try
            {
                ViewData["Title"] = "Plant Master";

                // Call the API method
                var result = await _apiClient.GetAllPlantAsync();

                // Serialize the result to JSON
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Pass JSON string to the view
                ViewData["PlantMasterData"] = jsonResult;

                return View();
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }


        // ===============================================================
        // 2️⃣ PLOT — Add / Edit Plant Page
        // ===============================================================
        [HttpGet]
        public IActionResult Plot(
            string mode = "add",
            long plant_id = 0,
            string plant_name = "",
            string plant_code = "",
            long enterprise_id = 0,
            string enterprise_name = "",
            long status_id = 0,
            string status_name = "",
            string street = "",
            string state = "",
            string zipcode = "",
            string country = ""
        )
        {
            var model = new YardManagementApplication.Models.PlantMasterModel
            {
                Plant_id = plant_id,
                Plant_name = plant_name,
                Plant_code = plant_code,
                Enterprise_id = enterprise_id,
                Enterprise_name = enterprise_name,
                Status_id = status_id,
                Status_name = status_name,
                Plant_address = $"{street}, {state}, {zipcode}, {country}".Trim(' ', ',')
            };

            ViewBag.Mode = mode.ToLower();
            TempData["PlantFormData"] = JsonConvert.SerializeObject(model);
            return View(model);
        }

        // ===============================================================
        // 3️⃣ JSON — Return all plants
        // ===============================================================
        [HttpGet]
        public IActionResult GetPlantsJson()
        {
            // TODO: Replace with API call
            return Json(Array.Empty<PlantMasterModel>());
        }

        // ===============================================================
        // INSERT NEW PLANT
        // ===============================================================
        [HttpPost]
        public async Task<IActionResult> InsertPlant([FromBody] PlantMasterModel model)
        {
            if (model == null)
            {
                return BadRequest("Plant data is null");
            }

            string currentUser = TempData["LoginUser"]?.ToString() ?? "System";


            if (TempData["PlantFormData"] != null)
            {
                var json = TempData["PlantFormData"].ToString();
                var modelFromTempData = JsonConvert.DeserializeObject<PlantMasterModel>(json);

            }

            try
            {
                var apiModel = new YardManagementApplication.PlantMasterModel
                {
                    Plant_name = model.Plant_name,
                    Plant_code = model.Plant_code,
                    Enterprise_id = model.Enterprise_id,
                    Enterprise_name = model.Enterprise_name,
                    Status_id = model.Status_id,
                    Status_name = model.Status_name,
                    Plant_address = model.Plant_address,
                    Coordinates = model.Coordinates,
                    Total_area = (double)model.Total_area,
                    Created_by = currentUser,
                    Created_at = DateTimeOffset.Now,
                    Version = 1,
                    Is_deleted = false
                };

                // Call API and get the new Plant ID

                var newPlantId = await _apiClient.InsertPlantAsync(apiModel);

                // Return JSON with the actual ID for frontend
                return Json(new { success = true, Plant_id = newPlantId });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        // ===============================================================
        // UPDATE EXISTING PLANT
        // ===============================================================
        [HttpPost]
        public async Task<IActionResult> UpdatePlant([FromBody] PlantMasterModel model)
        {
            if (model == null) return BadRequest("Plant data is null");
            string currentUser = TempData["LoginUser"]?.ToString() ?? "System";

            try
            {
                var updateModel = new YardManagementApplication.PlantMasterUpdateModel
                {
                    Plant_id = (long)model.Plant_id,
                    Plant_name = model.Plant_name,
                    Plant_code = model.Plant_code,
                    Enterprise_id = model.Enterprise_id,
                    Enterprise_name = model.Enterprise_name,
                    Status_id = model.Status_id,
                    Status_name = model.Status_name,
                    Plant_address = model.Plant_address,
                    Coordinates = model.Coordinates,
                    Total_area = (double?)model.Total_area,
                    Updated_by = currentUser,
                    Updated_at = DateTimeOffset.Now,
                    Version = (model.Version ?? 0) + 1,
                };

                await _apiClient.UpdatePlantAsync((long)model.Plant_id, updateModel);
                return Json(new { success = true, Plant_id = model.Plant_id });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }





        // ===============================================================
        // 5️⃣ DELETE — Soft Delete
        // ===============================================================
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] YardManagementApplication.Models.PlantMasterModel model)
        {
            try
            {
                // Set the user performing the deletion
                model.Updated_by = TempData["LoginUser"]?.ToString() ?? "System";

                // Call the API to soft-delete the plant
                await _apiClient.DeletePlantAsync(model.Plant_id);

                TempData["SuccessMessage"] = "Plant Master record deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }


    }
}
