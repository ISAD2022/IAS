using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class CAUOMAssignmentModel
    {        
        public int ID { get; set; }
        public string OM_NO { get; set; }
        public string CONTENTS_OF_OM { get; set; }
        public int DIV_ID { get; set; }
        public int STATUS { get; set; }
    }
}
