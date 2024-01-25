using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
{
    public class UserRoleDetailAdminPanelModel
    {        
        public string ROLE_ID { get; set; }
        public string GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
    }
}

