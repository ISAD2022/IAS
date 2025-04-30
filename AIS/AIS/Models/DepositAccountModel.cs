using System;

namespace AIS.Models
    {
    public class DepositAccountModel
        {
        public string BRANCH_NAME { get; set; }
        public double ACC_NUMBER { get; set; }
        public string ACCOUNTCATEGORY { get; set; }
        public DateTime OPENINGDATE { get; set; }
        public double CNIC { get; set; }
        public string TITLE { get; set; }
        public string CUSTOMERNAME { get; set; }
        public string BMVS_VERIFIED { get; set; }


        public string ACCOUNTSTATUS { get; set; }

        public DateTime LASTTRANSACTIONDATE { get; set; }

        public DateTime CNICEXPIRYDATE { get; set; }

        }
    }
