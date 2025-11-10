namespace YardManagementApplication.Models
{
    public class DepartmentModel
    {
        public long Department_id { get; set; }
        public string Department_code { get; set; }
        public string Department_name { get; set; }
        public string? Description { get; set; }
        public long Status_id { get; set; }
        public string Status_name { get; set; }
        public bool Is_deleted { get; set; }
        public string Created_by { get; set; }
        public DateTimeOffset Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int Version { get; set; }
    }
    public class DepartmentUpdateModel
    {
        public string Department_code { get; set; }
        public string Department_name { get; set; }
        public string? Description { get; set; }
        public long Status_id { get; set; }
        public bool Is_deleted { get; set; }
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }
        public int Version { get; set; }
    }

}