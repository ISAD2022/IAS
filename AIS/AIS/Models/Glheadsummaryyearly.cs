using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class Glheadsummaryyearlymodel
    {
        public int GLSUBCODE { get; set; }
        public int BRANCHID { get; set; }
        public string GLSUBNAME { get; set; }
    
        
     
        public double DEBIT_2021 { get; set; }
        public double CREDIT_2021 { get; set; }
        public double BALANCE_2021 { get; set; }


        public double DEBIT_2022 { get; set; }
        public double CREDIT_2022 { get; set; }
        public double BALANCE_2022 { get; set; }





    }
}
