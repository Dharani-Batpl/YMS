using System.ComponentModel.DataAnnotations;

namespace YardManagementApplication.Models
{
    public class QualityOprnModel
    {
        // Shift Performance Overview
        public int OrdersAssigned { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public int VinsTotal { get; set; }
        public int VinsScanned { get; set; }
        public int QualityResultsOk { get; set; }
        public int QualityResultsNok { get; set; }

        // Assigned Orders
        public List<AssignedOrderModel> AssignedOrders { get; set; } = new List<AssignedOrderModel>();

        // Inspect Details
        public string SelectedOrderNo { get; set; } = string.Empty;
        public List<VehicleMovement> VehicleMovements { get; set; } = new List<VehicleMovement>();
        public string VinNumber { get; set; } = string.Empty;
        public bool IsPass { get; set; }
    }

    public class AssignedOrderModel
    {
        public string OrderNo { get; set; } = string.Empty;
        public int TotalVins { get; set; }
        public int Scanned { get; set; }
        public int Ok { get; set; }
        public int Nok { get; set; }
        public int Progress { get; set; }
    }
}
