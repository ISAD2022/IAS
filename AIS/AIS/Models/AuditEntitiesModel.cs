using System;

namespace AIS.Models
{
    public class AuditEntitiesModel
    {        
        public int AUTID { get; set; }
        public string ENTITYCODE { get; set; }
        public string ENTITYTYPEDESC { get; set; }
        public DateTime? EFFECTIVE_FROM { get; set; }
        public DateTime? VALIDUNTIL { get; set; }
        public string ACTIVE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_ON { get; set; }
        public DateTime? RECORD_TIMESTAMP { get; set; }
        public string AUDITABLE { get; set; }
    }
}
