using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class ListOfSamplesModel
        {
        public int? ID { get; set; }
        public string SAMPLE_TYPE { get; set; }
        public string SAMPLE_PERCENTAGE { get; set; }       
    


    }
}
