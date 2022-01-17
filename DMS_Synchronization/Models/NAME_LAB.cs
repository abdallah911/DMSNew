namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Name_Lab
    {
        public long SQE_NO { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SQE_LAB { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long NAME_ID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string NAME_NAME { get; set; }

        [StringLength(250)]
        public string ADDRS { get; set; }
    }
}
