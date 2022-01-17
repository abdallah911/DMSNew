using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class Enum_RequestsViewModel
    {
        public long ID { get; set; }

        [DisplayName ("Company Name")]
        public string CompName { get; set; }

        [DisplayName("Description")]
        public string DESCRIPTION { get; set; }

        [DisplayName("Type")]
        public string TYPE { get; set; }

        [DisplayName("Image")]
        public string IMAGE { get; set; }

        [DisplayName("Card Id")]
        public string CARD_ID { get; set; }

        public Nullable<System.DateTime> REQ_DATE { get; set; }

        [DisplayName("Title")]
        public string TITEL { get; set; }

        [DisplayName("Employee Name")]
        public string EMP_ENAME { get; set; }

        [DisplayName("Request Type")]
        public string REQ_TYPE { get; set; }

        public Nullable<int> STATE { get; set; }

        [DisplayName("Notes")]
        public string NOTES { get; set; }
        public string REQUEST_TYP { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string REPLAYED_BY { get; set; }
        public Nullable<System.DateTime> REPLAYED_DATE { get; set; }
        public string APPROVAL_IMAGE { get; set; }
        public string SERV_PROVIDER { get; set; }
        public Nullable<double> APPROV_VALUE { get; set; }
        public string CONF_NOTS { get; set; }
        public string REPLAY { get; set; }
        public string CHANG_EMP_NAME { get; set; }
        public string PR_CODE { get; set; }
        public string BR_CODE { get; set; }

        [DisplayName("Provider Type")]
        public string TYP_ANAME { get; set; }

        [DisplayName("Provider Name")]
        public string PR_ENAME { get; set; }

        [DisplayName(" Send Email ")]
        [Required]
        [EmailAddress(ErrorMessage = "The email address is not valid")]
        public string MAIL_SEND { get; set; }
        public string APPROVALE_CODE { get; set; }

        [Required]
        public HttpPostedFileBase[] ImageFile { get; set; }
    }
}