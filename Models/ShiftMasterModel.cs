namespace YardManagementApplication.Models
{
    public class ShiftModel
    {
        public long Shift_id { get; set; }
        public string? Shift_name { get; set; }
        public string? Shift_description { get; set; }
        public long Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTime? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }

        public int? Version { get; set; }
    }
    public class ShiftUpdateModel
    {
        public long Shift_id { get; set; }
        public string? Shift_name { get; set; }
        public string? Shift_description { get; set; }
        public long? Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
        public int? Version { get; set; }
    }

}