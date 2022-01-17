namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Speciality
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int special_id { get; set; }

        [StringLength(100)]
        public string special_Aname { get; set; }

        [StringLength(100)]
        public string special_Ename { get; set; }

        [StringLength(100)]
        public string special_notes { get; set; }

        [StringLength(1)]
        public string ACTIVE { get; set; }

        [StringLength(500)]
        public string NOTES { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CREATED_DATE { get; set; }

        [StringLength(100)]
        public string CREATED_BY { get; set; }

        [StringLength(100)]
        public string UPDATE_BY { get; set; }

        [Column(TypeName = "date")]
        public DateTime? UPDATE_DATE { get; set; }
    }
}
