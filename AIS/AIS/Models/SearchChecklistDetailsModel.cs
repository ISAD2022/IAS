using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class SearchChecklistDetailsModel
    {
       
        public int ID { get; set; }
        public string PROCESS { get; set; }
        public string SUB_PROCESS { get; set; }
        public string PROCESS_DETAIL { get; set; }
        public string RISK { get; set; }
        
    }
}
