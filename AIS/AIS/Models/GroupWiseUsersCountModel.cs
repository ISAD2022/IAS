using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class GroupWiseUsersCountModel
    {        
        public string G_ID { get; set; }
        public string G_NAME { get; set; }       
        public string U_COUNT { get; set; }
       

    }
}
