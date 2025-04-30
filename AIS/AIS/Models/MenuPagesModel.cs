namespace AIS.Models
    {
    public class MenuPagesModel
        {
        public int Id { get; set; }
        public int Menu_Id { get; set; }
        public string Page_Name { get; set; }
        public string Page_Path { get; set; }
        public int Page_Order { get; set; }
        public string Status { get; set; }
        public string Sub_Menu { get; set; }
        public string Sub_Menu_Id { get; set; }
        public string Sub_Menu_Name { get; set; }
        public int Hide_Menu { get; set; }
        }
    }
