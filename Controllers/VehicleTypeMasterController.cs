// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : VehicleTypeMasterController.cs
// Module      : Masters
// Author      : Srinithi G
// Created On  : 2025-10-14
// Description : Controller for managing vehichle type master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author      | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Srinithi G  | Initial creation based on vehicle type master model structure.
// =================================================================================================
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;

namespace YardManagementApplication
{

    [Route("[controller]/[action]")]
    public class VehicleTypeMasterController : Controller
    {
        //  Strongly-typed API client dependency (generated v1 client)
        private readonly v1Client _apiClient;

        //  Inject API client via constructor
        public VehicleTypeMasterController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }

        // =====================================================
        //  GET /VehicleTypeMaster/Index - Load page & seed ViewData/ViewBag
        // =====================================================
        public async Task<IActionResult> Index()
        {
            try
            {
                //  Set page title for layout rendering
                ViewData["Title"] = "VehicleTypeMaster";

                //  Fetch all vehicle types from API client
                var result = await _apiClient.GetAllVehicleTypeAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<VehicleTypeModel>
                    {
                        new VehicleTypeModel()
                    };
                }
                //  Serialize data to JSON for use in JavaScript view
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);

                //  Pass serialized JSON into ViewData for view access
                ViewData["VehicleTypeMaster"] = jsonResult;

                //  Fetch dropdown lists from related API endpoints
                var categoryType = await _apiClient.CategoryTypeAsync();
                var status = await _apiClient.VehicleTypeStatusAsync();
                var fuelType = await _apiClient.FuelTypeAsync();
                var BrandName = await _apiClient.VehicleBrandAsync();

                //  Prepare select lists using utility methods
                ViewBag.BrandNameList = Utils.Utility.PrepareSelectList(BrandName);
                ViewBag.StatusList = Utility.PrepareSelectList(status);
                ViewBag.CategoryTypeList = Utility.PrepareSelectList(categoryType);
                ViewBag.FuelTypeList = Utility.PrepareSelectList(fuelType);

                //  Render view successfully
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



        // =====================================================
        //  POST /VehicleTypeMaster/Create - Create new Vehicle Type
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehicleTypeModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");

                //  Call API to insert or update record
                var result = await _apiClient.InsertVehicleTypeAsync(model);

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



        // =====================================================
        //  PUT /VehicleTypeMaster/Update - Update existing Vehicle Type
        // =====================================================
        [HttpPut()]
        public async Task<IActionResult> Update([FromBody] VehicleTypeUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                //  Call API to update record
                var result = await _apiClient.UpdateVehicleTypeAsync(model.Vehicle_type_id, model);

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

        // =====================================================
        //  PUT /VehicleTypeMaster/Delete - Soft delete existing Vehicle Type
        // =====================================================
        [HttpPut()]
        public async Task<IActionResult> Delete([FromBody] VehicleTypeMasterDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                //  Call API to perform delete operation
                var result = await _apiClient.DeleteVehicleTypeAsync(model.Vehicle_type_id);

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

        // =====================================================
        //  GET /VehicleTypeMaster/DownloadVehicleTypeMasterTemplate
        //          - Generate Excel template for VehicleTypeMaster
        // =====================================================
        public IActionResult DownloadVehicleTypeMasterTemplate()
        {
            //  Create memory stream for Excel generation
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                //  Create a new worksheet for VehicleTypeMasterTemplate
                var worksheet = package.Workbook.Worksheets.Add("VehicleTypeMasterTemplate");

                //  Define column headers based on VehicleTypeModel properties
                string[] headers = new string[]
                {
                    "Vehicle_type_id", "Vehicle_type_name", "Category_type_name", "Fuel_type_name",
                    "Description", "Status_name"
                };

                //  Populate header row and autofit columns
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Column(i + 1).AutoFit();
                }

                //  Save Excel to memory stream
                package.Save();
            }

            //  Reset stream position for download
            stream.Position = 0;
            string excelName = $"VehicleTypeMasterTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            //  Return Excel file to browser
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

    }
}
