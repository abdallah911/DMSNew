using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class CardColor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        //--------relations-----//
        public ICollection<ProposalStepTwo> ProposalStepTwos { get; set; }
    }
}