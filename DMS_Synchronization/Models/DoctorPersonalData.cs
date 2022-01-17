namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DoctorPersonalData
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string WorkType { get; set; }

        [Required]
        [StringLength(50)]
        public string Speciality { get; set; }

        [StringLength(50)]
        public string Image { get; set; }

        [StringLength(50)]
        public string StampImage { get; set; }

        public DateTime? BirthData { get; set; }

        [StringLength(50)]
        public string Religious { get; set; }

        public bool? Gender { get; set; }

        [StringLength(50)]
        public string Nationality { get; set; }

        [StringLength(50)]
        public string SocialStatus { get; set; }

        public int? KidsNumbers { get; set; }

        [StringLength(50)]
        public string MalitaryStatus { get; set; }

        public DateTime? DataOfEndMalitary { get; set; }

        [StringLength(50)]
        public string IdType { get; set; }

        [StringLength(50)]
        public string IdNumber { get; set; }

        public DateTime? IdStartData { get; set; }

        public DateTime? IdExpiredData { get; set; }

        [StringLength(50)]
        public string IdPlace { get; set; }

        [StringLength(50)]
        public string BloodType { get; set; }
    }
}
