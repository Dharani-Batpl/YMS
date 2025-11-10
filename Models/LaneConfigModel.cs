using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YardManagementApplication.Models
{
    [Table("lane", Schema = "yard")]
    public class laneModel
    {
        [Key]
        [Column("lane_id")]
        public long Lane_id { get; set; }

        [Column("yard_id")]
        public long Yard_id { get; set; }
        

        [Column("yard_name")]
        public string? Yard_name { get; set; }

        [Column("lane_code")]
        public string? Lane_code { get; set; }

        [Column("lane_name")]
        public string? Lane_name { get; set; }

        [Column("lane_type")]
        public string? Lane_type { get; set; }   // Runway / Parking

        [Column("capacity_cnt")]
        public int? Capacity_cnt { get; set; }

        [Column("total_slots")]
        public int Total_slots { get; set; }

        [Column("occupied_slots")]
        public int Occupied_slots { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("status_id")]
        public long Status_id { get; set; }

        [Column("status_name")]
        public string? Status_name { get; set; }

        [Column("is_deleted")]
        public bool Is_deleted { get; set; } = false;

        [Column("created_by")]
        public string? Created_by { get; set; }

        [Column("created_at")]
        public DateTimeOffset? Created_at { get; set; }

        [Column("updated_by")]
        public string? Updated_by { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? Updated_at { get; set; }

        [Column("version")]
        public int? Version { get; set; }
    }
}
