namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DoctorWorkPlace
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeId { get; set; }

        [Required]
        public string WorkPlace { get; set; }

        public string Address { get; set; }

        public bool PermenantExpenditure { get; set; }
    }
}
