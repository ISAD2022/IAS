using System;

namespace AIS.Models
    {
    public class AuditeeOldParasModel
        {
        public int? ID { get; set; }
        public string ENG_ID { get; set; }
        public string REF_P { get; set; }
        public int? ENTITY_CODE { get; set; }
        public int? TYPE_ID { get; set; }
        public string TYPE_DES { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string AUDIT_PERIOD_DES { get; set; }
        public string ENTITY_NAME { get; set; }
        public string PARA_CATEGORY { get; set; }
        public string REPORT_NAME { get; set; }
        public string PARA_NO { get; set; }
        public string MEMO_NO { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string OBS_ID { get; set; }
        public string AUDITEE_RESPONSE { get; set; }
        public string AUDITOR_REMARKS { get; set; }
        public DateTime? DATE_OF_LAST_COMPLIANCE_RECEIVED { get; set; }
        public string AUDITEDBY { get; set; }
        }
    }
