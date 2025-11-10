// =================================================================================================
// Version Control
// =================================================================================================
// File Name   : EnterpriseMasrterController.cs
// Module      : Masters
// Author      : Srinithi G
// Created On  : 2025-10-14
// Description : Controller for managing Enterprise master data.
// =================================================================================================

// =================================================================================================
// Change History
// =================================================================================================
// Version | Date       | Author           | Description
// -------------------------------------------------------------------------------------------------
// 1.0     | 2025-10-14 | Kavin R       | Initial creation based on EnterpriseModel structure.
// =================================================================================================
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Models;


namespace YardManagementApplication
{

    [Route("[controller]/[action]")]
    public class EnterpriseMasterController : Controller
    {
        private readonly v1Client _apiClient;

        public EnterpriseMasterController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "Enterprise Master";
                var result = await _apiClient.GetAllEnterpriseAsync();
                // If no data returned, create a empty row to capture all properties from model
                if (result == null || !result.Any())
                {
                    result = new List<EnterpriseModel>
                       {
                           new EnterpriseModel()
                       };
                }
                string jsonResult = System.Text.Json.JsonSerializer.Serialize(result);
                ViewData["EnterpriseMasterData"] = jsonResult;
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}

