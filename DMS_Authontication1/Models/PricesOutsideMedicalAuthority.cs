using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class PricesOutsideMedicalAuthority
    {
        public int Id { get; set; }
        public string Price { get; set; }
        //-------relations-------//
        public ICollection<ProposalOutsideMedicalAuthority> ProposalOutsideMedicalAuthorities { get; set; } = new List<ProposalOutsideMedicalAuthority>();

    }
}