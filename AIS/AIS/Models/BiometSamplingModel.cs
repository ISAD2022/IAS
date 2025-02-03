using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class BiometSamplingModel
        {
        public int ID { get; set; }
        public int EngId { get; set; }
        public string AcNo { get; set; }           
        public string AcTitle { get; set; }        
        public string CustName { get; set; }        
        public string Dob { get; set; }          
        public string Cell { get; set; }            
        public string Cnic { get; set; }            
        public string CnicExpiry { get; set; }    
        public string AcType { get; set; }          
        public string AcCat { get; set; }           
        public string AcOpeningDate { get; set; } 
        public string BmVeriDate { get; set; }   
        public string LastTransaction { get; set; }
        public string BmVerified { get; set; }       
        public string Observation { get; set; }    
        public string CheckBy { get; set; }        
        public string CheckedOn { get; set; }    
        public string NName { get; set; }          
        public string FName { get; set; }          
        public string NDob { get; set; }         
        public string NCell { get; set; }          
        public string NExpiry { get; set; }

        }
}
