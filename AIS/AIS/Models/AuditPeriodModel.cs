using System;

namespace AIS.Models
    {
    public class AuditPeriodModel
        {
        public int AUDITPERIODID { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public int STATUS_ID { get; set; }
        public string STATUS { get; set; }
        public string REMARKS_OUT { get; set; }
        public string IS_SUCCESS { get; set; }

        }
    }
