namespace AIS.Models
    {
    public class LoanCaseSampleTransactionsModel
        {

        public string DESCRIPTION { get; set; }
        public string MANUAL_VOUCHER_NO { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public decimal DR_AMOUNT { get; set; }
        public decimal CR_AMOUNT { get; set; }
        public string LN_ACCOUNT_ID { get; set; }
        public string CREATED_ON { get; set; }
        public string REMARKS { get; set; }
        public string REJECTION_DATE { get; set; }
        public string REVERSAL_DATE { get; set; }
        public string WORKING_DATE { get; set; }
        public string AUTHORIZATION_DATE { get; set; }
        public string MCO_RECEIPT_NO { get; set; }
        public string MCO_BOOK_NO { get; set; }

        }
    }
