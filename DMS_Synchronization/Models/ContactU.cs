namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ContactU
    {
        [Key]
        public int Contact_Id { get; set; }

        [StringLength(50)]
        public string Contact_Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Contact_Email { get; set; }

        [Required]
        [StringLength(250)]
        public string Contact_Message { get; set; }

        [Required]
        [StringLength(50)]
        public string Contact_Subject { get; set; }
    }
}
