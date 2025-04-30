namespace AIS.Models
    {
    public class CDMSMasterTransactionModel
        {

        public string TRANSACTION_ID { get; set; }
        public string ENTITY_NAME { get; set; }
        public string OLD_ACCOUNT_NO { get; set; }
        public string CNIC { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string TR_MASTER_CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string REMARKS { get; set; }
        public string TRANSACTION_DATE { get; set; }
        public string AUTHORIZATION_DATE { get; set; }
        public string? DR_AMOUNT { get; set; }
        public string? CR_AMOUNT { get; set; }
        public string? TO_ACCOUNT_ID { get; set; }
        public string TO_ACCOUNT_TITLE { get; set; }
        public string TO_ACCOUNT_NO { get; set; }
        public string? TO_ACC_BRANCH_ID { get; set; }
        public string INSTRUMENT_NO { get; set; }
        }
    }
