using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ManageObservations
    {        
        public int OBS_ID { get; set; }
       public string ENTITY_NAME { get; set; }
        public int MEMO_NO { get; set; }
        public string OBS_TEXT { get; set; }
        public string OBS_REPLY { get; set; }
        public int OBS_RISK_ID { get; set; }
        public string OBS_RISK { get; set; }
        public int OBS_STATUS_ID { get; set; }
        public string OBS_STATUS { get; set; }
        public string PERIOD { get; set; }
    }
}
