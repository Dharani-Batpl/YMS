namespace YardManagementApplication.Models
{
    public class DeliveryOrderModel
    {
        public long Do_id { get; set; }
        public string Do_number { get; set; }
        public DateTimeOffset do_at { get; set; }
        public long Customer_id { get; set; }
        public string? Customer_name { get; set; }
        public long? Shift_id { get; set; }
        public string? Shift_name { get; set; }
        public DateTime? Date_of_production { get; set; }
        public string? Vehicle_number { get; set; }
        public long? Transporter_id { get; set; }
        public string? Transporter_name { get; set; }
        public string? Driver_name { get; set; }
        public string? Driver_contact_no { get; set; }
        public long Delivery_location_id { get; set; }
        public string? Delivery_location_name { get; set; }
        public string? Delivery_location_code { get; set; }
        public string? Delivery_address { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }

        public string Priority { get; set; }
        public DateTimeOffset Planned_dispatch_at { get; set; }
        public DateTimeOffset? Actual_dispatch_at { get; set; }
        public bool? Is_deleted { get; set; }
        public string Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
        public List<DeliveryOrderDetailModel>? Details { get; set; } = new List<DeliveryOrderDetailModel>();
    }

    public class DeliveryOrderDetailModel
    {
        public long Do_detail_id { get; set; }
        public long Do_id { get; set; }

        public string Do_number { get; set; }               // Foreign key to delivery_order
        public int? Line_number { get; set; }

        public long? Product_id { get; set; }
        public string? Product_name { get; set; }

        public long Color_id { get; set; }
        public string? Color_name { get; set; }

        public long Brand_id { get; set; }
        public string? Brand_name { get; set; }
        public int Vehicle_type_id { get; set; }           // Vehicle Type ID (INT)
        public string? Vehicle_type_name { get; set; }      // Vehicle Type Name (NVARCHAR(255))
        public long Variant_id { get; set; }                // Model ID (BIGINT)
        public string? Variant_name { get; set; }             // Model Name (NVARCHAR(100))
        public string? Batch_lot_number { get; set; }

        public decimal Quantity_ordered { get; set; }
        public decimal? Quantity_dispatched { get; set; }

        public string? Uom { get; set; }
        public string? Storage_location { get; set; }
        public string? Pallet_id { get; set; }

        public string? So_number { get; set; }
        public string? Invoice_number { get; set; }
        public string? Gate_pass_number { get; set; }
        public string? Eway_bill_number { get; set; }
        public string? Freight_terms { get; set; }

        public string? Remarks { get; set; }
        public string? Gps_tracking_id { get; set; }

        public bool Is_deleted { get; set; }

        public string Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }

        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }

        public int? Version { get; set; }

    }


    public class DeliveryOrderUpdateModel
    {
        public long Do_id { get; set; }
        public string? Do_number { get; set; }
        public DateTimeOffset Do_at { get; set; }
        public long Customer_id { get; set; }
        public string? Customer_name { get; set; }
        public long? Shift_id { get; set; }
        public string? Shift_name { get; set; }
        public DateTime? Date_of_production { get; set; }
        public string? Vehicle_number { get; set; }
        public long? Transporter_id { get; set; }
        public string? Transporter_name { get; set; }
        public string? Driver_name { get; set; }
        public string? Driver_contact_no { get; set; }
        public long Delivery_location_id { get; set; }
        public string? Delivery_location_name { get; set; }
        public string? Delivery_location_code { get; set; }
        public string? Delivery_address { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }

        public string Priority { get; set; }
        public DateTimeOffset Planned_dispatch_at { get; set; }
        public DateTimeOffset? Actual_dispatch_at { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }

        public List<DeliveryOrderDetailUpdateModel>? Details { get; set; }
    }
    public class DeliveryOrderDetailUpdateModel
    {
        public long Do_detail_id { get; set; }

        public long? Do_id { get; set; }
        public string? Do_number { get; set; }
        public int? Line_number { get; set; }
        public long? Product_id { get; set; }
        public string? Product_name { get; set; }
        public long? Color_id { get; set; }
        public string? Color_name { get; set; }
        public long? Brand_id { get; set; }
        public string? Brand_name { get; set; }
        public int? Vehicle_type_id { get; set; }           // Vehicle Type ID (INT)
        public string? Vehicle_type_name { get; set; }      // Vehicle Type Name (NVARCHAR(255))
        public long Variant_id { get; set; }                // Model ID (BIGINT)
        public string? Variant_name { get; set; }             // Model Name (NVARCHAR(100))
        public string? Batch_lot_number { get; set; }
        public decimal? Quantity_ordered { get; set; }
        public decimal? Quantity_dispatched { get; set; }
        public string? Uom { get; set; }
        public string? Storage_location { get; set; }
        public string? Pallet_id { get; set; }
        public string? So_number { get; set; }
        public string? Invoice_number { get; set; }
        public string? Gate_pass_number { get; set; }
        public string? Eway_bill_number { get; set; }
        public string? Freight_terms { get; set; }
        public string? Remarks { get; set; }
        public string? Gps_tracking_id { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }

    }



    public class DeliveryOrderDeleteModel
    {
        public long Do_id { get; set; }
        public string Do_number { get; set; }
        public DateTimeOffset Do_at { get; set; }
        public long Customer_id { get; set; }
        public string Customer_name { get; set; }
        public long Shift_id { get; set; }
        public string Shift_name { get; set; }
        public DateTime Date_of_production { get; set; }
        public string Vehicle_number { get; set; }
        public long? Transporter_id { get; set; }
        public string? Transporter_name { get; set; }
        public string? Driver_name { get; set; }
        public string? Driver_contact_no { get; set; }
        public long Delivery_location_id { get; set; }
        public string Delivery_location_name { get; set; }
        public string? Delivery_location_code { get; set; }
        public string Delivery_address { get; set; }
        public long Status_id { get; set; }
        public string Status_name { get; set; }
        public DateTimeOffset? Planned_dispatch_at { get; set; }
        public DateTimeOffset? Actual_dispatch_at { get; set; }
        public bool Is_deleted { get; set; }
        public string Created_by { get; set; }
        public DateTimeOffset Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int Version { get; set; }
    }



}
