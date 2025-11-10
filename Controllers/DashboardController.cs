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
    public class DashboardController : Controller
    {
        private readonly v1Client _apiClient;

        public DashboardController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
               
                return View();
            }
            catch (Exception ex)
            {
                // Handle error
                return StatusCode(500, ex.Message);
            }
        }

      
    }
}
