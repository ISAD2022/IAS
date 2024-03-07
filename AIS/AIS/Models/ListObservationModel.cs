using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ListObservationModel
    {        
        public string ID { get; set; }
        public string MEMO { get; set; }
        public string HEADING { get; set; }
        public int RISK { get; set; }
        public string ANNEXURE_ID { get; set; }
        public string OBSERVATION_TEXT { get; set; }
        public int DAYS { get; set; }
        public string ATTACHMENTS { get; set; }
        public string LOANCASE { get; set; }
        public int NO_OF_INSTANCES { get; set; }
        public List<ObservationResponsiblePPNOModel> RESPONSIBLE_PPNO { get; set; }
    }
}
