using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web_DoChoi.Models;
using Web_DoChoi.Repository;

namespace Web_DoChoi.Controllers
{
    public class CollectionController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<CollectionController> _logger;

        public CollectionController(ILogger<CollectionController> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public IActionResult TimKiemSP(
                string? tuKhoa,
                int? danhMucId,
                int? nhaSanXuatId,
                List<string>? doTuoi,  // từ checkbox, ví dụ: "0-2", "2-6", "18+"
                List<string>? khoangGia,
                int page = 1)  // Thêm tham số page với giá trị mặc định là 1 cho phân trang
        {
            int pageSize = 12; // Mỗi trang có 12 sản phẩm

            var query = _dataContext.DoChois.AsQueryable();

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(sp => sp.TenDoChoi.Contains(tuKhoa));
            }

            // Lọc theo danh mục
            if (danhMucId.HasValue)
            {
                query = query.Where(sp => sp.FK_DanhMuc == danhMucId.Value);
            }

            // Lọc theo nhà sản xuất
            if (nhaSanXuatId.HasValue)
            {
                query = query.Where(sp => sp.FK_NhaSanXuat == nhaSanXuatId.Value);
            }

            // Lọc độ tuổi
            if (doTuoi != null && doTuoi.Any())
            {
                IQueryable<DoChoiModel> ageFilteredQuery = null;

                foreach (var ageRange in doTuoi)
                {
                    var ageParts = ageRange.Split('-');
                    if (ageParts.Length == 2)
                    {
                        if (int.TryParse(ageParts[0], out int startAge) && int.TryParse(ageParts[1], out int endAge))
                        {
                            var filteredByAge = query.Where(sp => sp.DoTuoiGioiHan >= startAge && sp.DoTuoiGioiHan <= endAge);

                            // Kết hợp kết quả lọc độ tuổi với nhau
                            ageFilteredQuery = ageFilteredQuery == null ? filteredByAge : ageFilteredQuery.Union(filteredByAge);
                        }
                    }
                }

                // Thay thế query gốc bằng query đã lọc độ tuổi
                if (ageFilteredQuery != null)
                {
                    query = ageFilteredQuery.Distinct(); // Lọc bỏ trùng
                }
            }

            // Lọc khoảng giá
            if (khoangGia != null && khoangGia.Any())
            {
                query = query.Where(sp =>
                    (khoangGia.Contains("0-300") && sp.GiaBan <= 300000) ||
                    (khoangGia.Contains("300-500") && sp.GiaBan > 300000 && sp.GiaBan <= 500000) ||
                    (khoangGia.Contains("500-700") && sp.GiaBan > 500000 && sp.GiaBan <= 700000) ||
                    (khoangGia.Contains("700-1000") && sp.GiaBan > 700000 && sp.GiaBan <= 1000000) ||
                    (khoangGia.Contains("1000+") && sp.GiaBan > 1000000)
                );
            }

            // Tính tổng số sản phẩm
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize); // Tổng số trang

            // Lấy sản phẩm cho trang hiện tại
            var sanPhams = query
                .Skip((page - 1) * pageSize)  // Bỏ qua các sản phẩm của các trang trước
                .Take(pageSize)  // Lấy số sản phẩm cho trang hiện tại
                .ToList();

            // Truyền thông tin vào ViewBag
            ViewBag.TuKhoa = tuKhoa;
            ViewBag.DanhMucId = danhMucId;
            ViewBag.NhaSanXuatId = nhaSanXuatId;
            ViewBag.DoTuoi = doTuoi;
            ViewBag.KhoangGia = khoangGia;
            ViewBag.SoLuongSP = sanPhams.Count;
            ViewBag.TotalPages = totalPages;  // Tổng số trang
            ViewBag.CurrentPage = page;  // Trang hiện tại

            return View(sanPhams); // Hoặc PartialView nếu chỉ muốn trả về một phần của view
        }



    }
}
