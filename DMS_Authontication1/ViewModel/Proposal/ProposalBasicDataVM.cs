using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.Proposal
{
    public class ProposalBasicDataVM
    {
        public int Id { get; set; }
        public string TypeProposal { get; set; }
        public string InsuredBefore { get; set; }
        public string ExClient { get; set; }
        public string CompName { get; set; }
        public Nullable<int> CompId { get; set; }
        public Nullable<int> ContractNo { get; set; }
        public bool InsuredOtherComp { get; set; }
        public int NumProposal { get; set; }
        public string Conclusion { get; set; }
        public string status { get; set; }
        public string OtherCompName { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}