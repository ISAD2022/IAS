using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class AuditeeOldParasPpnoModel
    {        
        public int? ID { get; set; }
        public string REF_P { get; set; }
        public string ENTITY_NAME { get; set; }
         public int? AUDIT_PERIOD { get; set; }
        public int? PARA_NO { get; set; }
        public string GIST_OF_PARAS { get; set; }
        public string VOL_I_II { get; set; }
        public string AMOUNT_INVOLVED { get; set; }
        public int? PARA_STATUS { get; set; }
       
      
    }
}
