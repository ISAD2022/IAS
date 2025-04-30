using System.Collections.Generic;

namespace AIS.Models
    {
    public class ManageObservations
        {
        public int OBS_ID { get; set; }
        public string ANNEXURE_ID { get; set; }
        public string ANNEXURE_CODE { get; set; }
        public string ENTITY_NAME { get; set; }
        public string ENTITY_ID { get; set; }
        public string PROCESS { get; set; }
        public string PROCESS_ID { get; set; }
        public string SUB_PROCESS { get; set; }
        public string SUB_PROCESS_ID { get; set; }
        public string Checklist_Details { get; set; }
        public string Checklist_Details_Id { get; set; }
        public string VIOLATION { get; set; }
        public string NATURE { get; set; }
        public int MEMO_NO { get; set; }
        public int DRAFT_PARA_NO { get; set; }
        public int FINAL_PARA_NO { get; set; }
        public string OBS_TEXT { get; set; }
        public string OBS_REPLY { get; set; }
        public string HEADING { get; set; }
        public string AUD_REPLY { get; set; }
        public string HEAD_REPLY { get; set; }
        public int OBS_RISK_ID { get; set; }
        public string OBS_RISK { get; set; }
        public int OBS_STATUS_ID { get; set; }
        public string OBS_STATUS { get; set; }
        public string PERIOD { get; set; }
        public int? NO_OF_INSTANCES { get; set; }
        public string AMOUNT { get; set; }
        public string PPNO_TEST { get; set; }
        public string INDICATOR { get; set; }
        public string TYPE_INDICATOR { get; set; }
        public List<ObservationResponsiblePPNOModel> RESPONSIBLE_PPs { get; set; }
        public List<AuditeeResponseEvidenceModel> ATTACHED_EVIDENCES { get; set; }
        }
    }
