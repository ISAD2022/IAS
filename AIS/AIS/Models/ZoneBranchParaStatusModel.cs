using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ZoneBranchParaStatusModel
    {
        public string Entity_Name { get; set; }
        public int Total_Paras { get; set; }
        public int Settled_Paras { get; set; }
        public int Unsettled_Paras { get; set; }
  
    }
}
