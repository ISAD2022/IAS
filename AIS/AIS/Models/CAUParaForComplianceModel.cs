namespace AIS.Models
    {
    public class CAUParaForComplianceModel
        {
        public string COM_ID { get; set; }
        public int? NEW_PARA_ID { get; set; }
        public int? OLD_PARA_ID { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string PARA_NO { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string INDICATOR { get; set; }
        public string CAU_STATUS { get; set; }
        public string CAU_NAME { get; set; }
        public string CAU_INSTRUCTIONS { get; set; }
        public string CAU_ASSIGNED_ENT_ID { get; set; }

        }
    }
