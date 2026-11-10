using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BE4.Models;
using PagedList;

namespace BE4.Models.ViewModel
{
    public class HomeProductVM
    {
        // Thông tin tìm kiếm
        public string SearchTerm { get; set; }

        // Thông tin phân trang
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        // Danh sách 10 sản phẩm bán chạy nhất
        public List<Product> FeaturedProducts { get; set; }

        // Danh sách 20 sản phẩm ít người mua nhất
        public IPagedList<Product> NewProducts { get; set; }
    }
}