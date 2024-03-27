using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class DepttWiseOutstandingParasModel
    {
        public int ID { get; set; }
        public string ENTITY_ID { get; set; }
        public string ENTITY_NAME { get; set; }
       
        public string AGE { get; set; }
        public string TOTAL_PARAS { get; set; }
     
       
    }
}
