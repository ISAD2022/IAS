namespace AIS.Models
    {
    public class AuditChecklistModel
        {
        public int T_ID { get; set; }
        public string HEADING { get; set; }
        public string RISK_SEQUENCE { get; set; }
        public string RISK_WEIGHTAGE { get; set; }
        public int ENTITY_TYPE { get; set; }
        public string ENTITY_TYPE_NAME { get; set; }
        public string STATUS { get; set; }
        }
    }
