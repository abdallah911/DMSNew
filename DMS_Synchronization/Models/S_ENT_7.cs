namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class S_Ent_7
    {
       // public int Id { get; set; }

        public long? S_NO { get; set; }

        public long? C_COMP_ID { get; set; }

        public long? S_ID { get; set; }

        [StringLength(150)]
        public string S_NAME { get; set; }
    }
}
