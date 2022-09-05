using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectionSolution.Models
{
    public class AuditChecklistDetailsModel
    {
        public int ID { get; set; }        
        public int S_ID { get; set; }
        public string S_NAME { get; set; }
        public int V_ID { get; set; }
        public string V_NAME { get; set; }
        public string HEADING { get; set; }
        public int RISK_ID { get; set; }
        public string RISK { get; set; }
        public int ROLE_RESP_ID { get; set; }
        public string ROLE_RESP { get; set; }
        public int PROCESS_OWNER_ID { get; set; }
        public string PROCESS_OWNER { get; set; }
        public string STATUS { get; set; }
    }
}
