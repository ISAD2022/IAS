namespace AIS.Models
    {
    public class AuditEntitiesModel
        {
        public int TYPE_ID { get; set; }
        public int AUTID { get; set; }
        public int? E_AUTID { get; set; }
        public string ENTITYCODE { get; set; }
        public string ENTITYTYPEDESC { get; set; }
        public string AUDITABLE { get; set; }
        public string AUDITEDBY { get; set; }
        public string AUDITED_BY_ENTITY { get; set; }
        public string AUDIT_TYPE { get; set; }
        public string D_RISK { get; set; }

        }
    }
