using System;

namespace AIS.Models
{
    public class UpdateUserModel
    {
        
        public int USER_ID { get; set; }      
        public int ROLE_ID { get; set; }
        public string PASSWORD { get; set; }
        public string PPNO { get; set; }
        public string ISACTIVE { get; set; }

    }
}
