namespace YardManagementApplication.Models
{
    public class RouteModel
    {
        public long Route_id { get; set; }
        public long Source_process_area_id { get; set; }
        public string? Source_process_area_name { get; set; }
        public long Destination_process_area_id { get; set; }
        public string? Destination_process_area_name { get; set; }
        public int? Sla_minutes_cnt { get; set; }
        public long Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int Version { get; set; }
    }


    public class RouteUpdateModel
    {
        public long Route_id { get; set; }
        public long? Source_process_area_id { get; set; }
        public string? Source_process_area_name { get; set; }
        public long? Destination_process_area_id { get; set; }
        public string? Destination_process_area_name { get; set; }
        public int? Sla_minutes_cnt { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
    }


    public class RouteMasterDeleteModel
    {
        public long Route_id { get; set; }
        public bool Is_deleted { get; set; }
        public string? Updated_by { get; set; }
    }



}
