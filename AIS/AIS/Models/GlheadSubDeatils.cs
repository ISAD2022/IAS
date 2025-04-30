using System.Collections.Generic;

namespace AIS.Models
    {
    public class GlHeadSubDetailsModel
        {
        public int BRANCHID { get; set; }
        public int GLSUBCODE { get; set; }



        public string GLSUBNAME { get; set; }
        public string DESCRIPTION { get; set; }



        public double BALANCE { get; set; }
        public double DEBIT { get; set; }
        public double CREDIT { get; set; }
        public List<GlHeadSubDetailsModel> GL_SUBDETAILS { get; set; }

        }
    }