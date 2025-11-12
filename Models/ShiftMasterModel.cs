namespace YardManagementApplication.Models
{
    public class ShiftModel
    {
        public long Shift_id { get; set; }
        public string? Shift_name { get; set; }
        public string? Shift_description { get; set; }
        public TimeSpan Start_Time { get; set; }
        public TimeSpan End_Time { get; set; }

        public string? Break_Details { get; set; }
        public bool Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }


    }
    public class ShiftUpdateModel
    {
        public long Shift_id { get; set; }
        public string? Shift_name { get; set; }
        public string? Shift_description { get; set; }
        public TimeSpan Start_Time { get; set; }
        public TimeSpan End_Time { get; set; }

        public string? Break_Details { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
       
    }

}