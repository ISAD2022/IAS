using System;
using System.Collections.Generic;

namespace AIS.Models
    {
    public class ObservationResponseModel
        {
        public int ID { get; set; }
        public int AU_OBS_ID { get; set; }
        public string REPLY { get; set; }
        public int REPLIEDBY { get; set; }
        public DateTime REPLIEDDATE { get; set; }
        public int LASTUPDATEDBY { get; set; }
        public DateTime LASTUPDATEDDATE { get; set; }
        public int OBS_TEXT_ID { get; set; }
        public int REPLY_ROLE { get; set; }
        public string REMARKS { get; set; }
        public string SUBMITTED { get; set; }
        public List<AuditeeResponseEvidenceModel> EVIDENCE_LIST { get; set; }

        }
    }
