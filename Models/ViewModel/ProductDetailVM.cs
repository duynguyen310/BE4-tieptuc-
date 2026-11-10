using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE4.Models.ViewModel
{
    public class ProductDetailVM
    {
        // Thông tin sản phẩm
        public Product Product { get; set; }

        // Số lượng đặt mua
        public int Quantity { get; set; } = 1;

        // Giá trị tạm tính
        public decimal EstimatedValue { get; set; }

        // Thông tin phân trang
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 3;

        // Danh sách 8 sản phẩm cùng danh mục
        public List<Product> RelatedProducts { get; set; }

        // Danh sách 8 sản phẩm bán chạy nhất cùng danh mục
        public IPagedList<Product> TopProducts { get; set; }
    }
}