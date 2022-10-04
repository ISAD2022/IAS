using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ListObservationModel
    {        
        public string ID { get; set; }
        public string MEMO { get; set; }
        public string OBSERVATION_TEXT { get; set; }
        public int DAYS { get; set; }
        public string ATTACHMENTS { get; set; }
        public string LOANCASE { get; set; }
    }
}
