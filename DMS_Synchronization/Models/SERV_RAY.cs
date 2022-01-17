namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Serv_Ray
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
        [StringLength(250)]
        public string PR_ANAME { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long DMS_CODE { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SERV_CODE { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SERV_AMOUNT { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(250)]
        public string SERV_ANAME { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(5)]
        public string GRUOP_TYPE { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short LOOK { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long GRUOP_ID { get; set; }

        [Key]
        [Column(Order = 10)]
        [StringLength(150)]
        public string GRUOP_NAME { get; set; }
    }
}
