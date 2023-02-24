using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace AIS.Models
{
    public class DraftReportSummaryModel
    {
        public int Total { get; set; }
        public int High { get; set; }
        public int Medium { get; set; }
        public int Low { get; set; }
        public int Settled { get; set; }
        public int Dropped { get; set; }
        public int AddtoDraft { get; set; }
        public string ReportName { get; set; }
    }
}
