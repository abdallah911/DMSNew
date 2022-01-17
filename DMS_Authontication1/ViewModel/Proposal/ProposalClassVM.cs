using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.Proposal
{
    public class ProposalClassVM
    {
        public ProposalClass ProposalClass { get; set; }
        public ProposalBenefitsVM proposalBenefits { get; set; }
    }
}