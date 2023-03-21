using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AuditeeAddressModel
    {        
        public int ENG_ID { get; set; }
        public int CODE{ get; set; }
        public string P_NAME { get; set; }
        public string NAME { get; set; }
        public string ADDRESS { get; set; }
        public DateTime DATE_OF_OPENING { get; set; }
        public string LICENSE { get; set; }
        public DateTime AUDIT_STARTDATE { get; set; }
        public DateTime AUDIT_ENDDATE { get; set; }
        public DateTime OPERATION_STARTDATE { get; set; }
        public DateTime OPERATION_ENDDATE{ get; set; }

    }
}
