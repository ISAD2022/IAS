using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class LoanCaseDetailModel
        {
        public string Name { get; set; }
        public string Cnic { get; set; }
        public string LoanCaseNo { get; set; }
        public DateTime? DisbDate { get; set; }
        public DateTime? AppDate { get; set; }
        public DateTime? CadReceiveDate { get; set; }
        public DateTime? SanctionDate { get; set; }
        public decimal DisbursedAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string McoPpno { get; set; }
        public string ManagerPpno { get; set; }
        public string RgmPpno { get; set; }
        public string CadReviewerPreS { get; set; }
        public string SanctionedByPpno { get; set; }

        }
}
