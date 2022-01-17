namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PrescriptionRoshitaDignosi
    {
        public int Id { get; set; }

        public int RositaId { get; set; }

        [Required]
        [StringLength(150)]
        public string DiagnoiseName { get; set; }
    }
}
