using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasBranchComplianceModel
    {
        public string AUDIT_PERIOD { get; set; }
        public string NAME { get; set; }
        public string PARA_NO { get; set; }
        public int? NEW_PARA_ID { get; set; }
        public int? OLD_PARA_ID { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string AUDITED_BY { get; set; }
        public string INDICATOR { get; set; }
        public string COMPLIANCE_ID { get; set; }
        public string COMPLIANCE_STATUS { get; set; }
        public string COMPLIANCE_CYCLE { get; set; }
    }
}
