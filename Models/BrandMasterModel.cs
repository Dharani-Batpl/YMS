namespace YardManagementApplication.Models
{
    public class VehicleBrandModel
    {
        public long? Brand_id { get; set; }
        public string? Brand_code { get; set; }
        public string? Brand_name { get; set; }
        public string? Manufacturer_name { get; set; }
        public long Country_id { get; set; }
        public string? Country_name { get; set; }
        public long Status_id { get; set; }
        public string? Status_name { get; set; }
        public string? Description { get; set; }
        public bool Is_deleted { get; set; }

        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }                 //  Record version for concurrency control
    }


    public class VehicleBrandUpdateModel
    {
        public long? Brand_id { get; set; }
        public string? Brand_code { get; set; }
        public string? Brand_name { get; set; }
        public string Manufacturer_name { get; set; }
        public long? Country_id { get; set; }
        public string? Country_name { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }
        public string? Description { get; set; }
        public bool Is_deleted { get; set; }

        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
    }


    public class BrandMasterDeleteModel
    {
        public long Brand_id { get; set; }
        public string? Brand_code { get; set; }
        public bool Is_deleted { get; set; }
        public string? Updated_by { get; set; }
    }



}
