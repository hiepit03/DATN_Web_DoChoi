using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_DoChoi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChiNhanhs",
                columns: table => new
                {
                    PK_ChiNhanh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChiNhanh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GioiThieu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiNhanhs", x => x.PK_ChiNhanh);
                });

            migrationBuilder.CreateTable(
                name: "DanhMucs",
                columns: table => new
                {
                    PK_DanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucs", x => x.PK_DanhMuc);
                });

            migrationBuilder.CreateTable(
                name: "LienHes",
                columns: table => new
                {
                    PK_LienHe = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LienHes", x => x.PK_LienHe);
                });

            migrationBuilder.CreateTable(
                name: "MaKhuyenMais",
                columns: table => new
                {
                    PK_MaKhuyenMai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMaGiamGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaTienApDung = table.Column<int>(type: "int", nullable: false),
                    GiaTienGiam = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaKhuyenMais", x => x.PK_MaKhuyenMai);
                });

            migrationBuilder.CreateTable(
                name: "NhaSanXuats",
                columns: table => new
                {
                    PK_NSX = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNSX = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Visible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaSanXuats", x => x.PK_NSX);
                });

            migrationBuilder.CreateTable(
                name: "OrderInfos",
                columns: table => new
                {
                    PK_Order = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderInfor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FK_DonHang = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInfos", x => x.PK_Order);
                });

            migrationBuilder.CreateTable(
                name: "Slides",
                columns: table => new
                {
                    PK_Slide = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slides", x => x.PK_Slide);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    PK_TaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChucVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.PK_TaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "TinTucs",
                columns: table => new
                {
                    PK_TinTuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTinTuc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinTucs", x => x.PK_TinTuc);
                });

            migrationBuilder.CreateTable(
                name: "DoChois",
                columns: table => new
                {
                    PK_DoChoi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDoChoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnhDoChoi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GiaNhapHang = table.Column<int>(type: "int", nullable: false),
                    GiaBan = table.Column<int>(type: "int", nullable: false),
                    PhanTramKhuyenMai = table.Column<int>(type: "int", nullable: false),
                    SoLuongDaBan = table.Column<int>(type: "int", nullable: false),
                    SoLuongConLai = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayNhapHang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoTuoiGioiHan = table.Column<int>(type: "int", nullable: false),
                    SoSao = table.Column<float>(type: "real", nullable: false),
                    SoLuotDanhGia = table.Column<int>(type: "int", nullable: false),
                    FK_DanhMuc = table.Column<int>(type: "int", nullable: false),
                    FK_NhaSanXuat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoChois", x => x.PK_DoChoi);
                    table.ForeignKey(
                        name: "FK_DoChois_DanhMucs_FK_DanhMuc",
                        column: x => x.FK_DanhMuc,
                        principalTable: "DanhMucs",
                        principalColumn: "PK_DanhMuc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoChois_NhaSanXuats_FK_NhaSanXuat",
                        column: x => x.FK_NhaSanXuat,
                        principalTable: "NhaSanXuats",
                        principalColumn: "PK_NSX",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiaChis",
                columns: table => new
                {
                    PK_DiaChi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiaChiCuThe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDTNhanHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenNguoiNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MacDinh = table.Column<bool>(type: "bit", nullable: false),
                    FK_TaiKhoan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaChis", x => x.PK_DiaChi);
                    table.ForeignKey(
                        name: "FK_DiaChis_TaiKhoans_FK_TaiKhoan",
                        column: x => x.FK_TaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "PK_TaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DonHangs",
                columns: table => new
                {
                    PK_DonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThoiGianDatHang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThaiDonHang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GhiChuDonHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonViVanChuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianGiaoHangDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongGiaTri = table.Column<int>(type: "int", nullable: false),
                    LoiNhuan = table.Column<int>(type: "int", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiThanhToan = table.Column<bool>(type: "bit", nullable: false),
                    NguoiNhan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SDT = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FK_MaKhuyenMai = table.Column<int>(type: "int", nullable: false),
                    FK_TaiKhoan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHangs", x => x.PK_DonHang);
                    table.ForeignKey(
                        name: "FK_DonHangs_MaKhuyenMais_FK_MaKhuyenMai",
                        column: x => x.FK_MaKhuyenMai,
                        principalTable: "MaKhuyenMais",
                        principalColumn: "PK_MaKhuyenMai",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DonHangs_TaiKhoans_FK_TaiKhoan",
                        column: x => x.FK_TaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "PK_TaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GioHangs",
                columns: table => new
                {
                    PK_GioHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_TaiKhoan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GioHangs", x => x.PK_GioHang);
                    table.ForeignKey(
                        name: "FK_GioHangs_TaiKhoans_FK_TaiKhoan",
                        column: x => x.FK_TaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "PK_TaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DanhGias",
                columns: table => new
                {
                    PK_DanhGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoSaoDanhGia = table.Column<int>(type: "int", nullable: false),
                    LoiDanhGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianThem = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FK_TaiKhoan = table.Column<int>(type: "int", nullable: false),
                    FK_DoChoi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGias", x => x.PK_DanhGia);
                    table.ForeignKey(
                        name: "FK_DanhGias_DoChois_FK_DoChoi",
                        column: x => x.FK_DoChoi,
                        principalTable: "DoChois",
                        principalColumn: "PK_DoChoi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGias_TaiKhoans_FK_TaiKhoan",
                        column: x => x.FK_TaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "PK_TaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHangs",
                columns: table => new
                {
                    PK_ChiTietDonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoLuongSP = table.Column<int>(type: "int", nullable: false),
                    FK_DoChoi = table.Column<int>(type: "int", nullable: false),
                    FK_DonHang = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHangs", x => x.PK_ChiTietDonHang);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_DoChois_FK_DoChoi",
                        column: x => x.FK_DoChoi,
                        principalTable: "DoChois",
                        principalColumn: "PK_DoChoi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHangs_DonHangs_FK_DonHang",
                        column: x => x.FK_DonHang,
                        principalTable: "DonHangs",
                        principalColumn: "PK_DonHang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietGioHangs",
                columns: table => new
                {
                    PK_ChiTietGioHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoLuongSP = table.Column<int>(type: "int", nullable: false),
                    FK_DoChoi = table.Column<int>(type: "int", nullable: false),
                    FK_GioHang = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietGioHangs", x => x.PK_ChiTietGioHang);
                    table.ForeignKey(
                        name: "FK_ChiTietGioHangs_DoChois_FK_DoChoi",
                        column: x => x.FK_DoChoi,
                        principalTable: "DoChois",
                        principalColumn: "PK_DoChoi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietGioHangs_GioHangs_FK_GioHang",
                        column: x => x.FK_GioHang,
                        principalTable: "GioHangs",
                        principalColumn: "PK_GioHang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_FK_DoChoi",
                table: "ChiTietDonHangs",
                column: "FK_DoChoi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHangs_FK_DonHang",
                table: "ChiTietDonHangs",
                column: "FK_DonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGioHangs_FK_DoChoi",
                table: "ChiTietGioHangs",
                column: "FK_DoChoi");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietGioHangs_FK_GioHang",
                table: "ChiTietGioHangs",
                column: "FK_GioHang");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGias_FK_DoChoi",
                table: "DanhGias",
                column: "FK_DoChoi");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGias_FK_TaiKhoan",
                table: "DanhGias",
                column: "FK_TaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_DiaChis_FK_TaiKhoan",
                table: "DiaChis",
                column: "FK_TaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_DoChois_FK_DanhMuc",
                table: "DoChois",
                column: "FK_DanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_DoChois_FK_NhaSanXuat",
                table: "DoChois",
                column: "FK_NhaSanXuat");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_FK_MaKhuyenMai",
                table: "DonHangs",
                column: "FK_MaKhuyenMai");

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_FK_TaiKhoan",
                table: "DonHangs",
                column: "FK_TaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_GioHangs_FK_TaiKhoan",
                table: "GioHangs",
                column: "FK_TaiKhoan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiNhanhs");

            migrationBuilder.DropTable(
                name: "ChiTietDonHangs");

            migrationBuilder.DropTable(
                name: "ChiTietGioHangs");

            migrationBuilder.DropTable(
                name: "DanhGias");

            migrationBuilder.DropTable(
                name: "DiaChis");

            migrationBuilder.DropTable(
                name: "LienHes");

            migrationBuilder.DropTable(
                name: "OrderInfos");

            migrationBuilder.DropTable(
                name: "Slides");

            migrationBuilder.DropTable(
                name: "TinTucs");

            migrationBuilder.DropTable(
                name: "DonHangs");

            migrationBuilder.DropTable(
                name: "GioHangs");

            migrationBuilder.DropTable(
                name: "DoChois");

            migrationBuilder.DropTable(
                name: "MaKhuyenMais");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "DanhMucs");

            migrationBuilder.DropTable(
                name: "NhaSanXuats");
        }
    }
}
