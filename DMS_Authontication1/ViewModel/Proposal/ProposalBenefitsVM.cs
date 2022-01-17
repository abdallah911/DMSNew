using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.Proposal
{
    public class ProposalBenefitsVM
    {
        public List<ProposalBenefitVM> BenefitsUnUse { get; set; }
        public List<ProposalBenefitVM> BenefitsUse { get; set; }
        
    }
}