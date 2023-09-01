using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasBranchComplianceModel
    {
        public string AUDIT_PERIOD { get; set; }
        public string NAME { get; set; }
        public string PARA_NO { get; set; }
        public string PARA_CATEGORY { get; set; }
        public string AU_OBS_ID { get; set; }
        public string ID { get; set; }
        public string REF_P { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string AMOUNT { get; set; }
        public string REVIEWER_REMARKS { get; set; }
        public string VOL_I_II { get; set; }
        public string STATUS_ID { get; set; }
        public string AUDITED_BY { get; set; }
    }
}
