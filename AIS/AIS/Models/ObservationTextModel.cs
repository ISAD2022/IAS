using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ObservationTextModel
    {        
        public int OBS_ID { get; set; }
        public string ANNEXURE_ID { get; set; }
        public string ANNEXURE { get; set; }
        public string PROCESS { get; set; }
        public string PROCESS_ID { get; set; }
        public string SUB_PROCESS { get; set; }
        public string SUB_PROCESS_ID { get; set; }
        public string CHECKLIST_DETAIL_ID { get; set; }
        public string CHECKLIST_DETAIL { get; set; }
        public string VIOLATION_ID { get; set; }
        public string VIOLATION { get; set; }
        public string NATURE_ID { get; set; }
        public string NATURE { get; set; }
        public string OBS_TEXT { get; set; }
        public string RISK_ID { get; set; }
        public string RISK { get; set; }        
        public string NO_OF_INSTANCES { get; set; }        
        public string INDICATOR { get; set; }        
        
    }
}
