namespace YardManagementApplication.Models
{
    public class TemplateModel
    {
        public long Template_id { get; set; }
        public string? Template_name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status_id { get; set; }
        public string? Status_name { get; set; }
        public bool? Is_deleted { get; set; }
        public DateTime? Temp_start_time { get; set; }
        public DateTime? Temp_end_time { get; set; }
        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Plant_id { get; set; }
        public string Plant_name { get; set; }
        public int? Version { get; set; }

        // Nested shifts
        public List<ShiftDetailsModel> Shiftdetails { get; set; } = new();

    }


    // ---------------------- Child Table Model ----------------------

    public class ShiftDetailsModel
    {
        public long Shift_details_id { get; set; }
        public long Template_id { get; set; } // FK to Template
        public long Shift_id { get; set; }
        public string? Shift_name { get; set; } = null!;
        public TimeSpan? Shift_start_time { get; set; }
        public TimeSpan? Shift_end_time { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }

        // Child collection
        public List<BreakDetailsModel> Breakdetails { get; set; } = new();
    }

    // ---------------------- Grandchild Table Model ----------------------

    public class BreakDetailsModel
    {
        public long Break_details_id { get; set; }
        public long Shift_details_id { get; set; } // FK to Shift
        public long Break_id { get; set; }
        public string? Break_name { get; set; } = null!;
        public TimeSpan? Break_start_time { get; set; }
        public TimeSpan? Break_end_time { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
    }

    // ---------------------- Update Models ----------------------
    public class TemplateUpdateModel
    {
        public long Template_id { get; set; }
        public string? Template_name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Status_id { get; set; }
        public string? Status_name { get; set; }
        public DateTime? Temp_start_time { get; set; }
        public DateTime? Temp_end_time { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int Version { get; set; }

        public List<ShiftDetailsUpdateModel> Shiftdetails { get; set; } = new();
    }

    public class ShiftDetailsUpdateModel
    {
        public long Shift_details_id { get; set; }
        public long Template_id { get; set; }
        public string? Shift_name { get; set; }
        public TimeSpan? Shift_start_time { get; set; }
        public TimeSpan? Shift_end_time { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }

        public List<BreakDetailsUpdateModel> Breakdetails { get; set; } = new();
    }

    public class BreakDetailsUpdateModel
    {
        public long Break_details_id { get; set; }
        public long Shift_details_id { get; set; }
        public string? Break_name { get; set; }
        public TimeSpan? Break_start_time { get; set; }
        public TimeSpan? Break_end_time { get; set; }
        public bool? Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int? Version { get; set; }
    }
    public class TemplateConfigurationDeleteModel
    {

        public long Template_id { get; set; }
        public bool Is_deleted { get; set; }
        public string? Updated_by { get; set; }
    }

   

}
