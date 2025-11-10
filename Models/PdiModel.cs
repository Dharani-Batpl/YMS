namespace YardManagementApplication.Models
{
    public class PdiModel
    {
        // Fields from stored procedure (PascalCase as returned by SP)
        public string? Do_Number { get; set; }
        public string? Variant_Name { get; set; }
        public DateTimeOffset? Dispatch_Date { get; set; }
        public int Quantity_Ordered { get; set; }
        public int Allocated { get; set; }
        public int Remaining { get; set; }
        public string? Delivery_Status { get; set; }
        public string? Delivery_State { get; set; }
        public int PDI_OK_Count { get; set; }
        public int PDI_NotOK_Count { get; set; }
        public decimal RemainingPercentage { get; set; }
        public string? pdi_inspector { get; set; }

        // Calculated properties
        public int Total_inspected => PDI_OK_Count + PDI_NotOK_Count;
        
        public decimal Quality_percentage => Total_inspected > 0 ? (decimal)PDI_OK_Count / Total_inspected * 100 : 0;
    }
}
