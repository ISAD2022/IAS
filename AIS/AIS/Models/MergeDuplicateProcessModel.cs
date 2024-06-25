using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class MergeDuplicateProcessModel
    {        
        public int ID { get; set; }
        public int M_ID { get; set; }
        public string HEADING { get; set; }

    }
}
