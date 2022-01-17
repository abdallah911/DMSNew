namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LicenseType")]
    public partial class LicenseType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string LicenseName { get; set; }
    }
}
