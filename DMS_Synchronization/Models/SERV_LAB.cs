namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Serv_Lab
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long LAB_CODE { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PR_CODE { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string PR_ANAME { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DMS_CODE { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SERV_CODE { get; set; }

        public long SERV_AMOUNT { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(50)]
        public string SERV_ANAME { get; set; }

        [Required]
        [StringLength(5)]
        public string GRUOP_TYPE { get; set; }

        public short LOOK { get; set; }

        public long GRUOP_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string GRUOP_NAME { get; set; }
    }
}
