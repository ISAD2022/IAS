using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class RiskProcessDetails
    {
        public int ID { get; set; }
        public int P_ID { get; set; }
        public string TITLE { get; set; }
        public string ACTIVE { get; set; }
    }
}
