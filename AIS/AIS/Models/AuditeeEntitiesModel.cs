using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AuditeeEntitiesModel
    {        
        public int? ENTITY_ID { get; set; }
        public int? CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string NAME { get; set; }
        public string ACTIVE { get; set; }
        public int? TYPE_ID { get; set; }
        public int? AUDITBY_ID { get; set; }
        public int? INSPECTEDBY_ID { get; set; }
        public int? COST_CENTER { get; set; }
        public int? ENG_ID { get; set; }
    }
}
