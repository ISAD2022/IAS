using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AnnexureModel
    {        
        public int ID { get; set; }
        public string ANNEX { get; set; }
        public string CODE { get; set; }
        public string HEADING { get; set; }
        public string VOL { get; set; }
        public string RISK_ID { get; set; }
        public string RISK { get; set; }
        public string PROCESS_ID { get; set; }
        public string PROCESS { get; set; }
        public string STATUS { get; set; }
      

    }
}
