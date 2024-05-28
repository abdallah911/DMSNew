using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;


namespace DMS_TEST.ViewModel
{
    public class ProviderServicesPermissionsViewModal
    {
        public int Id { get; set; }
        public string ProviderName { get; set; }
        public int ServiceCode { get; set; }
        public bool IsActive { get; set; }
        public string CompId { get; set; }
        public string ClassCode { get; set; }
        public string CardId { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public string UserId { get; set; }
    }
}