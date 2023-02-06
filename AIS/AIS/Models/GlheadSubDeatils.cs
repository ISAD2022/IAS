using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GlHeadSubDetailsModel
    {        
        public int BRANCHID { get; set; }
        public int GL_TYPE_ID { get; set; }
        public int GLSUBID{ get; set; }
        public int GLCODE{ get; set; }
        public string GLSUBNAME { get; set; }
        
        public DateTime DATETIME { get; set; }
        public double BALANCE { get; set; }
        public double RUNNING_DR { get; set; }
        public double RUNNING_CR { get; set; }
        public List<GlHeadSubDetailsModel> GL_SUBDETAILS { get; set; }

    }
}
