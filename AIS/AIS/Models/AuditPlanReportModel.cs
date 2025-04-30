namespace AIS.Models
    {
    public class AuditPlanReportModel
        {
        public string AUDITEDBY { get; set; }
        public string PARRENTOFFICE { get; set; }
        public string AUDITEENAME { get; set; }
        public int? ENTITYCODE { get; set; }
        public int? ANTITYID { get; set; }
        public string LASTAUDITOPSENDATE { get; set; }
        public string ENTITYRISK { get; set; }
        public string ENTITYSIZE { get; set; }
        public int? NORMALDAYS { get; set; }
        public int? REVENUEDAYS { get; set; }
        public int? TRAVELDAY { get; set; }
        public int? DISCUSSIONDAY { get; set; }
        public string AUDITSTARTDATE { get; set; }
        public string AUDITENDDATE { get; set; }
        public string TNAME { get; set; }
        public string TEAMLEAD { get; set; }
        }
    }
