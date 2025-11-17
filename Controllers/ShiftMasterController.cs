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
        // Inject typed API client

        private readonly v1Client _apiClient;

        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<ShiftModel> _validator;

        private readonly ILogger<ShiftMasterController> _logger;
        // Single constructor to inject both dependencies

        public ShiftMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<ShiftModel> validator, ILogger<ShiftMasterController> logger)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            string controller = nameof(ShiftMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );

            try
            {
                ViewData["Title"] = "Shift Master";
                // Call the generated client method - no URL needed
                var result = await _apiClient.GetAllShiftAsync();
                // If no data returned, create a empty row to capture all properties from model
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

                // Convert the result object to a JSON string
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Pass JSON string to the view using ViewData

                ViewData["ShiftMasterData"] = jsonResult;
                //  Fetch dropdown sources from API
                var statuses = await _apiClient.ShiftStatusAsync(); // <-- This maps to /api/v1/Dropdown/
                //var Country = await _apiClient.CountryAsync();
                //ViewBag.CountryList = Utils.Utility.PrepareSelectList(Country);
                ViewBag.StatusList = Utils.Utility.PrepareSelectList(statuses);
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
        public async Task<IActionResult> Create([FromBody] ShiftMasterModel model)
        {
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
           
            catch (ApiException<ProblemDetails> ex)
            {
                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
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
        public async Task<IActionResult> Update([FromBody] ShiftMasterModel model)
        {
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
            catch (ApiException<ProblemDetails> ex)
            {

                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
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
        public async Task<IActionResult> Delete([FromBody] ShiftModel model)
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
            catch (ApiException<ResponseModel> ex)
            {

                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                var problem = ex.Result;
                int statusCode = problem.Status != 0 ? problem.Status : ex.StatusCode;

                return StatusCode(statusCode, new
                {
                    status = statusCode,
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "This shift is mapped to an active template and cannot be deleted."
                });
            }
            catch (ApiException ex)
            {

                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                // fallback for other unexpected API exceptions
                return StatusCode(ex.StatusCode, new
                {
                    status = ex.StatusCode,
                    title = "Error",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {

                _logger.LogError(
                     ex,
                     "[ACTION ERROR] {controller}.{action} | Exception={error}",
                     controller, action, ex.Message
                 );
                // fallback for generic .NET exceptions
                return StatusCode(500, new
                {
                    status = 500,
                    title = "Error",
                    message = ex.Message
                });
            }

        }

        public IActionResult DownloadShiftMasterTemplate()
        {
            string controller = nameof(ShiftMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation(
                "[ACTION START] {controller}.{action} | User={user}",
                controller, action, user
            );


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
            string controller = nameof(ShiftMasterController);
            string action = nameof(Index);
            string user = HttpContext.Session.GetString("LoginUser") ?? "Unknown";

            _logger.LogInformation("[ACTION START] {controller}.{action} | User={user}",
                controller, action, user);

            try
            {
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
                        {
                            var cell = row.GetCell(c);

                            if (cell == null)
                            {
                                dict[headers[c]] = "";
                                continue;
                            }

                            // Handle Excel datetime/time cells
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

                // ---------------- Helper: Parse Time ----------------
                TimeSpan ParseTime(string t)
                {
                    if (string.IsNullOrWhiteSpace(t)) return TimeSpan.Zero;
                    if (TimeSpan.TryParse(t, out var ts)) return ts;
                    return TimeSpan.Zero;
                }

                string currentUser = HttpContext.Session.GetString("LoginUser") ?? "System";
                var allItems = new List<ShiftMasterModel>();

                // ---------------- MAP EXCEL TO MODEL ----------------
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

                    // Breaks
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

                    // ------------- FIX: MULTIPLE BREAKS JSON ----------------
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

                    // ---------------- MODEL ----------------
                    var model = new ShiftMasterModel
                    {
                        Shift_name = shiftName,
                        Shift_description = shiftDesc,
                        Start_time = ParseTime(startTime),
                        End_time = ParseTime(endTime),
                        Break_details = breakDetails,
                        Is_deleted = false,
                        Created_by = currentUser
                    };

                    allItems.Add(model);
                }

                // ---------------- CALL API ----------------
                var errors = new List<object>();
                int success = 0;

                foreach (var item in allItems)
                {
                    try
                    {
                        await _apiClient.UploadShiftAsync(item);
                        success++;
                    }
                    catch (ApiException<ResponseModel> ex)
                    {
                        errors.Add(new { name = item.Shift_name, error = ex.Result?.Detail ?? ex.Message });
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new { name = item.Shift_name, error = ex.Message });
                    }
                }

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
                return BadRequest(new
                {
                    status = "error",
                    message = ex.Message
                });
            }
        }

    }

}
