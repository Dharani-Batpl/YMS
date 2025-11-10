
namespace YardManagementApplication.Models
{
    //  Parent model representing a User Group with its screen permissions
    public class UserGroupModel
    {
        public long? user_group_id { get; set; }         //  PK; comment says "null for insert" (API may treat 0/omitted as new)
        public string user_group_name { get; set; }     //  Display name of the user group
        public string? description { get; set; }        //  Optional description
        public bool? is_deleted { get; set; } = false;   //  Soft delete flag (default false)
        public string? created_by { get; set; }          //  Audit - creator
        public DateTimeOffset? created_at { get; set; }  //  Audit - created timestamp
        public string? updated_by { get; set; }         //  Audit - last updater
        public DateTimeOffset? updated_at { get; set; } //  Audit - last update timestamp
        public int? version { get; set; } = 1;           //  Optimistic concurrency/versioning

        // CHILD: Screens (one-to-many)
        public List<UserGroupWithScreensModel>? screens { get; set; } = new List<UserGroupWithScreensModel>(); //  Child collection of screen permissions
    }

    // ===========================
    // CHILD: UserGroupWithScreens
    //  Child model representing a single screen permission row
    // ===========================
    public class UserGroupWithScreensModel
    {
        public long? user_group_screen_id { get; set; }      //  PK for mapping; null for insert
        public long? user_group_id { get; set; }             //  FK to parent user group
        public string? user_group_name { get; set; }          //  Redundant display name (denormalized)
        public long? module_id { get; set; }                 //  Module identifier
        public string? module_name { get; set; }             //  Module display name
        public long? screen_id { get; set; }                 //  Screen identifier
        public string? screen_name { get; set; }             //  Screen display name
        public long? permission_id { get; set; }             // screen permission id
        public string? permission { get; set; }              //  Permission string: 'Read','Write','Read/Write'
        public bool? is_deleted { get; set; } = false;       //  Soft delete flag for this mapping
        public string? created_by { get; set; }              //  Audit - creator
        public DateTimeOffset? created_at { get; set; }      //  Audit - created timestamp
        public string? updated_by { get; set; }             //  Audit - last updater
        public DateTimeOffset? updated_at { get; set; }     //  Audit - last update timestamp
        public int? version { get; set; } = 1;               //  Optimistic concurrency/versioning
    }

    //  Update DTO for User Group + its screens (partial updates supported via nullable fields)
    public class UserGroupUpdateModel
    {
        // ==========================
        // USER GROUP (parent)
        // ==========================
        public long? user_group_id { get; set; }         //  Required for update (target group id)
        public string? user_group_name { get; set; }    //  Optional new name
        public string? description { get; set; }        //  Optional description
        public bool? is_deleted { get; set; }           //  Optional soft delete flag
        public string? updated_by { get; set; }         //  Audit - updater
        public DateTimeOffset? updated_at { get; set; } //  Audit - update timestamp
        public int? version { get; set; }               //  Optional optimistic concurrency token

        // ==========================
        // USER GROUP SCREENS (child)
        // ==========================
        public List<UserGroupWithScreensUpdateModel>? screens { get; set; } //  Optional set of child updates/inserts/deletes
    }
    // ===========================
    //  Update DTO for a single screen permission mapping
    // ===========================
    public class UserGroupWithScreensUpdateModel
    {
        public long? user_group_screen_id { get; set; }      //  Existing row id; null => insert new mapping
        public long? user_group_id { get; set; }             //  FK to parent user group
        public string? user_group_name { get; set; }         //  Optional display name (if required)

        public long? module_id { get; set; }                 //  Target module id
        public string? module_name { get; set; }             //  Optional module name

        public long? screen_id { get; set; }                //  Target screen id
        public string? screen_name { get; set; }             //  Optional screen name
        public long? permission_id { get; set; }             // screen permission id
        public string? permission { get; set; }             //  'Read', 'Write', 'Read/Write' (nullable for partial update)
        public bool? is_deleted { get; set; }               //  Optional soft delete flag for this mapping

        public string? updated_by { get; set; }             //  Audit - updater
        public DateTimeOffset? updated_at { get; set; }     //  Audit - updated timestamp

        public int? version { get; set; }                    //  Optional optimistic concurrency token
    }
    // ===========================
    //  DTO for delete/soft-delete operations at the group level
    // ===========================
    public class UserGroupConfigurationDeleteModel
    {
        public long user_group_id { get; set; }          //  Target group id
        public bool? is_deleted { get; set; }            //  Soft delete flag to set
        public string? updated_by { get; set; }         //  Audit - updater
        public DateTimeOffset? updated_at { get; set; } //  Audit - updated timestamp
    }

}
