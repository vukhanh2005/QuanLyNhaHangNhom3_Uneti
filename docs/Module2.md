<!-- Vi Thái Học - 23103100054: Hướng dẫn Module 2 -->
# Module 2 — Quản lý và tra cứu bàn ăn

> Tích hợp main ngày 16/10/2026: đã nối giao diện Admin/Client tại `/BanAn`, giữ model món ăn và cấu hình LocalDB của main. Toàn bộ migration và snapshot cũ đã được xóa theo yêu cầu; các hướng dẫn migration bên dưới chỉ là lịch sử, không còn áp dụng. Database hiện có không bị thay đổi. Muốn khởi tạo database mới cần tạo migration mới từ model đã tích hợp; không áp dụng migration khởi tạo lên database đã có bảng. Kiểm thử tích hợp dùng `MODULE2_CURRENT_MODEL_TESTS=1` để tạo schema trong database test riêng.

> Cập nhật 14/10/2026: đã tích hợp Session và hai layout theo video. Xem [đối chiếu video, kiểm thử và hạn chế database hiện tại](Module2-Video-1013.md). Các kết quả 70 kiểm thử và trạng thái Git bên dưới là ghi nhận của lần tích hợp trước.

## 1. Kiến trúc và liên kết
Đã nối Controller/Razor với EF Core/SQL Server. Dùng nguyên LoaiBan của Module 1 và Session MaTaiKhoan (int), VaiTro (string Admin/User). Không sửa entity hoặc nghiệp vụ đăng nhập của nhóm. Chưa có PhieuDatBan để kiểm thử lịch sử đặt bàn.

