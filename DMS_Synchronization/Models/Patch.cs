namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Patchs")]
    public partial class Patch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Patch()
        {
            Roshitas = new HashSet<Roshita>();
        }

        public int Id { get; set; }

        public int? AmountOfClaimSubmitted { get; set; }

        [StringLength(150)]
        public string ServiceProvider { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReceivedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ClaimDate { get; set; }

        public int? ManualCount { get; set; }

        public int? SystemCount { get; set; }

        public int? ManualAmount { get; set; }

        public int? SystemAmount { get; set; }

        [StringLength(50)]
        public string ClaimType { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(50)]
        public string Groups { get; set; }

        public int? ServicesOfTheMonth { get; set; }

        [Column(TypeName = "date")]
        public DateTime? AcceptingDate { get; set; }

        public int? PatchId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartFrom { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndAt { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Roshita> Roshitas { get; set; }
    }
}
