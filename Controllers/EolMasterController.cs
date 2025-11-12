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
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.Collections;
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


        // Single constructor to inject both dependencies

        public EolMasterController(CsvUploadService csvUploadService, v1Client apiClient, IValidator<EolProductionModel> validator)

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
                var result = await _apiClient.GetAllEolProductionAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<EolProductionModel>
                    {
                        new EolProductionModel()
                    };
                }
                //  Serialize data for the view (Tabulator expects JSON)
                //string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);
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

                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleType(long brandId)
        {
            try
            {
                // Assuming _apiClient.VehicleTypeAsync fetches the vehicle types based on brandId
                var vehicleTypes = await _apiClient.VehicleTypeAsync(brandId);

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
            try
            {
                // Assuming _apiClient.VehicleTypeAsync fetches the vehicle types based on brandId
                var vehicleTypes = await _apiClient.VehicleVariantAsync(vehicleTypeId);

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
        //  update EOL data (HTTP PUT)
        // =====================================================
        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] EolProductionUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                //Call API for EOL insert
                var result = await _apiClient.UpdateEolProductionAsync(model.Vin,model);
               
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
        //  delete EOL data (HTTP PUT)
        // =====================================================
        [HttpPut()]
        public async Task<IActionResult> Delete([FromBody] EolMasterDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");
                model.Updated_at=DateTimeOffset.Now;
                //Call API for EOL insert
                var result = await _apiClient.DeleteEolProductionAsync(model.Vin);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadOld(IFormFile file)
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
            //catch (ApiException<ProblemDetails> ex)
            //{
            //    var problem = ex.Result;
            //    return StatusCode(problem.Status ?? ex.StatusCode, new
            //    {
            //        status = problem.Status ?? ex.StatusCode,
            //        title = problem.Title ?? "Error",
            //        message = problem.Detail ?? "An unexpected error occurred."
            //    });
            //}

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

    }
}

 //test               