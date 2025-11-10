using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using YardManagementApplication.Models;

namespace YardManagementApplication.Controllers
{
    public class QualityOprnController : Controller
    {
        private readonly v1Client _apiClient;

        public QualityOprnController(v1Client apiClient)
        {
            _apiClient = apiClient;
        }



        // Quality Operation Dashboard
        public async Task<IActionResult> Index()
        {
            try
            {

                var model = new QualityOprnModel();
                
                // Get shift performance data
                model.OrdersAssigned = await GetOrdersAssignedCount();
                model.InProgress = await GetInProgressCount();
                model.Completed = await GetCompletedCount();
                model.VinsTotal = await GetVinsTotalCount();
                model.VinsScanned = await GetVinsScannedCount();
                model.QualityResultsOk = await GetQualityResultsOkCount();
                model.QualityResultsNok = await GetQualityResultsNokCount();
                
                // Get assigned orders data
                model.AssignedOrders = await GetAssignedOrders();
                
                return View(model);
            }
            catch (Exception ex)
            {
                // Handle error
                return StatusCode(500, ex.Message);
            }
        }

        // Assigned Orders view
        public async Task<IActionResult> AssignedOrders()
        {
            try
            {
                var model = new QualityOprnModel();
                model.AssignedOrders = await GetAssignedOrders();
                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Search orders
        [HttpPost]
        public async Task<IActionResult> SearchOrders(string searchTerm)
        {
            try
            {
                var model = new QualityOprnModel();
                model.AssignedOrders = await GetAssignedOrders(searchTerm);
                return PartialView("_AssignedOrdersTable", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Inspect Details Modal
        public async Task<IActionResult> InspectDetails(string orderNo)
        {
            try
            {
                var model = new QualityOprnModel();
                model.SelectedOrderNo = orderNo;
                model.VehicleMovements = await GetVehicleMovementsByOrder(orderNo);
                return PartialView("_InspectDetails", model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Process VIN inspection
        [HttpPost]
        public async Task<IActionResult> ProcessVinInspection(string vinNumber, string orderNo, bool isPass)
        {
            try
            {
                var result = await UpdateVinQualityStatus(vinNumber, orderNo, isPass);
                return Json(new { success = result, message = result ? "VIN inspection updated successfully" : "Failed to update VIN inspection" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Private helper methods
        private async Task<int> GetOrdersAssignedCount()
        {
            try
            {
                // Call API to get orders assigned count from ops.vehicle_movement table
                // var response = await _apiClient.GetAsync("api/vehicle-movement/orders-assigned-count");
                // return await response.Content.ReadFromJsonAsync<int>();
                
                // For now, return sample data
                return 4;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<int> GetInProgressCount()
        {
            try
            {
                // Call API to get in progress count
                // var response = await _apiClient.GetAsync("api/vehicle-movement/in-progress-count");
                // return await response.Content.ReadFromJsonAsync<int>();
                
                return 2;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<int> GetCompletedCount()
        {
            try
            {
                // Call API to get completed count
                // var response = await _apiClient.GetAsync("api/vehicle-movement/completed-count");
                // return await response.Content.ReadFromJsonAsync<int>();
                
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<int> GetVinsTotalCount()
        {
            try
            {
                // Call API to get total VINS count
                // var response = await _apiClient.GetAsync("api/vehicle-movement/vins-total-count");
                // return await response.Content.ReadFromJsonAsync<int>();
                
                return 78;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<int> GetVinsScannedCount()
        {
            try
            {
                // Call API to get scanned VINS count
                // var response = await _apiClient.GetAsync("api/vehicle-movement/vins-scanned-count");
                // return await response.Content.ReadFromJsonAsync<int>();
                
                return 45;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<int> GetQualityResultsOkCount()
        {
            try
            {
                // Call API to get OK quality results count
                // var response = await _apiClient.GetAsync("api/vehicle-movement/quality-ok-count");
                // return await response.Content.ReadFromJsonAsync<int>();
                
                return 41;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<int> GetQualityResultsNokCount()
        {
            try
            {
                // Call API to get NOK quality results count
                // var response = await _apiClient.GetAsync("api/vehicle-movement/quality-nok-count");
                // return await response.Content.ReadFromJsonAsync<int>();
                
                return 4;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<List<AssignedOrderModel>> GetAssignedOrders(string searchTerm = "")
        {
            try
            {
                // Call API to get assigned orders from ops.vehicle_movement table
                // var response = await _apiClient.GetAsync($"api/vehicle-movement/assigned-orders?searchTerm={searchTerm}");
                // var orders = await response.Content.ReadFromJsonAsync<List<AssignedOrderModel>>();
                // return orders ?? new List<AssignedOrderModel>();
                
                // For now, return sample data
                var orders = new List<AssignedOrderModel>
                {
                    new AssignedOrderModel
                    {
                        OrderNo = "ORD-101",
                        TotalVins = 20,
                        Scanned = 8,
                        Ok = 7,
                        Nok = 1,
                        Progress = 40
                    },
                    new AssignedOrderModel
                    {
                        OrderNo = "ORD-102",
                        TotalVins = 18,
                        Scanned = 0,
                        Ok = 0,
                        Nok = 0,
                        Progress = 0
                    },
                    new AssignedOrderModel
                    {
                        OrderNo = "ORD-103",
                        TotalVins = 25,
                        Scanned = 25,
                        Ok = 23,
                        Nok = 2,
                        Progress = 100
                    },
                    new AssignedOrderModel
                    {
                        OrderNo = "ORD-104",
                        TotalVins = 15,
                        Scanned = 12,
                        Ok = 11,
                        Nok = 1,
                        Progress = 80
                    }
                };

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    orders = orders.Where(o => o.OrderNo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                return orders;
            }
            catch
            {
                return new List<AssignedOrderModel>();
            }
        }

        private async Task<List<Models.VehicleMovement>> GetVehicleMovementsByOrder(string orderNo)
        {
            try
            {
                // Call API to get vehicle movements by order number
                // var response = await _apiClient.GetAsync($"api/vehicle-movement/by-order/{orderNo}");
                // var movements = await response.Content.ReadFromJsonAsync<List<Models.VehicleMovement>>();
                // return movements ?? new List<Models.VehicleMovement>();
                
                // For now, return empty list
                return new List<Models.VehicleMovement>();
            }
            catch
            {
                return new List<Models.VehicleMovement>();
            }
        }

        private async Task<bool> UpdateVinQualityStatus(string vinNumber, string orderNo, bool isPass)
        {
            try
            {
                // Call API to update VIN quality status in database
                // var requestData = new { VinNumber = vinNumber, OrderNo = orderNo, IsPass = isPass };
                // var json = JsonSerializer.Serialize(requestData);
                // var content = new StringContent(json, Encoding.UTF8, "application/json");
                // var response = await _apiClient.PostAsync("api/vehicle-movement/update-quality-status", content);
                // return response.IsSuccessStatusCode;
                
                // For now, return true
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
