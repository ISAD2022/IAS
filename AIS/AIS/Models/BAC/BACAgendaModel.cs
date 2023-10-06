using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class BACAgendaModel
    {        
        public int ID { get; set; }
        public string MENO_NO { get; set; }
        public string MEETING_NO { get; set; }
        public string REMARKS { get; set; }
        public string SUBJECT { get; set; }
     
    }
}
