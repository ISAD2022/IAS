namespace AIS.Models
    {
    public class GetOldParasBranchComplianceModel
        {
        public string AUDIT_PERIOD { get; set; }
        public string AUDIT_DATE { get; set; }
        public string PARA_RISK { get; set; }
        public string NAME { get; set; }
        public string PARA_NO { get; set; }
        public int? NEW_PARA_ID { get; set; }
        public int? OLD_PARA_ID { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string AUDITOR_REMARKS { get; set; }
        public string AUDIT_BY_ID { get; set; }
        public string NEXT_R_ID { get; set; }
        public string PREV_R_ID { get; set; }
        public string STATUS_UP { get; set; }
        public string STATUS_DOWN { get; set; }
        public string INDICATOR { get; set; }
        public string PREV_ROLE { get; set; }
        public string NEXT_ROLE { get; set; }
        public string RECEIVED_FROM { get; set; }
        public string COM_ID { get; set; }

        }
    }
