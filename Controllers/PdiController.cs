using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class PdiController : Controller
    {
        private readonly v1Client _apiClient;
        private readonly ILogger<PdiController> _logger;

        public PdiController(v1Client apiClient, ILogger<PdiController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["Title"] = "PDI";
                
                try
                {
                    _logger.LogInformation("Calling GetAllDoForPdiAsync...");
                    var result = await _apiClient.GetAllDoForPdiAsync();
                    _logger.LogInformation("API call successful, received {Count} items", result?.Count ?? 0);
                    ViewData["PdiData"] = result;
                }
                catch (Exception apiEx)
                {
                    // Log the API error but continue with empty data
                    _logger.LogError(apiEx, "API call failed: {Message}", apiEx.Message);
                    ViewData["PdiData"] = new List<PdiModel>();
                }
                
                return View();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Controller error");
                return StatusCode(500, ex.Message);
            }
        }
        public async Task<IActionResult> GetVariantsForDo(string doNumber)
        {
            try
            {
                try
                {
                    _logger.LogInformation("Calling GetAllDoForPdiAsync...");
                    var result = await _apiClient.GetAllDoForPdiAsync();
                    _logger.LogInformation("API call successful, received {Count} items", result?.Count ?? 0);
                    ViewData["PdiData"] = result;
                }
                catch (Exception apiEx)
                {
                    // Log the API error but continue with empty data
                    _logger.LogError(apiEx, "API call failed: {Message}", apiEx.Message);
                    ViewData["PdiData"] = new List<PdiModel>();
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Controller error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVinLocation(long? i_supervisor_name, string? i_vin_serial_no, int? i_required_quantity, string? i_delivery_no)
        {
            try
            {
                
                var result = await _apiClient.GetVinLocationAsync(i_supervisor_name, i_vin_serial_no, i_required_quantity, i_delivery_no);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVinLocationAuto(int? i_required_quantity, string? i_delivery_no)
        {
            try
            {
                
                var result = await _apiClient.GetVinLocationAutoAsync(i_required_quantity, i_delivery_no);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> DoDetails(string doNumber)
        {
            try
            {
                ViewData["Title"] = "DO Details";
                
                // Call the actual GetDoDetails API
                var doDetailsData = await _apiClient.GetDoDetailsAsync(doNumber);
                
                if (doDetailsData == null || !doDetailsData.Any())
                {
                    return NotFound();
                }

                // Convert the API response to our DoDetailsModel
                var firstDoData = doDetailsData.First();

                // Try to parse quantities as decimals, then convert to int
                // Handle empty strings, nulls, and non-numeric values more robustly
                var quantityOrderedStr = firstDoData.Quantity_Ordered;
                var quantityDispatchedStr = firstDoData.Allocated;
                
                var quantityOrdered = 0;
                var quantityDispatched = 0;

                var doDetails = new DoDetailsModel
                {
                    do_number = firstDoData.Do_Number,
                    planned_dispatch_at = firstDoData.Dispatch_Date,
                    quantity_ordered = firstDoData.Quantity_Ordered,
                    allocated = firstDoData.Allocated,
                    remaining = firstDoData.Remaining,
                    progress = (decimal)firstDoData.RemainingPercentage,
                    variants = doDetailsData.Select(x =>
                    {
                        // Since quantities are already integers, use them directly
                        var qtyOrdered = x.Quantity_Ordered;
                        var qtyDispatched = x.Allocated;

                        // Debug logging for variants
                        System.Diagnostics.Debug.WriteLine($"Variant {x.Brand_Name}-{x.Variant_Name}: qtyOrdered={qtyOrdered}, qtyDispatched={qtyDispatched}, remaining={x.Remaining}, progress={x.RemainingPercentage}");

                        return new DoVariantModel
                        {
                            product_name = x.Variant_Name,
                            brand_name = x.Brand_Name,
                            quantity_ordered = qtyOrdered,
                            quantity_dispatched = qtyDispatched,
                            balance_needed = Math.Max(0, qtyOrdered - qtyDispatched)
                        };
                    }).ToList(),
                    vins = new List<DoVinModel>(), // VINs would come from a separate API call if needed
                    quality_inspector = string.Empty // Quality inspector would come from API if available
                };

                ViewData["DoDetails"] = doDetails;
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            try
            {
                var result = await _apiClient.GetDriversAsync();
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> InsertVehicleMovement([FromBody] List<VehicleMovement> vehicleMovements)
        {
            try
            {
                if (vehicleMovements == null || !vehicleMovements.Any())
                {
                    return Json(new { success = false, message = "VehicleMovement data is required" });
                }

                // Call the API client to insert the vehicle movements
                var result = await _apiClient.InsertVehicleMovementAsync(vehicleMovements);
                
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
