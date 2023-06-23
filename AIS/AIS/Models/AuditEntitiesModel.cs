using System;

namespace AIS.Models
{
    public class AuditEntitiesModel
    {
        public int TYPE_ID { get; set; }
        public int AUTID { get; set; }
        public string ENTITYCODE { get; set; }
        public string ENTITYTYPEDESC { get; set; }
        public string AUDITABLE { get; set; }
    }
}
