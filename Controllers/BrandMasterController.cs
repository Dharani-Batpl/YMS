// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : BrandMasrterController.cs
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
// 1.0     | 2025-10-14 | Srinithi G       | Initial creation based on Brand model structure.
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
    public class BrandMasterController : Controller
    {
        private readonly v1Client _apiClient;

       // Inject typed API client



        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<VehicleBrandModel> _validator;


        // Single constructor to inject both dependencies

        public BrandMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<VehicleBrandModel> validator)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

        }


        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Brand Master";
                // Call the generated client method - no URL needed
                var result = await _apiClient.GetAllVehicleBrandAsync();

                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<VehicleBrandModel>
                     {
                         new VehicleBrandModel()
                     };
                }

                // Convert the result object to a JSON string
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Pass JSON string to the view using ViewData

                ViewData["BrandMasterData"] = jsonResult;
                //  Fetch dropdown sources from API
                var statuses = await _apiClient.VehicleBrandStatusAsync(); // <-- This maps to /api/v1/Dropdown/
                var Country = await _apiClient.CountryAsync();
                ViewBag.CountryList = Utils.Utility.PrepareSelectList(Country);
                ViewBag.StatusList = Utils.Utility.PrepareSelectList(statuses);

                return View();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            //catch (ApiException<ProblemDetails> ex)
            //{
            //    // This catches structured API errors (with JSON body)
            //    var problem = ex.Result;

            //    return StatusCode(problem.Status ?? ex.StatusCode, new
            //    {
            //        status = problem.Status ?? ex.StatusCode,
            //        title = "Error",
            //        message = problem.Detail ?? "An unexpected error occurred."
            //    });
            //}

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleBrandModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");
                model.Version = 0;
                var result = await _apiClient.InsertVehicleBrandAsync(model);


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
                    title ="Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }

        }

        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] VehicleBrandUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                //  Call API to Update (by id)
                var result = await _apiClient.UpdateVehicleBrandAsync(model.Brand_id, model);

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
       

        [HttpPut()]
        public async Task<IActionResult> Delete([FromBody] BrandMasterDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                //  Call API delete endpoint (by id)
                var result = await _apiClient.DeleteVehicleBrandAsync(model.Brand_id);

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


        public IActionResult DownloadBrandMasterTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("BrandMasterTemplate");

                // Set headers based on your model properties
                string[] headers = new string[]
                {

                        "Brand_code", "Brand_name", "Manufacturer_name", "Country_name",
                        "Description", "Status_name","Created_by","Created_at","Updated_by","Updated_at"
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
            string excelName = $"BrandMasterTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        //public async Task<IActionResult> Upload(IFormFile file)
        //{
        //    try
        //    {
        //        if (file == null || file.Length == 0)
        //            return BadRequest("No file uploaded.");

        //        // Optional: check extension/MIME
        //        var nameOk = Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);
        //        var type = (file.ContentType ?? "").ToLowerInvariant();
        //        var typeOk = type.Contains("csv") || type == "application/vnd.ms-excel";
        //        if (!nameOk && !typeOk)
        //            return BadRequest("Only CSV files are allowed.");

        //        // Process the CSV file
        //        var res = _csvUploadService.ProcessCsvFile(file, _validator);

        //        // If there are valid records, proceed to insert them or handle them as needed
        //        if (res.ValidItems.Any())
        //        {
        //            foreach (var validItem in res.ValidItems)
        //            {
        //                var result = await _apiClient.InsertVehicleBrandAsync(validItem); // Insert valid records
        //            }

        //            // Generate CSV for invalid records
        //            string invalidRecordsCsv = null;
        //            if (res.InvalidItems.Any())
        //            {
        //                // Generate the CSV file for invalid records
        //                invalidRecordsCsv = _csvUploadService.CreateInvalidCsvWithErrors(res.InvalidItems);
        //            }

        //            // Return success response with download link for invalid records
        //            return Ok(new
        //            {
        //                status = "success",
        //                title = "Success",
        //                message = $"{res.ValidCount} records added successfully",
        //                invalidRecords = invalidRecordsCsv != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidRecordsCsv)) : null // Include CSV for invalid records
        //            });
        //        }

        //        // If no valid items, return success without sending data
        //        return Ok(new
        //        {
        //            status = "success",
        //            title = "No Records",
        //            message = "No valid records to insert."
        //        });

        //    }

        //    catch (ApiException<ProblemDetails> ex)
        //    {
        //        // This catches structured API errors (with JSON body)
        //        var problem = ex.Result;

        //        return StatusCode(problem.Status ?? ex.StatusCode, new
        //        {
        //            status = problem.Status ?? ex.StatusCode,
        //            title = "Error",
        //            message = problem.Detail ?? "An unexpected error occurred."
        //        });
        //    }
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Upload(IFormFile file)
        //{
        //    try
        //    {
        //        // 0) Guards
        //        if (file == null || file.Length == 0)
        //            return BadRequest("No file uploaded.");

        //        var nameOk = Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);
        //        var type = (file.ContentType ?? "").ToLowerInvariant();
        //        var typeOk = type.Contains("csv") || type == "application/vnd.ms-excel";
        //        if (!nameOk && !typeOk)
        //            return BadRequest("Only CSV files are allowed.");

        //        // 1) Parse + validate CSV (captures original CSV RowNumber for invalid rows, and RowMap for valids)
        //        var res = _csvUploadService.ProcessCsvFile<VehicleBrandModel>(file, _validator);

        //        // 2) Insert valid rows; collect API failures WITH row numbers from RowMap
        //        var apiFailuresWithRow = new List<(VehicleBrandModel Record, string Error, int? RowNumber)>();
        //        var successCount = 0;

        //        foreach (var item in res.ValidItems)
        //        {
        //            try
        //            {
        //                await _apiClient.InsertVehicleBrandAsync(item);
        //                successCount++;
        //            }
        //            catch (Exception ex)
        //            {
        //                var msg = ex is ApiException<ProblemDetails> apiEx && apiEx.Result != null
        //                    ? (apiEx.Result.Detail ?? apiEx.Message)
        //                    : ex.Message;

        //                // Lookup original CSV row number for this valid item
        //                res.RowMap.TryGetValue(item, out var row);
        //                apiFailuresWithRow.Add((item, msg ?? "API insert error", row));
        //            }
        //        }

        //        // 3) Build ONE CSV for all failures (validation + API), now WITH correct RowNumber for both
        //        string invalidCsv = _csvUploadService.CreateUnifiedInvalidCsv(res.InvalidItems, apiFailuresWithRow);

        //        // Encode to Base64 only if we have any failures
        //        string invalidBase64 = string.IsNullOrWhiteSpace(invalidCsv)
        //            ? null
        //            : Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidCsv));

        //        // 4) Prepare response
        //        var status = successCount > 0 ? "success" : "error";
        //        var title = successCount > 0 ? "Success" : "No Records";
        //        var message = successCount > 0
        //            ? $"{successCount} records added successfully"
        //            : "No valid records to insert.";

        //        return Ok(new
        //        {
        //            status,
        //            title,
        //            message,
        //            successCount,
        //            validationFailedCount = res.InvalidCount,
        //            apiFailedCount = apiFailuresWithRow.Count,
        //            invalidRecords = invalidBase64 // unified CSV (RowNumber + ErrorMessages + model fields)
        //        });
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        return StatusCode(499, new
        //        {
        //            status = "error",
        //            title = "Client Closed Request",
        //            message = "The upload was cancelled."
        //        });
        //    }
        //    catch (ApiException<ProblemDetails> ex)
        //    {
        //        var problem = ex.Result;
        //        return StatusCode(problem.Status ?? ex.StatusCode, new
        //        {
        //            status = problem.Status ?? ex.StatusCode,
        //            title = "Error",
        //            message = problem.Detail ?? "An unexpected error occurred."
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            status = "error",
        //            title = "Server Error",
        //            message = ex.Message
        //        });
        //    }
        //}
    }
}

