using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class HrChronicEmployee
    {
        public string CardId { get; set; }

        public string EmployeeFN { get; set; }

        public string EmployeeSN { get; set; }

        public string EmployeeTN { get; set; }

        public int? ProviderCode { get; set; }
        public string ProviderName { get; set; }

        public string PharmacyName { get; set; }

        public string GroupName { get; set; }

        public string UserName { get; set; }

        public string CreatedDate { get; set; }

        public long RoshitaId { get; set; }

    }
}