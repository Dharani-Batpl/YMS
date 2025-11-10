// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : TemplateConfigurationController.cs
// Module      : Masters
// Author      : Srinithi G
// Created On  : 2025-10-14
// Description : Controller for managing Template Configuration master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author       | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Srinithi G   | Initial creation based on Template Configuration Model structure.
// =================================================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Models;
using YardManagementApplication.Utils; // <-- use AppJson & Utility

namespace YardManagementApplication
{
    [Route("[controller]/[action]")]
    public class TemplateConfigurationController : Controller
    {
        // -----------------------------------------------------
        // Dependencies
        // -----------------------------------------------------
        private readonly v1Client _apiClient;

        public TemplateConfigurationController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }

        // =====================================================
        // GET /TemplateConfiguration/Index - Load page & seed ViewData/ViewBag
        // =====================================================
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "TemplateConfiguration";

                var result = await _apiClient.GetAllTemplatesAsync();

                // Seed an empty row so the grid shows all columns on first load
                if (result == null || !result.Any())
                {
                    result = new List<TemplateModel> { new TemplateModel() };
                }

                // Use common utility for consistent date formatting:
                // - DateTime       => MM/dd/yyyy
                // - DateTimeOffset => MM/dd/yyyy HH:mm:ss (24h)
                var jsonOptions = AppJson.CreateDateOnlyOptions();

                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result, jsonOptions);
                ViewData["TemplateConfiguration"] = jsonResult;

                // Fetch dropdown sources from API
                var statuses = await _apiClient.TemplateStatusAsync();
                var shifts = await _apiClient.ShiftAsync();
                var breaks = await _apiClient.BreakAsync();
                var plant = await _apiClient.PlantListAsync();

                ViewBag.StatusList = Utility.PrepareSelectList(statuses);
                ViewBag.ShiftList = Utility.PrepareSelectList(shifts);
                ViewBag.BreakList = Utility.PrepareSelectList(breaks);
                ViewBag.PlantList = Utility.PrepareSelectList(plant);

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
        // POST /TemplateConfiguration/Create - Create new template
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TemplateModel model)
        {
            try
            {
                model.Created_by = HttpContext.Session.GetString("LoginUser");

                // Cascade audit/ids to children
                if (model.Shiftdetails != null && model.Shiftdetails.Count > 0)
                {
                    foreach (var shift in model.Shiftdetails)
                    {
                        shift.Template_id = model.Template_id;
                        shift.Created_by = model.Created_by;

                        if (shift.Breakdetails != null && shift.Breakdetails.Count > 0)
                        {
                            foreach (var breakDetail in shift.Breakdetails)
                            {
                                breakDetail.Shift_details_id = shift.Shift_details_id;
                                breakDetail.Created_by = model.Created_by;
                            }
                        }
                    }
                }

                var result = await _apiClient.InsertTemplateAsync(model);

                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Template created successfully."
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
        // PUT /TemplateConfiguration/Update - Update existing template
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] TemplateUpdateModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                if (model.Shiftdetails != null && model.Shiftdetails.Count > 0)
                {
                    foreach (var shift in model.Shiftdetails)
                    {
                        shift.Template_id = model.Template_id;
                        shift.Updated_by = model.Updated_by;

                        if (shift.Breakdetails != null && shift.Breakdetails.Count > 0)
                        {
                            foreach (var breakDetail in shift.Breakdetails)
                            {
                                breakDetail.Shift_details_id = shift.Shift_details_id;
                                breakDetail.Updated_by = model.Updated_by;
                            }
                        }
                    }
                }

                var result = await _apiClient.UpdateTemplateAsync(model.Template_id, model);

                return Ok(new
                {
                    status = result.Status,
                    title = result.Title ?? "Success",
                    message = result.Detail ?? "Template updated successfully."
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
        // PUT /TemplateConfiguration/Delete - Soft delete by id
        // =====================================================
        [HttpPut]
        public async Task<IActionResult> Delete([FromBody] TemplateConfigurationDeleteModel model)
        {
            try
            {
                model.Updated_by = HttpContext.Session.GetString("LoginUser");

                var result = await _apiClient.DeleteTemplateAsync(model.Template_id);

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
        // POST /TemplateConfiguration/Upload - (stub)
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                // TODO: implement CSV processing if/when needed

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
                    title = problem.Title ?? "Error",
                    message = problem.Detail ?? "An unexpected error occurred."
                });
            }
        }
    }
}
