namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RoshitaPharmcyApproved")]
    public partial class RoshitaPharmcyApproved
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Pharmacy { get; set; }

        [StringLength(100)]
        public string Branch { get; set; }

        public long RoshitaId { get; set; }
    }
}
