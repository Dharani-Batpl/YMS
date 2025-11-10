// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : UserGroupConfigurationController.cs
// Module      : Masters
// Author      : Dhanalakshmi D
// Created On  : 2025-10-14
// Description : Controller for managing user group configuration data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Dhanalakshmi D   | Initial creation based on user group configuration model structure.
// =================================================================================================
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;

namespace YardManagementApplication
{
    [Route("[controller]/[action]")]
    public class UserGroupConfigurationController : Controller
    {
        //  Typed API client injected via DI
        private readonly v1Client _apiClient;

        //  Constructor DI
        private readonly CsvUploadService _csvUploadService;

        private readonly IValidator<UserGroupModel> _validator;


        // Single constructor to inject both dependencies

        public UserGroupConfigurationController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<UserGroupModel> validator)

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
                var result = await _apiClient.GetAllUserGroupAsync();

                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<UserGroupModel>
                    {
                        new UserGroupModel()
                    };
                }

                //  Serialize data for the view (Tabulator expects JSON)
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                //  Pass JSON to view
                ViewData["UserGroupConfigurationData"] = jsonResult;

                //  Fetch dropdown sources from API
                var modules = await _apiClient.ModuleAsync();
                var permission = await _apiClient.PermissionStatusAsync();

                // Convert API lists to SelectList for UI controls
                ViewBag.modules = Utils.Utility.PrepareSelectList(modules);
                ViewBag.permission = Utils.Utility.PrepareSelectList(permission);

                //  Return view
                return View();

            }
            catch (Exception ex)
            {
                //  Fail with HTTP 500 and message
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetScreensByModule(long moduleId)
        {
            try
            {
                // Assuming _apiClient.VehicleTypeAsync fetches the vehicle types based on brandId
                var screens = await _apiClient.ScreensAsync(moduleId);

                // If vehicle types are found, return the data in the required format
                if (screens != null)
                {
                    return Ok(new
                    {
                        screens = screens.Select(vt => new
                        {
                            vt.Id,        // Assuming the vehicle type has an Id
                            vt.Name       // Assuming the vehicle type has a Name
                        }).ToList()
                    });
                }

                // If no vehicle types are found, return an empty list
                return Ok(new { screens = new List<object>() });
            }
            catch (Exception ex)
            {
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
        // POST: Create User Group
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserGroupModel model)
        {
            try
            {
                //  Audit fields (created)
                model.Created_by = HttpContext.Session.GetString("LoginUser");

                //  Initialize child rows if present
                if (model.Screens != null && model.Screens.Count > 0)
                {
                    foreach (var screen in model.Screens)
                    {
                        screen.User_group_name = model.User_group_name; //  copy group name to child
                        screen.Created_by = model.Created_by;      //  audit copy
                    }
                }

                //  Call API to create group
                var result = await _apiClient.InsertUserGroupAsync(model);

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
        // PUT: Update User Group
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserGroupUpdateModel model)
        {
            try
            {
                //  Audit fields (updated)
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                //  Initialize child rows if present
                if (model.Screens != null && model.Screens.Count > 0)
                {
                    foreach (var screen in model.Screens)
                    {
                        screen.User_group_id = model.User_group_id;
                        screen.User_group_name = model.User_group_name; //  copy group name to child
                        screen.Updated_by = model.Updated_by;      //  audit copy
                    }
                }


                //  Call API to update by id
                var result = await _apiClient.UpdateUserGroupAsync(model.User_group_id, model);

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
        // PUT: Delete User Group
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] UserGroupConfigurationDeleteModel model)
        {
            try
            {
                //  Audit fields (updated—used for delete/soft delete context)
                model.updated_by = HttpContext.Session.GetString("LoginUser");

                //  API delete call by id
                var result = await _apiClient.DeleteUserGroupAsync(model.user_group_id);

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
        //  bulk Rework data upload
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
                var res = _csvUploadService.ProcessCsvFile<UserGroupModel>(file, _validator);

                // 2) Insert valid rows; collect API failures (no row numbers)
                var apiFailures = new List<(UserGroupModel Record, string Error)>();
                var successCount = 0;

                foreach (var item in res.ValidItems)
                {
                    try
                    {
                        await _apiClient.InsertUserGroupAsync(item);
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

                // 3) Build ONE CSV for all failures (validation + API), without RowNumber column
                string invalidCsv = _csvUploadService.CreateUnifiedInvalidCsv(res.InvalidItems, apiFailures);

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
                    invalidRecords = invalidBase64 // unified CSV (ErrorMessages + model fields)
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
