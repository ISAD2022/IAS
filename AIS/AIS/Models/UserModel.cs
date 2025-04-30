namespace AIS.Models
    {
    public class UserModel
        {

        public int ID { get; set; }
        public string SessionId { get; set; }
        public string IPAddress { get; set; }
        public string MACAddress { get; set; }
        public string FirstMACCardAddress { get; set; }
        public string Name { get; set; }
        public string PPNumber { get; set; }
        public string Email { get; set; }
        public string IsActive { get; set; }
        public string UserLocationType { get; set; }
        public int? UserPostingAuditZone { get; set; }
        public string DivName { get; set; }
        public int? UserPostingDiv { get; set; }
        public int? RelationshipId { get; set; }
        public string DeptName { get; set; }
        public int? UserPostingDept { get; set; }
        public string BranchName { get; set; }
        public int? UserPostingBranch { get; set; }
        public string ZoneName { get; set; }
        public int? UserPostingZone { get; set; }
        public string UserRole { get; set; }
        public string UserGroup { get; set; }
        public int? UserGroupID { get; set; }
        public int? UserRoleID { get; set; }
        public int? UserEntityID { get; set; }
        public int? UserParentEntityID { get; set; }
        public int? UserEntityCode { get; set; }
        public int? UserParentEntityCode { get; set; }
        public string UserEntityName { get; set; }
        public string UserParentEntityName { get; set; }
        public int? UserEntityTypeID { get; set; }
        public int? UserParentEntityTypeID { get; set; }
        public string UserRoleName { get; set; }
        public string ErrorMsg { get; set; }
        public bool isAuthenticate { get; set; }
        public bool isAlreadyLoggedIn { get; set; }
        public bool passwordChangeRequired { get; set; }
        public string changePassword { get; set; }
        }
    }
