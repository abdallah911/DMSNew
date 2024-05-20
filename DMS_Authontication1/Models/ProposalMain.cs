using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public enum ProposalCoverage{
        Previous,
        Later,
        PreviousAndLater
    }
    public class ProposalMain
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string CompanyEnglishName { get; set; }
        public string CompanyArabicName { get; set; }
        public string CompanyTelePhone { get; set; }
        public string AdministratorName { get; set; }
        public string AdministratorPhone { get; set; }
        public DateTime ContractDate { get; set; }
        public int DoctorVisitsCount { get; set; }
        public decimal DoctorVisitsDuration { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public bool InsuranceExists { get; set; }
        public int MedicationPoolVal { get; set; }
        public decimal MedicationPoolPercentage { get; set; }
        public int PrexPoolVal { get; set; }
        public decimal PrexPoolPercentage { get; set; }
        public ProposalCoverage PrexPoolCoverage { get; set; }
        public int ExceptionPoolVal { get; set; }
        public decimal ExceptionPoolPercentage { get; set; }
        public ProposalCoverage ExceptionPoolCoverage { get; set; }
        public int ChronicPoolVal { get; set; }
        public decimal ChronicPoolPercentage { get; set; }
        public ProposalCoverage ChronicPoolCoverage { get; set; }
        public int CriticalPoolVal { get; set; }
        public decimal CriticalPoolPercentage { get; set; }
        public ProposalCoverage CriticalPoolCoverage { get; set; }

        public string ConsumptionFileName { get; set; }

        [Range(1, 20, ErrorMessage = "Value must be at least 1 and less than 20")]
        public int CategoriesCount { get; set; }


        //------------relations--------------//
        //-----Broker------//
        public bool IsBroker { get; set; }
        public Broker Broker { get; set; }
        public int? BrokerId { get; set; }
        public decimal? BrokerPercentage { get; set; }
        //------User-------//
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        //-----ComapnyActivity---//
        public CompanyActivity CompanyActivity { get; set; }
        public int CompanyActivityId { get; set; }
        //-----Area---//
        public Area Area { get; set; }
        public int AreaId { get; set; }
        //---------step two-----------//
        public ICollection<ProposalStepTwo> ProposalStepTwos { get; set; }
        //-----------step three----------//
        public ICollection<ProposalInsideMedicalAuthority> ProposalInsiceMedicalAuthorities { get; set; }

        //---------step three--------//

        public ICollection<ProposalOutsideMedicalAuthority> ProposalOutsideMedicalAuthorities { get; set; }

    }
}