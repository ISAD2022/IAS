namespace AIS.Models
    {
    public class ComplianceOSParasModel
        {
        public string ENTITY_ID { get; set; }
        public string PARENT_ID { get; set; }
        public string ENTITY_NAME { get; set; }
        public string ENTITY_TYPE { get; set; }
        public string REPORTING_OFFICE { get; set; }
        public string TOTAL_PARAS { get; set; }
        public string TOTAL_SETTLED_PARAS { get; set; }
        public string TOTAL_OUTSTANDING_PARAS { get; set; }
        public string SETTLEMENT_PERCENTAGE { get; set; }
        public string OUTSTANDING_PERCENTAGE { get; set; }
        public string COMPLIANCE_PENDING_OS_PARAS { get; set; }
        public string ZERO_COMPLIANCE_PARAS { get; set; }

        }
    }
