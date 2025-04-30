using System;
using System.Collections.Generic;

namespace AIS.Models
    {
    public class ObservationModel
        {
        public int ID { get; set; }
        public int? BRANCH_ID { get; set; }
        public string OBSERVATION_TEXT { get; set; }
        public string HEADING { get; set; }
        public string OBSERVATION_TEXT_PLAIN { get; set; }
        public int ENGPLANID { get; set; }
        public string ANNEXURE_ID { get; set; }
        public int STATUS { get; set; }
        public string STATUS_NAME { get; set; }
        public int ENTEREDBY { get; set; }
        public DateTime ENTEREDDATE { get; set; }
        public int LASTUPDATEDBY { get; set; }
        public DateTime LASTUPDATEDDATE { get; set; }
        public string AMOUNT_INVOLVED { get; set; }
        public int REPLYBY { get; set; }
        public DateTime REPLYDATE { get; set; }
        public int LASTREPLYBY { get; set; }
        public DateTime LASTREPLYDATE { get; set; }
        public DateTime MEMO_DATE { get; set; }
        public int SEVERITY { get; set; }
        public int MEMO_NUMBER { get; set; }
        public string RESPONSIBILITY_ASSIGNED { get; set; }
        public string DSA_ISSUED { get; set; }
        public int TRANSACTION_ID { get; set; }
        public int RISKMODEL_ID { get; set; }
        public int? PROCESS_ID { get; set; }
        public int? SUBCHECKLIST_ID { get; set; }
        public int? CHECKLISTDETAIL_ID { get; set; }
        public int V_CAT_ID { get; set; }
        public string NO_OF_INSTANCES { get; set; }
        public int V_CAT_NATURE_ID { get; set; }
        public int? OTHER_ENTITY_ID { get; set; }
        public string TEAM_LEAD { get; set; }
        public string AUDITEE_REPLY { get; set; }
        public string AUDITOR_RECOM { get; set; }
        public string HEAD_RECOM { get; set; }
        public string QA_RECOM { get; set; }
        public string QA_GIST { get; set; }
        public List<ObservationResponsiblePPNOModel> RESPONSIBLE_PPNO { get; set; }

        }
    }
