using System.Collections.Generic;

namespace AIS.Models
    {
    public class AddAuditPeriodModel
        {
        public string DESCRIPTION { get; set; }
        public string STARTDATE { get; set; }
        public string ENDDATE { get; set; }
        public List<int> DEPARTMENT_IDS { get; set; }
        }
    }
