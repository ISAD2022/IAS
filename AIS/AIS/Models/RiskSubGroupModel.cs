using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
    {
    public class RiskSubGroupModel
        {
        public int GR_ID { get; set; }
        public int S_GR_ID { get; set; }
        public string DESCRIPTION { get; set; }
        [NotMapped]
        public string GROUP_DESC { get; set; }


        }
    }
