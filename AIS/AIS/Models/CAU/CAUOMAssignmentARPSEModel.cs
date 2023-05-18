using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class CAUOMAssignmentARPSEModel
    {        
        public int ID { get; set; }
        public string OM_NO { get; set; }
        public int PARA_ID { get; set; }
        public DateTime PAC_DATES { get; set; }
        public string REPORT_FREQUENCY { get; set; }
        public string CONTENTS_OF_OM { get; set; }        
        public string STATUS { get; set; }
        public string STATUS_DES { get; set; }
        public string KEY { get; set; }
    }
}
