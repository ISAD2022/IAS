using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasForComplianceReviewer
    {
        public string REPLIEDBY { get; set; }
        public string REPLY { get; set; }
        public DateTime REPLIEDDATE { get; set; }
        public DateTime LASTUPDATEDDATE { get; set; }
        public string REMARKS { get; set; }
        public string LASTUPDATEDBY { get; set; }
        public int? EVIDENCE_ID { get; set; }
        public string SUBMITTED { get; set; }
        public int? ID { get; set; }
        public int? ENTITY_ID { get; set; }
        public string REF_P { get; set; }
        public string AUDITEDBY { get; set; }
        public string C_STATUS { get; set; }


    }
}
