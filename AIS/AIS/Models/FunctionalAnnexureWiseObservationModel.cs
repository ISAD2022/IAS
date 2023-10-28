using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class FunctionalAnnexureWiseObservationModel
    {
        public int ID { get; set; }
        public string D_ID { get; set; }
        public string NAME { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string PARA_CATEGORY { get; set; }
        public string PARA_NO { get; set; }
    }
}
