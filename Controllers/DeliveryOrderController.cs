// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : DeliveryOrderController.cs
// Module      : Masters
// Author      : Srinithi G
// Created On  : 2025-10-14
// Description : Controller for managing Reason master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Srinithi G       | Initial creation based on Delivery Order Model structure.
// =================================================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;

namespace YardManagementApplication
{

    [Route("[controller]/[action]")]
    public class DeliveryOrderController : Controller
    {
        private readonly v1Client _apiClient;

        public DeliveryOrderController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Delivery Order";
                // Call the generated client method - no URL needed
                var result = await _apiClient.GetAllDeliveryOrdersAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<DeliveryOrderModel>
                       {
                           new DeliveryOrderModel()
                       };
                }
                // Convert the result object to a JSON string
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                // Pass JSON string to the view using ViewData

                ViewData["DeliveryOrderData"] = jsonResult;
                //  Fetch dropdown sources from API
                var Color = await _apiClient.ColorAsync(); // <-- This maps to /api/v1/Dropdown/
                var brand = await _apiClient.VehicleBrandAsync();
                var Customername = await _apiClient.DealerAsync();
               
                var location = await _apiClient.LocationAsync();
                ViewBag.ColorList = Utils.Utility.PrepareSelectList(Color);
                ViewBag.brandList = Utils.Utility.PrepareSelectList(brand);
                ViewBag.locationList = Utils.Utility.PrepareSelectList(location);
                ViewBag.CustomerList = Utils.Utility.PrepareSelectList(Customername);
               

                return View();
            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        public async Task<IActionResult> Create([FromBody] DeliveryOrderModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");

                if (model.Details != null && model.Details.Count > 0)
                {
                    foreach (var screen in model.Details)
                    {
                        screen.Do_number = model.Do_number; //  copy group name to child
                        screen.Created_by = model.Created_by;      //  audit copy
                    }
                }
                
                var result = await _apiClient.InsertDeliveryOrderAsync(model);

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });


            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        public async Task<IActionResult> Update([FromBody] DeliveryOrderUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                if (model.Details != null && model.Details.Count > 0)
                {
                    foreach (var screen in model.Details)
                    {
                        screen.Do_id = model.Do_id;
                        screen.Do_number = model.Do_number; //  copy group name to child
                        screen.Updated_by = model.Updated_by;      //  audit copy
                    }
                }


                //  Call API to Update (by id)
                var result = await _apiClient.UpdateDeliveryOrderAsync(model.Do_number, model);

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });

            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        public async Task<IActionResult> Delete([FromBody] DeliveryOrderDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                //  Call API delete endpoint (by id)
                var result = await _apiClient.DeleteDeliveryOrderAsync(model.Do_number);

                // Return JSON for common JS toast
                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });

            }
            catch (ApiException<ProblemDetails> ex)
            {
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


        public IActionResult DownloadDeliveryOrderTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("DeliveryOrderTemplate");

                // Set headers based on your model properties
                string[] headers = new string[]
                {

                        "Brand_code", "Brand_name", "Manufacturer_name", "Country_name",
                        "Description", "Status_name","Created_by","Created_at","Updated_by","Updated_at"
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
            string excelName = $"DeliveryOrderTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }


        [HttpGet]
        public async Task<IActionResult> GetVehicleTypesByBrand(long brandId)
        {
            try
            {
                //return View(Index);
                var data = await _apiClient.VehicleTypeAsync(brandId); // update API if needed
                return Ok(Utils.Utility.PrepareSelectList(data));
            }
            catch (ApiException<ProblemDetails> ex)
            {
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

        [HttpGet]
        public async Task<IActionResult> GetVariantsByVehicleType(long vehicleTypeId)
        {
            try
            {
                //return View(Index);
                var data = await _apiClient.VehicleVariantAsync(vehicleTypeId); // Only pass vehicleTypeId
                return Ok(Utils.Utility.PrepareSelectList(data));
            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                // Optional: check extension/MIME
                var nameOk = Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);
                var type = (file.ContentType ?? "").ToLowerInvariant();
                var typeOk = type.Contains("csv") || type == "application/vnd.ms-excel";
                if (!nameOk && !typeOk)
                    return BadRequest("Only CSV files are allowed.");

                // Process the CSV file
               // var res = _csvUploadService.ProcessCsvFile(file, _validator);

                // If there are valid records, proceed to insert them or handle them as needed
                //if (res.ValidItems.Any())
                //{
                //    foreach (var validItem in res.ValidItems)
                //    {
                //        var result = await _apiClient.InsertVehicleModelAsync(validItem); // Insert valid records
                //    }

                //    // Generate CSV for invalid records
                //    //string invalidRecordsCsv = null;
                //    //if (res.InvalidItems.Any())
                //    //{
                //    //    // Generate the CSV file for invalid records
                //    //    invalidRecordsCsv = _csvUploadService.CreateInvalidCsvWithErrors(res.InvalidItems);
                //    //}

                //    // Return success response with download link for invalid records
                //    return Ok(new
                //    {
                //        status = "success",
                //        title = "Success",
                //        //message = $"{res.ValidCount} records added successfully",
                //        //invalidRecords = invalidRecordsCsv != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidRecordsCsv)) : null // Include CSV for invalid records
                //    });
                //}

                // If no valid items, return success without sending data
                return Ok(new
                {
                    status = "success",
                    title = "No Records",
                    message = "No valid records to insert."
                });

            }
            catch (ApiException<ProblemDetails> ex)
            {
                // This catches structured API errors (with JSON body)
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }


    }
    
}

