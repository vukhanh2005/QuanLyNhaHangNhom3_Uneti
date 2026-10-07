<!-- Họ và tên: Vi Thái Học
Mã sinh viên: 23103100054
Nội dung thực hiện: Phân tích và hướng dẫn Module 2. -->

# Module 2 — Quản lý và tra cứu bàn ăn

## 1. Phân tích tài liệu và liên kết module

Nguồn: đề tài Word trong thư mục gốc, mục 6, 9–17, 20–21.
Module 2 chịu trách nhiệm CRUD bàn ăn, cập nhật trạng thái, tìm theo tên/vị trí,
lọc loại/số chỗ/trạng thái, bốn kiểu sắp xếp và phân trang kết hợp.
Module 1 sở hữu LoaiBan, đăng nhập và Session; Module 3 sở hữu PhieuDatBan;
Module 4 quản lý quá trình sử dụng bàn.

Trạng thái triển khai: CHƯA HOÀN THÀNH.
Repository chưa có LoaiBan và đăng nhập chưa ghi Session.
Chưa tạo entity thay thế vì yêu cầu của chủ dự án không cho phép tự tạo LoaiBan.
Chưa tạo migration hoặc thay đổi database.
Phạm vi đã được người dùng xác nhận: chỉ làm phần không cần LoaiBan.
Không tạo entity LoaiBan hoặc Controller CRUD bỏ qua kiểm tra khóa ngoại.

## 2. Cấu trúc

Đã bổ sung:
- Models/BanAnTrangThai.cs
- ViewModels/BanAnFormViewModel.cs
- ViewModels/BanAnIndexViewModel.cs
- Queries/BanAnQueryExtensions.cs
- Queries/BanAnPagination.cs
- Queries/BanAnFormLogic.cs
- Filters/Module2AdminAttribute.cs
- Views/BanAn/_Form.cshtml, Create.cshtml, Edit.cshtml
- scripts/Test-Module2.ps1, scripts/Module2Checks.cs.txt

Đã cập nhật validation Models/BanAn.cs.
Còn cần BanAnController, Index/Details/Delete View, cấu hình quan hệ,
migration và seed sau khi xác định entity LoaiBan dùng chung.

## 3. Entity và quan hệ

BanAn giữ namespace hiện có để không phá tham chiếu.
Tên và vị trí bắt buộc, dài tối đa 100; mô tả nullable và tối đa 500;
số chỗ và mã loại lớn hơn 0; trạng thái chỉ nhận ba giá trị hợp lệ.
Sau khi có LoaiBan chính thức, thêm navigation LoaiBan và khóa ngoại MaLoaiBan
với DeleteBehavior.Restrict. Tên bàn cần unique index ở database để chống
trùng khi hai yêu cầu lưu đồng thời, bên cạnh AnyAsync trong Controller.

PhieuDatBan chưa tồn tại: chỉ ghi chú navigation, không tạo entity của Module 3.
Khi tích hợp cần FK PhieuDatBan.MaBan với Restrict và AnyAsync trước khi xóa.
Nếu có lịch sử đặt bàn thì chặn xóa, hướng dẫn chuyển sang Tạm ngừng phục vụ.

## 4. ViewModel

BanAnFormViewModel chỉ chứa các trường được chỉnh sửa, không nhận MaBan.
BanAnIndexViewModel giữ điều kiện tra cứu, kết quả, tổng bản ghi và trang hiện tại.
PageSize = 5; danh sách trống quy ước trang 1/1.

## 5. CRUD và phân quyền

Controller chưa triển khai vì cần LoaiBan chính thức.
BanAnFormLogic đã cung cấp Normalize, ValidateNameAsync, ApplyTo và FromEntity.
ValidateNameAsync dùng AnyAsync, loại trừ currentId khi sửa và ghi lỗi vào ModelState.
ApplyTo chỉ cập nhật trường được phép, giữ nguyên MaBan. Đây là hàm hỗ trợ,
chưa được gọi qua endpoint CRUD. Controller tương lai phải gọi validation đầy đủ,
kiểm tra LoaiBan tồn tại và ModelState.IsValid trước khi ApplyTo/SaveChangesAsync.
AnyAsync không thay thế unique index: khi tích hợp migration cần thêm ràng buộc
tên duy nhất và xử lý lỗi lưu đồng thời.
Dự kiến GET/POST Create, Edit, Delete có Module2Admin và POST có anti-forgery.
Mã bàn lấy từ route và dùng tìm entity hiện có; không cập nhật khóa chính.
ModelState invalid phải tải lại danh sách loại bàn.

Hợp đồng Session dự kiến:
- MaTaiKhoan: SetInt32, số nguyên dương.
- VaiTro: SetString("Admin") hoặc SetString("User").
Cần đối chiếu với Module 1 trước khi tích hợp.
Module2AdminAttribute hiện trả HTTP 403 nếu không đủ quyền.
Không có endpoint giả lập đăng nhập hoặc tự cấp quyền Admin.

## 6. Search

TraCuu dùng Contains với TenBan hoặc ViTri và tự Trim từ khóa trước khi lọc.
Không tải toàn bộ database.

## 7. Filter

Các Where nối tiếp trên cùng IQueryable để điều kiện kết hợp bằng AND.
Lọc số chỗ là khớp chính xác, không phải số chỗ tối thiểu.

## 8. Sort

name_asc, name_desc, seat_asc, seat_desc.
ThenBy(MaBan) đảm bảo thứ tự ổn định giữa các trang khi số chỗ bằng nhau.

