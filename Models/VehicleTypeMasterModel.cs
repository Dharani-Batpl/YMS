namespace YardManagementApplication.Models
{  // =====================================================
    //  Represents the full vehicle type entity with metadata
    // =====================================================
    public class VehicleTypeModel
    {
        public long? Vehicle_type_id { get; set; }          // Unique identifier for the vehicle type
        public string? Vehicle_type_code { get; set; }      // Code representing the vehicle type
        public string? Vehicle_type_name { get; set; }      // Name of the vehicle type
        public int? Brand_id { get; set; }                 // Identifier of the Brand Type
        public string? Brand_name { get; set; }            // Name of the Brand Type
        public long Category_type_id { get; set; }          // Identifier for the category type
        public string? Category_type_name { get; set; }     // Name of the category type
        public long? Fuel_type_id { get; set; }             // Identifier for the fuel type
        public string? Fuel_type_name { get; set; }         // Name of the fuel type
        public long? Status_id { get; set; }                // Identifier for the status
        public string? Status_name { get; set; }            // Name of the status
        public string? Description { get; set; }            // Description of the vehicle type
        public bool? Is_deleted { get; set; }               // Flag indicating soft deletion
        public string? Created_by { get; set; }              // User who created the record
        public DateTimeOffset? Created_at { get; set; }      // Timestamp when record was created
        public string? Updated_by { get; set; }             // User who last updated the record
        public DateTimeOffset? Updated_at { get; set; }     // Timestamp when record was last updated
    }

    // =====================================================
    //  Represents partial update (PUT) model for vehicle type
    // =====================================================
    public class VehicleTypeUpdateModel
    {
        public long? Vehicle_type_id { get; set; }          // Unique identifier for the vehicle type
        public string? Vehicle_type_code { get; set; }      // Code representing the vehicle type
        public string? Vehicle_type_name { get; set; }      // Name of the vehicle type
        public int? Brand_id { get; set; }                 // Identifier of the Brand Type
        public string? Brand_name { get; set; }            // Name of the Brand Type
        public long Category_type_id { get; set; }          // Identifier for the category type
        public string? Category_type_name { get; set; }     // Name of the category type
        public long? Fuel_type_id { get; set; }             // Identifier for the fuel type
        public string? Fuel_type_name { get; set; }         // Name of the fuel type
        public long? Status_id { get; set; }                // Identifier for the status
        public string? Status_name { get; set; }            // Name of the status
        public string? Description { get; set; }            // Description of the vehicle type
        public bool? Is_deleted { get; set; }               // Flag indicating soft deletion
        public string Created_by { get; set; }              // User who created the record
        public DateTimeOffset Created_at { get; set; }      // Timestamp when record was created
        public string? Updated_by { get; set; }             // User who last updated the record
        public DateTimeOffset? Updated_at { get; set; }     // Timestamp when record was last updated
    }


    // =====================================================
    //  Represents delete (soft delete) model for vehicle type
    // =====================================================
    public class VehicleTypeMasterDeleteModel
    {
        public long Vehicle_type_id { get; set; }           // Unique identifier for the vehicle type
        public bool Is_deleted { get; set; }                // Flag indicating soft deletion
        public string? Updated_by { get; set; }             // User who deleted/updated the record
        public DateTimeOffset? Updated_at { get; set; }     // Timestamp when record was deleted/updated
    }
}
