using System;
using System.Collections.Generic;

namespace AIS.Models
    {
    public class AssignedObservations
        {
        public int ID { get; set; }
        public int? RESP_ID { get; set; }
        public int OBS_ID { get; set; }
        public int OBS_TEXT_ID { get; set; }
        public int EDITABLE { get; set; }
        public string RESPONSE_ID { get; set; }
        public string GIST { get; set; }
        public int ASSIGNEDTO_ROLE { get; set; }
        public int ASSIGNEDBY { get; set; }
        public DateTime? ASSIGNED_DATE { get; set; }
        public int LASTUPDATEDBY { get; set; }
        public DateTime? LASTUPDATEDDATE { get; set; }
        public string IS_ACTIVE { get; set; }
        public string REPLIED { get; set; }
        public string OBSERVATION_TEXT { get; set; }
        public string PROCESS { get; set; }
        public string SUB_PROCESS { get; set; }
        public string CHECKLIST_DETAIL { get; set; }
        public string VIOLATION { get; set; }
        public string NATURE { get; set; }
        public string REPLY_TEXT { get; set; }
        public string STATUS { get; set; }
        public string STATUS_ID { get; set; }
        public string ENTITY_NAME { get; set; }
        public int? CAN_REPLY { get; set; }
        public string MEMO_DATE { get; set; }
        public string MEMO_NUMBER { get; set; }
        public string MEMO_REPLY_DATE { get; set; }
        public string AUDIT_YEAR { get; set; }
        public string OPERATION_STARTDATE { get; set; }
        public string OPERATION_ENDDATE { get; set; }
        public List<ObservationResponsiblePPNOModel> RESPONSIBLE_PPNOs { get; set; }
        }
    }
