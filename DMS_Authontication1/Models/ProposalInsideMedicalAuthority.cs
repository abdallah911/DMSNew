using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public class ProposalInsideMedicalAuthority
    {
        [Key]
        public int Id { get; set; }
        public bool HospitalsResidenceServiceLimitOption { get; set; }
        public bool HospitalsResidenceServicePercentageoption { get; set; }
        public decimal? HospitalsResidenceServiceLimit { get; set; }
        public decimal? HospitalsResidenceServicePercentage { get; set; }

        public bool OutsideClinicsLimitOption { get; set; }
        public bool OutsideClinicsPercentageoption { get; set; }
        public decimal? OutsideClinicsLimit { get; set; }
        public decimal? OutsideClinicsPercentage { get; set; }

        public bool ExaminationAndAnalysisLimitOption { get; set; }
        public bool ExaminationAndAnalysisPercentageoption { get; set; }
        public decimal? ExaminationAndAnalysisLimit { get; set; }
        public decimal? ExaminationAndAnalysisPercentage { get; set; }

        public bool PhysicalTherapyLimitOption { get; set; }
        public bool PhysicalTherapyPercentageoption { get; set; }
        public decimal? PhysicalTherapyLimit { get; set; }
        public decimal? PhysicalTherapyPercentage { get; set; }

        public bool DailyTherapyLimitOption { get; set; }
        public bool DailyTherapyPercentageoption { get; set; }
        public decimal? DailyTherapyLimit { get; set; }
        public decimal? DailyTherapyPercentage { get; set; }

        public bool ChronicTherapyLimitOption { get; set; }
        public bool ChronicTherapyPercentageoption { get; set; }
        public decimal? ChronicTherapyLimit { get; set; }
        public decimal? ChronicTherapyPercentage { get; set; }

        public bool NatChildBirthLimitOption { get; set; }
        public bool NatChildBirthPercentageoption { get; set; }
        public decimal? NatChildBirthLimit { get; set; }
        public decimal? NatChildBirthPercentage { get; set; }

        public bool CaesChildBirthLimitOption { get; set; }
        public bool CaesChildBirthPercentageoption { get; set; }
        public decimal? CaesChildBirthLimit { get; set; }
        public decimal? CaesChildBirthPercentage { get; set; }

        public bool LegalAbortionLimitOption { get; set; }
        public bool LegalAbortionPercentageoption { get; set; }
        public decimal? LegalAbortionLimit { get; set; }
        public decimal? LegalAbortionPercentage { get; set; }

        public bool PregFollowUpLimitOption { get; set; }
        public bool PregFollowUpPercentageoption { get; set; }
        public decimal? PregFollowUpLimit { get; set; }
        public decimal? PregFollowUpPercentage { get; set; }

        public bool AdvancedDentalServiceLimitOption { get; set; }
        public bool AdvancedDentalServicePercentageoption { get; set; }
        public decimal? AdvancedDentalServiceLimit { get; set; }
        public decimal? AdvancedDentalServicePercentage { get; set; }

        public bool BasicDentalServiceLimitOption { get; set; }
        public bool BasicDentalServicePercentageoption { get; set; }
        public decimal? BasicDentalServiceLimit { get; set; }
        public decimal? BasicDentalServicePercentage { get; set; }

        public bool OpticsLimitOption { get; set; }
        public bool OpticsPercentageoption { get; set; }
        public decimal? OpticsLimit { get; set; }
        public decimal? OpticsPercentage { get; set; }

        public int IntensiveCareDaysCount { get; set; }
        public int DailyRoshitasCountPerMonth { get; set; }
        public bool CoronaVaccineCoverage { get; set; }
        public int ClassCode { get; set; }

        //----------relations------------//
        public int ProposalMainId { get; set; }
        public ProposalMain ProposalMain { get; set; }
    }
}