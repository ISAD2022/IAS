using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectionSolution.Models
{
    public class AddAuditPeriodModel
    {        
        public string DESCRIPTION { get; set; }
        public string STARTDATE { get; set; }
        public string ENDDATE { get; set; }
        public List<int> DEPARTMENT_IDS { get; set; }
    }
}
