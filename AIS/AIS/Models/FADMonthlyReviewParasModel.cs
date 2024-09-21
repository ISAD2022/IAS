using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class FADMonthlyReviewParasModel
    {
        public string REPORTING_OFFICE {  get; set; }
        public string PLACE_OF_POSTING {  get; set; }
        public string CHILD_CODE {  get; set; }
        public string OPENING_BALANCE {  get; set; }
        public string PARA_ADDED {  get; set; }
        public string TOTAL {  get; set; }
        public string SETTLED {  get; set; }
        public string OUTSTANDING {  get; set; } 

    }
}
