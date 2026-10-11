// Họ và tên: Vi Thái Học
// Mã sinh viên: 23103100054
// Nội dung thực hiện: Tạo migration tích hợp bàn ăn với loại bàn, khóa ngoại và ràng buộc dữ liệu.
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang.Migrations
{
    /// <inheritdoc />
    public partial class AddBanAn : Migration
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
                name: "LoaiBans");
        }
    }
}
