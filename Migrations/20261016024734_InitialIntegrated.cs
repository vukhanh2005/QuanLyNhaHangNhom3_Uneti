using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang.Migrations
{
    /// <inheritdoc />
    public partial class InitialIntegrated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiBans",
                columns: table => new
                {
                    MaLoaiBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiBans", x => x.MaLoaiBan);
                });

            migrationBuilder.CreateTable(
                name: "MonAn",
                columns: table => new
                {
                    MaMon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LoaiMon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonAn", x => x.MaMon);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VaiTro = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "BanAns",
                columns: table => new
                {
                    MaBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaLoaiBan = table.Column<int>(type: "int", nullable: false),
                    SoChoNgoi = table.Column<int>(type: "int", nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BanAns", x => x.MaBan);
                    table.CheckConstraint("CK_BanAns_SoChoNgoi", "[SoChoNgoi] > 0");
                    table.CheckConstraint("CK_BanAns_TrangThai", "[TrangThai] IN (N'Sẵn sàng', N'Đang sử dụng', N'Tạm ngừng phục vụ')");
                    table.ForeignKey(
                        name: "FK_BanAns_LoaiBans_MaLoaiBan",
                        column: x => x.MaLoaiBan,
                        principalTable: "LoaiBans",
                        principalColumn: "MaLoaiBan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BanAns_MaLoaiBan",
                table: "BanAns",
                column: "MaLoaiBan");

            migrationBuilder.CreateIndex(
                name: "IX_BanAns_TenBan",
                table: "BanAns",
                column: "TenBan",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BanAns");

            migrationBuilder.DropTable(
                name: "MonAn");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "LoaiBans");
        }
    }
}
