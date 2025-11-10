
// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : BreakMasterController.cs
// Module      : Masters
// Author      : Dhanalakshmi D
// Created On  : 2025-10-14
// Description : Controller for managing break master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Dhanalakshmi D   | Initial creation based on break master model structure.
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
    public class BreakMasterController : Controller
    {
        private readonly v1Client _apiClient;





        // Inject typed API client



        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<BreakTimeModel> _validator;


        // Single constructor to inject both dependencies

        public BreakMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<BreakTimeModel> validator)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

        }
        // =====================================================
        //  Render main page with grid data + dropdowns
        // =====================================================
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Break Master";

                //  Fetch users list from API
                var result = await _apiClient.GetAllBreakTimeAsync();

                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<BreakTimeModel>
                       {
                           new BreakTimeModel()
                       };
                }

                //  Serialize data for the view (Tabulator expects JSON)
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                //  Pass JSON to view
                ViewData["BreakMasterData"] = jsonResult;

                //  Fetch dropdown sources from API   
                var statuses = await _apiClient.BreakTimeStatusAsync();

                // Convert API lists to SelectList for UI controls
                ViewBag.StatusList = Utility.PrepareSelectList(statuses);

                //  Return view
                return View();
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title ="Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }
        // =====================================================
        //  Create Break (HTTP POST)
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BreakTimeModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");
                model.Version = 0;

                // Call generated client to create
                var result = await _apiClient.InsertBreakTimeAsync(model);

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title =  "Success",
                    message = result.Detail ?? "Record updated successfully"
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

        // =====================================================
        //  Update Break (HTTP PUT)
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] BreakTimeUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                model.Updated_at = DateTimeOffset.Now;

                // Call generated client to create
                var result = await _apiClient.UpdateBreakTimeAsync(model.Break_id, model);

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record updated successfully"
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
        // =====================================================
        //  Delete break (HTTP PUT calling API delete by id)
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] BreakMasterDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                model.Updated_at = DateTimeOffset.Now;

                //  Call API delete endpoint (by id)
                var result = await _apiClient.DeleteBreakTimeAsync(model.Break_id);

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record updated successfully"
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

        // =====================================================
        /// Download an Excel template for Break Master bulk upload
         // =====================================================
        public IActionResult DownloadBreakMasterTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BreakMasterTemplate");

                // Set headers based on your model properties
                string[] headers = new string[]
                {
                    "Break_id", "Break_name", "Break_description",
                    "Status_id", "Status_name", "Is_deleted",
                    "Created_by", "Created_at", "Updated_by", "Updated_at", "Version"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Column(i + 1).AutoFit();
                }

                // Optionally: Add data validation / formatting (e.g., TRUE/FALSE for is_deleted, date formats)
                // Example: worksheet.Cells[2, 6, 1000, 6].DataValidation.AddListDataValidation().Items.Add("TRUE");

                package.Save();
            }

            stream.Position = 0;
            string excelName = $"BreakMasterTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
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
                        var result = await _apiClient.InsertBreakTimeAsync(validItem); // Insert valid records
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
                    title = "No Records",
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
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }
    }
}
