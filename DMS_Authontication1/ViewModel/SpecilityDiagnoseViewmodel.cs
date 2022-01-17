using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class SpecilityDiagnoseViewmodel
    {
        public int SPEC_ID { get; set; }
        public int Id { get; set; }
        [Required]
        public string DIAG_ANAME { get; set; }
        [Required]
        public string DIAG_ENAME { get; set; }
        public string ACTIVE { get; set; }
        public List<Diagnosi> DiagnosisList { get; set; }
    }
}