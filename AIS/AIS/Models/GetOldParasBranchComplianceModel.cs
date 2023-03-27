using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasBranchComplianceModel
    {
        public string AUDIT_PERIOD { get; set; }
        public string NAME { get; set; }
        public int? PARA_NO { get; set; }
        public int? ID { get; set; }
        public string REF_P { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string VOL_I_II { get; set; }
    }
}
