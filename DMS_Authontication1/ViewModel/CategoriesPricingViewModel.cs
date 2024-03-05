using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class CategoriesPricingViewModel
    {
        public int Id { get; set; }
        public string Prices { get; set; }//prices seperated by a comma
        //-------relations------//
        public int UserId { get; set; }
        public int ProposalMainId { get; set; }
    }
}