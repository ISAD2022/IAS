using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class SpecialAuditPlanModel
    {        
        public string PLAN_ID { get; set; }
        public string AUDITED_BY { get; set; }
        public string REPORTING_OFFICE { get; set; }
        public string ENTITY_NAME { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string NATURE { get; set; }
        public string NO_DAYS { get; set; }
        public string FIELD_VISIT { get; set; }

    }
}
