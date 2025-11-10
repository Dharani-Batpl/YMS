using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YardManagementApplication.Models
{
    [Table("slot", Schema = "yard")]
    public class SlotModel
    {
        [Key]
        [Column("slot_id")]
        public long Slot_id { get; set; }

        [Column("lane_id")]
        public long Lane_id { get; set; }

        [Column("lane_name")]
        public string? Lane_name { get; set; }

        [Column("slot_code")]
        public string? Slot_code { get; set; }

        [Column("slot_name")]
        public string? Slot_name { get; set; }

        [Column("slot_type")]
        public string? Slot_type { get; set; } // Small / Large / Compact etc.

        [Column("coordinates")]
        public string? Coordinates { get; set; } // Polygon JSON

        [Column("is_occupied")]
        public bool? Is_occupied { get; set; }

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
