namespace AIS.Models
    {
    public class AuditeeAddressModel
        {
        public int ENG_ID { get; set; }
        public int CODE { get; set; }
        public string P_NAME { get; set; }
        public string NAME { get; set; }
        public string ADDRESS { get; set; }
        public string DATE_OF_OPENING { get; set; }
        public string LICENSE { get; set; }
        public string AUDIT_STARTDATE { get; set; }
        public string AUDIT_ENDDATE { get; set; }
        public string OPERATION_STARTDATE { get; set; }
        public string OPERATION_ENDDATE { get; set; }


        public string HIGH { get; set; }
        public string MEDIUM { get; set; }
        public string LOW { get; set; }


        public string SETTLED_HIGH { get; set; }
        public string SETTLED_MEDIUM { get; set; }
        public string SETTLED_LOW { get; set; }


        public string OPEN_HIGH { get; set; }
        public string OPEN_MEDIUM { get; set; }
        public string OPEN_LOW { get; set; }


        }
    }
