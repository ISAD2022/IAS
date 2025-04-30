using System;

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
        public string McoPPNo { get; set; }
        public string McoName { get; set; }
        public string ManagerPPNo { get; set; }
        public string ManagerName { get; set; }
        public string RgmPPNo { get; set; }
        public string RgmName { get; set; }
        public string CadReviewerPPNo { get; set; }
        public string CadReviewerName { get; set; }
        public string CadAuthorizerPPNo { get; set; }
        public string CadAuthorizerName { get; set; }

        }
    }
