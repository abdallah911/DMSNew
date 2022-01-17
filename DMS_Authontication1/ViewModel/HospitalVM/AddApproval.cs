using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HospitalVM
{
    public class AddApproval
    {
        [Required]
        public string Note { get; set; }

        [Required]
        public HttpPostedFileBase[] ImageFile { get; set; }
    }
}