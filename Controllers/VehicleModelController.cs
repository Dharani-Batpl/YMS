// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : VehicleModelController.cs
// Module      : Masters
// Author      : Kavin R
// Created On  : 2025-10-14
// Description : Controller for managing Vehicle Model master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Kavin R  | Initial creation based on Vehicle Model structure.
// =================================================================================================

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;



namespace YardManagementApplication
{
    [Route("[controller]/[action]")]
    public class VehicleModelController : Controller
    {

        private readonly v1Client _apiClient;

        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<VehicleModel> _validator;

        public VehicleModelController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<VehicleModel> validator)
        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

        }
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Vehicle Model Master";
                var result = await _apiClient.GetAllVehicleModelAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<VehicleModel>
                       {
                           new VehicleModel()
                       };
                }
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                ViewData["VehicleModelData"] = jsonResult;

                var BrandName = await _apiClient.VehicleBrandAsync();
                var statuses = await _apiClient.UserStatusAsync();

                ViewBag.StatusList = Utils.Utility.PrepareSelectList(statuses);
                ViewBag.BrandNameList = Utils.Utility.PrepareSelectList(BrandName);
                ViewBag.ModelYearList = Utils.Utility.GetModelYearList();

                return View();
            }
            catch (ApiException<ProblemDetails> ex)
            {
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");
                model.Created_at = DateTimeOffset.Now;
                model.Is_deleted = false;
                string[] Code = model.Model_year_code.Split('-');
                model.Version = 1;

                model.Model_year_code = Code[0];

                var result = await _apiClient.InsertVehicleModelAsync(model);

                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record Inserted successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] VehicleModelUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                model.Updated_at = DateTimeOffset.Now;
                string[] Code = model.Model_year_code.Split('-');
                model.Model_year_code = Code[0];

                var result = await _apiClient.UpdateVehicleModelAsync(model.Variant_id, model);

                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] VehicleModelDeleteModel model)
        {
            try
            {
                model.updated_by = HttpContext.Session.GetString("LoginUser");

                var result = await _apiClient.DeleteVehicleModelAsync(model.variant_id);

                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record Deleted successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }

        }

        public IActionResult DownloadVehicleModelTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("VehicleModelTemplate");

                // Set headers based on your model properties
                string[] headers = new string[]
                {
                    "Model_id", "Model_code", "Model_name",
                    "Brand_name", "Vehicle_type_name",
                    "Model_year", "Status_name", "Description",
                    "Created_by", "Created_at",
                    "Updated_by", "Updated_at",
                    "Version"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Column(i + 1).AutoFit();
                }

                // Optionally: Add some data validation or formatting (e.g., dropdowns, date format)

                package.Save();
            }

            stream.Position = 0;
            string excelName = $"VehicleModelTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleType(long brandid)
        {
            try
            {

                var result = await _apiClient.VehicleTypeAsync(brandid);

                // Return JSON for common JS toast
                return Ok(new
                {
                    variantList = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "error",
                    title = "Error",
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                // Optional: check extension/MIME
                var nameOk = Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);
                var type = (file.ContentType ?? "").ToLowerInvariant();
                var typeOk = type.Contains("csv") || type == "application/vnd.ms-excel";
                if (!nameOk && !typeOk)
                    return BadRequest("Only CSV files are allowed.");

                // Process the CSV file
                var res = _csvUploadService.ProcessCsvFile(file, _validator);

                // If there are valid records, proceed to insert them or handle them as needed
                if (res.ValidItems.Any())
                {
                    foreach (var validItem in res.ValidItems)
                    {
                        var result = await _apiClient.InsertVehicleModelAsync(validItem); // Insert valid records
                    }

                    // Generate CSV for invalid records
                    string invalidRecordsCsv = null;
                    if (res.InvalidItems.Any())
                    {
                        // Generate the CSV file for invalid records
                        invalidRecordsCsv = _csvUploadService.CreateInvalidCsvWithErrors(res.InvalidItems);
                    }

                    // Return success response with download link for invalid records
                    return Ok(new
                    {
                        status = "success",
                        title = "Success",
                        message = $"{res.ValidCount} records added successfully",
                        invalidRecords = invalidRecordsCsv != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidRecordsCsv)) : null // Include CSV for invalid records
                    });
                }

                // If no valid items, return success without sending data
                return Ok(new
                {
                    status = "success",
                    title = "Errors",
                    message = "No valid records to insert."
                });

            }
            catch (ApiException<ProblemDetails> ex)
            {
                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }
    }
}
