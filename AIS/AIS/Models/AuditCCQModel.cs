using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AuditCCQModel
    {
        public int ID { get; set; }
        public int ENTITY_ID { get; set; }
        public string QUESTIONS { get; set; }
        public int CONTROL_VIOLATION_ID { get; set; }
        public string RISK_ID { get; set; }
        public string STATUS { get; set; }

    }
}
