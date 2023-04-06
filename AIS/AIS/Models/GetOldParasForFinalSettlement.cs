using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasForFinalSettlement
    {
      
        public string REPORTINGOFFICE { get; set; } 
        public string REF_P { get; set; }
        public string AUDITEENAME { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string PARA_NO { get; set; }
        public string GISTOFPARA { get; set; }
        public decimal AMOUNT_INVOLVED { get; set; }
        public string REPLY { get; set; }
        public int? REPLIEDBY { get; set; }
        public string REPLIEDDATE { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public string LASTUPDATEDDATE { get; set; }
        public string REMARKS { get; set; }
        
        public string IMP_REMARKS { get; set; }
        public string SUBMITTED { get; set; }
        public string AUDITEDBY { get; set; }
        public int? ENTITY_ID { get; set; }
        public string C_STATUS { get; set; }
        public int? EVIDENCE_ID { get; set; }
        public int? ID { get; set; }



    }
}
