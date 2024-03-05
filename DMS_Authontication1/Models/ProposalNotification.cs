
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class ProposalNotification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }
        public bool IsRead { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsReminder { get; set; }
        public string SentTo { get; set; }
        public string DetailsUrl { get; set; }
        //Relations
        public ProposalMain ProposalMain { get; set; }
        public int ProposalMainId { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
    }
}