using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YardManagementApplication.Models
{
    [Table("plant", Schema = "org")]
    public class PlantMasterModel
    {
        [Key]
        [Column("plant_id")]
        public long Plant_id { get; set; }

        [Column("enterprise_id")]
        public long Enterprise_id { get; set; }

        [Column("enterprise_name")]
        public string Enterprise_name { get; set; }

        [Column("plant_code")]
        public string Plant_code { get; set; }

        [Column("plant_name")]
        public string Plant_name { get; set; }

        [Column("status_id")]
        public long Status_id { get; set; }

        [Column("status_name")]
        public string Status_name { get; set; }

        [Column("is_deleted")]
        public bool Is_deleted { get; set; } = false;

        [Column("created_by")]
        public string Created_by { get; set; }

        [Column("created_at")]
        public DateTimeOffset Created_at { get; set; } = DateTimeOffset.Now;

        [Column("updated_by")]
        public string? Updated_by { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? Updated_at { get; set; }

        [Column("version")]
        public int Version { get; set; } = 1;

        [Column("coordinates", TypeName = "nvarchar(max)")]
        public string Coordinates { get; set; }

        [Column("total_area")]
        public decimal Total_area { get; set; }

        [Column("plant_address")]
        public string Plant_address { get; set; }
    }
}
