using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.Models
{
    public enum ApprovalStepTwo
    {
        No,
        Yes,
        PriorApproval
    }
    public class ProposalStepTwo
    {
        [Key]
        public int Id { get; set; }
        public decimal AnnualCoverageCeiling { get; set; }
        public int ParticipantsCount { get; set; }
        public decimal Price { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        //public int AvgAge { get; set; }
        public ApprovalStepTwo ChecksInsideHospital { get; set; }
        public ApprovalStepTwo PhysicalTherapyInsideHospital { get; set; }
        public ApprovalStepTwo OutsideClincInsideHospital { get; set; }
        public ApprovalStepTwo DentalServicesInsideHospital { get; set; }
        public string EmployeesDataFileName { get; set; }
        public int ClassCode { get; set; }

        public int Accidents { get; set; }
        public int Death { get; set; }
        //----------relations--------------//
        public int AgeAvgId { get; set; }
        public AgeAvg AgeAvg { get; set; }
        public int CardColorId { get; set; }
        public CardColor CardColor { get; set; }
        public int ResidenceDegreeId { get; set; }
        public ResidenceDegree ResidenceDegree { get; set; }
        public int MedicalNetworkId { get; set; }
        public MedicalNetwork MedicalNetwork { get; set; }
        public int ProposalMainId { get; set; }
        public ProposalMain ProposalMain { get; set; }
    }
}