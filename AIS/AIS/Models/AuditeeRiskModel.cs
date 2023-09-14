using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace AIS.Models
{
    public class AuditeeRiskModel
    {
      
        public string RISK_AREAS { get; set; }       
        public string MAX_NUMBER { get; set; }
        public string MARKS { get; set; }
    }
}
