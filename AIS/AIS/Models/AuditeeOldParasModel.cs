using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AuditeeOldParasModel
    {        
        public int? ID { get; set; }
        public string REF_P { get; set; }
        public int? ENTITY_CODE { get; set; }
        public int? TYPE_ID { get; set; }
        public string TYPE_DES { get; set; }
        public int? AUDIT_PERIOD { get; set; }
        public string AUDIT_PERIOD_DES { get; set; }
        public string ENTITY_NAME { get; set; }
        public int? PARA_NO { get; set; }
        public string MEMO_NO { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string AUDITEE_RESPONSE { get; set; }
        public string AUDITOR_REMARKS { get; set; }
        public DateTime? DATE_OF_LAST_COMPLIANCE_RECEIVED { get; set; }
        public int? AUDITEDBY { get; set; }
    }
}
