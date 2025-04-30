namespace AIS.Models
    {
    public class UserRelationshipModel
        {

        public int SR { get; set; }
        public int ENTITY_REALTION_ID { get; set; }

        public int PARENT_ENTITY_TYPEID { get; set; }
        public int CHILD_ENTITY_TYPEID { get; set; }
        public int ENTITY_ID { get; set; }
        public string STATUS { get; set; }

        public string FIELD_NAME { get; set; }


        public string PARENT_NAME { get; set; }
        public string ENTITYTYPEDESC { get; set; }

        public string DESCRIPTION { get; set; }

        public string CHILD_NAME { get; set; }
        public string C_NAME { get; set; }
        public string C_TYPE_ID { get; set; }

        public string COMPLICE_BY { get; set; }

        public string AUDIT_BY { get; set; }
        public string GM_OFFICE { get; set; }
        public string REPORTING { get; set; }

        public string ACTIVE { get; set; }

        }
    }