using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "The field is required.")]
        public string Name { get; set; }
        public Nullable<DateTime> CreatedOn { get; set; } = DateTime.Now;

        //----------relations------------//

    }
}