using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GetAuditeeParasModel
    {        
        public int? MEMO_NUMBER { get; set; }
        public int? STATUS { get; set; }
        public string TEXT { get; set; }
        public string REPLY { get; set; }
        public string RECOMMENDATION { get; set; }
        public string REMARKS { get; set; }
        public string REPLYROLE { get; set; }
        public string HEADREMARKS { get; set; }
        
    }
}
