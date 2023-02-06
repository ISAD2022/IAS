using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GlHeadDetailsModel
    { 
        public int BRANCHID { get; set; }
        public string DESCRIPTION { get; set; }
        public int GLSUBCODE{ get; set; }
        public string GLSUBNAME { get; set; }
        public DateTime DATETIME { get; set; }
        public double DEBIT { get; set; }
        public double CREDIT { get; set; }
        public double BALANCE { get; set; }
    }
}
