using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class DraftDSAList
    {        
        public int ID { get; set; }
        public int ENTITY_ID { get; set; }
        public int AUDITED_BY { get; set; }
        public string DSA_BODY { get; set; }       
        public DateTime? CREATED_AT { get; set; }       
    }
}
