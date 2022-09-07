using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class DepositAccountModel
    {        
        public string NAME { get; set; }
        public double ACC_NUMBER { get; set; }
        public string ACCOUNTCATEGORY { get; set; }
        public DateTime OPENINGDATE { get; set; }
        public double CNIC { get; set; }
        public string ACC_TITLE { get; set; }
        public string CUST_NAME { get; set; }
        public double OLDACCOUNTNO { get; set; }

        public string ACCOUNTSTATUS  { get; set; }
        
        public DateTime LASTTRANSACTIONDATE { get; set; }
        
        public DateTime CNICEXPIRYDATE { get; set; }

     
        public List<DepositAccountModel> D_A_D { get; set; }


    }
}
