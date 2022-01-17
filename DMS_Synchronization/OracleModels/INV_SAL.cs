using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Synchronization.OracleModels
{
    public class INV_SAL
    {
        public long INVT_NO { get; set; }
        public string INVT_NAM { get; set; }
        public int DOSE { get; set; }
        public int DURATION { get; set; }
        public int T_DURATION { get; set; }
        public int COUNT { get; set; }
        public bool KIND_EX { get; set; }
        public long INV_ID { get; set; }
        public string GRUOP_TYPE { get; set; }
        public double AMOUNT { get; set; }
    }
}