namespace YardManagementApplication.Models
{
    public class EnterpriseModel
    {
        public long Enterprise_id { get; set; }
        public string Enterprise_code { get; set; }
        public string Enterprise_name { get; set; }
        public string? Description { get; set; }
        public long Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
    }

    public class EnterpriseUpdateModel
    {
        public long Enterprise_id { get; set; }
        public string? Enterprise_code { get; set; }
        public string? Enterprise_name { get; set; }
        public string? Description { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
    }

    public class EnterpriseDeleteModel
    {
        public long Enterprise_id { get; set; }
        public bool Is_deleted { get; set; }
        public string? Updated_by { get; set; }
    }

    public class EnterpriseViewModel
    {
        public long Enterprise_id { get; set; }
        public string? Enterprise_code { get; set; }
        public string? Enterprise_name { get; set; }
        public string? Description { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool? Is_deleted { get; set; }
    }
}
