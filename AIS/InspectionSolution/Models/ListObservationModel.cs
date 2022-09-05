using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectionSolution.Models
{
    public class ListObservationModel
    {        
        public string ID { get; set; }
        public string MEMO { get; set; }
        public int DAYS { get; set; }
        public string ATTACHMENTS { get; set; }
        public string LOANCASE { get; set; }
    }
}
