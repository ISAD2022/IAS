namespace AIS.Models
    {
    public class ComplianceFlowModel
        {
        public int ID { get; set; }
        public string ENTITY_TYPE_ID { get; set; }
        public string ENTITY_TYPE_NAME { get; set; }
        public string GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public string NEXT_GROUP_ID { get; set; }
        public string NEXT_GROUP_NAME { get; set; }
        public string PREV_GROUP_ID { get; set; }
        public string PREV_GROUP_NAME { get; set; }
        public string COMP_UP_STATUS { get; set; }
        public string COMP_UP_STATUS_DESC { get; set; }
        public string COMP_DOWN_STATUS { get; set; }
        public string COMP_DOWN_STATUS_DESC { get; set; }

        }
    }
