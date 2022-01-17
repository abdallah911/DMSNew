using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.Proposal
{
    public class ProposalBasicDataDetalisVM
    {
        public ProposalBasicData proposalBasicData { get; set; }
        public ProposalRenewalSummary ProposalRenewalSummary { get; set; }
        public  ProposalComment ProposalComment { get; set; }



    }
}