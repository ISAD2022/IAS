using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ObservationReversalModel
    {        
    
        public string ENG_ID { get; set; }
        public string TEAM_NAME { get; set; }
        public string AUDIT_START_DATE { get; set; }
        public string AUDIT_END_DATE { get; set; }
        public string OP_START_DATE { get; set; }
        public string OP_END_DATE { get; set; }
        public string ENTITY_ID { get; set; }
        public string STATUS_ID { get; set; }
        public string STATUS { get; set; }
      

    }
}
