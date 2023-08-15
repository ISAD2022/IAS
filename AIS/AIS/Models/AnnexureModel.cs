using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AnnexureModel
    {        
        public int ID { get; set; }
        public string CODE { get; set; }
        public string HEADING { get; set; }
        public string VOL { get; set; }
        public string STATUS { get; set; }
      

    }
}
