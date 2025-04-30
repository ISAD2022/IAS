namespace AIS.Models
    {
    public class RiskRatingModelForBranchesWorking
        {

        public string MainProcessRiskSequence { get; set; }
        public string MainProcess { get; set; }
        public string MainProcessWeightAssigned { get; set; }
        public string SubProcessRiskSequence { get; set; }
        public string SubProcess { get; set; }
        public string SubProcessWeightAssigned { get; set; }
        public string High { get; set; }
        public string Medium { get; set; }
        public string Low { get; set; }
        public string TotalNoOfTest { get; set; }
        public string AvailableWeightedScore { get; set; }
        public string AvailableProcessScore { get; set; }
        public string TotalHigh { get; set; }
        public string TotalMedium { get; set; }
        public string TotalLow { get; set; }
        public string TotalObservations { get; set; }
        public string TotalScoreSubProcess { get; set; }
        public string WeightedAverageScore { get; set; }
        public string TotalScoreProcess { get; set; }
        public string WeightedAverageScoreOverall { get; set; }



        }
    }
