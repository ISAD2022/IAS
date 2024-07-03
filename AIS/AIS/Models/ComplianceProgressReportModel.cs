using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ComplianceProgressReportModel
    {
       
        public string PPNO { get; set; }
        public string NAME { get; set; }
        public string TOTAL { get; set; }
        public string REFERRED_BACK { get; set; }
        public string RECOMMENDED { get; set; }
        
    }
}
