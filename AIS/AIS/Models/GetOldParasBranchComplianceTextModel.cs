using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetOldParasBranchComplianceTextModel
    {
        public string CHECKLIST { get; set; }
        public string SUBCHECKLIST { get; set; }
        public string CHECKLISTDETAIL { get; set; }
        public string PARA_TEXT { get; set; }
        public string REPLY { get; set; }
        public string PARA_CATEGORY { get; set; }
        public string BRANCH_REPLY { get; set; }
        public string ZONE_REPLY { get; set; }
        public string ZONE_REPLY_NEW { get; set; }
        public string IMP_REPLY { get; set; }
        public string HEAD_REPLY { get; set; }
        public List<ObservationResponsiblePPNOModel> RESPONSIBLE_PPs { get; set; }
        public List<ObservationResponsiblePPNOModel> UPDATED_RESPONSIBLE_PPs_BY_IMP { get; set; }
        public List<AuditeeResponseEvidenceModel> EVIDENCES { get; set; }

    }
}
