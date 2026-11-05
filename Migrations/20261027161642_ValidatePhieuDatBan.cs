using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyNhaHang.Migrations
{
    /// <inheritdoc />
    public partial class ValidatePhieuDatBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhieuDatBans_MaBan",
                table: "PhieuDatBans");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySuDung",
                table: "PhieuDatBans",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDatBans_MaBan_NgaySuDung",
                table: "PhieuDatBans",
                columns: new[] { "MaBan", "NgaySuDung" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_PhieuDatBans_SoNguoi",
                table: "PhieuDatBans",
                sql: "[SoNguoi] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PhieuDatBans_ThoiGian",
                table: "PhieuDatBans",
                sql: "[GioKetThuc] > [GioBatDau]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PhieuDatBans_TrangThai",
                table: "PhieuDatBans",
                sql: "[TrangThai] IN (0, 1, 2, 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhieuDatBans_MaBan_NgaySuDung",
                table: "PhieuDatBans");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PhieuDatBans_SoNguoi",
                table: "PhieuDatBans");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PhieuDatBans_ThoiGian",
                table: "PhieuDatBans");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PhieuDatBans_TrangThai",
                table: "PhieuDatBans");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySuDung",
                table: "PhieuDatBans",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuDatBans_MaBan",
                table: "PhieuDatBans",
                column: "MaBan");
        }
    }
}
