using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class DoasgeViewModel
    {
        [Required]
        public int DosageId { get; set; }
        [Required]
        public string DosageFormName { get; set; }
        [Required]
        public int S_ID_ID { get; set; }
        [Required]
        public int S_Coun { get; set; }

        public List<DosageForm> DosageFormList { get; set; }

    }
}