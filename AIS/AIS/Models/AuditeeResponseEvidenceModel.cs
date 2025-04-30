namespace AIS.Models
    {
    public class AuditeeResponseEvidenceModel
        {
        public int? TEXT_ID { get; set; }
        public int? COM_ID { get; set; }
        public int? SEQUENCE { get; set; }
        public int LENGTH { get; set; }
        public long IMAGE_LENGTH { get; set; }
        public string IMAGE_TYPE { get; set; }
        public string IMAGE_DATA { get; set; }
        public string IMAGE_NAME { get; set; }
        public string FILE_NAME { get; set; }
        public bool COVER_IMAGE { get; set; }
        public string RESPONSE_ID { get; set; }
        public string FILE_ID { get; set; }


        }
    }
