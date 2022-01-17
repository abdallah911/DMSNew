using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class DoctorPermissionViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool Status { get; set; }
        public string permissionType { get; set; }

        public string PermissionName { get; set; }

        //public MonthlyPermission MonthlyPermission { get; set; }
    }
}