## 9. Pagination

BanAnPagination.LoadPageAsync đã thực hiện AsNoTracking, TraCuu,
CountAsync, giới hạn page trong 1..TotalPages, Skip/Take rồi ToListAsync.
Controller sau này truyền query Include(LoaiBan) vào hàm này.
Chưa tích hợp Controller/View hoặc kiểm thử SQL Server; giữ query string trên giao diện chưa hoàn thành.

## 10. Index

Cần hiển thị MaBan, TenBan, LoaiBan.TenLoaiBan, SoChoNgoi, ViTri, TrangThai.
Form GET chứa keyword, maLoaiBan, soChoNgoi, trangThai, sortOrder.
Liên kết phân trang phải mang đủ năm điều kiện. Reset dẫn về Index không query.
Nút quản trị chỉ hiện khi Session hợp lệ nhưng Controller vẫn phải kiểm tra quyền.

## 11. Các view CRUD

Create/Edit dùng partial _Form, asp-for, asp-items, validation message,
textarea và validation scripts.
Details/Delete chưa triển khai, chờ quan hệ LoaiBan.

## 12. Migration

FILE DÙNG CHUNG – CẦN TRAO ĐỔI VỚI NHÓM TRƯỚC KHI MERGE.

Không tạo migration từ model thiếu entity đã có trong snapshot:
Snapshot hiện có TaiKhoan và AppDbContext đã đăng ký TaiKhoans.
Phải giữ đăng ký này để migration không vô tình sinh DropTable(TaiKhoans).
Phải thống nhất model dùng chung, lấy migration mới nhất của nhóm,
kiểm tra Up/Down trước khi update database.

Sau khi giải quyết liên kết:
```powershell
dotnet ef migrations add AddBanAn
dotnet ef database update
```
Máy hiện chưa có lệnh dotnet ef; có thể dùng Package Manager Console trong
Visual Studio với Add-Migration và Update-Database sau khi model hoàn chỉnh.

## 13. Seed

Cần ít nhất 20 bàn, 2/4/6/8 chỗ, nhiều vị trí và cả ba trạng thái.
Lấy MaLoaiBan từ dữ liệu thật trong database, không hard-code FK chưa tồn tại.
Seed phải chạy lại được mà không nhân đôi dữ liệu.
Không seed tài khoản hoặc phiếu đặt bàn thuộc module khác.

## 14. Kiểm thử

Chạy kiểm tra phần độc lập:
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Test-Module2.ps1
```
31 kiểm tra đã đạt: Data Annotation, trạng thái Sẵn sàng, từ khóa có khoảng trắng,
query kết hợp trên dữ liệu bộ nhớ,
bốn kiểu sort, biên phân trang và hợp đồng Session.
Đã kiểm tra chuẩn hóa form, giữ mã bàn khi cập nhật và dịch LINQ sang SQL Server
bằng ToQueryString (WHERE/ORDER BY/OFFSET/FETCH NEXT).
Không kết nối database; chưa kiểm thử thực thi LoadPageAsync, AnyAsync hoặc CRUD HTTP.

Checklist tích hợp còn cần thực hiện:
1. Thêm hợp lệ: lưu và về Index.
2. Thiếu tên: lỗi tại form.
3. Trùng tên: không lưu.
4. Số chỗ 0: lỗi.
5. Số chỗ âm: lỗi.
6. Sửa hợp lệ và giữ nguyên MaBan.
7. Xóa bàn chưa có quan hệ.
8. Bàn có phiếu: chặn xóa, lịch sử còn nguyên.
9. Tìm tên tồn tại.
10. Tên không tồn tại: thông báo rỗng.
11. Tìm vị trí.
12. Lọc loại.
13. Lọc số chỗ.
14. Lọc trạng thái.
15. Kết hợp ba bộ lọc.
16. Tên A–Z.
17. Tên Z–A.
18. Số chỗ tăng.
19. Số chỗ giảm.
20. Trang đầu không có Previous hoạt động.
21. Trang giữa.
22. Trang cuối không có Next hoạt động.
23. Previous về đúng trang.
24. Next đến đúng trang.
25. Search + Filter.
26. Search + Sort.
27. Filter + Sort.
28. Search + Filter + Sort + Pagination.
29. Chuyển trang giữ đủ điều kiện.
30. Reset xóa điều kiện.
31. Admin truy cập Create.
32. Khách truy cập Create: 403.
33. Khách truy cập Edit trực tiếp: 403.
34. Khách truy cập Delete trực tiếp: 403.

Bổ sung: POST thiếu anti-forgery, loại không tồn tại, trạng thái giả,
tên toàn khoảng trắng, page âm/quá lớn, hai yêu cầu tạo trùng đồng thời.

## 15. Git

Làm trên Module2-VTH, không sửa main. Các phiên bản trước đã commit/push;
những thay đổi của lượt triển khai hiện tại chưa commit/push.
Mã sinh viên: 23103100054. Gợi ý chia commit theo phần việc:
- [MaSV] [BanAn] Them validation va trang thai ban
- [MaSV] [BanAn] Them ViewModel va truy van tra cuu
- [MaSV] [BanAn] Them phan quyen va giao dien form
- [MaSV] [BanAn] Hoan thien CRUD va quan he du lieu
- [MaSV] [BanAn] Them phan trang migration va du lieu mau
- [MaSV] [BanAn] Kiem thu va huong dan tich hop

