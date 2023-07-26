using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasforComplianceSettlement
    {
        public int? ID { get; set; }
        public string REPORTINGOFFICE { get; set; }
        public string AUDITEENAME { get; set; }
        public string AUDITPERIOD { get; set; }
        public string PARANO { get; set; }
        public string GISTOFPARA { get; set; }
        public string AMOUNT { get; set; }
        public string REPLIEDBY { get; set; }
        public string VOL_I_II { get; set; }
        public string REPLY { get; set; }
        public DateTime REPLIEDDATE { get; set; }
        public DateTime LASTUPDATEDDATE { get; set; }
        public string REMARKS { get; set; }
        public string PARA_CATEGORY { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public int? EVIDENCE_ID { get; set; }
        public string REVIEWED_BY { get; set; }
        public string REVIEWER_REMARKS { get; set; }
        public string REVIEWER_RECOMMENDATION { get; set; }
        public string SUBMITTED { get; set; }
        public string REF_P { get; set; }
        public string AUDITEDBY { get; set; }
        public string C_STATUS { get; set; }

    }
}
