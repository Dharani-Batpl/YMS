namespace YardManagementApplication.Models
{
    public class TemplateModel
    {
        public long? template_id { get; set; }
        public long? assigned_shift1 { get; set; }
        public long? assigned_shift2 { get; set; }
        public long? assigned_shift3 { get; set; }
        public long? assigned_shift4 { get; set; }
        public long? assigned_shift5 { get; set; }
        public string template_name { get; set; }
        public string? template_description { get; set; }

        public string? assigned_shift1_name { get; set; }
        public string? assigned_shift2_name { get; set; }
        public string? assigned_shift3_name { get; set; }
        public string? assigned_shift4_name { get; set; }
        public string? assigned_shift5_name { get; set; }
        public bool? is_deleted { get; set; }
        public string? created_by { get; set; }
        public DateTimeOffset? created_at { get; set; }
        public string? updated_by { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        public DateTime? effective_from { get; set; }

        //public string? shift_name { get; set; }

    }

    public class TemplateResponseModel : TemplateModel
    {
        public string? shift_name { get; set; }
    }


    // ---------------------- Update Models ----------------------
    public class TemplateUpdateModel
    {
        public long template_id { get; set; }
        public long? assigned_shift1 { get; set; }
        public long? assigned_shift2 { get; set; }
        public long? assigned_shift3 { get; set; }
        public long? assigned_shift4 { get; set; }
        public long? assigned_shift5 { get; set; }
        public string template_name { get; set; }
        public string updated_by { get; set; }
        public string? template_description { get; set; }
        public DateTime? effective_from { get; set; }

        //public string? shift_name { get; set; }
        public bool is_deleted { get; set; }
    }

    
    public class TemplateConfigurationDeleteModel
    {

        public long template_id { get; set; }
        public bool is_deleted { get; set; }
        public string? updated_by { get; set; }
    }

   

}
