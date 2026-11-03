<!-- Họ và tên: Vi Thái Học | Mã sinh viên: 23103100054
     Nội dung thực hiện: Đối chiếu video và tích hợp Module 2 vào giao diện của nhóm. -->
# Module 2 — Tích hợp theo video 1013.mp4

Nguồn: `D:/1013.mp4`, dài khoảng 14 phút 05 giây. Các mốc là khoảng thời gian; lời nói được đối chiếu với màn hình minh họa, không phải bản chép nguyên văn.

| Mốc video | Yêu cầu | Thực hiện |
| --- | --- | --- |
| 00:00–00:26 | Khách xem danh sách, tìm kiếm/phân trang; dùng hai layout của nhóm | View Client dùng ClientLayout; Admin dùng AdminLayout qua _ViewStart |
| 00:40–02:30 | Nút Bàn ăn gọi BanAnController.Index; đọc Session, chưa đăng nhập về Login; đọc VaiTro | Nối menu hai layout vào Index; Module2Session bảo vệ toàn controller |
| 04:46–06:52 | Nội dung nằm trong RenderBody; cùng Index nhưng trả view theo vai trò | Index trả Admin hoặc Client cùng dữ liệu đã phân trang; partial _List dùng chung phần tra cứu |
| 07:01–09:05 | Kiểm tra Session, phân nhánh Admin/khách | Admin vào view Admin, User vào Client; Session thiếu hoặc vai trò không hợp lệ bị chặn |
| 12:04–14:05 | View khách dùng ClientLayout, hiện danh sách/tìm kiếm/phân trang; controller chọn view | Khách xem danh sách/chi tiết, không có nút CRUD; quyền Admin kiểm tra tại máy chủ cho cả GET và POST |

## Luồng sử dụng

1. Truy cập `/BanAn` khi chưa đăng nhập → `/Account/Login`.
2. Đăng nhập bằng chức năng hiện có của nhóm, chọn Bàn ăn.
3. Admin được thêm, sửa, xóa, đổi trạng thái qua form Sửa và tra cứu.
4. User được xem danh sách/chi tiết, tìm tên/vị trí, lọc loại bàn/số chỗ/trạng thái, sắp xếp và phân trang.
5. Chuyển trang giữ điều kiện; đăng xuất rồi truy cập lại yêu cầu đăng nhập.

Không tạo tài khoản/đăng nhập mới hoặc thêm nghiệp vụ đặt bàn/thanh toán. Không sửa entity, repository, service, controller Module 1.

## Các điểm tích hợp

- `Filters/Module2SessionAttribute.cs`: yêu cầu tài khoản và vai trò hợp lệ từ Session.
- `Controllers/BanAnController.cs`: gắn filter, chọn view Admin/Client.
- `Views/BanAn/Admin.cshtml`, `Client.cshtml`, `_ViewStart.cshtml`, `_List.cshtml`: tách giao diện theo vai trò, chuyển nội dung Index cũ sang partial; URL giữ nguyên.
- Các view Create/Edit/Delete dùng AdminLayout; Details chọn layout theo Session.
- `Views/Shared/AdminLayout.cshtml`, `ClientLayout.cshtml`: nối menu Bàn ăn.
- `Views/TrangChu/Admin.cshtml`, `Client.cshtml`: nối liên kết từ trang chủ.
- `wwwroot/css/ban-an-client.css`: CSS chỉ áp dụng trong .ban-an-client, không sửa CSS chung.

## Kiểm chứng 14/10/2026

Build Module2Check thành công, không lỗi; 28 cảnh báo nullable có sẵn ở phần tài khoản.
36 kiểm tra logic/phân quyền, 23 kiểm tra SQL/Controller và 32 kiểm tra HTTP đạt trên database test riêng.
HTTP kiểm tra đăng nhập thật qua PasswordHasher, hai layout, tra cứu/phân trang, chuyển Login, chặn GET/POST khách với token hợp lệ, CRUD Admin và đăng xuất.
Chưa kiểm tra trực quan bằng trình duyệt trên nhiều kích thước màn hình.

### Hạn chế database hiện tại

Kiểm thử migration mặc định dừng ở PendingModelChangesWarning: model không khớp snapshot. AddBanAn và NVKMigration cũng cùng tạo bảng LoaiBans; nhóm cần thống nhất lại lịch sử migration. Lượt này không sửa/xóa migration đã có.

TaiKhoan.MatKhau có StringLength(50), trong khi PasswordHasher tạo chuỗi dài hơn. Phần tài khoản cần xử lý độ dài cột cùng migration; đăng nhập ứng dụng thật phụ thuộc schema/dữ liệu thực tế.

Để kiểm chứng riêng Module 2, chế độ bổ sung tạo schema từ model trong database GUID tạm và nới cột mật khẩu **chỉ trong database test**. Chế độ này không chứng minh migration hợp lệ và không thay đổi database chính.

```powershell
dotnet build QuanLyNhaHang.csproj --configuration Module2Check
$env:MODULE2_SQL_TESTS = '1'
$env:MODULE2_CURRENT_MODEL_TESTS = '1'
$env:MODULE2_APP_DLL = (Resolve-Path bin/Module2Check/net10.0/QuanLyNhaHang.dll).Path
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Test-Module2.ps1
Remove-Item Env:MODULE2_SQL_TESTS, Env:MODULE2_CURRENT_MODEL_TESTS, Env:MODULE2_APP_DLL
```

Bỏ MODULE2_CURRENT_MODEL_TESTS để kiểm tra migration mặc định sau khi nhóm xử lý baseline. Database test được xóa trong finally.
