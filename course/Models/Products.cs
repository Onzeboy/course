using static NuGet.Packaging.PackagingConstants;

namespace course.Models
{
    public class Products
    {
        public int ID { get; set; }
        public string NameP { get; set; }
        public int price { get; set; }
        public string? image { get; set; }
        public ICollection<Cart> Cart { get; } = new List<Cart>();
    }
}
