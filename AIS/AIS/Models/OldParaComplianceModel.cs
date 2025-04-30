using System;

namespace AIS.Models
    {
    public class OldParaComplianceModel
        {
        public int ParaRef { get; set; }
        public DateTime? ComplianceDate { get; set; }
        public string AuditeeCompliance { get; set; }
        public string AuditorRemarks { get; set; }
        public string CnIRecommendation { get; set; }

        }
    }
