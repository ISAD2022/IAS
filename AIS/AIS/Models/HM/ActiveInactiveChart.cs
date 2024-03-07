using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ActiveInactiveChart
    {        
        public string Active_Count { get; set; }
        public string Inactive_Count { get; set; }
   
    }
}
