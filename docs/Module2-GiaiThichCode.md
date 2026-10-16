<!-- Họ và tên: Vi Thái Học | Mã sinh viên: 23103100054
     Nội dung thực hiện: Hướng dẫn đọc và giải thích logic Module 2. -->
# Cách giải thích Module 2

## 1. Bắt đầu từ một yêu cầu của người dùng

Người dùng bấm Bàn ăn → trình duyệt gửi yêu cầu đến BanAnController.Index.
Trước khi chạy action, Module2Session kiểm tra mã tài khoản và vai trò trong Session.
Chưa đăng nhập thì chuyển đến Login. Vai trò không hợp lệ thì trả lỗi 403 (không có quyền).

## 2. Danh sách bàn ăn

Index nhận điều kiện tìm kiếm từ URL và đưa vào BanAnIndexViewModel.
Include(LoaiBan) lấy thông tin loại bàn để hiển thị tên thay vì chỉ có mã.
BanAnPagination gọi TraCuu trong BanAnQueryExtensions để tìm kiếm, lọc và sắp xếp.

- Tìm kiếm: tên bàn **hoặc** vị trí chứa từ khóa.
- Lọc: các Where nối tiếp nhau có nghĩa là phải thỏa mãn **đồng thời** loại bàn, số chỗ và trạng thái được chọn.
- Sắp xếp: switch chọn tên tăng/giảm hoặc số chỗ tăng/giảm. ThenBy(MaBan) giữ thứ tự ổn định khi hai bàn có cùng giá trị sắp xếp.
- Phân trang: CountAsync đếm kết quả đã lọc; giới hạn trang từ 1 đến trang cuối; Skip bỏ qua các bàn của trang trước, Take lấy số bàn của trang hiện tại.
- Ví dụ: 12 kết quả, mỗi trang 5 bàn → 3 trang. Trang 2 bỏ 5 bàn đầu và lấy 5 bàn tiếp theo.

IQueryable là truy vấn chưa lấy dữ liệu về. ToListAsync chỉ chạy sau Skip/Take, nên database trả về dữ liệu của một trang, không tải toàn bộ rồi ẩn trên trình duyệt.
Index chọn view Admin hoặc Client dựa vào Session. Hai view dùng chung partial _List để không lặp lại phần tra cứu.
Các liên kết phân trang mang theo bộ lọc và kiểu sắp xếp trên URL.

## 3. Thêm và sửa

GET Create/Edit mở form và lấy danh sách loại bàn từ database.
POST Create/Edit nhận dữ liệu được nhập, kiểm tra ModelState và các quy tắc nghiệp vụ:

- Tên không được trống/trùng, số chỗ phải dương, vị trí bắt buộc, trạng thái hợp lệ.
- BanAnFormLogic loại bỏ khoảng trắng thừa và tìm tên trùng bằng AnyAsync.
- Khi sửa, bỏ qua chính mã bàn đang sửa trong truy vấn kiểm tra trùng tên.
- ValidateFormAsync kiểm tra loại bàn tồn tại.
- ApplyTo chỉ chép các trường được phép sửa; không chép mã bàn từ form vào khóa chính.

Hợp lệ thì SaveChangesAsync lưu database và chuyển về danh sách. Có lỗi thì giữ dữ liệu đã nhập, nạp lại dropdown và hiển thị lỗi.
Unique index vẫn bảo vệ tên bàn nếu hai người lưu cùng một tên vào cùng thời điểm.

## 4. Xóa

GET Delete chỉ hiện trang xác nhận. POST Delete mới thực hiện xóa.
Bàn đang sử dụng không được xóa. Nếu database báo bàn có dữ liệu liên quan, thông báo cho người dùng và không xóa.
Kiểm tra lịch sử PhieuDatBan còn chờ tích hợp Module 3.

## 5. Phân quyền và bảo vệ dữ liệu

Module2Session kiểm tra đăng nhập cho toàn bộ controller. Module2Admin kiểm tra quyền cho từng thao tác quản lý.
Ẩn nút ở giao diện chỉ giúp người dùng nhìn đúng chức năng; filter ở máy chủ mới ngăn khách tự gọi URL hoặc gửi POST.
ValidateAntiForgeryToken kiểm tra token của form để chống gửi yêu cầu giả mạo.
CancellationToken cho phép hủy công việc khi yêu cầu bị hủy; async/await giúp chờ database mà không giữ luồng xử lý bận.

## Thứ tự đọc code khi bảo vệ

1. Filters/Module2SessionAttribute.cs và Module2AdminAttribute.cs: ai được truy cập/làm gì.
2. Controllers/BanAnController.cs: nhận yêu cầu và điều phối.
3. Queries/BanAnQueryExtensions.cs: tìm kiếm, lọc, sắp xếp.
4. Queries/BanAnPagination.cs: đếm, giới hạn trang, Skip/Take.
5. Queries/BanAnFormLogic.cs: chuẩn hóa, kiểm tra tên, chép dữ liệu form.
6. ViewModels và Views/BanAn: dữ liệu truyền ra giao diện và cách hiển thị.

Đợt chỉnh này giữ nguyên URL, quy tắc nghiệp vụ, truy vấn tại database và giao diện Admin/Client; chỉ viết rõ các bước xử lý để dễ đọc hơn.
