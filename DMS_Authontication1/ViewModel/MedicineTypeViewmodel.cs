using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class MedicineTypeViewmodel
    {
        [Required]
        public string MedicineTypeName  { get; set; }
        public List<MedicineType> medtypelist { get; set; }
    }
}