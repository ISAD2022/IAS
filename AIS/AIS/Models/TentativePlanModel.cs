using System;

namespace AIS.Models
{
    public class TentativePlanModel
    {        
        public int AUDIT_PERIOD_ID { get; set; }
        public int AUDIT_ZONE_ID { get; set; }
        public string ZONE_NAME { get; set; }
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public int RISK_ID { get; set; }
        public int BR_SIZE { get; set; }
        public string FREQUENCY_DESCRIPTION { get; set; }
        public int NO_OF_DAYS { get; set; }
      
    }
}
