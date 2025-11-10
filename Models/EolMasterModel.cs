using System;

namespace YardManagementApplication.Models
{
    public class EolProductionModel
    {
        public long Eol_production_id { get; set; }
        public string Vin { get; set; } = default!;
        public string Production_order_id { get; set; } = default!;
        public string? Product_description { get; set; }
        public long? Shift_id { get; set; }
        public string? Shift_name { get; set; }
        public DateTime? Date_of_production { get; set; }
        public string Eol_quality_inspector_id { get; set; } = default!;
        public string? Eol_quality_inspector_name { get; set; }
        public long? Color_id { get; set; }
        public string? Color_name { get; set; }
        public long Brand_id { get; set; }
        public string? Brand_name { get; set; }
        public long Vehicle_type_id { get; set; }
        public string? Vehicle_type_name { get; set; }
        public long Variant_id { get; set; }
        public string? Variant_name { get; set; }
        public string? Batch_lot_number { get; set; }
        public string Line_id { get; set; } = default!;
        public string Shop_id { get; set; } = default!;
        public DateTimeOffset? Completion_at { get; set; }
        public long? Quality_status_id { get; set; }
        public string? Quality_status_name { get; set; }
        public int? Transit_status { get; set; }  // DB default 0
        public string? Certificate_id { get; set; }
        public bool Is_deleted { get; set; }
        public string Created_by { get; set; } = default!;
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; } = 0;
    }


    public class EolProductionUpdateModel
    {
        public long? Eol_production_id { get; set; }
        public string? Vin { get; set; }
        public string? Production_order_id { get; set; }
        public string? Product_description { get; set; }
        public long? Shift_id { get; set; }
        public string? Shift_name { get; set; }
        public DateTime? Date_of_production { get; set; }
        public string? Eol_quality_inspector_id { get; set; }
        public string? Eol_quality_inspector_name { get; set; }
        public long? Color_id { get; set; }
        public string? Color_name { get; set; }
        public long? Brand_id { get; set; }
        public string? Brand_name { get; set; }
        public long? Vehicle_type_id { get; set; }
        public string? Vehicle_type_name { get; set; }
        public long? Variant_id { get; set; }
        public string? Variant_name { get; set; }
        public string? Batch_lot_number { get; set; }
        public string? Line_id { get; set; }
        public string? Shop_id { get; set; }
        public DateTimeOffset? Completion_at { get; set; }
        public long? Quality_status_id { get; set; }
        public string? Quality_status_name { get; set; }
        public int? Transit_status { get; set; }
        public string? Certificate_id { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
    }

    public class EolMasterDeleteModel
    {
        public string? Vin { get; set; }
        public bool Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
    }
}
