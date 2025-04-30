using System.ComponentModel.DataAnnotations.Schema;
namespace AIS.Models
    {
    public class AuditTeamModel
        {
        public int ID { get; set; }
        public int T_ID { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }
        public int AUDIT_DEPARTMENT { get; set; }
        public int TEAMMEMBER_ID { get; set; }
        public string IS_TEAMLEAD { get; set; }
        [NotMapped]
        public string EMPLOYEENAME { get; set; }
        [NotMapped]
        public string PLACE_OF_POSTING { get; set; }
        public string STATUS { get; set; }
        }
    }
