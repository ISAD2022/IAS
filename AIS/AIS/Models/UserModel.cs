using System;

namespace AIS.Models
{
    public class UserModel
    {
        
        public int ID { get; set; }
        public string Name { get; set; }
        public string PPNumber { get; set; }
        public string Email { get; set; }
        public string IsActive { get; set; }
        public string UserLocationType { get; set; }
        public int? UserPostingAuditZone { get; set; }
        public int? UserPostingDiv { get; set; }
        public int? UserPostingDept { get; set; }
        public int? UserPostingBranch { get; set; }
        public int? UserPostingZone { get; set; }
        public int? UserGroupID { get; set; }
        public int? UserRoleID { get; set; }
    }
}
