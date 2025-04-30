using System.ComponentModel.DataAnnotations.Schema;

namespace AIS.Models
    {
    public class SubEntitiesModel
        {
        public int ID { get; set; }
        public string NAME { get; set; }
        public int DIV_ID { get; set; }
        public int DEP_ID { get; set; }
        public string STATUS { get; set; }
        [NotMapped]
        public string Division_Name { get; set; }
        [NotMapped]
        public string Department_Name { get; set; }

        }
    }
