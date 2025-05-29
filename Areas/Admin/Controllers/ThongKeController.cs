using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Rotativa.AspNetCore;
using System.Security.Cryptography.X509Certificates;
using Web_DoChoi.Repository;
using Web_DoChoi.ViewModel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Web_DoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]  // Chỉ cho phép người dùng có role "admin" truy cập
    public class ThongKeController : Controller
    {   
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ThongKeController(DataContext dataContext, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = dataContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Khởi tạo viewmodel
            var vm = new ThongKeTongQuanViewModel();
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var total = await _dataContext.DonHangs
                            .Where(d => d.ThoiGianDatHang >= today && d.ThoiGianDatHang < tomorrow && d.TrangThaiDonHang == "Giao Hàng Thành Công")
                            .SumAsync(d => (int?)d.TongGiaTri);

            vm.doanhthu = total.HasValue ? total.Value : 0;

            vm.sldonhang = await _dataContext.DonHangs
                                .CountAsync(d => d.ThoiGianDatHang >= today && d.ThoiGianDatHang < tomorrow && d.TrangThaiDonHang == "Giao Hàng Thành Công");
            var total2 = await _dataContext.ChiTietDonHangs
            .Where(ct => ct.DonHang.ThoiGianDatHang >= today && ct.DonHang.ThoiGianDatHang < tomorrow && ct.DonHang.TrangThaiDonHang == "Giao Hàng Thành Công")
            .SumAsync(ct => (int?)ct.SoLuongSP);

            vm.soluongspdaban = total2 ?? 0;

            vm.soluongkhachhanh = await _dataContext.TaiKhoans.CountAsync();

            // 4. Tạo dữ liệu cho biểu đồ 8 ngày gần nhất
            vm.ChartDataPoints = new List<ChartDataPointViewModel>();
            for (int i = 7; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var nextDate = date.AddDays(1);

                // Tính vốn: tổng (Giá nhập * SL) từ chi tiết đơn hàng cùng ngày
                var von = await _dataContext.ChiTietDonHangs
                    .Where(ct => ct.DonHang.ThoiGianDatHang >= date && ct.DonHang.ThoiGianDatHang < nextDate && ct.DonHang.TrangThaiDonHang == "Giao Hàng Thành Công")
                    .SumAsync(ct => ct.DoChoi.GiaNhapHang * ct.SoLuongSP);

                // Tính doanh thu: tổng TổngTien từ đơn hàng cùng ngày
                var doanhThu = await _dataContext.DonHangs
                    .Where(d => d.ThoiGianDatHang >= date && d.ThoiGianDatHang < nextDate && d.TrangThaiDonHang == "Giao Hàng Thành Công")
                    .SumAsync(d => d.TongGiaTri);

                vm.ChartDataPoints.Add(new ChartDataPointViewModel
                {
                    DateLabel = date,
                    Von = (int)von,
                    DoanhThu = (int)doanhThu,
                    LoiNhuan = (int)(doanhThu - von)
                });
            }

            vm.Topdochoibanchay = await _dataContext.DoChois
                    .OrderByDescending(dc => dc.SoLuongDaBan)
                    .Take(10)
                    .ToListAsync();

            return View(vm);
        }

        public async Task<IActionResult> TonKho(string keyword, DateTime? fromDate, DateTime? toDate, string export, string filter)
        {
            // Gán điều kiện lọc cho ViewBag
            ViewBag.Keyword = keyword;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd"); // format để binding vào input[type=date]
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");


            // Nếu export != null => xử lý xuất PDF / Excel
            if (export == "pdf")
            {
                return RedirectToAction("XuatTonKhoPDF", new
                {
                    keyword,
                    fromDate = fromDate?.ToString("yyyy-MM-dd"),
                    toDate = toDate?.ToString("yyyy-MM-dd")
                });
            }
            else if (export == "excel")
            {
                return RedirectToAction("XuatExcel", new
                {
                    keyword,
                    fromDate = fromDate?.ToString("yyyy-MM-dd"),
                    toDate = toDate?.ToString("yyyy-MM-dd")
                });
            }

            // Còn nếu là filter hoặc làm mới (bấm nút "Làm mới")
            var query = _dataContext.DoChois.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(dc => dc.TenDoChoi.Contains(keyword));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(dc => dc.NgayNhapHang >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(dc => dc.NgayNhapHang <= toDate.Value);
            }

            var result = await query.ToListAsync();
            return View(result);
        }

        [HttpGet]
        public IActionResult XuatTonKhoPDF(string keyword, DateTime? fromDate, DateTime? toDate)
        {
            var query = _dataContext.DoChois.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(dc => dc.TenDoChoi.Contains(keyword));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(dc => dc.NgayNhapHang >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(dc => dc.NgayNhapHang <= toDate.Value);
            }

            var result = query.ToList();

            return new ViewAsPdf("XuatTonKhoPDF", result)
            {
                FileName = "BaoCao_TonKho.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        public async Task<IActionResult> XuatExcel(string keyword, DateTime? fromDate, DateTime? toDate)
        {
            var query = _dataContext.DoChois.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(dc => dc.TenDoChoi.Contains(keyword));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(dc => dc.NgayNhapHang >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(dc => dc.NgayNhapHang <= toDate.Value);
            }

            var result = await query.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Tồn Kho");

                // Header
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Mã SP";
                worksheet.Cell(1, 3).Value = "Tên sản phẩm";
                worksheet.Cell(1, 4).Value = "Nhập";
                worksheet.Cell(1, 5).Value = "Đã bán";
                worksheet.Cell(1, 6).Value = "Tồn";

                // Style cho header
                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Đổ dữ liệu
                int row = 2;
                int stt = 1;
                foreach (var item in result)
                {
                    var nhap = item.SoLuongConLai + item.SoLuongDaBan;
                    worksheet.Cell(row, 1).Value = stt++;
                    worksheet.Cell(row, 2).Value = item.PK_DoChoi;
                    worksheet.Cell(row, 3).Value = item.TenDoChoi;
                    worksheet.Cell(row, 4).Value = nhap;
                    worksheet.Cell(row, 5).Value = item.SoLuongDaBan;
                    worksheet.Cell(row, 6).Value = item.SoLuongConLai;
                    row++;
                }

                // Tự động giãn cột
                worksheet.Columns().AdjustToContents();

                // Xuất file
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    var fileName = $"TonKho_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }

        //doanh thu theo nam
        public IActionResult DoanhThu(int? fromYear, int? toYear, string export = "none")
        {
            int from = fromYear ?? 2020;
            int to = toYear ?? DateTime.Now.Year;

            // Lấy thống kê theo năm có dữ liệu
            var thongKe = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang.Year >= from && dh.ThoiGianDatHang.Year <= to && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Year)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(g.Key, 1, 1),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToList();

            // ✅ Bổ sung các năm không có đơn hàng (thêm với giá trị 0)
            for (int year = from; year <= to; year++)
            {
                if (!thongKe.Any(x => x.DateLabel.Year == year))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(year, 1, 1),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            // Sắp xếp theo năm
            thongKe = thongKe.OrderBy(x => x.DateLabel).ToList();

            // Gửi thông tin lọc xuống view
            ViewBag.FromYear = from;
            ViewBag.ToYear = to;

            // Xử lý xuất Excel hoặc PDF nếu cần
            if (export == "excel")
            {
                return RedirectToAction("XuatDoanhThuTheoNamExcel", new { fromYear, toYear });
            }
            else if (export == "pdf")
            {
                return RedirectToAction("XuatDoanhThuTheoNamPDF", new
                {
                    fromYear = from,
                    toYear = to
                });
            }

            return View(thongKe);
        }

        public IActionResult XuatDoanhThuTheoNamPDF(int fromYear = 2020, int toYear = 0)
        {
            if (toYear == 0) toYear = DateTime.Now.Year;

            var fromDate = new DateTime(fromYear, 1, 1);
            var toDate = new DateTime(toYear, 12, 31);

            // Lấy dữ liệu trong khoảng năm
            var donHangs = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang >= fromDate && dh.ThoiGianDatHang <= toDate && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .ToList(); // EF sẽ xử lý xong ở đây

            // Nhóm theo năm và tính toán
            var thongKe = donHangs
                .GroupBy(dh => dh.ThoiGianDatHang.Year)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(g.Key, 1, 1),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                })
                .ToList();

            // Bổ sung các năm không có đơn hàng (năm trống)
            for (int year = fromYear; year <= toYear; year++)
            {
                if (!thongKe.Any(x => x.DateLabel.Year == year))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(year, 1, 1),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            // Sắp xếp theo năm
            thongKe = thongKe.OrderBy(x => x.DateLabel.Year).ToList();

            ViewBag.FromYear = fromYear;
            ViewBag.ToYear = toYear;

            return new ViewAsPdf("XuatDoanhThuTheoNamPDF", thongKe)
            {
                FileName = $"BaoCao_DoanhThu_{fromYear}_{toYear}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        public async Task<IActionResult> XuatDoanhThuTheoNamExcel(int? fromYear, int? toYear)
        {
            int from = fromYear ?? 2020;
            int to = toYear ?? DateTime.Now.Year;

            var thongKe = await _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang.Year >= from && dh.ThoiGianDatHang.Year <= to && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Year)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(g.Key, 1, 1),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToListAsync();

            // Bổ sung năm không có đơn
            for (int year = from; year <= to; year++)
            {
                if (!thongKe.Any(x => x.DateLabel.Year == year))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(year, 1, 1),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            thongKe = thongKe.OrderBy(x => x.DateLabel.Year).ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DoanhThuNam");

                // Header
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Năm";
                worksheet.Cell(1, 3).Value = "Vốn (₫)";
                worksheet.Cell(1, 4).Value = "Doanh thu (₫)";
                worksheet.Cell(1, 5).Value = "Lợi nhuận (₫)";

                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // Data
                int row = 2;
                int stt = 1;
                foreach (var item in thongKe)
                {
                    worksheet.Cell(row, 1).Value = stt++;
                    worksheet.Cell(row, 2).Value = item.DateLabel.Year;
                    worksheet.Cell(row, 3).Value = item.Von;
                    worksheet.Cell(row, 4).Value = item.DoanhThu;
                    worksheet.Cell(row, 5).Value = item.LoiNhuan;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    var fileName = $"DoanhThuNam_{from}_{to}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
        }

        //doanh thu theo thang
        public IActionResult DoanhThuTheoThang(int? year, string export = "none")
        {
            int selectedYear = year ?? DateTime.Now.Year;

            // Lấy thống kê theo tháng trong năm có dữ liệu
            var thongKe = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang.Year == selectedYear && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Month)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(selectedYear, g.Key, 1),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToList();

            // ✅ Bổ sung các tháng không có đơn hàng (thêm với giá trị 0)
            for (int month = 1; month <= 12; month++)
            {
                if (!thongKe.Any(x => x.DateLabel.Month == month))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(selectedYear, month, 1),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            // Sắp xếp theo tháng
            thongKe = thongKe.OrderBy(x => x.DateLabel.Month).ToList();

            // Gửi thông tin năm xuống view
            ViewBag.Year = selectedYear;

            // Xử lý xuất Excel hoặc PDF nếu cần
            if (export == "excel")
            {
                return RedirectToAction("XuatDoanhThuTheoThangExcel", new { year = selectedYear });
            }
            else if (export == "pdf")
            {
                return RedirectToAction("XuatDoanhThuTheoThangPDF", new { year = selectedYear });
            }

            return View(thongKe);
        }

        public async Task<IActionResult> XuatDoanhThuTheoThangExcel(int year)
        {
            var data = await _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang.Year == year && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Month)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(year, g.Key, 1),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToListAsync();

            // Bổ sung các tháng còn thiếu
            for (int month = 1; month <= 12; month++)
            {
                if (!data.Any(x => x.DateLabel.Month == month))
                {
                    data.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(year, month, 1),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            data = data.OrderBy(x => x.DateLabel.Month).ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("DoanhThuTheoThang");

                ws.Cell(1, 1).Value = "Tháng";
                ws.Cell(1, 2).Value = "Vốn";
                ws.Cell(1, 3).Value = "Doanh thu";
                ws.Cell(1, 4).Value = "Lợi nhuận";

                var header = ws.Range("A1:D1");
                header.Style.Font.Bold = true;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.DateLabel.Month;
                    ws.Cell(row, 2).Value = item.Von;
                    ws.Cell(row, 3).Value = item.DoanhThu;
                    ws.Cell(row, 4).Value = item.LoiNhuan;
                    row++;
                }

                ws.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"DoanhThu_Thang_{year}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        public IActionResult XuatDoanhThuTheoThangPDF(int year)
        {
            var data = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang.Year == year && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Month)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(year, g.Key, 1),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToList();

            for (int month = 1; month <= 12; month++)
            {
                if (!data.Any(x => x.DateLabel.Month == month))
                {
                    data.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(year, month, 1),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            data = data.OrderBy(x => x.DateLabel.Month).ToList();

            ViewBag.Year = year;

            return new ViewAsPdf("XuatDoanhThuTheoThangPDF", data)
            {
                FileName = $"BaoCao_DoanhThu_Thang_{year}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        //doanh thu theo ngay
        public IActionResult DoanhThuTheoNgay(int? month, int? year, string export = "none")
        {
            int selectedYear = year ?? DateTime.Now.Year;
            int selectedMonth = month ?? DateTime.Now.Month;

            // Lấy thống kê theo ngày trong tháng có dữ liệu
            var thongKe = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang.Year == selectedYear && dh.ThoiGianDatHang.Month == selectedMonth && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Day)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(selectedYear, selectedMonth, g.Key),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToList();

            // Bổ sung các ngày thiếu nếu không có dữ liệu
            var daysInMonth = DateTime.DaysInMonth(selectedYear, selectedMonth);
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (!thongKe.Any(x => x.DateLabel.Day == day))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(selectedYear, selectedMonth, day),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            // Sắp xếp theo ngày
            thongKe = thongKe.OrderBy(x => x.DateLabel.Day).ToList();

            // Gửi thông tin năm và tháng xuống view
            ViewBag.Year = selectedYear;
            ViewBag.Month = selectedMonth;

            // Xử lý xuất Excel hoặc PDF nếu cần
            if (export == "excel")
            {
                return XuatExcelTheoNgay(selectedYear, selectedMonth);
            }
            else if (export == "pdf")
            {
                return XuatDoanhThuTheoNgayPDF(selectedYear, selectedMonth);
            }
            return View(thongKe);
        }


        public IActionResult XuatExcelTheoNgay(int year, int month)
        {
            // Lấy thống kê theo ngày trong tháng
            var thongKe = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang.Year == year && dh.ThoiGianDatHang.Month == month && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Day)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(year, month, g.Key),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToList();

            // Bổ sung các ngày thiếu nếu không có dữ liệu
            var daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (!thongKe.Any(x => x.DateLabel.Day == day))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(year, month, day),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("DoanhThuTheoNgay");

                // Header
                ws.Cell(1, 1).Value = "Ngày";
                ws.Cell(1, 2).Value = "Vốn";
                ws.Cell(1, 3).Value = "Doanh thu";
                ws.Cell(1, 4).Value = "Lợi nhuận";

                // Đổ dữ liệu vào Excel
                int row = 2;
                foreach (var item in thongKe)
                {
                    ws.Cell(row, 1).Value = item.DateLabel.Day; // Ngày
                    ws.Cell(row, 2).Value = item.Von;           // Vốn
                    ws.Cell(row, 3).Value = item.DoanhThu;      // Doanh thu
                    ws.Cell(row, 4).Value = item.LoiNhuan;      // Lợi nhuận
                    row++;
                }

                // Tự động giãn cột
                ws.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"DoanhThuTheoNgay_{year}_{month}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        public IActionResult XuatDoanhThuTheoNgayPDF(int year, int month)
        {
            // Lấy thống kê theo ngày trong tháng
            var thongKe = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang.Year == year && dh.ThoiGianDatHang.Month == month && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Day)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = new DateTime(year, month, g.Key),
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToList();

            // Bổ sung các ngày thiếu nếu không có dữ liệu
            var daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (!thongKe.Any(x => x.DateLabel.Day == day))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = new DateTime(year, month, day),
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            // Sắp xếp theo ngày
            thongKe = thongKe.OrderBy(x => x.DateLabel.Day).ToList();

            // Gửi thông tin năm và tháng xuống view
            ViewBag.Year = year;
            ViewBag.Month = month;

            return new ViewAsPdf("XuatDoanhThuTheoNgayPDF", thongKe)
            {
                FileName = $"DoanhThuTheoNgay_{year}_{month}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        public IActionResult DoanhThuTheoNgayDenNgay(DateTime? startDate, DateTime? endDate, string export = "none")
        {
            DateTime selectedStartDate = startDate ?? DateTime.Now.AddDays(-7);  // Default to the past 7 days
            DateTime selectedEndDate = endDate ?? DateTime.Now;  // Default to today

            // Lấy thống kê từ ngày đến ngày
            var thongKe = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang >= selectedStartDate && dh.ThoiGianDatHang <= selectedEndDate && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Date) // Group by date
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = g.Key,
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                })
                .ToList();

            // ✅ Bổ sung các ngày không có đơn hàng (thêm với giá trị 0)
            for (DateTime date = selectedStartDate; date <= selectedEndDate; date = date.AddDays(1))
            {
                if (!thongKe.Any(x => x.DateLabel.Date == date.Date))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = date,
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            // Sắp xếp theo ngày
            thongKe = thongKe.OrderBy(x => x.DateLabel).ToList();

            // Gửi thông tin ngày bắt đầu và kết thúc xuống view
            ViewBag.StartDate = selectedStartDate;
            ViewBag.EndDate = selectedEndDate;

            if (export == "excel")
            {
                return XuatExcelTheoNgayDenNgay(selectedStartDate, selectedEndDate);
            }
            else if (export == "pdf")
            {
                return XuatDoanhThuTheoNgayDenNgayPDF(selectedStartDate, selectedEndDate);
            }

            return View(thongKe);
        }

        public IActionResult XuatExcelTheoNgayDenNgay(DateTime startDate, DateTime endDate)
        {
            var thongKe = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang >= startDate && dh.ThoiGianDatHang <= endDate && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Date)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = g.Key,
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToList();

            // Bổ sung ngày không có đơn hàng
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (!thongKe.Any(x => x.DateLabel.Date == date.Date))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = date,
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            thongKe = thongKe.OrderBy(x => x.DateLabel).ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("DoanhThu");

                ws.Cell(1, 1).Value = "Ngày";
                ws.Cell(1, 2).Value = "Vốn";
                ws.Cell(1, 3).Value = "Doanh thu";
                ws.Cell(1, 4).Value = "Lợi nhuận";

                int row = 2;
                foreach (var item in thongKe)
                {
                    ws.Cell(row, 1).Value = item.DateLabel.ToString("dd/MM/yyyy");
                    ws.Cell(row, 2).Value = item.Von;
                    ws.Cell(row, 3).Value = item.DoanhThu;
                    ws.Cell(row, 4).Value = item.LoiNhuan;
                    row++;
                }

                ws.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"DoanhThu_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
        }

        public IActionResult XuatDoanhThuTheoNgayDenNgayPDF(DateTime startDate, DateTime endDate)
        {
            var thongKe = _dataContext.DonHangs
                .Where(dh => dh.ThoiGianDatHang >= startDate && dh.ThoiGianDatHang <= endDate && dh.TrangThaiDonHang == "Giao Hàng Thành Công")
                .GroupBy(dh => dh.ThoiGianDatHang.Date)
                .Select(g => new ChartDataPointViewModel
                {
                    DateLabel = g.Key,
                    Von = g.Sum(d => d.TongGiaTri - d.LoiNhuan),
                    DoanhThu = g.Sum(d => d.TongGiaTri),
                    LoiNhuan = g.Sum(d => d.LoiNhuan)
                }).ToList();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (!thongKe.Any(x => x.DateLabel.Date == date.Date))
                {
                    thongKe.Add(new ChartDataPointViewModel
                    {
                        DateLabel = date,
                        Von = 0,
                        DoanhThu = 0,
                        LoiNhuan = 0
                    });
                }
            }

            thongKe = thongKe.OrderBy(x => x.DateLabel).ToList();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return new ViewAsPdf("XuatDoanhThuTheoNgayDenNgayPDF", thongKe)
            {
                FileName = $"DoanhThu_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

    }
}
