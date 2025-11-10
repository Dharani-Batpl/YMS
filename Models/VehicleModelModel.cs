namespace YardManagementApplication.Models
{
    // Main VehicleModel
    public class VehicleModel
    {
        public long? variant_id { get; set; }  // model_id
        public string? variant_code { get; set; }  // model_code
        public string? variant_name { get; set; }  // model_name
        public long? brand_id { get; set; }  // brand_id
        public string? brand_name { get; set; }  // brand_name
        public long? vehicle_type_id { get; set; }  // vehicle_type_id
        public string? vehicle_type_name { get; set; }
        public int? model_year_id { get; set; }// vehicle_type_name
        public int? model_year { get; set; }  // model_year
        public string? description { get; set; }  // description
        public string? wmi_code { get; set; }  // wmi_code
        public string? attr_4_8 { get; set; }  // attr_4_8
        public string? check_digit { get; set; }  // check_digit
        public string? model_year_code { get; set; }  // model_year_code
        public string? plant_code { get; set; }  // plant_code
        public int? next_sequence_cnt { get; set; }  // next_sequence_cnt
        public byte? seq_pad_length_cnt { get; set; }  // seq_pad_length_cnt
        public string? vin_prefix { get; set; }  // vin_prefix
        public long? status_id { get; set; }  // status_id
        public string? status_name { get; set; }  // status_name
        public bool? is_deleted { get; set; } = false; // is_deleted
        public string? created_by { get; set; }  // created_by
        public DateTimeOffset? created_at { get; set; }  // created_at
        public string? updated_by { get; set; }  // updated_by
        public DateTimeOffset? updated_at { get; set; }  // updated_at
        public int version { get; set; }  // version
    }

    // VehicleModel Update Model
    public class VehicleUpdateModel
    {
        public long variant_id { get; set; }  // model_id
        public string? variant_code { get; set; }  // model_code
        public string? variant_name { get; set; }  // model_name
        public long? brand_id { get; set; }  // brand_id
        public string? brand_name { get; set; }  // brand_name
        public long? vehicle_type_id { get; set; }  // vehicle_type_id
        public string? vehicle_type_name { get; set; }  // vehicle_type_name
        public int? model_year { get; set; }  // model_year
        public string? description { get; set; }  // description
        public string? wmi_code { get; set; }  // wmi_code
        public string? attr_4_8 { get; set; }  // attr_4_8
        public string? check_digit { get; set; }  // check_digit
        public string? model_year_code { get; set; }  // model_year_code
        public string? plant_code { get; set; }  // plant_code
        public int? next_sequence_cnt { get; set; }  // next_sequence_cnt
        public byte? seq_pad_length_cnt { get; set; }  // seq_pad_length_cnt
        public string? status_name { get; set; }  // status_name
        public long? status_id { get; set; }  // status_id
        public string? updated_by { get; set; }  // updated_by
        public DateTimeOffset? updated_at { get; set; }  // updated_at
    }

    public class VehicleModelDeleteModel
    {
        public long variant_id { get; set; }  // model_id
        public bool? is_deleted { get; set; }  // is_deleted
        public string? updated_by { get; set; }  // updated_by
        public DateTimeOffset? updated_at { get; set; }  // updated_at
    }

    public class VehicleModelViewModel
    {
        public long variant_id { get; set; }  // model_id
        public string variant_code { get; set; }  // model_code
        public string variant_name { get; set; }  // model_name
        public long brand_id { get; set; }  // brand_id
        public string brand_name { get; set; }  // brand_name
        public long vehicle_type_id { get; set; }  // vehicle_type_id
        public string vehicle_type_name { get; set; }  // vehicle_type_name
        public int model_year { get; set; }  // model_year
        public string? description { get; set; }  // description
        public string wmi_code { get; set; }  // wmi_code
        public string attr_4_8 { get; set; }  // attr_4_8
        public string check_digit { get; set; }  // check_digit
        public string model_year_code { get; set; }  // model_year_code
        public string plant_code { get; set; }  // plant_code
        public int next_sequence_cnt { get; set; }  // next_sequence_cnt
        public byte seq_pad_length_cnt { get; set; }  // seq_pad_length_cnt
        public string vin_prefix { get; set; }  // vin_prefix
        public long status_id { get; set; }  // status_id
        public string status_name { get; set; }  // status_name
        public bool is_deleted { get; set; }  // is_deleted
    }
}
