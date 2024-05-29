namespace course.Models
{
    public class History
    {
        public int ID { get; set; }
        public string userID { get; set; }
        public string employer { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public DateTime orderdate { get; set; }
        public string adresscity { get; set; }
        public string adressstreet { get; set; }
        public string adresshome { get; set; }
    }
}
