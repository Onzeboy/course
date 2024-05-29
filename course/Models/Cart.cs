using static NuGet.Packaging.PackagingConstants;

namespace course.Models
{
    public class Cart
    {
        public int ID { get; set; }
        public string userID { get; set; } 
        public int? ProductsHistoryID { get; set; }
        public ProductsHistory? ProductsHistory { get; set; }
    }
}
