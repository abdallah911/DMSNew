using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class IndemnityMasterVm
    {
        public int Type { get; set; }
        public string CardId { get; set; }
        public string RelatedCardId { get; set; }
        //public string Related { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccount { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string NationalId { get; set; }
        public string Phone { get; set; }
        //public string[] FileCards { get; set; }
        //public string[] serCards { get; set; }
        public IndemnityVM[] IndemnityCardsImages { get; set; }
        public IndemnityServiceVm[] ServiceTest { get; set; }


    }
}