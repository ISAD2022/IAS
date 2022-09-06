using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ClosingDraftTeamDetailsModel
    {        
        public int ID { get; set; }
        public int ENG_PLAN_ID { get; set; }
        public int TEAM_MEM_PPNO { get; set; }
        public DateTime? JOINING_DATE { get; set; }
        public DateTime? COMPLETION_DATE { get; set; }
        public string ISTEAMLEAD { get; set; }
        public string MEMBER_NAME { get; set; }
       
    }
}
