using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class OldParasModel
    {
        public int ID { get; set; }
        public string REF_P { get; set; }
        public string ENTITY_CODE { get; set; }
        public string TYPE_ID { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string ENTITY_NAME { get; set; }
        public string PARA_NO { get; set; }
        public string RESPONSIBLE_PP_NO { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string ENTTY_NAME { get; set; }
        public string ANNEXURE { get; set; }
        public string AMOUNT_INVOLVED { get; set; }
        public string VOL_I_II { get; set; }
        public string AUDITED_BY { get; set; }
        public int PROCESS { get; set; }
        public int SUB_PROCESS { get; set; }
        public int PROCESS_DETAIL { get; set; }
        public int STATUS { get; set; }
        public string PARA_TEXT { get; set; }

    }
}
