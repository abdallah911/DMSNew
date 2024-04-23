using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class BasicDentalService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //-----relations----//

        public ICollection<ProposalInsideMedicalAuthority> ProposalInsideMedicalAuthoritys { get; set; }

    }
}