using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class AgeAvg
    {
        public int Id { get; set; }
        public int Age { get; set; }
        //------relations-------//
        public ICollection<ProposalStepTwo> ProposalStepTwos { get; set; }
    }
}