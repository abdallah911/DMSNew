using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class ModulesPagesViewModel
    {
        public string ModuleName { get; set; }
        public string PageName { get; set; }
        public bool FullControl { get; set; }
        public bool Preview { get; set; }
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ActivationControl { get; set; }
        public int PageId { get; set; }
        public string RoleId { get; set; }
        public string UserId { get; set; }
    }
}