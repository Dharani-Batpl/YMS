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
        // Inject typed API client
        private readonly v1Client _apiClient;

        //  Constructor DI
        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<EolProductionModel> _validator;

        private readonly ILogger<EolMasterController> _logger;


        // Single constructor to inject both dependencies

        public EolMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<EolProductionModel> validator, ILogger<EolMasterController> logger)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

            _logger = logger;

        }

        // =====================================================
        //  Render main page with grid data + dropdowns
        // =====================================================
        public async Task<IActionResult> Index()
        {
           

            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";
          
            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
               
                //  Fetch users list from API
                var result = await _apiClient.GetAllEolProductionAsync();

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result?.Count() ?? 0
                );
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<EolProductionModel>
                    {
                        new EolProductionModel()
                    };
                }
                //  Serialize data for the view (Tabulator expects JSON)
                var jsonOptions = AppJson.CreateUiOptions();

                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result, jsonOptions);


                //  Pass JSON to view
                ViewData["EolMasterData"] = jsonResult;

                //  Fetch dropdown sources from API
                var brand = await _apiClient.VehicleBrandAsync();
                var color = await _apiClient.ColorAsync();
                var QualityStatus = await _apiClient.VehicleQualityStatusAsync();

                // Convert API lists to SelectList for UI controls
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
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleType(long brandId)
        {
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";


            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                // Assuming _apiClient.VehicleTypeAsync fetches the vehicle types based on brandId
                var vehicleTypes = await _apiClient.VehicleTypeAsync(brandId);


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, vehicleTypes?.Count() ?? 0
                );
                // If vehicle types are found, return the data in the required format
                if (vehicleTypes != null)
                {
                    return Ok(new
                    {
                        vehicletypeList = vehicleTypes.Select(vt => new
                        {
                            vt.Id,        // Assuming the vehicle type has an Id
                            vt.Name       // Assuming the vehicle type has a Name
                        }).ToList()
                    });
                }

                // If no vehicle types are found, return an empty list
                return Ok(new { vehicletypeList = new List<object>() });
            }
            catch (Exception ex)
            {

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

        [HttpGet]
        public async Task<IActionResult> GetVariant(long vehicleTypeId)
        {
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                // Assuming _apiClient.VehicleTypeAsync fetches the vehicle types based on brandId
                var vehicleTypes = await _apiClient.VehicleVariantAsync(vehicleTypeId);

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, vehicleTypes?.Count() ?? 0
                );


                // If vehicle types are found, return the data in the required format
                if (vehicleTypes != null)
                {
                    return Ok(new
                    {
                        variantList = vehicleTypes.Select(vt => new
                        {
                            vt.Id,        // Assuming the vehicle type has an Id
                            vt.Name       // Assuming the vehicle type has a Name
                        }).ToList()
                    });
                }

                // If no vehicle types are found, return an empty list
                return Ok(new { variantList = new List<object>() });
            }
            catch (Exception ex)
            {
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

        // =====================================================
        //  Create EOL data (HTTP POST)
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EolProductionModel model)
        {
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
              
                //DateTime completionDate = DateTime.ParseExact(model.Completion_at.ToString("yyyy/MM/dd"), "yyyy/MM/dd", null);
                string formattedCompletionDate = model.Completion_at.Value.ToString("yyyy/MM/dd");
                DateTime completionDate = DateTime.ParseExact(formattedCompletionDate, "yyyy/MM/dd", null);
                DateTime productionDate = DateTime.Today;

                model.Completion_at = new DateTimeOffset(completionDate);
                model.Date_of_production = productionDate;

                //Call API for EOL insert
                var result = await _apiClient.InsertEolProductionAsync(model);

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | {Status}",
                    controller, action, result.Status
                );

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (Exception ex)
            {
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
        // =====================================================
        //  update EOL data (HTTP PUT)
        // =====================================================
        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] EolProductionUpdateModel model)
        {
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

                //Call API for EOL insert
                var result = await _apiClient.UpdateEolProductionAsync(model.Vin,model);
                _logger.LogInformation(
                 "[ACTION INFO] {controller}.{action} | {Status}",
                 controller, action, result.Status
             );
                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (Exception ex)
            {

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

        // =====================================================
        //  delete EOL data (HTTP PUT)
        // =====================================================
        [HttpPut()]
        public async Task<IActionResult> Delete([FromBody] EolMasterDeleteModel model)
        {
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
                //Call API for EOL insert
                var result = await _apiClient.DeleteEolProductionAsync(model.Vin);
                _logger.LogInformation(
                   "[ACTION INFO] {controller}.{action} | {Status}",
                   controller, action, result.Status
                );

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (Exception ex)
            {

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadOld(IFormFile file)
        {
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                // 0) Guards
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");
                var nameOk = Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);
                var type = (file.ContentType ?? "").ToLowerInvariant();
                var typeOk = type.Contains("csv") || type == "application/vnd.ms-excel";
                if (!nameOk && !typeOk)
                    return BadRequest("Only CSV files are allowed.");
                // 1) Parse + validate CSV
                var res = _csvUploadService.ProcessCsvFile<EolProductionModel>(file, _validator);
                // 2) Insert valid rows; collect API failures (no row numbers)
                var apiFailures = new List<(EolProductionModel Record, string Error)>();
                var successCount = 0;
                foreach (var item in res.ValidItems)
                {
                    try
                    {
                        await _apiClient.UploadEolProductionAsync(item);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        var msg = ex is ApiException<ResponseModel> apiEx && apiEx.Result != null
                            ? (apiEx.Result.Detail ?? apiEx.Message)
                            : ex.Message;
                        apiFailures.Add((item, msg ?? "API insert error"));
                    }
                }
                // 3) Build ONE CSV for all failures (validation + API),
                //    but include ONLY the columns that were uploaded (plus ErrorMessages)
                string invalidCsv = _csvUploadService.CreateUnifiedInvalidCsv(
                    res.InvalidItems,
                    apiFailures,
                    res.UploadedHeaders // << key: only uploaded columns are exported
                );
                // Encode to Base64 only if we have any failures
                string invalidBase64 = string.IsNullOrWhiteSpace(invalidCsv)
                    ? null
                    : Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidCsv));
                // 4) Prepare response
                var status = successCount > 0 ? "success" : "error";
                var title = successCount > 0 ? "Success" : "No Records";
                var message = successCount > 0
                    ? $"{successCount} records added successfully"
                    : "No valid records to insert.";

                _logger.LogInformation(
                   "[ACTION INFO] {controller}.{action} | Success count: {Status}",
                   controller, action, status
               );
                return Ok(new
                {
                    status,
                    title,
                    message,
                    successCount,
                    validationFailedCount = res.InvalidCount,
                    apiFailedCount = apiFailures.Count,
                    invalidRecords = invalidBase64 // unified CSV (ErrorMessages + only uploaded columns)
                });
            }
            catch (OperationCanceledException)
            {
                

                _logger.LogError(
                     "error",
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, "The upload was cancelled."
                 );
                return StatusCode(499, new
                {
                    status = "error",
                    title = "Client Closed Request",
                    message = "The upload was cancelled."
                });
            }
            

            catch (Exception ex)
            {


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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            string controller = nameof(EolMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


            try
            {
                // -----------------------------------------------------------------------
                // 0) Validate file
                // -----------------------------------------------------------------------
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext != ".xlsx" && ext != ".xls")
                    return BadRequest("Only Excel files (.xlsx/.xls) are allowed.");

                // -----------------------------------------------------------------------
                // 1) Parse Excel
                // -----------------------------------------------------------------------
                List<Dictionary<string, string>> excelRows = new();
                List<string> uploadedHeaders = new();

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

                // Current logged-in user
                string currentUser = HttpContext.Session.GetString("LoginUser") ?? "System";

                // -----------------------------------------------------------------------
                // 2) Map Excel → API model using JSON property names
                // -----------------------------------------------------------------------
                var allItems = new List<EolProductionModel>();

                foreach (var rowDict in excelRows)
                {
                    var mapped = new Dictionary<string, string>();

                    // Normalize & map columns
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

                    // Assign values using JsonProperty names
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

                // -----------------------------------------------------------------------
                // 3) CALL API FOR EACH ROW + CAPTURE EXACT ERRORS
                // -----------------------------------------------------------------------
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

                // -----------------------------------------------------------------------
                // 4) RETURN PROPER HTTP STATUS
                // -----------------------------------------------------------------------
                if (apiErrors.Count > 0)
                {
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
           