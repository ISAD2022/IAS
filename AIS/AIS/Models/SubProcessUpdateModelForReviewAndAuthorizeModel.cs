using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class SubProcessUpdateModelForReviewAndAuthorizeModel
    {
        public int P_ID { get; set; }
        public int NEW_P_ID { get; set; }
        public int SP_ID { get; set; }
        public int NEW_SP_ID { get; set; }
        public string PROCESS_NAME { get; set; }
        public string NEW_PROCESS_NAME { get; set; }
        public string SUB_PROCESS_NAME { get; set; }
        public string NEW_SUB_PROCESS_NAME { get; set; }
        public string COMMENTS { get; set; }
       

    }
}
