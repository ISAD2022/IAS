using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AuditPeriodModel
    {        
        public int ID { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        public int STATUS_ID { get; set; }
        
    }
}
