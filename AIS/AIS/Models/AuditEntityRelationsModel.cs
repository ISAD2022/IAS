namespace AIS.Models
    {
    public class AuditEntityRelationsModel
        {
        public int ENTITY_REALTION_ID { get; set; }
        public int PARENT_ENTITY_TYPEID { get; set; }
        public int CHILD_ENTITY_TYPEID { get; set; }
        public int ID { get; set; }
        public string STATUS { get; set; }
        public string PARENT_NAME { get; set; }
        public string CHILD_NAME { get; set; }


        }
    }
