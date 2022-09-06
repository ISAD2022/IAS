using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GlHeadDetailsModel
    { 
        public int BRANCHID { get; set; }
        public string GLDESP { get; set; }
        public int GLCODE{ get; set; }
        public string GLSUBNAME { get; set; }
        public DateTime MONTHEND { get; set; }
        public double RUNNING_DR { get; set; }
        public double RUNNING_CR { get; set; }
        public double BALANCE { get; set; }
    }
}
