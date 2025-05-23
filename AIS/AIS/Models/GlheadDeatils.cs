using System;

namespace AIS.Models
    {
    public class GlHeadDetailsModel
        {
        public int BRANCHID { get; set; }
        public string DESCRIPTION { get; set; }
        public int GL_TYPEID { get; set; }
        public string GLSUBNAME { get; set; }
        public DateTime DATETIME { get; set; }
        public DateTime DAY_END_BALANCE_DATE { get; set; }
        public double DEBIT { get; set; }
        public double CREDIT { get; set; }
        public double BALANCE { get; set; }
        public double TEAM_MEM_PPNO { get; set; }
        public string NAME { get; set; }

        public int GLSUBCODE { get; set; }


        }
    }
