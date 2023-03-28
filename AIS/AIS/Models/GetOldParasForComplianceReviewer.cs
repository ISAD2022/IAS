using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasForComplianceReviewer
    {
        public string AUDITEENAME { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string PARA_NO { get; set; }
        public string GISTOFPARA { get; set; }
        public double AMOUNT_INVOLVED { get; set; }
        public string REF_P { get; set; }
        public string ID { get; set; }

        /* public string LASTUPDATEDBY { get; set; }
         public int? EVIDENCE_ID { get; set; }
         public string SUBMITTED { get; set; }
         public int? PARA_NO { get; set; }
         public int? ENTITY_ID { get; set; }

         public string AUDITEDBY { get; set; }
         public string C_STATUS { get; set; }
        */









    }
}
