using System;

namespace AIS.Models
    {
    public class AuditPlanEngagementModel
        {

        public string AUDITPERIOD { get; set; }
        public string PARENT_OFFICE { get; set; }
        public string ENITIY_NAME { get; set; }
        public DateTime? AUDIT_STARTDATE { get; set; }
        public DateTime? AUDIT_ENDDATE { get; set; }
        public int? TRAVEL_DAY { get; set; }
        public int? REVENUE_RECORD_DAY { get; set; }
        public int? DISCUSSION_DAY { get; set; }
        public string TEAM_NAME { get; set; }
        //public string MEMBER_NAME { get; set; }
        public string STATUS { get; set; }


        }
    }
