using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class CategoriesPricing
    {
        public int Id { get; set; }
        public string Prices { get; set; }//prices seperated by a comma
        //-------relations------//
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }
        public ProposalMain ProposalMain { get; set; }
        public int ProposalMainId { get; set; }
    }
}