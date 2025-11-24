// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : HolidayMasterController.cs
// Module      : Masters
// Author      : Dhanalakshmi D
// Created On  : 2025-10-14
// Description : Controller for managing holiday master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Dhanalakshmi D   | Initial creation based on holiday master model structure.
// =================================================================================================
// 1.1     | 2025-11-24 | Dharani T   | Modified the Upload method to handle Excel file uploads for bulk creation.
// =================================================================================================

using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;   // <-- IMPORTANT: to use AppJson & Utility

namespace YardManagementApplication
{
    [Route("[controller]/[action]")]
    public class HolidayMasterController : Controller
    {
        // -----------------------------------------------------
        // Dependencies
        // -----------------------------------------------------
        private readonly v1Client _apiClient;
        private readonly CsvUploadService _csvUploadService;
        private readonly IValidator<HolidayModel> _validator;
        private readonly ILogger<HolidayMasterController> _logger;



        // Single constructor to inject dependencies
        public HolidayMasterController(
            CsvUploadService csvUploadService,
            v1Client apiClient,
            IValidator<HolidayModel> validator, ILogger<HolidayMasterController> logger)
        {
            _csvUploadService = csvUploadService;
            _apiClient = apiClient;
            _validator = validator;
            _logger = logger;
        }

        // =====================================================
        // GET /HolidayMaster/Index - Load page & seed ViewData/ViewBag
        // =====================================================
        public async Task<IActionResult> Index()
        {
            // Log action start
            string controller = nameof(HolidayMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );
            try
            {

                ViewData["Title"] = "Holiday Master";

                var result = await _apiClient.GetAllHolidayAsync();

                // Ensure at least one row so the grid shows columns
                if (result == null || !result.Any())
                {
                    result = new List<HolidayModel> { new HolidayModel() };
                }

                
                var jsonOptions = AppJson.CreateDateOnlyOptions();


                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result, jsonOptions);
                ViewData["HolidayMasterData"] = jsonResult;

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result?.Count() ?? 0
                );
                // Dropdowns
                var statuses = await _apiClient.HolidayStatusAsync();
                var holidayType = await _apiClient.HolidayTypeAsync();
                ViewBag.StatusList = Utility.PrepareSelectList(statuses);
                ViewBag.HolidayTypeList = Utility.PrepareSelectList(holidayType);

                return View();
            }
            catch (ApiException<ProblemDetails> ex)
            {
                var problem = ex.Result;

                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

        // =====================================================
        // POST /HolidayMaster/Create - Create new holiday
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HolidayModel model)
        {
            // Log action start
            string controller = nameof(HolidayMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");
              


                var result = await _apiClient.InsertHolidayAsync(model);


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result
                );


                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {

                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
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
        // PUT /HolidayMaster/Update - Update existing holiday (partial)
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] HolidayUpdateModel model)
        {
            // Log action start
            string controller = nameof(HolidayMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                var result = await _apiClient.UpdateHolidayAsync(model.Holiday_id, model);




                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result
                );

                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
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
        // PUT /HolidayMaster/Delete - Soft delete by id
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] HolidayMasterDeleteModel model)
        {
            // Log action start
            string controller = nameof(HolidayMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                var result = await _apiClient.DeleteHolidayAsync(model.Holiday_id);



                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result
                );


                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }

      
        // -----------------------------------------------------
        // UPLOAD HOLIDAY DATA VIA EXCEL
        // -----------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            //Log action start
            string controller = nameof(HolidayMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".xlsx" && ext != ".xls")
                    return BadRequest("Only Excel files (.xlsx/.xls) allowed.");


                List<Dictionary<string, string>> excelRows = new();
                List<string> headers = new();

                // Read Excel file
                using (var stream = file.OpenReadStream())
                {
                    IWorkbook wb = ext == ".xlsx" ? new XSSFWorkbook(stream) : new HSSFWorkbook(stream);
                    var sheet = wb.GetSheetAt(0);
                    var header = sheet.GetRow(0);

                    for (int i = 0; i < header.LastCellNum; i++)
                        headers.Add(header.GetCell(i)?.ToString()?.Trim() ?? $"Col{i}");

                    for (int r = 1; r <= sheet.LastRowNum; r++)
                    {
                        var row = sheet.GetRow(r);
                        if (row == null) continue;

                        var dict = new Dictionary<string, string>();
                        for (int c = 0; c < headers.Count; c++)
                            dict[headers[c]] = row.GetCell(c)?.ToString()?.Trim() ?? "";

                        excelRows.Add(dict);
                    }
                }

                string currentUser = HttpContext.Session.GetString("LoginUser") ?? "System";


                var allItems = new List<HolidayModel>();

                // Map Excel rows to HolidayModel
                foreach (var row in excelRows)
                {
                    string norm(string x) =>
                        x.Trim().ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");

                    var mapped = new Dictionary<string, string>();

                    foreach (var kv in row)
                    {
                        string key = norm(kv.Key);
                        switch (key)
                        {
                            case "holidayname": mapped["holiday_name"] = kv.Value; break;
                            case "holidaydescription": mapped["holiday_description"] = kv.Value; break;
                            case "holidaytype": mapped["holiday_type_name"] = kv.Value; break;
                            case "holidaydate": mapped["holiday_date"] = kv.Value; break;
                        }
                    }

                    var model = new HolidayModel
                    {
                        Holiday_name = mapped.GetValueOrDefault("holiday_name"),
                        Description = mapped.GetValueOrDefault("holiday_description"),
                        Holiday_type_name = mapped.GetValueOrDefault("holiday_type_name"),
                        Is_deleted = false,
                        Created_by = currentUser
                    };

                    if (DateTime.TryParse(mapped.GetValueOrDefault("holiday_date"), out var dt))
                        model.Holiday_date = dt;

                    allItems.Add(model);
                }

                // Upload each template via API
                int successCount = 0;
                var apiErrors = new List<object>();

                foreach (var item in allItems)
                {
                    try
                    {
                        await _apiClient.UploadHolidayAsync(item);
                        successCount++;
                    }
                    catch (ApiException<ResponseModel> ex)
                    {
                        // Log API exception
                        _logger.LogError(
                             ex,
                             "[ACTION ERROR] {controller}.{action} | Exception={error}",
                             controller, action, ex.Message
                         );

                        apiErrors.Add(new
                        {

                            error = ex.Result?.Detail ?? ex.Message
                        });
                    }
                    catch (Exception ex)
                    {
                        // Log general exception
                        _logger.LogError(
                             ex,
                             "[ACTION ERROR] {controller}.{action} | Exception={error}",
                             controller, action, ex.Message
                         );

                        apiErrors.Add(new
                        {

                            error = ex.Message
                        });
                    }
                }


                if (apiErrors.Count > 0)
                {
                    // Return errors if any
                    return BadRequest(new
                    {
                        status = "error",
                        title = "Upload Failed",
                        message = $"{apiErrors.Count} record(s) failed.",
                        errors = apiErrors
                    });
                }

                return Ok(new
                {
                    status = "success",
                    title = "Success",
                    message = $"{successCount} records added successfully.",
                    successCount
                });
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );

                return BadRequest(new
                {
                    status = "error",
                    title = "Error",
                    message = ex.Message
                });
            }
        }

    }
}
