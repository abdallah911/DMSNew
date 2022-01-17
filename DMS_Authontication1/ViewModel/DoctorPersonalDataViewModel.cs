using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class DoctorPersonalDataViewModel
    {
        public string UserId { get; set; }
        public string TypeId { get; set; }
        public string WorkType { get; set; }
        public string Speciality { get; set; }
        public string Image { get; set; }
        public string StampImage { get; set; }
        public Nullable<System.DateTime> BirthData { get; set; }
        public string Religious { get; set; }
        public Nullable<bool> Gender { get; set; }
        public string Nationality { get; set; }
        public string SocialStatus { get; set; }
        public Nullable<int> KidsNumbers { get; set; }
        public string MalitaryStatus { get; set; }
        public Nullable<System.DateTime> DataOfEndMalitary { get; set; }
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        public Nullable<System.DateTime> IdStartData { get; set; }
        public Nullable<System.DateTime> IdExpiredData { get; set; }
        public string IdPlace { get; set; }
        public string BloodType { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public HttpPostedFileBase StampImageFile { get; set; }
       // public virtual AspNetUser AspNetUser { get; set; }
    }
}