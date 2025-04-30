using System;
using System.Collections.Generic;

namespace AIS.Models
    {
    public class JoiningModel
        {
        public int ENG_PLAN_ID { get; set; }
        public int ENTITY_ID { get; set; }
        public int ENTITY_CODE { get; set; }
        public string ENTITY_NAME { get; set; }
        public string TEAM_NAME { get; set; }
        public string AUDIT_PERIOD { get; set; }
        public string STATUS { get; set; }
        public string RISK { get; set; }
        public string SIZE { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public List<JoiningTeamModel> TEAM_DETAILS { get; set; }

        }
    }
