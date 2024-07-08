using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class SettledParasModel
    {
        public string ID { get; set; }
        public string REPORTING_OFFICE { get; set; }
        public string PLACE_OF_POSTING { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string PARA_NO { get; set; }
        public string STATUS { get; set; }
     

    }
}
