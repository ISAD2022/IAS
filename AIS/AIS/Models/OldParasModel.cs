using System.Collections.Generic;

namespace AIS.Models
    {
    public class OldParasModel
        {
        public int ID { get; set; }
        public string REF_P { get; set; }
        public string ENTITY_CODE { get; set; }
        public string ENTITY_ID { get; set; }
        public string AU_OBS_ID { get; set; }
        public string TYPE_ID { get; set; }
        public string RISK_ID { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string ENTITY_NAME { get; set; }
        public string PARA_NO { get; set; }
        public string RESPONSIBLE_PP_NO { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string PARA_STATUS { get; set; }
        public string ENTTY_NAME { get; set; }
        public string ANNEXURE { get; set; }
        public string AMOUNT_INVOLVED { get; set; }
        public string VOL_I_II { get; set; }
        public string AUDITED_BY { get; set; }
        public string AUDITEDBY { get; set; }

        public string ENT_TYPE { get; set; }
        public int STATUS { get; set; }
        public string IND { get; set; }
        public string PARA_TEXT { get; set; }
        public string ENTERED_BY { get; set; }
        public string MAKER_REMARKS { get; set; }
        public string REVIEWER_REMARKS { get; set; }

        public int PROCESS { get; set; }
        public int SUB_PROCESS { get; set; }
        public int PROCESS_DETAIL { get; set; }

        public List<ObservationResponsiblePPNOModel> PARA_RESP { get; set; }

        }
    }
