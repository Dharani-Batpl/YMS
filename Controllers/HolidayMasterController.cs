// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : HolidayMasterController.cs
// Module      : Masters
// Author      : Dhanalakshmi D
// Created On  : 2025-10-14
// Description : Controller for managing holiday master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Dhanalakshmi D   | Initial creation based on holiday master model structure.
// =================================================================================================

using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Helpers;
using YardManagementApplication.Models;
using YardManagementApplication.Utils;   // <-- IMPORTANT: to use AppJson & Utility

namespace YardManagementApplication
{
    [Route("[controller]/[action]")]
    public class HolidayMasterController : Controller
    {
        // -----------------------------------------------------
        // Dependencies
        // -----------------------------------------------------
        private readonly v1Client _apiClient;
        private readonly CsvUploadService _csvUploadService;
        private readonly IValidator<HolidayModel> _validator;

        // Single constructor to inject dependencies
        public HolidayMasterController(
            CsvUploadService csvUploadService,
            v1Client apiClient,
            IValidator<HolidayModel> validator)
        {
            _csvUploadService = csvUploadService;
            _apiClient = apiClient;
            _validator = validator;
        }

        // =====================================================
        // GET /HolidayMaster/Index - Load page & seed ViewData/ViewBag
        // =====================================================
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Holiday Master";

                var result = await _apiClient.GetAllHolidayAsync();

                // Ensure at least one row so the grid shows columns
                if (result == null || !result.Any())
                {
                    result = new List<HolidayModel> { new HolidayModel() };
                }

                // Use the COMMON utility for consistent date formatting:
                // - DateTime      => MM/dd/yyyy
                // - DateTimeOffset=> MM/dd/yyyy HH:mm:ss (24h)
                // Old line
                // var jsonOptions = AppJson.CreateUiOptions();

                // New line
                var jsonOptions = AppJson.CreateDateOnlyOptions();


                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result, jsonOptions);
                ViewData["HolidayMasterData"] = jsonResult;

                // Dropdowns
                var statuses = await _apiClient.HolidayStatusAsync();
                var holidayType = await _apiClient.HolidayTypeAsync();
                ViewBag.StatusList = Utility.PrepareSelectList(statuses);
                ViewBag.HolidayTypeList = Utility.PrepareSelectList(holidayType);

                return View();
            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        // POST /HolidayMaster/Create - Create new holiday
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HolidayModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");
                model.Version = 0;
             
                    
                var result = await _apiClient.InsertHolidayAsync(model);

                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        // PUT /HolidayMaster/Update - Update existing holiday (partial)
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] HolidayUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                var result = await _apiClient.UpdateHolidayAsync(model.Holiday_id, model);

                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        // PUT /HolidayMaster/Delete - Soft delete by id
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] HolidayMasterDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                var result = await _apiClient.DeleteHolidayAsync(model.Holiday_id);

                return Ok(new
                {
                    status = result.Status,
                    title = "Success",
                    message = result.Detail ?? "Record updated successfully"
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
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
        // GET /HolidayMaster/DownloadHolidayMasterTemplate - Export Excel template
        // =====================================================
        public IActionResult DownloadHolidayMasterTemplate()
        {
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("HolidayMasterTemplate");

                string[] headers = new[]
                {
                    "holiday_id", "holiday_name", "holiday_date",
                    "holiday_type_id", "holiday_type_name", "description",
                    "created_by", "created_at", "updated_by", "updated_at"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Column(i + 1).AutoFit();
                }

                package.Save();
            }

            stream.Position = 0;
            string excelName = $"HolidayMasterTemplate-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        // =====================================================
        // POST /HolidayMaster/Upload - CSV upload for bulk insert
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var nameOk = Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase);
                var type = (file.ContentType ?? "").ToLowerInvariant();
                var typeOk = type.Contains("csv") || type == "application/vnd.ms-excel";
                if (!nameOk && !typeOk)
                    return BadRequest("Only CSV files are allowed.");

                var res = _csvUploadService.ProcessCsvFile(file, _validator);

                if (res.ValidItems.Any())
                {
                    foreach (var validItem in res.ValidItems)
                    {
                        await _apiClient.InsertHolidayAsync(validItem);
                    }

                    string invalidRecordsCsv = null;
                    if (res.InvalidItems.Any())
                    {
                        invalidRecordsCsv = _csvUploadService.CreateInvalidCsvWithErrors(res.InvalidItems);
                    }

                    return Ok(new
                    {
                        status = "success",
                        title = "Success",
                        message = $"{res.ValidCount} records added successfully",
                        invalidRecords = invalidRecordsCsv != null
                            ? Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidRecordsCsv))
                            : null
                    });
                }

                return Ok(new
                {
                    status = "success",
                    title = "No Records",
                    message = "No valid records to insert."
                });
            }
            catch (ApiException<ProblemDetails> ex)
            {
                var problem = ex.Result;

                return StatusCode(problem.Status ?? ex.StatusCode, new
                {
                    status = problem.Status ?? ex.StatusCode,
                    title = "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }
    }
}
