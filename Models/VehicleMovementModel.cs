namespace YardManagementApplication.Models
{
    public class VehicleMovementModel
    {
        public long VehicleMovementId { get; set; }
        public string VinSerialNo { get; set; } = string.Empty;
        public string? ProductionOrderNo { get; set; }
        public string? ProductDescription { get; set; }
        public long ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public DateTime DateOfProduction { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public long? ColorId { get; set; }
        public string? ColorName { get; set; }
        public long BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? BatchLotNumber { get; set; }
        public string? LineId { get; set; }
        public long? OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public int QualityStatus { get; set; }
        public int TransitStatus { get; set; }
        public string? TransitStatusName { get; set; }
        public string? CertificateNo { get; set; }
        public string? VehicleTypeName { get; set; }
        public string? Priority { get; set; }
        public string? Orgin { get; set; }
        public string? OrginSlot { get; set; }
        public string? Destination { get; set; }
        public string? DestinationSlot { get; set; }
        public DateTime? ScanStartTime { get; set; }
        public DateTime CompletionAt { get; set; }
        public int? PlannedTravelTime { get; set; }
        public int? ActualTravelTime { get; set; }
        public int? SlaTime { get; set; }
        public string? DeliveryNo { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public int Version { get; set; }
    }

    public record usp_vehicle_assignment
    {
       
        public string i_vin_serial_no { get; set; } 
        public int i_operator_id { get; set; }
        public string? i_operator_name { get; set; }
        public string? i_supervisor_name { get; set; }
        public string? i_origin { get; set; }
        public string? i_origin_slot { get; set; }
        public string? i_destination { get; set; }
        public string? i_destination_slot { get; set; }
        public string? i_priority { get; set; }

    }

    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int AccessTokenExpiryMinutes { get; set; }
        public int RefreshTokenExpiryDays { get; set; }
    }

    /// <summary>
    /// Represents a row from [ops].[vw_vehicle_movement_summary]
    /// Used for the top-level status cards (Assigned, In Progress, Completed, At Risk).
    /// </summary>
    public class VehicleMovementSummary
    {
        // e.g., "Assigned", "In Progress"
        public string status_group { get; set; }

        // The count of vehicles in that group
        public int total_vehicles { get; set; }
    }

    /// <summary>
    /// Represents a row from [ops].[vw_operator_status_overview]
    /// Used for the driver/operator status overview cards.
    /// </summary>
    public class OperatorStatusOverview
    {
        // Operator Identifier
        public int driver_id { get; set; }

        // Operator's Name
        public string driver_name { get; set; }

        // Total tasks ever assigned to this driver (within the lookback window)
        public int total_tasks { get; set; }

        // SUM(CASE WHEN vd.status_group IN ('Assigned', 'In Progress') THEN 1 ELSE 0 END)
        public int active_picks { get; set; }

        // SUM(CASE WHEN vd.status_group = 'Completed' AND CAST(vd.completion_at AS DATE) = CAST(GETDATE() AS DATE) THEN 1 ELSE 0 END)
        public int completed_today { get; set; }

        // MAX(vd.destination)
        public string last_location { get; set; }

        // MAX(vd.completion_at)
        public DateTime? last_activity_time { get; set; }

        // DATEDIFF(MINUTE, MAX(vd.completion_at), SYSDATETIME())
        public int minutes_since_last_activity { get; set; }

        // 'Busy' or 'Available'
        public string availability_status { get; set; }
    }

    /// <summary>
    /// Represents a row from [ops].[vw_live_picklist_orders]
    /// Used for the detail grid showing active vehicle movements.
    /// </summary>
    public class LivePicklistOrder
    {
        // vm.vehicle_movement_id
        public int picklist_id { get; set; }

        // vm.vin_serial_no
        public string vin { get; set; }

        // CONCAT(vm.origin, ' → ', vm.destination)
        public string route { get; set; }

        // sg.status_group (e.g., "Assigned", "In Progress", "At Risk")
        public string status { get; set; }

        // DATEADD(MINUTE, vm.sla_time, vm.scan_start_time)
        public DateTime? sla_due { get; set; }

        // DATEDIFF(MINUTE, vm.scan_start_time, SYSDATETIME())
        public int elapsed_minutes { get; set; }

        // vm.driver_name
        public string oprator { get; set; }

        // sm.status_name
        public string detailed_status { get; set; }

        // vm.completion_at (Nullable, as it's often NULL for non-completed items)
        public DateTime? completion_at { get; set; }
    }
}
