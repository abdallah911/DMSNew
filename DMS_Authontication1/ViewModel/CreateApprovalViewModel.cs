using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DMS_Authontication1.ViewModel
{
    public class CreateApprovalViewModel
    {
         public string cod { get; set; }
        public Int32 comp { get; set; }
        public string crd { get; set; }
        public string fx { get; set; }
        [Required]
        public string emil { get; set; }
        public Int64 mob { get; set; }

        public string rcdat { get; set; }
        public string sedat { get; set; }
        public string apptyp { get; set; }
        public Int64 pvdnum { get; set; }
        public string rply { get; set; }
        public string medrply { get; set; }
        public string nts { get; set; }
        public double appval { get; set; }
        public double valaft { get; set; }
        public string srvtyp { get; set; }
        public string pvd { get; set; }
        public string rato { get; set; }
        public string mxamun { get; set; }
        public byte[] appimg { get; set; }
        public string contr { get; set; }
        public string mxamutcontr { get; set; }
        public string clss { get; set; }
        public string cretby { get; set; }
        public DateTime cretdat { get; set; }
        public string anam { get; set; }
        public string enam { get; set; }
        public DateTime birth { get; set; }
        public DateTime strtdat { get; set; }
        public DateTime enddat { get; set; }
        public double totcon { get; set; }
        public string pvdnam { get; set; }
        public Int32 sbcod { get; set; }
        public string cmpnam { get; set; }
        public Int64 digcod { get; set; }
        public string dignam { get; set; }
        public string flg { get; set; }
        public string flg2 { get; set; }
        public string chpcent { get; set; }
        public string rsnrecol { get; set; }
       public string vist { get; set; }
        public string Image { get; set; }
        // public HttpPostedFileBase ImageFile { get; set; }

        public HttpPostedFileWrapper ImageFile { get; set; }
        public string EditReason { get; set; }
    }
}