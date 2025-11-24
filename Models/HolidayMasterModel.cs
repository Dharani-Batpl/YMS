namespace YardManagementApplication.Models
{
    // =====================================================
    //  Represents the full Holiday entity with metadata
    // =====================================================
    public class HolidayModel
    {

        public long Holiday_id { get; set; }            //  Unique identifier for the holiday
        public string Holiday_name { get; set; }        //  Name/title of the holiday
        public DateTime? Holiday_date { get; set; }      //  Date on which the holiday occurs 
        public long Holiday_type_id { get; set; }       //  Type ID (maps to holiday type master)        
        public string? Holiday_type_name { get; set; }   //  Readable holiday type name     
        public string? Description { get; set; }        //  Optional description of the holiday
      
        public bool Is_deleted { get; set; }            //  Logical delete flag 
        public string? Created_by { get; set; }          //  User who created this record
        public DateTimeOffset? Created_at { get; set; }  //  Timestamp of creation        
        public string? Updated_by { get; set; }         //  User who last updated this record 
        public DateTimeOffset? Updated_at { get; set; } //  Timestamp of last update
        public int? Version { get; set; }                //  Record version for concurrency control
    }

    // =====================================================
    //  Represents partial update (PATCH) model for Holiday
    // =====================================================
    public class HolidayUpdateModel
    {

        public long Holiday_id { get; set; }                //  Unique identifier for the holiday
        public string? Holiday_name { get; set; }           //  Optional name field for update
        public DateTime? Holiday_date { get; set; }         //  Optional date field for update
        public long? Holiday_type_id { get; set; }          //  Optional type ID for update
        public string? Holiday_type_name { get; set; }      //  Optional type name for update    
        public string? Description { get; set; }            //  Optional description for update 
       
        public bool? Is_deleted { get; set; }               //  Optional delete flag for update
        public string? Updated_by { get; set; }             //  Optional user performing the update
        public DateTimeOffset? Updated_at { get; set; }     //  Optional timestamp of update
    }

    // =====================================================
    //  Represents delete (soft delete) model for Holiday
    // =====================================================
    public class HolidayMasterDeleteModel
    {

        public long Holiday_id { get; set; }                //  Unique identifier for the holiday
        public bool Is_deleted { get; set; }                //  Delete flag
        public string? Updated_by { get; set; }             //  Optional user performing deletion
        public DateTimeOffset? Updated_at { get; set; }     //  Timestamp of deletion
    }

}
