
using System.ComponentModel.DataAnnotations;

namespace YardManagementApplication.Models
{
    //  Full user read model (represents complete user record)
    public class AppUserModel
    {
        //  Primary identifier
        public long? User_id { get; set; }

        //  Login username
        public string? Employee_id { get; set; }
        
        //  User password (store/handle securely at runtime)
        public string? Password { get; set; }

        //  Optional first/last name
        public string? First_name { get; set; }
        public string? Last_name { get; set; }

        //  Optional computed/display name
        public string? Full_name { get; set; }

        //  Contact details
        public string? Email { get; set; }

        public string? Country_code { get; set; }

        public string? Contact_number { get; set; }


        //  Department (id + display name)
        public long? Department_id { get; set; }

        public string? department_name { get; set; } = null;

        //  User group (id + display name)
        public long? User_group_id { get; set; }

        public string? user_group_name { get; set; } = null;

        //  Status (id + name)
        public long? Status_id { get; set; }

        public string? status_name { get; set; } = null;

        //  Soft delete flag
        public bool Is_deleted { get; set; }

        //  Audit - created
        public string? Created_by { get; set; }
        public DateTimeOffset? Created_at { get; set; }

        //  Audit - updated
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }

        //  Version for concurrency
        public int? Version { get; set; }
    }

    //  Partial update model (nullable fields allow selective updates)
    public class AppUserUpdateModel
    {
        //  Target user id (required for update)
        public long User_id { get; set; }

        //  Optional fields to update
        public string? Employee_id { get; set; }
        public string? Password { get; set; }
        public string? First_name { get; set; }
        public string? Last_name { get; set; }
        public string? Email { get; set; }
        public string? Country_code { get; set; }
        public string? Contact_number { get; set; }

        //  Department (id + display name)
        public long? Department_id { get; set; }
        public string? Department_name { get; set; }

        //  User group (id + display name)
        public long? User_group_id { get; set; }
        public string? User_group_name { get; set; }

        //  Status (id + name)
        public long? Status_id { get; set; }
        public string Status_name { get; set; }

        //  Soft delete flag (nullable for partial updates)
        public bool? Is_deleted { get; set; }

        //  Audit fields for update
        public string? Updated_by { get; set; }
        public DateTimeOffset? Updated_at { get; set; }

        //  Optional version for concurrency
        public int? Version { get; set; }
    }

    //  Role master model
    public class RoleModel
    {
        //  Role identifier and name
        public int Id { get; set; }
        public string Name { get; set; }

        //  Optional description
        public string? Description { get; set; }

        //  Audit timestamps (Created defaults to now)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = null;
    }

}
