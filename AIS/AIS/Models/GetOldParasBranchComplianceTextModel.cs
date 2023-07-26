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
        public string PARA_CATEGORY { get; set; }
        public List<ObservationResponsiblePPNOModel> RESPONSIBLE_PPs { get; set; }

    }
}
