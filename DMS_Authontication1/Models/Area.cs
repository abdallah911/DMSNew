using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //------relations-------//
        public ICollection<ProposalMain> ProposalMain { get; set; }
        
    }
}