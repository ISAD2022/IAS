using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class MergeDuplicateChecklistModel
    {        
        public int ID { get; set; }
        public string HEADING { get; set; }
        public string NEW_COUNT { get; set; }
        public string OLD_COUNT { get; set; }

    }
}
