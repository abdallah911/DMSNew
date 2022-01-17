namespace DMS_Synchronization.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class CardAcceptionReason
    {
        public int id { get; set; }

        public int AcceptionId { get; set; }

        public int AcceptionReasonsId { get; set; }

        public virtual Acception Acception { get; set; }

        public virtual AcceptionReason AcceptionReason { get; set; }
    }
}
