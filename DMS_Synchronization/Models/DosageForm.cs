namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DosageForm")]
    public partial class DosageForm
    {
        public int Id { get; set; }

        public int DosageId { get; set; }

        [Required]
        [StringLength(500)]
        public string DosageFormName { get; set; }

        public int? S_ID_ID { get; set; }

        public int? S_COUN { get; set; }
    }
}
