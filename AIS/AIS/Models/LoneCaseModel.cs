using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class LoanCaseModel
    {        
        public int BRANCHID { get; set; }
        public double CNIC { get; set; }
        public int LOAN_CASE_NO { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string FATHERNAME { get; set; }
        public double DISBURSED_AMOUNT { get; set; }
        public double PRIN { get; set; }
        public double MARKUP { get; set; }

        public int GLSUBCODE  { get; set; }

        public double LOAN_DISB_ID { get; set; }

        public DateTime DISB_DATE { get; set; }
        public DateTime VALID_UNTIL { get; set; }

        public int DISB_STATUSID { get; set; }





    }
}
