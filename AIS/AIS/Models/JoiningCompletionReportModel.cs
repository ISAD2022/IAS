using System;

namespace AIS.Models
    {
    public class JoiningCompletionReportModel
        {
        public string AUDIT_BY { get; set; }
        public string AUDITEE_NAME { get; set; }
        public string TEAM_NAME { get; set; }
        public int PPNO { get; set; }
        public string NAME { get; set; }
        public string TEAM_LEAD { get; set; }
        public DateTime? JOINING_DATE { get; set; }
        public DateTime? COMPLETION_DATE { get; set; }
        public string STATUS { get; set; }

        }
    }
