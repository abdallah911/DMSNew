using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DMS_Authontication1.Models
{
    public class ProposalNotificationApplicationUser
    {
        //----relations----/
        [Key]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser  { get; set; }
        public int ProposalNotificationId { get; set; }
        public ProposalNotification ProposalNotification { get; set; }
    }
}