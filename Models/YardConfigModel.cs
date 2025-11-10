using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YardManagementApplication.Models
{
    [Table("yard", Schema = "yard")]
    public class YardModel
    {
        [Key]
        [Column("yard_id")]
        public long Yard_id { get; set; }

        [Column("plant_id")]
        public long Plant_id { get; set; }

        [Column("plant_name")]
        public string Plant_name { get; set; } = string.Empty;

        [Column("yard_code")]
        public string Yard_code { get; set; } = string.Empty;

        [Column("yard_name")]
        public string Yard_name { get; set; } = string.Empty;

        [Column("yard_type")]
        public string? Yard_type { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("coordinates", TypeName = "nvarchar(max)")]
        public string? Coordinates { get; set; }

        [Column("area_acres")]
        public decimal? Area_acres { get; set; }

        [Column("capacity_cnt")]
        public int? Capacity_cnt { get; set; }

        [Column("status_id")]
        public long Status_id { get; set; }

        [Column("status_name")]
        public string Status_name { get; set; } = string.Empty;

        [Column("is_deleted")]
        public bool Is_deleted { get; set; } = false;

        [Column("created_by")]
        public string Created_by { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTimeOffset? Created_at { get; set; } = DateTimeOffset.Now;

        [Column("updated_by")]
        public string? Updated_by { get; set; }

        [Column("updated_at")]
        public DateTimeOffset? Updated_at { get; set; }

        [Column("version")]
        public int Version { get; set; } = 1;
    }



}
