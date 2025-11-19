namespace YardManagementApplication.Models
{
    public class TemplateModel
    {
        public long? template_id { get; set; }
        public long? shift_id { get; set; }
        public string template_name { get; set; }
        public string? template_description { get; set; }
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
        public long shift_id { get; set; }
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
