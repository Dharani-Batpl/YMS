// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : UserConfigurationController.cs
// Module      : Masters
// Author      : Dhanalakshmi D
// Created On  : 2025-10-14
// Description : Controller for managing user configuration master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Dhanalakshmi D   | Initial creation based on user configuration model structure.
// =================================================================================================
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;

namespace YardManagementApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class UserConfigurationController : Controller
    {
        // Inject typed API client
        private readonly v1Client _apiClient;

        //  Constructor DI
        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<AppUserModel> _validator;


        // Single constructor to inject both dependencies

        public UserConfigurationController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<AppUserModel> validator)

        {

            _csvUploadService = csvUploadService;

            _apiClient = apiClient;

            _validator = validator;

        }

        // =====================================================
        //  Render main page with grid data + dropdowns
        // =====================================================
        public async Task<IActionResult> Index()
        {
            try
            {
                //  Fetch users list from API
                var result = await _apiClient.GetAllUsersAsync();

                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<AppUserModel>
                    {
                        new AppUserModel()
                    };
                }
                //  Serialize data for the view (Tabulator expects JSON)
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                //  Pass JSON to view
                ViewData["UserConfigurationData"] = jsonResult;

                //  Fetch dropdown sources from API
                var statuses = await _apiClient.UserStatusAsync();
                var groups = await _apiClient.UserGroupAsync();
                var depts = await _apiClient.DepartmentAsync();

                // Convert API lists to SelectList for UI controls
                ViewBag.StatusList = Utils.Utility.PrepareSelectList(statuses);
                ViewBag.UserGroupList = Utils.Utility.PrepareSelectList(groups);
                ViewBag.DepartmentList = Utils.Utility.PrepareSelectList(depts);

                //  Return view
                return View();
            }
            catch (Exception ex)
            {
                //  Bubble up error with HTTP 500
                return StatusCode(500, ex.Message);
            }
        }

        // =====================================================
        //  Create user (HTTP POST)
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppUserModel model)
        {
            try
            {          
                // Read username from session (null-safe)
                model.Created_by = HttpContext.Session.GetString("LoginUser");
                var result = await _apiClient.InsertUserAsync(model);

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record added successfully"
                });
            }
            catch (Exception ex)
            {
                if (ex is ApiException<ProblemDetails> apiEx && apiEx.Result != null)
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
        //  Update user (HTTP PUT)
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AppUserUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                var result = await _apiClient.UpdateUserAsync(model.User_id, model);

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
                if (ex is ApiException<ProblemDetails> apiEx && apiEx.Result != null)
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
        //  Delete user (HTTP PUT calling API delete by id)
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] AppUserUpdateModel model)
        {
            try
            {
                //  Set audit fields
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                //  Call API delete endpoint (by id)
                var result = await _apiClient.DeleteUserAsync(model.User_id);

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record deleted successfully"
                });
            }
            catch (Exception ex)
            {
                if (ex is ApiException<ProblemDetails> apiEx && apiEx.Result != null)
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
        //  handling file upload
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
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
                var res = _csvUploadService.ProcessCsvFile<AppUserModel>(file, _validator);

                // 2) Insert valid rows; collect API failures (no row numbers)
                var apiFailures = new List<(AppUserModel Record, string Error)>();
                var successCount = 0;

                foreach (var item in res.ValidItems)
                {
                    try
                    {
                        await _apiClient.UploadUserAsync(item);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        var msg = ex is ApiException<ProblemDetails> apiEx && apiEx.Result != null
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
                return StatusCode(499, new
                {
                    status = "error",
                    title = "Client Closed Request",
                    message = "The upload was cancelled."
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                var problem = ex.Result;
                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "error",
                    title = "Server Error",
                    message = ex.Message
                });
            }
        }
    }
}
