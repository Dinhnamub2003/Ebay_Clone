using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Model.ProductModel
{
    public class ViewCreateProductModel
    {
        [Required]
        public string ProductName { get; set; } = null!;


        [Required(ErrorMessage = "Description must not empty.")]

        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        public string? BrandName { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int? CategoryId { get; set; }  // Đảm bảo CategoryId có thể là null khi không chọn


        // Lưu danh sách ảnh upload
        public List<IFormFile>? Images { get; set; }
    }
}
