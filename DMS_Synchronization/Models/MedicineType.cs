namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MedicineType")]
    public partial class MedicineType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string MedicineTypeName { get; set; }
    }
}
