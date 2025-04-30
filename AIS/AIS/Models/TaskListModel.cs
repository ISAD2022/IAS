using System;

namespace AIS.Models
    {
    public class TaskListModel
        {
        public int ID { get; set; }
        public int ENG_PLAN_ID { get; set; }
        public int TEAM_ID { get; set; }
        public int SEQUENCE_NO { get; set; }
        public int TEAMMEMBER_PPNO { get; set; }
        public int ENTITY_ID { get; set; }
        public int ENTITY_CODE { get; set; }
        public int ENTITY_TYPE { get; set; }
        public string ENTITY_TYPE_DESC { get; set; }
        public string ENTITY_NAME { get; set; }
        public string AUDIT_START_DATE { get; set; }
        public string AUDIT_END_DATE { get; set; }
        public int STATUS_ID { get; set; }
        public string ENG_STATUS { get; set; }
        public string ENG_NEXT_STATUS { get; set; }
        public string ISACTIVE { get; set; }
        public string TEAM_NAME { get; set; }
        public string EMP_NAME { get; set; }
        public string WORKING_PAPER { get; set; }
        public string PRE_INFO { get; set; }
        public string AUDIT_YEAR { get; set; }
        public string ISCLOSE { get; set; }
        public DateTime OPERATION_STARTDATE { get; set; }
        public DateTime OPERATION_ENDDATE { get; set; }

        }
    }
