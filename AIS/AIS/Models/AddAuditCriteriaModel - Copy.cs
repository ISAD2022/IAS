using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AuditCriteriaModel
    {        
        public int ID { get; set; }
        public int ENTITY_ID { get; set; }
        public string ENTITY_NAME { get; set; }
        public int SIZE_ID { get; set; }
        public int SIZE { get; set; }
        public int RISK_ID { get; set; }
        public int FREQUENCY_ID { get; set; }
        public int NO_OF_DAYS { get; set; }
        public string VISIT { get; set; }
        public int APPROVAL_STATUS { get; set; }
        public int AUDITPERIODID { get; set; }
    }
}