## 2. File
Models/BanAn.cs, BanAnTrangThai.cs; ViewModels/BanAnFormViewModel.cs, BanAnIndexViewModel.cs; Queries/BanAnFormLogic.cs, BanAnQueryExtensions.cs, BanAnPagination.cs; Filters/Module2AdminAttribute.cs; Controllers/BanAnController.cs; Views/BanAn/*; Data/BanAnSeeder.cs; scripts/Test-Module2.ps1 và hai file Module2*Checks.cs.txt.

## 3. Entity và file dùng chung
BanAn giữ namespace hiện có; navigation LoaiBan dùng entity QuanLyNhaHang.Models.LoaiBan. FK Restrict, unique index TenBan, CHECK số chỗ > 0 và ba trạng thái hợp lệ.

FILE DÙNG CHUNG – CẦN TRAO ĐỔI VỚI NHÓM TRƯỚC KHI MERGE:
- AppDbContext.cs: chỉ thêm quan hệ/index/check BanAn.
- Program.cs: chỉ thêm chế độ seed gọi rõ ràng.
- Views/Shared/_Layout.cshtml: thêm liên kết Bàn ăn.
- Migration AddBanAn và snapshot.

## 4. ViewModel
Form chứa các trường được sửa và dropdown. MaBan chỉ phục vụ URL; POST Edit lấy id từ route và không đổi khóa chính entity. Index giữ điều kiện tra cứu và thông tin trang, PageSize = 5.

## 5. CRUD và phân quyền
GET Index/Details yêu cầu đăng nhập. Index trả view Admin hoặc Client theo Session; Details dùng layout tương ứng. GET/POST Create/Edit/Delete kiểm tra Admin tại máy chủ; POST có anti-forgery. Kiểm tra tên trùng bằng AnyAsync, Edit loại trừ chính id; kiểm tra LoaiBan tồn tại và tải lại dropdown khi lỗi. Unique index bảo vệ lưu trùng đồng thời. Edit cho phép đổi trạng thái. Delete GET không xóa; POST chặn bàn đang sử dụng và xử lý lỗi FK. Chưa có rowversion chống hai người sửa cùng lúc.

Khi Module 3 thêm PhieuDatBan: thêm navigation, FK Restrict và AnyAsync kiểm tra lịch sử trước Delete. Không tạo entity giả, không tự đổi trạng thái khi tạo phiếu.

## 6. Search
Trim keyword; Contains tên bàn hoặc vị trí. Null/rỗng không lọc. So sánh hoa/thường/dấu phụ thuộc SQL Server collation.

## 7. Filter
Loại bàn, số chỗ chính xác, trạng thái kết hợp bằng AND trên cùng IQueryable. Dropdown có Tất cả; số chỗ lấy distinct từ database.

## 8. Sort
name_asc, name_desc, seat_asc, seat_desc. Sort lạ về name_asc. ThenBy(MaBan) giữ thứ tự ổn định.

## 9. Pagination
Include(LoaiBan) → AsNoTracking → Search/Filter/Sort → CountAsync → giới hạn page → Skip/Take → ToListAsync. Trang âm về 1, quá lớn về cuối, rỗng quy ước 1/1. Không tải toàn bảng trước phân trang.

## 10. Index
Địa chỉ /BanAn; hiện tên loại bàn và các trường bắt buộc. Link trước/sau/số trang giữ keyword, maLoaiBan, soChoNgoi, trangThai, sortOrder. Tìm lại quay về trang 1; Reset bỏ điều kiện. Nút quản trị chỉ hiện với Admin.

## 11. View CRUD
Create/Edit dùng _Form với select/textarea/validation. Details/Delete dùng _Details. Delete có xác nhận POST. Thành công thông báo TempData. Form được bảo vệ bằng token chống giả mạo.

## 12. Migration và chạy
AddBanAn tạo LoaiBans và BanAns vì migration cũ chỉ có TaiKhoans; không sửa schema TaiKhoans. Nếu nhóm đã tạo hai bảng thủ công hoặc có migration mới, phải thống nhất baseline trước khi áp dụng, không xóa database để né conflict. Chưa áp dụng lên database chính.

Công cụ EF 10.0.4 đã cài cục bộ trong obj/module2-tools. Tại thư mục dự án:
```powershell
dotnet build QuanLyNhaHang.csproj
.\obj\module2-tools\dotnet-ef migrations list
.\obj\module2-tools\dotnet-ef database update
dotnet run --launch-profile http
```
Máy có dotnet-ef dùng `dotnet ef database update`; Visual Studio PMC dùng `Update-Database`. Migration đã được tạo, không tạo lại AddBanAn. Sau khi khởi động, mở /BanAn trên địa chỉ ứng dụng.

## 13. Seed
Tạo ít nhất một loại bàn hoạt động bằng Module 1 trước (nên có bốn loại theo đề), sau đó:
```powershell
dotnet run -- --seed-ban-an
```
Chỉ thêm Bàn 01..20 còn thiếu; dùng FK thật, 2/4/6/8 chỗ, năm vị trí và ba trạng thái. Không tạo tài khoản/loại bàn/phiếu trong database chính. Không tự seed khi chạy web.

## 14. Kiểm thử
```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Test-Module2.ps1
```
Chạy SQL Server LocalDB và HTTP:
```powershell
dotnet build QuanLyNhaHang.csproj --configuration Module2Check
$env:MODULE2_SQL_TESTS = '1'
$env:MODULE2_APP_DLL = (Resolve-Path bin/Module2Check/net10.0/QuanLyNhaHang.dll).Path
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Test-Module2.ps1
Remove-Item Env:MODULE2_SQL_TESTS
Remove-Item Env:MODULE2_APP_DLL
```
Cần LocalDB MSSQLLocalDB và cổng 15397 trống. Script tạo database QuanLyNhaHang_Module2Test_<GUID>, chạy migration, tạo fixture và xóa đúng database test trong finally. Không dùng database chính. Tài khoản test có mật khẩu ngẫu nhiên.

Đã đạt 31 kiểm tra logic + 21 SQL/Controller + 18 HTTP: CRUD thật, tên trùng, FK, validation, seed lặp lại, phân trang kết hợp, link giữ điều kiện, Session Admin/User, anti-forgery và không sửa mã qua form. Chưa kiểm thử PhieuDatBan hoặc quan sát giao diện bằng ảnh trình duyệt.

Checklist bảo vệ: thêm hợp lệ/thiếu tên/trùng tên; số chỗ 0/âm; sửa hợp lệ/trùng tên; xóa và xác nhận; bảo vệ lịch sử (chờ Module 3); tìm tên/vị trí/không có kết quả; ba filter riêng/kết hợp; bốn sort; trang đầu/giữa/cuối/trước/sau; tổ hợp search/filter/sort/page; giữ query; Reset; Admin vào Create và khách bị chặn Create/Edit/Delete trực tiếp.

## 15. Git
Nhánh Module2-VTH. Chưa commit/push lượt tích hợp này; giữ nguyên thay đổi chưa commit của người dùng ở BanAnQueryExtensions.cs. Gợi ý:
- 23103100054-ViThaiHoc.Lien ket BanAn voi LoaiBan va them migration
- 23103100054-ViThaiHoc.Them CRUD va giao dien tra cuu ban an
- 23103100054-ViThaiHoc.Them seed va kiem thu SQL Server HTTP
