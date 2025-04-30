using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
    {
    public class BranchModel
        {
        public int BRANCHID { get; set; }
        public int ZONEID { get; set; }
        public string BRANCHNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string ISACTIVE { get; set; }
        public string BRANCHCODE { get; set; }
        public int BRANCH_SIZE_ID { get; set; }
        [NotMapped]
        public string BRANCH_SIZE { get; set; }
        [NotMapped]
        public string ZONE_NAME { get; set; }
        }
    }
