using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AuditeeResponseEvidenceModel
    {        
        public int? OBS_ID { get; set; }
        public int? OBS_TEXT_ID { get; set; }
        public int? SEQUENCE { get; set; }
        public int LENGTH { get; set; }
        public string IMAGE_TYPE { get; set; }
        public string IMAGE_DATA { get; set; }
        public string IMAGE_NAME { get; set; }
        public bool COVER_IMAGE { get; set; }
        public string PARA_REF { get; set; }
        public string RESPONSE_ID { get; set; }


    }
}
