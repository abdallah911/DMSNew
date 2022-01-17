namespace DMS_Synchronization.Models
{
    using DMS_Synchronization.BaseEntity;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Roshita")]
    public partial class Roshita
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Roshita()
        {
            //RoshitaDetails = new HashSet<RoshitaDetail>();
        }

        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CardId { get; set; }

        //[Required]
        [StringLength(50)]
        public string Speciality { get; set; }

        [StringLength(50)]
        public string Diagnose1 { get; set; }

        [StringLength(50)]
        public string Diagnose2 { get; set; }

        [StringLength(50)]
        public string diagnose3 { get; set; }

        [Required]
        [StringLength(50)]
        public string RoshetaType { get; set; }

        public int Limit { get; set; }

        public double CompanyPercent { get; set; }

        public double OverInsurance { get; set; }

        public double TotalValue { get; set; }

        public double CompanyPayment { get; set; }

        public double PersonPayment { get; set; }

        public double Cash { get; set; }

        [StringLength(50)]
        public string Manager { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreatedDate { get; set; }

        public int? PatchId { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        public double? ClaimNumber { get; set; }

        public bool IsSync { get; set; }

        public DateTime SyncDate { get; set; }
        public string SyncBy { get; set; }
        public long Oracle_Id { get; set; }

        //public virtual Patch Patch { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        // public virtual ICollection<RoshitaDetail> RoshitaDetails { get; set; }
    }
}
