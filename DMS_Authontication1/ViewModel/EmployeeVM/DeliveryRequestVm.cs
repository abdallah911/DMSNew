using DMS_TEST.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.EmployeeVM
{
    public class DeliveryRequestVm
    {
        public long EmployeeDataId { get; set; }

        [DisplayName("Governorate")]
        public int CountryId { get; set; }

        [DisplayName("Region")]
        public int RegionId { get; set; }

        public string NeighborHood { get; set; }


        [DisplayName("Street Name")]
        public string StreetName { get; set; }


        [DisplayName("Bulding No ")]
        public int BuldingNo { get; set; }


        [DisplayName("Flat No ")]
        public int FlatNo { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone must be 11 numbers")]
        public string Phone1 { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone must be 11 numbers")]
        public string Phone2 { get; set; }

        public string CardId { get; set; }


        [DisplayName("Provider")]
        public Nullable<int> ProviderId { get; set; }


        [DisplayName("Branch")]
        public Nullable<long> BranchId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }

        [DisplayName("Time To Deliverd")]
        public string Time { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }

        public List<ChronicViewModel> Medicines { get; set; }
    }
}