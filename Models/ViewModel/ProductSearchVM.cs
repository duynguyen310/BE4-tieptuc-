using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BE4.Models.ViewModel
{
    public class ProductSearchVM
    {
        public string SearchTerm { get; set; }

        // Khoảng giá tìm kiếm
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Tiêu chí sắp xếp
        public string SortOrder { get; set; }

        // Thông tin phân trang
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Danh sách sản phẩm kết quả
        public IPagedList<Product> Products { get; set; }
    }
}