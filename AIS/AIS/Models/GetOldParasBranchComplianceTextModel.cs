using System.Collections.Generic;

namespace AIS.Models
    {
    public class GetOldParasBranchComplianceTextModel
        {

        public string PARA_TEXT { get; set; }
        public string OBS_TEXT { get; set; }
        public string PARA_TEXT_ID { get; set; }
        public string GIST_OF_PARA { get; set; }
        public List<ObservationResponsiblePPNOModel> RESPONSIBLE_PPs { get; set; }
        public List<ObservationResponsiblePPNOModel> UPDATED_RESPONSIBLE_PPs_BY_IMP { get; set; }
        public List<AuditeeResponseEvidenceModel> EVIDENCES { get; set; }

        }
    }
