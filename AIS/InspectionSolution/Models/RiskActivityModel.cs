using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectionSolution.Models
{
    public class RiskActivityModel
    {        
        public int ACTIVITY_ID { get; set; }
        public int S_GR_ID { get; set; }
        public int MAX_NUMBER { get; set; }
        public string DESCRIPTION { get; set; }
        [NotMapped]
        public string SUB_GROUP_DESC { get; set; }

    }
}
