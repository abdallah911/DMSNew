namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MedicineGroup")]
    public partial class MedicineGroup
    {
        [Key]
        [StringLength(50)]
        public string GroupId { get; set; }

        [Required]
        [StringLength(500)]
        public string GroupName { get; set; }

        [StringLength(50)]
        public string GroupType { get; set; }
    }
}
