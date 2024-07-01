using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel
{
    public class RenewalInsideMedicalAuthorityViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a value for Hospitals Residence Service Limit Option.")]
        public bool HospitalsResidenceServiceLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Hospitals Residence Service Percentage Option.")]
        public bool HospitalsResidenceServicePercentageoption { get; set; }

        [ConditionalRequired(nameof(HospitalsResidenceServiceLimitOption), true, ErrorMessage = "Hospitals Residence Service Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Hospitals Residence Service Limit must be between 0 and the maximum value.")]
        public decimal? HospitalsResidenceServiceLimit { get; set; }

        [ConditionalRequired(nameof(HospitalsResidenceServicePercentageoption), true, ErrorMessage = "Hospitals Residence Service Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Hospitals Residence Service Percentage must be between 0 and 100.")]
        public decimal? HospitalsResidenceServicePercentage { get; set; }
        [Required(ErrorMessage = "Please provide a value for Outside Clinics Limit Option.")]
        public bool OutsideClinicsLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Outside Clinics Percentage Option.")]
        public bool OutsideClinicsPercentageoption { get; set; }

        [ConditionalRequired(nameof(OutsideClinicsLimitOption), true, ErrorMessage = "Outside Clinics Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Outside Clinics Limit must be between 0 and the maximum value.")]
        public decimal? OutsideClinicsLimit { get; set; }

        [ConditionalRequired(nameof(OutsideClinicsPercentageoption), true, ErrorMessage = "Outside Clinics Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Outside Clinics Percentage must be between 0 and 100.")]
        public decimal? OutsideClinicsPercentage { get; set; }

        [Required(ErrorMessage = "Please provide a value for Examination and Analysis Limit Option.")]
        public bool ExaminationAndAnalysisLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Examination and Analysis Percentage Option.")]
        public bool ExaminationAndAnalysisPercentageoption { get; set; }

        [ConditionalRequired(nameof(ExaminationAndAnalysisLimitOption), true, ErrorMessage = "Examination and Analysis Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Examination and Analysis Limit must be between 0 and the maximum value.")]
        public decimal? ExaminationAndAnalysisLimit { get; set; }

        [ConditionalRequired(nameof(ExaminationAndAnalysisPercentageoption), true, ErrorMessage = "Examination and Analysis Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Examination and Analysis Percentage must be between 0 and 100.")]
        public decimal? ExaminationAndAnalysisPercentage { get; set; }

        [Required(ErrorMessage = "Please provide a value for Physical Therapy Limit Option.")]
        public bool PhysicalTherapyLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Physical Therapy Percentage Option.")]
        public bool PhysicalTherapyPercentageoption { get; set; }

        [ConditionalRequired(nameof(PhysicalTherapyLimitOption), true, ErrorMessage = "Physical Therapy Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Physical Therapy Limit must be between 0 and the maximum value.")]
        public decimal? PhysicalTherapyLimit { get; set; }

        [ConditionalRequired(nameof(PhysicalTherapyPercentageoption), true, ErrorMessage = "Physical Therapy Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Physical Therapy Percentage must be between 0 and 100.")]
        public decimal? PhysicalTherapyPercentage { get; set; }

        [Required(ErrorMessage = "Please provide a value for Daily Therapy Limit Option.")]
        public bool DailyTherapyLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Daily Therapy Percentage Option.")]
        public bool DailyTherapyPercentageoption { get; set; }

        [ConditionalRequired(nameof(DailyTherapyLimitOption), true, ErrorMessage = "Daily Therapy Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Daily Therapy Limit must be between 0 and the maximum value.")]
        public decimal? DailyTherapyLimit { get; set; }

        [ConditionalRequired(nameof(DailyTherapyPercentageoption), true, ErrorMessage = "Daily Therapy Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Daily Therapy Percentage must be between 0 and 100.")]
        public decimal? DailyTherapyPercentage { get; set; }

        [Required(ErrorMessage = "Please provide a value for Chronic Therapy Limit Option.")]
        public bool ChronicTherapyLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Chronic Therapy Percentage Option.")]
        public bool ChronicTherapyPercentageoption { get; set; }

        [ConditionalRequired(nameof(ChronicTherapyLimitOption), true, ErrorMessage = "Chronic Therapy Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Chronic Therapy Limit must be between 0 and the maximum value.")]
        public decimal? ChronicTherapyLimit { get; set; }

        [ConditionalRequired(nameof(ChronicTherapyPercentageoption), true, ErrorMessage = "Chronic Therapy Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Chronic Therapy Percentage must be between 0 and 100.")]
        public decimal? ChronicTherapyPercentage { get; set; }

        [Required(ErrorMessage = "Please provide a value for Nat Child Birth Limit Option.")]
        public bool NatChildBirthLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Nat Child Birth Percentage Option.")]
        public bool NatChildBirthPercentageoption { get; set; }

        [ConditionalRequired(nameof(NatChildBirthLimitOption), true, ErrorMessage = "Nat Child Birth Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Nat Child Birth Limit must be at least 0.")]
        public decimal? NatChildBirthLimit { get; set; }

        [ConditionalRequired(nameof(NatChildBirthPercentageoption), true, ErrorMessage = "Nat Child Birth Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Nat Child Birth Percentage must be between 0 and 100.")]
        public decimal? NatChildBirthPercentage { get; set; }

        [Required(ErrorMessage = "Please provide a value for Caesarean Child Birth Limit Option.")]
        public bool CaesChildBirthLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Caesarean Child Birth Percentage Option.")]
        public bool CaesChildBirthPercentageoption { get; set; }

        [ConditionalRequired(nameof(CaesChildBirthLimitOption), true, ErrorMessage = "Caesarean Child Birth Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Caesarean Child Birth Limit must be at least 0.")]
        public decimal? CaesChildBirthLimit { get; set; }

        [ConditionalRequired(nameof(CaesChildBirthPercentageoption), true, ErrorMessage = "Caesarean Child Birth Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Caesarean Child Birth Percentage must be between 0 and 100.")]
        public decimal? CaesChildBirthPercentage { get; set; }

        [Required(ErrorMessage = "Please provide a value for Legal Abortion Limit Option.")]
        public bool LegalAbortionLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Legal Abortion Percentage Option.")]
        public bool LegalAbortionPercentageoption { get; set; }

        [ConditionalRequired(nameof(LegalAbortionLimitOption), true, ErrorMessage = "Legal Abortion Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Legal Abortion Limit must be at least 0.")]
        public decimal? LegalAbortionLimit { get; set; }

        [ConditionalRequired(nameof(LegalAbortionPercentageoption), true, ErrorMessage = "Legal Abortion Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Legal Abortion Percentage must be between 0 and 100.")]
        public decimal? LegalAbortionPercentage { get; set; }

        [Required(ErrorMessage = "Please provide a value for Pregnancy Follow-Up Limit Option.")]
        public bool PregFollowUpLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Pregnancy Follow-Up Percentage Option.")]
        public bool PregFollowUpPercentageoption { get; set; }

        [ConditionalRequired(nameof(PregFollowUpLimitOption), true, ErrorMessage = "Pregnancy Follow-Up Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Pregnancy Follow-Up Limit must be at least 0.")]
        public decimal? PregFollowUpLimit { get; set; }

        [ConditionalRequired(nameof(PregFollowUpPercentageoption), true, ErrorMessage = "Pregnancy Follow-Up Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Pregnancy Follow-Up Percentage must be between 0 and 100.")]
        public decimal? PregFollowUpPercentage { get; set; }


        [Required(ErrorMessage = "Please provide a value for Advanced Dental Service Limit Option.")]
        public bool AdvancedDentalServiceLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Advanced Dental Service Percentage Option.")]
        public bool AdvancedDentalServicePercentageoption { get; set; }

        [ConditionalRequired(nameof(AdvancedDentalServiceLimitOption), true, ErrorMessage = "Advanced Dental Service Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Advanced Dental Service Limit must be at least 0.")]
        public decimal? AdvancedDentalServiceLimit { get; set; }

        [ConditionalRequired(nameof(AdvancedDentalServicePercentageoption), true, ErrorMessage = "Advanced Dental Service Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Advanced Dental Service Percentage must be between 0 and 100.")]
        public decimal? AdvancedDentalServicePercentage { get; set; }


        [Required(ErrorMessage = "Please provide a value for Basic Dental Service Limit Option.")]
        public bool BasicDentalServiceLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Basic Dental Service Percentage Option.")]
        public bool BasicDentalServicePercentageoption { get; set; }

        [ConditionalRequired(nameof(BasicDentalServiceLimitOption), true, ErrorMessage = "Basic Dental Service Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Basic Dental Service Limit must be at least 0.")]
        public decimal? BasicDentalServiceLimit { get; set; }

        [ConditionalRequired(nameof(BasicDentalServicePercentageoption), true, ErrorMessage = "Basic Dental Service Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Basic Dental Service Percentage must be between 0 and 100.")]
        public decimal? BasicDentalServicePercentage { get; set; }


        [Required(ErrorMessage = "Please provide a value for Optics Limit Option.")]
        public bool OpticsLimitOption { get; set; }

        [Required(ErrorMessage = "Please provide a value for Optics Percentage Option.")]
        public bool OpticsPercentageoption { get; set; }

        [ConditionalRequired(nameof(OpticsLimitOption), true, ErrorMessage = "Optics Limit is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Optics Limit must be at least 0.")]
        public decimal? OpticsLimit { get; set; }

        [ConditionalRequired(nameof(OpticsPercentageoption), true, ErrorMessage = "Optics Percentage is required.")]
        [Range(0, 100, ErrorMessage = "Optics Percentage must be between 0 and 100.")]
        public decimal? OpticsPercentage { get; set; }


        [Required(ErrorMessage = "Please provide a value for Intensive Care Days Count.")]
        [Range(0, int.MaxValue, ErrorMessage = "Intensive Care Days Count must be at least 0.")]
        public int IntensiveCareDaysCount { get; set; }

        [Required(ErrorMessage = "Please provide a value for Daily Roshitas Count Per Month.")]
        [Range(0, int.MaxValue, ErrorMessage = "Daily Roshitas Count Per Month must be at least 0.")]
        public int DailyRoshitasCountPerMonth { get; set; }

        [Required(ErrorMessage = "Please provide a value for Corona Vaccine Coverage.")]
        public bool CoronaVaccineCoverage { get; set; }
        public string ClassCode { get; set; }
        //----------relations------------//
        [Required(ErrorMessage = "Please provide a value for Proposal Main Id.")]
        public int MainId { get; set; }

        public decimal? HospitalsResidenceServiceLimitOld { get; set; }
        public decimal? HospitalsResidenceServicePercentageOld { get; set; }
        public decimal? OutsideClinicsLimitOld { get; set; }
        public decimal? OutsideClinicsPercentageOld { get; set; }
        public decimal? ExaminationAndAnalysisLimitOld { get; set; }
        public decimal? ExaminationAndAnalysisPercentageOld { get; set; }
        public decimal? PhysicalTherapyLimitOld { get; set; }
        public decimal? PhysicalTherapyPercentageOld { get; set; }
        public decimal? DailyTherapyLimitOld { get; set; }
        public decimal? DailyTherapyPercentageOld { get; set; }
        public decimal? ChronicTherapyLimitOld { get; set; }
        public decimal? ChronicTherapyPercentageOld { get; set; }
        public decimal? NatChildBirthLimitOld { get; set; }
        public decimal? NatChildBirthPercentageOld { get; set; }
        public decimal? CaesChildBirthLimitOld { get; set; }
        public decimal? CaesChildBirthPercentageOld { get; set; }
        public decimal? LegalAbortionLimitOld { get; set; }
        public decimal? LegalAbortionPercentageOld { get; set; }
        public decimal? PregFollowUpLimitOld { get; set; }
        public decimal? PregFollowUpPercentageOld { get; set; }
        public decimal? AdvancedDentalServiceLimitOld { get; set; }
        public decimal? AdvancedDentalServicePercentageOld { get; set; }
        public decimal? BasicDentalServiceLimitOld { get; set; }
        public decimal? BasicDentalServicePercentageOld { get; set; }
        public decimal? OpticsLimitOld { get; set; }
        public decimal? OpticsPercentageOld { get; set; }
        public int IntensiveCareDaysCountOld { get; set; }
        public int DailyRoshitasCountPerMonthOld { get; set; }
        public bool CoronaVaccineCoverageOld { get; set; }

        public int CompId { get; set; }
        public int ContractNo { get; set; }
        public int CountClass { get; set; }
        public int typAction { get; set; }
        public int MainIdOld { get; set; }


        public int? NaturalBirthType { get; set; }
        public int? NaturalBirthCovaregType { get; set; }
        public int? CaesarBirthType { get; set; }
        public int? CaesarBirthCovaregType { get; set; }
        public int? FollowUpPregType { get; set; }
        public int? FollowUpPregCovaregType { get; set; }
        public int? AdvancedDentalType { get; set; }
        public int? AdvancedDentalCovaregType { get; set; }
        public int? BasicDentalType { get; set; }
        public int? BasicDentalCovaregType { get; set; }
        public int? LegalAbortionType { get; set; }
        public int? LegalAbortionCovaregType { get; set; }
        public int? DentalVisit { get; set; }
        public int? OpticalVisit { get; set; }
        public int? BirthVisit { get; set; }
        [Required(ErrorMessage = "Transport Ambulance Percent is required.")]
        [Range(0, 100, ErrorMessage = "Transport Ambulance Percent must be between 0 and 100")]
        public int TransportAmbulancePercent { get; set; }
        public List<int> AdvancDentalService { get; set; }
        public List<int> BascDentalService { get; set; }
        public string Notes { get; set; }

    }
}