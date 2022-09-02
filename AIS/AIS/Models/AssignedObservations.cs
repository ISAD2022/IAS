using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AssignedObservations
    {        
        public int ID { get; set; }
        public int OBS_ID { get; set; }
        public int OBS_TEXT_ID { get; set; }
        public int ASSIGNEDTO_ROLE { get; set; }
        public int ASSIGNEDBY { get; set; }
        public DateTime? ASSIGNED_DATE { get; set; }
        public int LASTUPDATEDBY { get; set; }
        public DateTime? LASTUPDATEDDATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string REPLIED { get; set; }
        public string OBSERVATION_TEXT { get; set; }
        public string STATUS { get; set; }
        public string ENTITY_NAME { get; set; }
        public string MEMO_DATE { get; set; }
        public string MEMO_REPLY_DATE{ get; set; }


    }
}
