// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : ReasonMasrterController.cs
// Module      : Masters
// Author      : Srinithi G
// Created On  : 2025-10-14
// Description : Controller for managing Reason master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Srinithi G       | Initial creation based on Reason model structure.
// =================================================================================================
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ReasonMasterController : Controller
    {
        private readonly v1Client _apiClient;





        // Inject typed API client



        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<ReasonModel> _validator;


        // Single constructor to inject both dependencies

        public ReasonMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<ReasonModel> validator)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Reason Master";
                // Call the generated client method - no URL needed
                var result = await _apiClient.GetAllReasonAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<ReasonModel>
                       {
                           new ReasonModel()
                       };
                }
                // Convert the result object to a JSON string
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Pass JSON string to the view using ViewData
                ViewData["ReasonMasterData"] = jsonResult;


                //  Fetch dropdown sources from API
                var statuses = await _apiClient.ReasonStatusAsync();

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
        public async Task<IActionResult> Create([FromBody] ReasonModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");
                //model.Created_at = DateTimeOffset.Now;

                var result = await _apiClient.InsertReasonAsync(model);
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
                    title =  "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] ReasonUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                //model.Updated_at = DateTimeOffset.Now;

                //  Call API to update (by id)
                var result = await _apiClient.UpdateReasonAsync(model.Reason_id, model);
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
                    title =  "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }


        //  Delete user (HTTP PUT calling API delete by id)
        [HttpPut()]
        public async Task<IActionResult> Delete([FromBody] ReasonDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                //model.Updated_at = DateTimeOffset.Now;

                //  Call API delete endpoint (by id)
                var result = await _apiClient.DeleteReasonAsync(model.Reason_id);
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

        //  Generate downloadable Excel template for bulk user upload
        public IActionResult DownloadReasonMasterTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                //  Create worksheet
                var worksheet = package.Workbook.Worksheets.Add("ReasonMasterTemplate");

                // Set headers based on your model properties
                string[] headers = new string[]
                {
                    "Reason_id", "Reason_code", "Reason_name", "Status_id",
                    "Status_name", "Is_deleted", "Updated_by", "Updated_at", "Version"
                };

                //  Write headers + autosize columns
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Column(i + 1).AutoFit();
                }


                //  Save to stream
                package.Save();
            }
            //  Reset stream position and return as file
            stream.Position = 0;
            string excelName = $"ReasonMasterTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        [HttpPut]
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
                        var result = await _apiClient.InsertReasonAsync(validItem); // Insert valid records
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

