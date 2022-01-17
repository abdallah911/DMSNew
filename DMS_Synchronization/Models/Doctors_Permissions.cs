namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Doctors_Permissions
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int PermissionId { get; set; }

        public bool Status { get; set; }

        [StringLength(50)]
        public string permissionType { get; set; }

        public virtual DailyPermission DailyPermission { get; set; }

        public virtual MonthlyPermission MonthlyPermission { get; set; }
    }
}
