namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RoshitaDetail
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string MedicienCode { get; set; }

        [Required]
        [StringLength(150)]
        public string MedicienName { get; set; }

        public int Dose { get; set; }

        public int Duration { get; set; }

        public int? TotalDuration { get; set; }

        public int? TotalUnits { get; set; }

        public double? Amount { get; set; }

        public bool IsDealed { get; set; }

        public long RoshitaID { get; set; }

        [StringLength(50)]
        public string PaymentGroup { get; set; }

        public bool IsSync { get; set; }

        public DateTime SyncDate { get; set; }
        public string SyncBy { get; set; }

        //public virtual Roshita Roshita { get; set; }
    }
}
