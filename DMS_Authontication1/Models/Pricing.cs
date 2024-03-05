using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class Pricing
    {
        [Key]
        public int Id { get; set; }
        public string Prices { get; set; }
    
        public int CategoriesCount { get; set; }


        //------------relations--------------//
        //------User-------//
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        //--------Offer----------//
        public ProposalMain ProposalMain { get; set; }
        public int ProposalMainId { get; set; }

    }
}