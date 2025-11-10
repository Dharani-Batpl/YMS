using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace YardManagementApplication.Models
{

    public class VehicleMovement
    {
        public Int64 vehicle_movement_id { get; set; }

        public string? vin_serial_no { get; set; }

        public string? production_order_no { get; set; }
        public string? product_description { get; set; }
        public Int64 shift_id { get; set; }
        public string? shift_name { get; set; }
        public DateTime date_of_production { get; set; }
        public Int64 product_id { get; set; }
        public string? product_name { get; set; }
        public Int64 color_id { get; set; }
        public string? color_name { get; set; }
        public Int64 brand_id { get; set; }
        public string? brand_name { get; set; }
        public string? batch_lot_number { get; set; }
        public string? line_id { get; set; }
        public Int64 operator_id { get; set; }
        public string? operator_name { get; set; }
        public DateTime completion_at { get; set; }
        public int quality_status { get; set; }
        public int transit_status { get; set; }
        public string? certificate_no { get; set; }
        public string? delivery_no { get; set; }
        public bool is_deleted { get; set; }
        public string? created_by { get; set; }
        public DateTime created_at { get; set; }
        public string? updated_by { get; set; }
        public DateTime? updated_at { get; set; }
        public int version { get; set; }
    }
}
