namespace AIS.Models
    {
    public class COSORiskModel
        {
        public string AUDIT_PERIOD { get; set; }
        public string DEPT_NAME { get; set; }
        public string RATING_FACTORS { get; set; }
        public int SUB_FACTORS { get; set; }
        public int MAX_SCORE { get; set; }
        public int FINAL_SCORE { get; set; }
        public int WEIGHT_ASSIGNED { get; set; }
        public int NO_OF_OBSERVATIONS { get; set; }
        public int WEIGHTED_AVERAGE_SCORE { get; set; }
        public string AUDIT_RATING { get; set; }
        public string FINAL_AUDIT_RATING { get; set; }
        public string STATUS { get; set; }
        }
    }
