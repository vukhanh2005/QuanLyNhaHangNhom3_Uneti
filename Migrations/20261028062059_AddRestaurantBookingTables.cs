using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang.Migrations
{
    public partial class AddRestaurantBookingTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhieuDatBans",
                columns: table => new
                {
                    MaPhieuDatBan = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    MaKhachHang = table.Column<int>(
                        type: "int",
                        nullable: false),

                    MaBan = table.Column<int>(
                        type: "int",
                        nullable: false),

                    NgayGioDat = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    SoNguoi = table.Column<int>(
                        type: "int",
                        nullable: false),

                    TrangThai = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),

                    GhiChu = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true),

                    NgayTao = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_PhieuDatBans",
                        x => x.MaPhieuDatBan);

                    table.ForeignKey(
                        name: "FK_PhieuDatBans_BanAns_MaBan",
                        column: x => x.MaBan,
                        principalTable: "BanAns",
                        principalColumn: "MaBan",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_PhieuDatBans_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDatBans_MaBan",
                table: "PhieuDatBans",
                column: "MaBan");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDatBans_MaKhachHang",
                table: "PhieuDatBans",
                column: "MaKhachHang");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhieuDatBans");
        }
    }
}
