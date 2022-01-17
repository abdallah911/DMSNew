namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int USER_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string USER_NAME { get; set; }

        [Required]
        [StringLength(15)]
        public string USER_PWD { get; set; }

        public bool USER_PWD_NEW { get; set; }

        public DateTime LOG_DATE { get; set; }

        public int LOK_1 { get; set; }

        public int USER_CO { get; set; }

        public int USER_ID_ID { get; set; }

        public int USER_ID_ID_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string USER_N { get; set; }

        public int KIND_NO { get; set; }

        [StringLength(500)]
        public string ADDRS { get; set; }

        public bool VALIDIT_USER { get; set; }

        public int GOVER_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string GOVER_NAME { get; set; }

        public bool DELEV { get; set; }

        [StringLength(25)]
        public string TEL { get; set; }

        [StringLength(25)]
        public string MOB { get; set; }

        [StringLength(50)]
        public string ERYA { get; set; }

        public virtual Governate Governate { get; set; }

        public virtual Serv_Providers Serv_Providers { get; set; }
    }
}
