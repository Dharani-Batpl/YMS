// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : TemplateConfigurationController.cs
// Module      : Masters
// Author      : Srinithi G
// Created On  : 2025-10-14
// Description : Controller for managing Template Configuration master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author       | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Srinithi G   | Initial creation based on Template Configuration Model structure.
// =================================================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Models;
using YardManagementApplication.Utils; // <-- use AppJson & Utility

namespace YardManagementApplication
{
    [Route("[controller]/[action]")]
    public class TemplateConfigurationController : Controller
    {
        // -----------------------------------------------------
        // Dependencies
        // -----------------------------------------------------
        private readonly v1Client _apiClient;

        private readonly ILogger<TemplateConfigurationController> _logger;

        public TemplateConfigurationController(v1Client apiClient, ILogger<TemplateConfigurationController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        // =====================================================
        // GET /TemplateConfiguration/Index - Load page & seed ViewData/ViewBag
        // =====================================================
        public async Task<IActionResult> Index()
        {
            string controller = nameof(TemplateConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                ViewData["Title"] = "TemplateConfiguration";

                var result = await _apiClient.GetAllTemplatesAsync();

                // Seed an empty row so the grid shows all columns on first load
                if (result == null || !result.Any())
                {
                    result = new List<TemplateModel> { new TemplateModel() };
                }


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result?.Count() ?? 0
                );


                // Use common utility for consistent date formatting:
                // - DateTime       => MM/dd/yyyy
                // - DateTimeOffset => MM/dd/yyyy HH:mm:ss (24h)
                var jsonOptions = AppJson.CreateDateOnlyOptions();
                var shiftResult = await _apiClient.GetAllShiftAsync();

                if (shiftResult == null || !shiftResult.Any())
                {
                    shiftResult = new List<ShiftMasterModel> { new ShiftMasterModel() };
                }
                else
                {
                    shiftResult = shiftResult
                        .Where(s => s.Is_deleted == false)
                        .ToList();
                }
                ViewBag.ShiftList = shiftResult
                    .Select(s => new SelectListItem
                    {
                        Text = s.Shift_name,   // label
                        Value = s.Shift_id.ToString() // value
                    })
                    .ToList();

                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result, jsonOptions);
                ViewData["TemplateConfiguration"] = jsonResult;

                // Fetch dropdown sources from API
                //var statuses = await _apiClient.TemplateStatusAsync();
                //var shifts = await _apiClient.ShiftAsync();
                //var breaks = await _apiClient.BreakAsync();
                //var plant = await _apiClient.PlantListAsync();

                //ViewBag.StatusList = Utility.PrepareSelectList(statuses);
                //ViewBag.ShiftList = Utility.PrepareSelectList(shifts);
                //ViewBag.BreakList = Utility.PrepareSelectList(breaks);
                //ViewBag.PlantList = Utility.PrepareSelectList(plant);
                _logger.LogInformation(
                   "[ACTION SUCCESS] {controller}.{action} | Message={msg}",
                   controller, action, "Page loaded successfully"
                );

                return View();
            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        // POST /TemplateConfiguration/Create - Create new template
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TemplateModel model)
        {
            string controller = nameof(TemplateConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");
               
                
                Console.WriteLine("Incoming Payload: " + Newtonsoft.Json.JsonConvert.SerializeObject(model));         


                var result = await _apiClient.InsertTemplateAsync(model);

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | result={count}",
                    controller, action, result
                );

                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Template created successfully."
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {

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
        // PUT /TemplateConfiguration/Update - Update existing template
        // =====================================================
            [HttpPut]
            public async Task<IActionResult> Update([FromBody] TemplateUpdateModel model)
            {
                string controller = nameof(TemplateConfigurationController);
                string action = nameof(Index);
                string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

                _logger.LogInformation(
                    "[ACTION START] {controller}.{action} | User={user}",
                    controller, action, user
                );

            try
            {
                //model.updated_by = HttpContext.Session.GetString("LoginUser");

                // Map MVC model → API model
                var apiModel = new TemplateUpdateModel
                {
                    template_id = model.template_id,
                    template_name = model.template_name,
                    template_description = model.template_description,
                    shift_id = model.shift_id,
                    effective_from = model.effective_from,
                    is_deleted = model.is_deleted
                };


                var result = await _apiClient.UpdateTemplateAsync(model.template_id, apiModel);

                _logger.LogInformation(
                        "[ACTION INFO] {controller}.{action} | result={count}",
                        controller, action, result
                    );

                return Ok(new
                    {
                        status = result.Status,
                        title = result.Title ?? "Success",
                        message = result.Detail ?? "Template updated successfully."
                    });
                }
                catch (ApiException<ProblemDetails> ex)
                {

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

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            string controller = nameof(TemplateConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                // ---------------- VALIDATION ----------------
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".xlsx" && ext != ".xls")
                    return BadRequest("Only Excel files (.xlsx/.xls) allowed.");

                // ---------------- READ EXCEL ----------------
                List<Dictionary<string, string>> excelRows = new();
                List<string> headers = new();

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

                // ---------------- MAP EXCEL → MODEL ----------------
                var allItems = new List<TemplateResponseModel>();

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
                            case "templatename": mapped["template_name"] = kv.Value; break;
                            case "templatedescription": mapped["template_description"] = kv.Value; break;
                            case "shiftname": mapped["shift_name"] = kv.Value; break;
                            case "effectivefrom": mapped["effective_from"] = kv.Value; break;
                        }
                    }

                    var model = new TemplateResponseModel
                    {
                        Template_name = mapped.GetValueOrDefault("template_name"),
                        Template_description = mapped.GetValueOrDefault("template_description"),
                        Shift_name = mapped.GetValueOrDefault("shift_name"),
                        Is_deleted = false,
                        Updated_by = currentUser
                    };

                    if (DateTime.TryParse(mapped.GetValueOrDefault("effective_from"), out var dt))
                        model.Effective_from = dt;

                    allItems.Add(model);
                }

                // ---------------- CALL API ----------------
                var errors = new List<object>();
                int success = 0;

                foreach (var item in allItems)
                {
                    try
                    {
                        await _apiClient.UploadTemplateAsync(item); // API endpoint for insert
                        success++;
                    }
                    catch (ApiException<ResponseModel> ex)
                    {

                        _logger.LogError(
                             ex,
                             "[ACTION ERROR] {controller}.{action} | Exception={error}",
                             controller, action, ex.Message
                         );
                        errors.Add(new
                        {
                            name = item.Template_name,
                            error = ex.Result?.Detail ?? ex.Message
                        });
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError(
                             ex,
                             "[ACTION ERROR] {controller}.{action} | Exception={error}",
                             controller, action, ex.Message
                         );
                        errors.Add(new
                        {
                            name = item.Template_name,
                            error = ex.Message
                        });
                    }
                }

                // ---------------- RETURN RESULT ----------------
                if (errors.Count > 0)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "Upload Completed With Errors",
                        message = $"{errors.Count} record(s) failed.",
                        errors
                    });
                }

                return Ok(new
                {
                    status = "success",
                    title = "Upload Completed",
                    message = $"{success} records added successfully.",
                    successCount = success
                });
            }
            catch (Exception ex)
            {

                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

    }

}
