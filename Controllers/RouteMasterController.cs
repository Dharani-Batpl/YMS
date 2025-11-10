// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : RouteMasrterController.cs
// Module      : Masters
// Author      : Srinithi G
// Created On  : 2025-10-14
// Description : Controller for managing Route master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Srinithi G       | Initial creation based on Route model structure.
// =================================================================================================

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;

namespace YardManagementApplication
{
    [Route("[controller]/[action]")]
    public class RouteMasterController : Controller
    {
        private readonly v1Client _apiClient;





        // Inject typed API client



        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<RouteModel> _validator;


        // Single constructor to inject both dependencies

        public RouteMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<RouteModel> validator)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Route Master";
                // Call the generated client method - no URL needed
                var result = await _apiClient.GetAllRouteAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<RouteModel>
                       {
                           new RouteModel()
                       };
                }
                // Convert the result object to a JSON string
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                ViewData["RouteMasterData"] = jsonResult;
                //  Fetch dropdown sources from API
                var statuses = await _apiClient.RouteStatusAsync(); // <-- This maps to /api/v1/Dropdown/
                var Source = await _apiClient.RouteSourceAreaAsync();
                //var destination = await _apiClient.RouteDestinationAreaAsync();
                ViewBag.SourceList = Utils.Utility.PrepareSelectList(Source);
                //ViewBag.DestinationList = Utils.Utility.PrepareSelectList(destination);
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
                    title =  "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RouteModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");
                // model.Created_at = DateTimeOffset.Now;
                //  Call API
                var result = await _apiClient.InsertRouteAsync(model);
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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RouteUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                //model.Updated_at = DateTimeOffset.Now;

                //Call API to Update(by id)
                var result = await _apiClient.UpdateRouteAsync(model.Route_id, model);

                //Return JSON for common JS toast
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

        [HttpGet]
        public async Task<IActionResult> GetDestination(long source)
        {
            try
            {

                var result = await _apiClient.DestinationAsync(source);

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




        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] RouteMasterDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                //  Call API delete endpoint (by id)
                var result = await _apiClient.DeleteRouteAsync(model.Route_id);

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

        public IActionResult DownloadRouteMasterTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("RouteMasterTemplate");

                string[] headers = new string[]
                {
                    "Source_process_area_name",
                    "destination_process_area_name",
                    "Sla_minutes_cnt"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Column(i + 1).AutoFit();
                }

                package.Save();
            }

            stream.Position = 0;
            string excelName = $"RouteMasterTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpPost]
        public async Task<IActionResult> UploadRouteMasterTemplate(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var totalRows = worksheet.Dimension.End.Row;
                var totalCols = worksheet.Dimension.End.Column;

                var colIndexMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int col = 1; col <= totalCols; col++)
                {
                    var colName = worksheet.Cells[1, col].Text.Trim();
                    if (!string.IsNullOrEmpty(colName))
                        colIndexMap[colName] = col;
                }

                string GetCellValue(int row, string colName)
                {
                    return colIndexMap.TryGetValue(colName, out int col)
                        ? worksheet.Cells[row, col].Text.Trim()
                        : null;
                }

                var validRows = new List<RouteModel>();
                var invalidRows = new List<(RouteModel row, List<string> errors)>();

                for (int row = 2; row <= totalRows; row++)
                {
                    var model = new RouteModel
                    {
                        //Source_process_area_name = GetCellValue(row, "source_process_area_name"),
                        //Destination_process_area_name = GetCellValue(row, "destination_process_area_name"),
                        //Sla_minutes_cnt = int.TryParse(GetCellValue(row, "sla_minutes_cnt"), out int sla) ? sla : 0,
                        //Source_process_area_id = 0,
                        //Destination_process_area_id = 0,
                        Status_id = 1,
                        Status_name = "Active",
                        Is_deleted = false,
                        Created_by = "System",
                        Created_at = DateTimeOffset.Now,
                        Version = 1
                    };

                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext(model);
                    bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

                    if (isValid)
                        validRows.Add(model);
                    else
                        invalidRows.Add((model, validationResults.Select(v => v.ErrorMessage ?? string.Empty).ToList()));
                }

                // Example response
                return Ok(new
                {
                    success = true,
                    validCount = validRows.Count,
                    invalidCount = invalidRows.Count
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
                        var result = await _apiClient.InsertRouteAsync(validItem); // Insert valid records
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
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }
    }
}
