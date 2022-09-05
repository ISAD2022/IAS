using System;

namespace InspectionSolution.Models
{
    public class AuditEntitiesModel
    {        
        public int AUTID { get; set; }
        public string ENTITYCODE { get; set; }
        public string ENTITYTYPEDESC { get; set; }
        public string AUDITABLE { get; set; }
    }
}
