# Tích hợp TDGB vào main

Giữ AuthService qua IAuthService, cấu hình BanAn và cột MonAn.HinhAnh của main.
Đăng ký mới tạo TaiKhoan và KhachHang trong cùng SaveChanges. Tài khoản cũ không tự tạo hồ sơ thiếu thông tin.

Migration AddKhachHang nối tiếp InitialIntegrated; chỉ tạo bảng KhachHangs và khóa ngoại/index.
Sau khi cập nhật mã nguồn, chạy `dotnet ef database update` trên database đã dùng InitialIntegrated.
Không dùng migration Migra11 của nhánh cũ để tránh tạo trùng bảng.

Kiểm thử trên LocalDB riêng, không sửa database chính:
```powershell
dotnet build
$env:MODULE2_SQL_TESTS = '1'
$env:MODULE2_APP_DLL = (Resolve-Path bin/Debug/net10.0/QuanLyNhaHang.dll).Path
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Test-Module2.ps1
Remove-Item Env:MODULE2_SQL_TESTS, Env:MODULE2_APP_DLL
```
Bao gồm nâng cấp migration giữ ảnh món ăn, đăng ký hợp lệ/không hợp lệ/trùng tên,
CRUD món ăn, anti-forgery, phân quyền và hồi quy Module 2.
