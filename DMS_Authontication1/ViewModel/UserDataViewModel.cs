using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DMS_TEST.ViewModel
{
    public class UserDataViewModel
    {
        public string Id { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public string PhoneNumder1 { get; set; }
        public string Provider { get; set; }
        public string Address { get; set; }
        public string TypeId { get; set; }
    }
}