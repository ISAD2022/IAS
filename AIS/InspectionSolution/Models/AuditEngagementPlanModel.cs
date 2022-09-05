using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectionSolution.Models
{
    public class AuditEngagementPlanModel
    {        
        public int? ENG_ID { get; set; }
        public int? PERIOD_ID { get; set; }
        public int? ENTITY_TYPE { get; set; }        
        public int? AUDIT_ZONEID { get; set; }
        public DateTime? AUDIT_STARTDATE { get; set; }
        public DateTime? AUDIT_ENDDATE { get; set; }
        public int? CREATEDBY { get; set; }
        public DateTime? CREATED_ON { get; set; }
        public int? LASTUPDATEDBY { get; set; }
        public DateTime? LASTUPDATEDDATE { get; set; }
        public string TEAM_NAME { get; set; }
        public int? STATUS { get; set; }
        public int? TEAM_ID { get; set; }
        public int? ENTITY_ID { get; set; }
        public int? ENTITY_CODE { get; set; }


    }
}
