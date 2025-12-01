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
// 1.1     | 2025-11-13 | Dharani T   | Modified the Upload method to handle Excel file uploads for bulk template creation.
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
        //  Render main page 
        // =====================================================
        public async Task<IActionResult> Index()
        {
            // Log action start
            string controller = nameof(TemplateConfigurationController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {     
                // Fetch all templates from API
                var result = await _apiClient.GetAllTemplatesAsync();
              
                if (result == null || !result.Any())
                {
                    result = new List<TemplateResponseModel> { new TemplateResponseModel() };
                }


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result?.Count() ?? 0
                );

                // Fetch all shifts for dropdown
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
                        Text = s.Shift_name,   
                        Value = s.Shift_id.ToString() 
                    })
                    .ToList();

                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result, jsonOptions);
                ViewData["TemplateConfiguration"] = jsonResult;

            
                _logger.LogInformation(
                   "[ACTION SUCCESS] {controller}.{action} | Message={msg}",
                   controller, action, "Page loaded successfully"
                );

                return View();
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                return StatusCode(500, ex.Message);
            }
        }

        // -----------------------------------------------------
        // CREATE / Insert new template
        // -----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TemplateModel model)
        {
            // Log action start
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

                // Insert new template via API
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
            catch (Exception ex)
            {
                // Log error
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                if (ex is ApiException<ResponseModel> apiEx && apiEx.Result != null)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "Error",
                        message = apiEx.Result.Detail
                    });
                }

                return BadRequest(new
                {
                    status = "error",
                    title = "Error",
                    message = ex.Message
                });
            }
        }

        // -----------------------------------------------------
        // UPDATE template
        // -----------------------------------------------------
        [HttpPut]
            public async Task<IActionResult> Update([FromBody] TemplateUpdateModel model)
            {
                // Log action start
                string controller = nameof(TemplateConfigurationController);
                string action = nameof(Index);
                string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

                _logger.LogInformation(
                    "[ACTION START] {controller}.{action} | User={user}",
                    controller, action, user
                );

            try
            {
                

                // Map MVC model → API model
                var apiModel = new TemplateUpdateModel
                {
                    Template_id = model.Template_id,
                    Template_name = model.Template_name,
                    Template_description = model.Template_description,
                    Assigned_shift1 = model.Assigned_shift1,
                    Assigned_shift2 = model.Assigned_shift2,
                    Assigned_shift3 = model.Assigned_shift3,
                    Assigned_shift4 = model.Assigned_shift4,
                    Assigned_shift5 = model.Assigned_shift5,
                    Effective_from = model.Effective_from,
                    Is_deleted = model.Is_deleted
                };
                apiModel.Updated_by = HttpContext.Session.GetString("LoginUser");
                apiModel.Updated_at = DateTimeOffset.Now;

                // Update template via API
                var result = await _apiClient.UpdateTemplateAsync(model.Template_id, apiModel);

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
            catch (Exception ex)
            {
                // Log error

                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );

                if (ex is ApiException<ResponseModel> apiEx && apiEx.Result != null)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "Error",
                        message = apiEx.Result.Detail
                    });
                }

                return BadRequest(new
                {
                    status = "error",
                    title = "Error",
                    message = ex.Message
                });
            }
        }

        // NO DELETE METHOD AS ACTIVE/INACTIVE UPDATE IS USED

        // -----------------------------------------------------
        // UPLOAD TEMPLATE DATA VIA EXCEL
        // -----------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            //Log action start
            string controller = nameof(TemplateConfigurationController);
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

                // -------------------------------------------
                // ❗ STOP if headers exist but NO data rows
                // -------------------------------------------
                if (excelRows.Count == 0)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        title = "No Data",
                        message = "Template contains no data rows."
                    });
                }   


                string currentUser = HttpContext.Session.GetString("LoginUser") ?? "System";


                var allItems = new List<TemplateModel>();

                // Map Excel rows to TemplateResponseModel
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
                            case "shiftname1":
                                 mapped["assigned_shift1_name"] = string.IsNullOrWhiteSpace(kv.Value) ? null : kv.Value;
                                break;

                            case "shiftname2":
                                mapped["assigned_shift2_name"] = string.IsNullOrWhiteSpace(kv.Value) ? null : kv.Value;
                                break;

                            case "shiftname3":
                                mapped["assigned_shift3_name"] = string.IsNullOrWhiteSpace(kv.Value) ? null : kv.Value;
                                break;

                            case "shiftname4":
                                mapped["assigned_shift4_name"] = string.IsNullOrWhiteSpace(kv.Value) ? null : kv.Value;
                                break;

                            case "shiftname5":
                                mapped["assigned_shift5_name"] = string.IsNullOrWhiteSpace(kv.Value) ? null : kv.Value;
                                break;

                            case "effectivefrom": mapped["effective_from"] = kv.Value; break;
                        }
                    }

                    var model = new TemplateModel
                    {
                        Template_name = mapped.GetValueOrDefault("template_name"),
                        Template_description = mapped.GetValueOrDefault("template_description"),
                        Assignedd_shift1_name = mapped.GetValueOrDefault("assigned_shift1_name"),
                        Assignedd_shift2_name = mapped.GetValueOrDefault("assigned_shift2_name"),
                        Assignedd_shift3_name = mapped.GetValueOrDefault("assigned_shift3_name"),
                        Assignedd_shift4_name = mapped.GetValueOrDefault("assigned_shift4_name"),
                        Assignedd_shift5_name = mapped.GetValueOrDefault("assigned_shift5_name"),
                        Created_by=currentUser,
                        Is_deleted = false,
                        Updated_by = currentUser
                    };

                    if (DateTime.TryParse(mapped.GetValueOrDefault("effective_from"), out var dt))
                        model.Effective_from = dt;

                    allItems.Add(model);
                }

                // Upload each template via API
                int successCount = 0;
                var apiErrors = new List<object>();

                foreach (var item in allItems)
                {
                    try
                    {
                        await _apiClient.UploadTemplateAsync(item);
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
