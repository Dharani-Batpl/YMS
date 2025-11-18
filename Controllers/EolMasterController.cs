// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : EolMasterController.cs
// Module      : Masters
// Author      : Dhanalakshmi D
// Created On  : 2025-10-14
// Description : Controller for managing EOL master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Dhanalakshmi D   | Initial creation based on EOL model structure.
// =================================================================================================
// 1.1     | 2025-11-13 | Dharani T   | Modified the Upload method to handle Excel file uploads for bulk eol creation.
// =================================================================================================

using FluentValidation;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YardManagementApplication
{

    [Route("[controller]/[action]")]
    public class EolMasterController : Controller
    {
        // -----------------------------------------------------
        // Dependencies
        // -----------------------------------------------------
        private readonly v1Client _apiClient;

        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<EolProductionModel> _validator;

        private readonly ILogger<EolMasterController> _logger;

        public EolMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<EolProductionModel> validator, ILogger<EolMasterController> logger)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

            _logger = logger;

        }

        // =====================================================
        //  Render main page 
        // =====================================================
        public async Task<IActionResult> Index()
        {

            // Log action start
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";
          
            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {                   
                // Fetch EOL Production Data
                var result = await _apiClient.GetAllEolProductionAsync();

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result?.Count() ?? 0
                );
                
                if (result == null || !result.Any())
                {
                    result = new List<EolProductionModel>
                    {
                        new EolProductionModel()
                    };
                }
                
                var jsonOptions = AppJson.CreateUiOptions();

                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result, jsonOptions);

                // Pass data to ViewData
                ViewData["EolMasterData"] = jsonResult;

                var brand = await _apiClient.VehicleBrandAsync();
                var color = await _apiClient.ColorAsync();
                var QualityStatus = await _apiClient.VehicleQualityStatusAsync();

                // Prepare Select Lists
                ViewBag.BrandList = Utils.Utility.PrepareSelectList(brand);
                ViewBag.ColorList = Utils.Utility.PrepareSelectList(color);
                ViewBag.QualityStatusList = Utils.Utility.PrepareSelectList(QualityStatus);

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
        // Get Vehicle Type based on Brand ID 
        // -----------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetVehicleType(long brandId)
        {
            // Log action start
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";


            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                // Fetch Vehicle Types
                var vehicleTypes = await _apiClient.VehicleTypeAsync(brandId);

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, vehicleTypes?.Count() ?? 0
                );
               
                if (vehicleTypes != null)
                {
                    return Ok(new
                    {
                        vehicletypeList = vehicleTypes.Select(vt => new
                        {
                            vt.Id,       
                            vt.Name     
                        }).ToList()
                    });
                }

              
                return Ok(new { vehicletypeList = new List<object>() });
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
        // Get Variant based on Vehicle Type ID
        // -----------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetVariant(long vehicleTypeId)
        {
            // Log action start
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                // Fetch Variants
                var vehicleTypes = await _apiClient.VehicleVariantAsync(vehicleTypeId);

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, vehicleTypes?.Count() ?? 0
                );

                
                if (vehicleTypes != null)
                {
                    return Ok(new
                    {
                        variantList = vehicleTypes.Select(vt => new
                        {
                            vt.Id,        
                            vt.Name     
                        }).ToList()
                    });
                }

              
                return Ok(new { variantList = new List<object>() });
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
                        message = apiEx.Result.Detail // <- Pass Detail here
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
        // CREATE / Insert new EOL data 
        // -----------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EolProductionModel model)
        {
            // Log action start
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


            try
            {

                model.Created_by = HttpContext.Session.GetString("LoginUser");           
              
                string formattedCompletionDate = model.Completion_at.Value.ToString("yyyy/MM/dd");
                DateTime completionDate = DateTime.ParseExact(formattedCompletionDate, "yyyy/MM/dd", null);
                DateTime productionDate = DateTime.Today;

                model.Completion_at = new DateTimeOffset(completionDate);
                model.Date_of_production = productionDate;

                // Insert EOL Production Data
                var result = await _apiClient.InsertEolProductionAsync(model);

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | {Status}",
                    controller, action, result.Status
                );

                
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
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
        // UPDATE EOL data 
        // -----------------------------------------------------
        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] EolProductionUpdateModel model)
        {
            // Log action start
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                // Update EOL Production Data

                var result = await _apiClient.UpdateEolProductionAsync(model.Vin,model);
                
                _logger.LogInformation(
                 "[ACTION INFO] {controller}.{action} | {Status}",
                 controller, action, result.Status
             );
             
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
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
        // DELETE EOL [Soft Delete]
        // -----------------------------------------------------
        [HttpPut()]
        public async Task<IActionResult> Delete([FromBody] EolMasterDeleteModel model)
        {

            // Log action start
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                model.Updated_at=DateTimeOffset.Now;

                // Delete EOL Production Data
                var result = await _apiClient.DeleteEolProductionAsync(model.Vin);
                _logger.LogInformation(
                   "[ACTION INFO] {controller}.{action} | {Status}",
                   controller, action, result.Status
                );

              
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
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
        // UPLOAD EOL DATA VIA EXCEL
        // -----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Log action start
            string controller = nameof(EolMasterController);
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
                    return BadRequest("Only Excel files (.xlsx/.xls) are allowed.");

                List<Dictionary<string, string>> excelRows = new();
                List<string> uploadedHeaders = new();

                // Read Excel file
                using (var stream = file.OpenReadStream())
                {
                    IWorkbook workbook = ext == ".xlsx"
                        ? new XSSFWorkbook(stream)
                        : new HSSFWorkbook(stream);

                    var sheet = workbook.GetSheetAt(0);
                    if (sheet == null)
                        return BadRequest("Excel has no sheets.");

                    var headerRow = sheet.GetRow(0);
                    if (headerRow == null)
                        return BadRequest("Excel header row missing.");

                    for (int i = 0; i < headerRow.LastCellNum; i++)
                        uploadedHeaders.Add(headerRow.GetCell(i)?.ToString()?.Trim() ?? $"Column{i}");

                    for (int r = 1; r <= sheet.LastRowNum; r++)
                    {
                        var row = sheet.GetRow(r);
                        if (row == null) continue;

                        var dict = new Dictionary<string, string>();
                        for (int c = 0; c < uploadedHeaders.Count; c++)
                            dict[uploadedHeaders[c]] = row.GetCell(c)?.ToString()?.Trim() ?? "";

                        excelRows.Add(dict);
                    }
                }

                
                string currentUser = HttpContext.Session.GetString("LoginUser") ?? "System";

                 var allItems = new List<EolProductionModel>();

                // Map Excel rows to EolProductionModel
                foreach (var rowDict in excelRows)
                {
                    var mapped = new Dictionary<string, string>();

                  
                    foreach (var kv in rowDict)
                    {
                        string norm = kv.Key.Trim().ToLower()
                            .Replace(" ", "")
                            .Replace("_", "")
                            .Replace("-", "");

                        switch (norm)
                        {
                            case "vin": mapped["vin"] = kv.Value; break;
                            case "productionorderid": mapped["production_order_id"] = kv.Value; break;
                            case "productdescription": mapped["product_description"] = kv.Value; break;

                            case "shiftname": mapped["shift_name"] = kv.Value; break;
                            case "dateofproduction": mapped["date_of_production"] = kv.Value; break;

                            case "eolqualityinspectorid": mapped["eol_quality_inspector_id"] = kv.Value; break;
                            case "eolqualityinspectorname": mapped["eol_quality_inspector_name"] = kv.Value; break;

                            case "colorname": mapped["color_name"] = kv.Value; break;
                            case "brandname": mapped["brand_name"] = kv.Value; break;
                            case "vehicletypename": mapped["vehicle_type_name"] = kv.Value; break;
                            case "variantname": mapped["variant_name"] = kv.Value; break;

                            case "batchlotnumber": mapped["batch_lot_number"] = kv.Value; break;
                            case "lineid": mapped["line_id"] = kv.Value; break;
                            case "shopid": mapped["shop_id"] = kv.Value; break;

                            case "completionat": mapped["completion_at"] = kv.Value; break;
                            case "qualitystatusname": mapped["quality_status_name"] = kv.Value; break;
                            case "certificateid": mapped["certificate_id"] = kv.Value; break;

                            default: break;
                        }
                    }

                    var model = new EolProductionModel();

                   
                    foreach (var prop in typeof(EolProductionModel).GetProperties())
                    {
                        var jsonAttr = prop.GetCustomAttribute<JsonPropertyAttribute>();
                        if (jsonAttr == null) continue;

                        string jsonName = jsonAttr.PropertyName;

                        if (jsonName == "created_by")
                        {
                            prop.SetValue(model, currentUser);
                            continue;
                        }

                        if (mapped.ContainsKey(jsonName))
                        {
                            try
                            {
                                object value;

                                if (prop.PropertyType == typeof(DateTimeOffset?) ||
                                    prop.PropertyType == typeof(DateTimeOffset))
                                {
                                    if (DateTimeOffset.TryParse(mapped[jsonName], out var dt))
                                        value = dt;
                                    else
                                        continue;
                                }
                                else
                                {
                                    value = Convert.ChangeType(mapped[jsonName], prop.PropertyType);
                                }

                                prop.SetValue(model, value);
                            }
                            catch { }
                        }
                    }

                    allItems.Add(model);
                }

                // Upload each item and collect results
                int successCount = 0;
                var apiErrors = new List<object>();

                foreach (var item in allItems)
                {
                    try
                    {
                        await _apiClient.UploadEolProductionAsync(item);
                        successCount++;
                        _logger.LogInformation(
                      "[ACTION INFO] {controller}.{action} | successCount: {successCount}",
                      controller, action, successCount
                  );
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
                            vin = item.Vin,
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
                            vin = item.Vin,
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
           