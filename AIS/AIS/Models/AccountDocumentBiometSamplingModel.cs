namespace AIS.Models
    {
    public class AccountDocumentBiometSamplingModel
        {
        public string OldAccountNo { get; set; }
        public string PageNo { get; set; }
        public string Name { get; set; }
        public byte[] DocImage { get; set; } // Assuming document image is stored as binary data
        public string DocRemarks { get; set; }
        }
    }
