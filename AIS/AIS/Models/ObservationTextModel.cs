using System.Collections.Generic;

namespace AIS.Models
    {
    public class ObservationTextModel
        {
        public int OBS_ID { get; set; }
        public string CP_ID { get; set; }
        public string CP { get; set; }
        public string PSN { get; set; }
        public string PSN_ID { get; set; }
        public string CD_ID { get; set; }
        public string CD { get; set; }
        public string INSTANCES { get; set; }
        public string AMOUNT { get; set; }
        public string TEXT { get; set; }
        public string TITLE { get; set; }
        public string RISK_ID { get; set; }
        public string RISK { get; set; }
        public string IND { get; set; }
        public string OBS_REPLY { get; set; }
        public List<ObservationResponsiblePPNOModel> RESPONSIBLE_PPs { get; set; }

        }
    }
