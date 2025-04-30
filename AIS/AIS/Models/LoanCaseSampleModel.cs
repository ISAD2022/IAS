using System;

namespace AIS.Models
    {
    public class LoanCaseSampleModel
        {

        public string TYPE { get; set; }
        public string SCHEME { get; set; }
        public string L_PURPOSE { get; set; }
        public string LC_NO { get; set; }
        public string CNIC { get; set; }
        public string CUSTOMERNAME { get; set; }
        public DateTime APP_DATE { get; set; }
        public DateTime DISB_DATE { get; set; }
        public decimal DEV_AMOUNT { get; set; }
        public decimal OUTSTANDING { get; set; }
        public string LOAN_DISB_ID { get; set; }


        }
    }
