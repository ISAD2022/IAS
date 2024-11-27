using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class SeriousFraudulentObsGM
        {
        public int PARENT_ID { get; set; }
        public string P_NAME { get; set; }
        public string TOTAL_NO { get; set; }
        public string A1 { get; set; }
        public string C_A1 { get; set; }
        public string C_AMOUNT { get; set; }
        public string PER_INV { get; set; }

    }
}
