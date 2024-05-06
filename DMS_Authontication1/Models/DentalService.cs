using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class DentalService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //-----relations----//

        public ICollection<ProposalOutsideMedicalAuthority> ProposalOutsideMedicalAuthoritys { get; set; }
    }
}