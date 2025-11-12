// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : DepartmentMasterController.cs
// Module      : Masters
// Author      : Sujitha B
// Created On  : 2025-10-18
// Description : Controller for managing Reason master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-18 | Sujitha B      | Initial creation based on Department model structure.
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
    public class ShiftMasterController : Controller
    {
        // Inject typed API client

        private readonly v1Client _apiClient;

        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<ShiftModel> _validator;


        // Single constructor to inject both dependencies

        public ShiftMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<ShiftModel> validator)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

        }


        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Shift Master";
                // Call the generated client method - no URL needed
                var result = await _apiClient.GetAllShiftAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<ShiftModel>
                       {
                           new ShiftModel()
                       };
                }
                // Convert the result object to a JSON string
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Pass JSON string to the view using ViewData

                ViewData["ShiftMasterData"] = jsonResult;
                //  Fetch dropdown sources from API
                var statuses = await _apiClient.ShiftStatusAsync(); // <-- This maps to /api/v1/Dropdown/
                //var Country = await _apiClient.CountryAsync();
                //ViewBag.CountryList = Utils.Utility.PrepareSelectList(Country);
                ViewBag.StatusList = Utils.Utility.PrepareSelectList(statuses);
                return View();
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShiftModel model)
        {
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
                model.Version = 1;

               
                var result = await _apiClient.InsertShiftAsync(model);

                return Ok(new
                {
                    status = result.Status,
                    title =  "Success",
                    message = result.Detail ?? "Record created successfully"
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




        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] ShiftUpdateModel model)
        {
            try
            {

                model.Updated_by = HttpContext.Session.GetString("LoginUser");


                var result = await _apiClient.UpdateShiftAsync(model.Shift_id, model);

              
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


        [HttpPut()]
        public async Task<IActionResult> Delete([FromBody] ShiftModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                // Validate that Department_id is provided
                if (model.Shift_id == null || model.Shift_id <= 0)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "Invalid Data",
                        message = "shift ID is required for delete."
                    });
                }

                long id = model.Shift_id;

                var result = await _apiClient.DeleteShiftAsync(id);

                return Ok(new
                {
                    status = result.Status,
                    title =  "Success",
                    message = result.Detail ?? "Record deleted successfully"
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

        public IActionResult DownloadShiftMasterTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("ShiftMasterTemplate");

                // Set headers based on your model properties
                string[] headers = new string[]
                {

                        "Shift_name",
                        "Shidt_description", "Status_name","Created_by","Created_at","Updated_by","Updated_at"
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
            string excelName = $"ShiftMasterTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

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
                        var result = await _apiClient.InsertShiftAsync(validItem); // Insert valid records
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
                    title =  "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }
    }
}