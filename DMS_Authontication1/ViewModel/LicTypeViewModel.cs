using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class LicTypeViewModel
    {
        [Required]
        [Display(Name = "License Name")]
        public string licType { get; set; }
        public List<LicenseType> Licenses { get; set; }
    }
}