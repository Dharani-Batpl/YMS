// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : DepartmentMasterController.cs
// Module      : Masters
// Author      : Sujitha B / Dharani T
// Created On  : 2025-10-18
// Description : Controller for managing Reason master data.
// Modified On  : 2025-11-11
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-18 | Sujitha B      | Initial creation based on Department model structure.
// =================================================================================================
// 1.1     | 2025-11-13 | Dharani T   | Modified the Upload method to handle Excel file uploads for bulk shift creation.
// =================================================================================================


using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Net;
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
        // -----------------------------------------------------
        // Dependencies
        // -----------------------------------------------------
        private readonly v1Client _apiClient;

        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<ShiftModel> _validator;

        private readonly ILogger<ShiftMasterController> _logger;       

        public ShiftMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<ShiftModel> validator, ILogger<ShiftMasterController> logger)

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
            string controller = nameof(ShiftMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {

                // Fetch all Shift records
                var result = await _apiClient.GetAllShiftAsync();
              
                if (result == null || !result.Any())
                {
                    result = new List<ShiftMasterModel>
                       {
                           new ShiftMasterModel()
                       };
                }

                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | RecordsFetched={count}",
                    controller, action, result?.Count() ?? 0
                );

               
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);
               
                ViewData["ShiftMasterData"] = jsonResult;

                // Fetch Shift Statuses
                var statuses = await _apiClient.ShiftStatusAsync(); 
                
                ViewBag.StatusList = Utils.Utility.PrepareSelectList(statuses);
               
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
        // CREATE / Insert new Shift
        // -----------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShiftMasterModel model)
        {
            // Log action start
            string controller = nameof(ShiftMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


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

                // Insert new Shift record

                var result = await _apiClient.InsertShiftAsync(model);


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | Result {result}",
                    controller, action, result
                );

                return Ok(new
                {
                    status = result.Status,
                    title =  "Success",
                    message = result.Detail ?? "Record created successfully"
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
        // UPDATE Shift
        // -----------------------------------------------------

        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] ShiftMasterModel model)
        {
            // Log action start
            string controller = nameof(ShiftMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                // Update Shift record
                var result = await _apiClient.UpdateShiftAsync(model.Shift_id, model);


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | resul:{result}",
                    controller, action, result
                );


                return Ok(new
                {
                    status = result.Status,
                    title =  "Success",
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
        // DELETE Shift [Soft Delete]
        // -----------------------------------------------------
        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] ShiftModel model)
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
                model.Updated_at = DateTimeOffset.Now;

                // Delete Shift record
                var result = await _apiClient.DeleteShiftAsync(model.Shift_id);


                _logger.LogInformation(
                    "[ACTION INFO] {controller}.{action} | result={result}",
                    controller, action, result
                );


                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record deleted successfully"
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
        // UPLOAD SHIFT DATA VIA EXCEL
        // -----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Log action start
            string controller = nameof(ShiftMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation("[ACTION START] {controller}.{action} | User={user}",
                controller, action, user);

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
                        {
                            var cell = row.GetCell(c);

                            if (cell == null)
                            {
                                dict[headers[c]] = "";
                                continue;
                            }

                            
                            if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                            {
                                dict[headers[c]] = cell.DateCellValue.Value.ToString("HH:mm:ss");
                            }
                            else
                            {
                                dict[headers[c]] = cell.ToString()?.Trim() ?? "";
                            }
                        }

                        excelRows.Add(dict);
                    }
                }


                //TimeSpan ParseTime(string t)
                //{
                //    if (string.IsNullOrWhiteSpace(t)) return TimeSpan.Zero;
                //    if (TimeSpan.TryParse(t, out var ts)) return ts;
                //    return TimeSpan.Zero;
                //}
                TimeSpan ParseTime(string t)
                {
                    if (string.IsNullOrWhiteSpace(t))
                        return TimeSpan.Zero;

                    if (TimeSpan.TryParse(t, out var ts))
                        return ts;

                    return TimeSpan.Zero;
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
                var allItems = new List<ShiftMasterModel>();

                // Map Excel rows to ShiftMasterModel
                foreach (var row in excelRows)
                {
                    string norm(string x) =>
                        x.Trim().ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");

                    string Get(string key)
                    {
                        var kv = row.FirstOrDefault(x => norm(x.Key) == key);
                        return kv.Value ?? "";
                    }

                    string shiftName = Get("shiftname");
                    string shiftDesc = Get("shiftdescription");
                    string startTime = Get("starttime");
                    string endTime = Get("endtime");

                    
                    string b1in = Get("break1in");
                    string b1out = Get("break1out");
                    string b1desc = Get("break1description");

                    string b2in = Get("break2in");
                    string b2out = Get("break2out");
                    string b2desc = Get("break2description");

                    string b3in = Get("break3in");
                    string b3out = Get("break3out");
                    string b3desc = Get("break3description");

                    string b4in = Get("break4in");
                    string b4out = Get("break4out");
                    string b4desc = Get("break4description");

                    string b5in = Get("break5in");
                    string b5out = Get("break5out");
                    string b5desc = Get("break5description");

                  
                    string FormatToHHmm(string t)
                    {
                        if (string.IsNullOrWhiteSpace(t)) return "";
                        if (TimeSpan.TryParse(t, out var ts))
                            return ts.ToString(@"hh\:mm");
                        if (DateTime.TryParse(t, out var dt))
                            return dt.ToString("HH:mm");
                        return t;
                    }

                    var breakArray = new JArray();

                    void AddBreak(int index, string bi, string bo, string bd)
                    {
                        if (!string.IsNullOrWhiteSpace(bi) ||
                            !string.IsNullOrWhiteSpace(bo) ||
                            !string.IsNullOrWhiteSpace(bd))
                        {
                            var obj = new JObject
                            {
                                [$"Break{index}In"] = FormatToHHmm(bi),
                                [$"Break{index}Out"] = FormatToHHmm(bo),
                                [$"Break{index}Description"] = bd ?? ""
                            };
                            breakArray.Add(obj);
                        }
                    }

                    AddBreak(1, b1in, b1out, b1desc);
                    AddBreak(2, b2in, b2out, b2desc);
                    AddBreak(3, b3in, b3out, b3desc);
                    AddBreak(4, b4in, b4out, b4desc);
                    AddBreak(5, b5in, b5out, b5desc);

                    string breakDetails = breakArray.Count > 0
                        ? JsonConvert.SerializeObject(breakArray)
                        : null;

                 
                    var model = new ShiftMasterModel
                    {
                        Shift_name = shiftName,
                        Shift_description = shiftDesc,
                        Start_time = (startTime),
                        End_time = (endTime),
                        Break_details = breakDetails,
                        Is_deleted = false,
                        Created_by = currentUser
                    };

                    allItems.Add(model);
                }

                // Upload each shift via API
                var apiErrors = new List<object>();
                int successCount = 0;

                foreach (var item in allItems)
                {
                    try
                    {
                        await _apiClient.UploadShiftAsync(item);
                        successCount++;
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
