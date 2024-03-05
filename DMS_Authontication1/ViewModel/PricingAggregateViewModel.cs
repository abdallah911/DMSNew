using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class PricingAggregateViewModel
    {
        public ProposalMain ProposalMain { get; set; }
        public List<ProposalStepTwo> ProposalStepTwo { get; set; }
        public List<ProposalInsideMedicalAuthority> ProposalInsideMedicalAuthorities { get; set; }
        public List<ProposalOutsideMedicalAuthority> ProposalOutsideMedicalAuthorities { get; set; }
    }
}