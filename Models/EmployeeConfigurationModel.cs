namespace YardManagementApplication.Models
{
    public class EmployeeModel
    {
        public string? Employee_id { get; set; }
        public string? Employee_code { get; set; }
        public string? First_name { get; set; }
        public string Middle_name { get; set; }
        public string? Last_name { get; set; }
        public string? Department_id { get; set; }
        public string? Department_name { get; set; }
        public string? Reporting_to_id { get; set; }
        public string? Reporting_to_name { get; set; }
        public string? Address { get; set; }
        public string? Contact_number { get; set; }
        public string? Country_code { get; set; }
        public string? Emergency_country_code { get; set; }
        public string? Emergency_contact_number { get; set; }
        public string? Email { get; set; }
        public int? Employee_type_id { get; set; }
        public string? Employee_type { get; set; }
        public string? User_id { get; set; }
        public string? User_name { get; set; }
        public string? User_group_id { get; set; }
        public string? User_group_name { get; set; }
        public string? Skill_id { get; set; }
        public string? Skill_type { get; set; }
        public string? Skill_level_id { get; set; }
        public string? Skill_level_name { get; set; }
        public int? Certificate_type_id { get; set; }
        public string? Certificate_type_name { get; set; }
        public string? Certification_date { get; set; }
        public string? Expiry_date { get; set; }
        public string? Status_id { get; set; }
        public string? Status_name { get; set; }
        public string? Is_deleted { get; set; }
        public string? Created_by { get; set; }
        public DateTime? Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
        public int? Version { get; set; }
    }


    public class EmployeeUpdateModel
    {
        public string? Employee_id { get; set; }
        public string? Employee_code { get; set; }
        public string? First_name { get; set; }
        public string Middle_name { get; set; }
        public string? Last_name { get; set; }
        public string? Department_id { get; set; }
        public string? Address { get; set; }
        public string? Department_name { get; set; }
        public string? Reporting_to_id { get; set; }
        public string? Reporting_to_name { get; set; }
        public string? Contact_number { get; set; }

        public string? Country_code { get; set; }

        public string? Emergency_country_code { get; set; }


        public string? Emergency_contact_number { get; set; }
        public string? Email { get; set; }
        public int? Employee_type_id { get; set; }
        public string? Employee_type { get; set; }
        //public string? User_id { get; set; }
        //public string? User_name { get; set; }
        //public string? User_group_id { get; set; }
        //public string? User_group_name { get; set; }
        public string? Skill_id { get; set; }
        public string? Skill_type { get; set; }
        public string? Skill_level_id { get; set; }
        public string? Skill_level_name { get; set; }
        public int? Certificate_type_id { get; set; }

        public string? Certificate_type_name { get; set; }
        public string? Certification_date { get; set; }

        public string? Blood_group { get; set; }
        public string? Expiry_date { get; set; }
        public string? Status_id { get; set; }
        public string? Status_name { get; set; }
        public string? Is_deleted { get; set; }
        public string Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public string? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
    }
    public class EmployeeDeleteModel
    {
        public long Employee_id { get; set; }
        public string? employee_code { get; set; }
        public bool is_deleted { get; set; }
        public string? updated_by { get; set; }
    }

    //public class DropdownModel
    //{
    //    public int id { get; set; }
    //    public string Name { get; set; }
    //}


}
