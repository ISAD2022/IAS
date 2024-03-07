using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class EngagementObservationsForStatusReversalModel
    {        
    
        public string ID { get; set; }
        public string MEMO_NO { get; set; }
        public string ASSIGNED_TO { get; set; }
        public string MEMO_DATE { get; set; }
        public string STATUS { get; set; }
      

    }
}
