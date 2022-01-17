namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class MedicinesDiagnosi
    {
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        public string MedicineId { get; set; }

        public int DiagnoiseId { get; set; }

        public virtual Diagnosis Diagnosis { get; set; }

        public virtual MedicineData MedicineData { get; set; }
    }
}
