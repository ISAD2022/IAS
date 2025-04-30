using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
    {
    public class RiskProcessTransactions
        {
        public int ID { get; set; }
        public int V_ID { get; set; }
        public int PD_ID { get; set; }
        public int DIV_ID { get; set; }
        public string DIV_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string CONTROL_OWNER { get; set; }
        public string ACTION { get; set; }
        public int RISK_WEIGHTAGE { get; set; }
        public string RISK { get; set; }
        public int RISK_MAX_NUMBER { get; set; }
        [NotMapped]
        public string SUB_PROCESS_NAME { get; set; }
        [NotMapped]
        public string PROCESS_NAME { get; set; }
        [NotMapped]
        public string PROCESS_STATUS { get; set; }
        [NotMapped]
        public string PROCESS_COMMENTS { get; set; }
        [NotMapped]
        public string VIOLATION_NAME { get; set; }

        }
    }
