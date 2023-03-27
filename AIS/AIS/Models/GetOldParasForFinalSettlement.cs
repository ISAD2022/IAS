using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasForFinalSettlement
    {
        public int? ID { get; set; }
        public string REF_P { get; set; }
        public string REPLY { get; set; }
        public string REPLIEDBY { get; set; }
        public DateTime REPLIEDDATE { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public DateTime LASTUPDATEDDATE { get; set; }
        public string REMARKS { get; set; }
        public string SUBMITTED { get; set; }
        public string AUDITEDBY { get; set; }
        public int? ENTITY_ID { get; set; }
        public int? EVIDENCE_ID { get; set; }
        public string REVIEWED_BY { get; set; }
        public string REVIEWER_REMARKS { get; set; }
        public string REVIEWER_RECOMMENDATION { get; set; }
        public string IMP_REMARKS { get; set; }
        public string IMP_OFFICER{ get; set; }
        public DateTime REVIEWDDATE { get; set; }
        public string INCHARGE { get; set; }
        public string STATUS { get; set; }
        public int? OBS_TEXT_ID { get; set; }
        
        
        


    }
}
