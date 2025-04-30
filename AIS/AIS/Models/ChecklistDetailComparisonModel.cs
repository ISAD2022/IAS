namespace AIS.Models
    {
    public class ChecklistDetailComparisonModel
        {


        public int ID { get; set; }
        public string PROCESS { get; set; }
        public string PROCESS_ID { get; set; }
        public string SUB_PROCESS { get; set; }
        public string PROCESS_DETAIL { get; set; }

        public string VIOLATION { get; set; }
        public string FUNCTIONAL_RESP { get; set; }
        public string ROLE_RESP { get; set; }
        public string RISK { get; set; }
        public string ANNEXURE { get; set; }

        public string NEW_SUB_PROCESS { get; set; }
        public string NEW_PROCESS_DETAIL { get; set; }

        public string NEW_VIOLATION { get; set; }
        public string NEW_FUNCTIONAL_RESP { get; set; }
        public string NEW_ROLE_RESP { get; set; }
        public string NEW_RISK { get; set; }

        public string NEW_ANNEXURE { get; set; }
        public string STATUS { get; set; }

        public string N_D_ID { get; set; }
        public string N_S_ID { get; set; }
        public string N_V_ID { get; set; }
        public string N_RISK_ID { get; set; }
        public string N_ROLE_RESP_ID { get; set; }
        public string N_OWNER_ID { get; set; }
        public string N_ANNEX_ID { get; set; }

        }
    }
