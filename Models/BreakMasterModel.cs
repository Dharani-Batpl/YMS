namespace YardManagementApplication.Models
{
    // =====================================================
    //  Represents the full Break Time entity with metadata
    // =====================================================
    public class BreakTimeModel
    {
        public long? Break_id { get; set; }                //  Unique identifier for the break
        public string? Break_name { get; set; }            //  Name/title of the break
        public string? Break_description { get; set; }     //  Optional description of the break
        public long? Status_id { get; set; }              //  Status ID (maps to status master)
        public string? Status_name { get; set; }          //  Readable status name
        public bool Is_deleted { get; set; }             //  Logical delete flag
        public string? Created_by { get; set; }           //  User who created this record
        public DateTimeOffset? Created_at { get; set; }   //  Timestamp of creation
        public string? Updated_by { get; set; }           //  User who last updated this record
        public DateTimeOffset? Updated_at { get; set; }   //  Timestamp of last update
        public int? Version { get; set; }                 //  Record version for concurrency control
    }

    // =====================================================
    //  Represents partial update (PUT) model for Break Time
    // =====================================================
    public class BreakTimeUpdateModel
    {
        public long Break_id { get; set; }                //  Unique identifier for the break
        public string? Break_name { get; set; }           //  Optional name field for update
        public string? Break_description { get; set; }    //  Optional description field for update
        public long? Status_id { get; set; }              //  Optional status ID for update
        public string? Status_name { get; set; }          //  Optional status name for update
        public string? Updated_by { get; set; }           //  Optional user performing the update
        public DateTimeOffset? Updated_at { get; set; }   //  Optional timestamp of update
    }

    // =====================================================
    //  Represents delete (soft delete) model for Break Time
    // =====================================================
    public class BreakMasterDeleteModel
    {
        public long Break_id { get; set; }                //  Unique identifier for the break
        public bool Is_deleted { get; set; }              //  Delete flag
        public string? Updated_by { get; set; }           //  Optional user performing deletion
        public DateTimeOffset? Updated_at { get; set; }   //  Timestamp of deletion
    }


}
