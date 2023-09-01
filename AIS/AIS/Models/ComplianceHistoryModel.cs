using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ComplianceHistoryModel
    {
        public int ID { get; set; }
        public string REF_P { get; set; }
        public string OBS_ID { get; set; }
        public string REPLY { get; set; }
        public string REPLIED_DATE { get; set; }
        public string REVIEWER_REMARKS { get; set; }
        public string REVIEWED_ON { get; set; }
        public string IMP_REMARKS { get; set; }
        public string IMP_REMARKS_ON { get; set; }
        public string REMARKS { get; set; }
        public string REMARKS_ON { get; set; }
        public string STATUS_NAME { get; set; }
        public string EVIDENCE_ID { get; set; }
        public string PARA_CATEGORY { get; set; }
        }
}
