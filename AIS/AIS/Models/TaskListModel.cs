using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class TaskListModel
    {        
        public int ID { get; set; }
        public int ENTITY_ID { get; set; }
        public string ENTITY_NAME { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public int STATUS_ID { get; set; }
        public string STATUS { get; set; }
        public int PP_NUMBER { get; set; }
        public string EMP_NAME { get; set; }
        public string TEAM_NAME { get; set; }


    }
}
