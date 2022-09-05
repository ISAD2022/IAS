using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InspectionSolution.Models
{
    public class EmailCredentailsModel
    {        
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
