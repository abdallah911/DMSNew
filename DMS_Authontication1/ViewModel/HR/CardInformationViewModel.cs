using DMS_Authontication1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DMS_Authontication1.ViewModel.HR
{
    public class CardInformationViewModel
    {   
        public string EmployeeName { get; set; }        
        public string BirthDate { get; set; }       
        public string Age { get; set; }       
        public string Gender { get; set; }       
        public string SpecificDate { get; set; }       
        public string StartDate { get; set; }      
        public string EndDate { get; set; }        
        public string MaxAmount { get; set; }     
        public string ClassName { get; set; }       
        public string HospitalDegree { get; set; }        
        public string ExceptionPayment { get; set; }       
        public string ExceptionOver { get; set; }             
        public string OldCard { get; set; }     
        public string NationalId { get; set; }        
        public string MedicalNetwork { get; set; }    
        public string CardColor { get; set; }       
        public string Mobile1 { get; set; }       
        public string Mobile2 { get; set; }

    }
}