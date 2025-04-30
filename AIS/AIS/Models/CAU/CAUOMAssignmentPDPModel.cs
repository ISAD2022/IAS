using System;

namespace AIS.Models
    {
    public class CAUOMAssignmentPDPModel
        {
        public int ID { get; set; }
        public string OM_NO { get; set; }
        public int PARA_ID { get; set; }
        public DateTime DAC_DATES { get; set; }
        public string REPORT_FREQUENCY { get; set; }
        public string CONTENTS_OF_OM { get; set; }
        public int STATUS { get; set; }
        public string STATUS_DES { get; set; }
        public string KEY { get; set; }
        }
    }
