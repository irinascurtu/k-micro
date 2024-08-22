using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        [MinLength(2)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }

        public decimal FullPrice { get => Price * 1.19M; }
    }
}
