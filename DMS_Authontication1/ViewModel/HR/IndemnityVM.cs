using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class IndemnityVM
    {
        public string PhotoCardId { get; set; }
        public HttpPostedFileBase[] NationalIdPDF { get; set; }
       
        
    }
}