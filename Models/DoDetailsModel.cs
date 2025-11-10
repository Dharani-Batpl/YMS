namespace YardManagementApplication.Models
{
    public class DoDetailsModel
    {
        public string do_number { get; set; }
        public DateTimeOffset? planned_dispatch_at { get; set; }
        public int quantity_ordered { get; set; }
        public int allocated { get; set; }
        public int remaining { get; set; }
        public decimal progress { get; set; }
        public List<DoVariantModel> variants { get; set; } = new List<DoVariantModel>();
        public List<DoVinModel> vins { get; set; } = new List<DoVinModel>();
        public string quality_inspector { get; set; }
    }

    public class DoVariantModel
    {
        public string product_name { get; set; }
        public string brand_name { get; set; }
        public int quantity_ordered { get; set; }
        public int quantity_dispatched { get; set; }
        public int balance_needed { get; set; }
    }

    public class DoVinModel
    {
        public string vin_number { get; set; }
        public string variant_name { get; set; }
        public string status { get; set; }
    }

    public class VinSearchResultModel
    {
        public string vin_number { get; set; }
        public string product_name { get; set; }
        public string brand_name { get; set; }
        public string location { get; set; }
        public string status { get; set; }
    }

    
    public class PdiVinDetails
    {
        public string vin_serial_no { get; set; }
        public string product_name { get; set; }
        public string brand_name { get; set; }
        public string slot_code { get; set; }
        public string status { get; set; }
        public int? slot_id { get; set; }
    }
}
