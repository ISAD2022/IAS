using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class DraftDSAGuidelines
    {
        public int ID { get; set; }
        public string PARTICULARS { get; set; }
        public string STATUS { get; set; }

    }
}
