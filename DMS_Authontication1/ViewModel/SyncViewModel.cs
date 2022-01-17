using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class SyncViewModel
    {
        [Display(Name ="Type Of Synchronize : ")]
        public string title { get; set; }

        [Required]
        public Type type { get; set; }
    }
    public enum Type
    {
        Push = 1,
        Pull = 2,
        Both = 3
    }
}