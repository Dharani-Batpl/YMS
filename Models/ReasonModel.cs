namespace YardManagementApplication.Models
{
    public class ReasonModel
    {
        public long Reason_id { get; set; }
        public string Reason_code { get; set; }
        public string Reason_name { get; set; }
        public long Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTimeOffset    Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
    }

    public class ReasonUpdateModel
    {
        public long Reason_id { get; set; }
        public string? Reason_code { get; set; }
        public string? Reason_name { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
    }

    public class ReasonDeleteModel
    {
        public long Reason_id { get; set; }
        public bool Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
    }

    public class ReasonViewModel
    {
        public long Reason_id { get; set; }
        public string? Reason_code { get; set; }
        public string? Reason_name { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool? Is_deleted { get; set; }
    }

    public class DropdownModel1
    {
        public long Id { get; set; }      // Typically the primary key
        public string Name { get; set; } = null!; // Display name
    }
}
