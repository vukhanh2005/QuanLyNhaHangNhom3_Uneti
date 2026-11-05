<!-- Họ và tên: Vi Thái Học | Mã sinh viên: 23103100054
     Nội dung thực hiện: Hướng dẫn chống ghi đè bàn ăn khi nhiều Admin sửa đồng thời. -->
# Chống ghi đè khi sửa bàn ăn

Sử dụng optimistic concurrency với cột SQL Server `rowversion` (không phải ngày giờ).

1. Khi mở Edit, hệ thống lấy RowVersion của bàn và đưa vào hidden input dưới dạng Base64.
2. Khi lưu, server kiểm tra token có đủ 8 byte và còn khớp dữ liệu hiện tại hay không.
3. EF dùng token cũ làm OriginalValue: câu UPDATE có điều kiện cả MaBan và RowVersion.
4. Nếu người khác cập nhật sau khi kiểm tra nhưng trước khi ghi, UPDATE không sửa được dòng nào và EF phát sinh DbUpdateConcurrencyException.
5. Khi xung đột, nội dung form vẫn được giữ để người dùng xem lại; không tự thay token hoặc tự ghi đè. Bấm “Tải lại dữ liệu mới nhất” để mở lại form; thao tác này bỏ các thay đổi chưa lưu.

## Cách trình diễn

- Hai Admin (hoặc hai cửa sổ) mở cùng một bàn ở trang Sửa.
- Admin A đổi mô tả và lưu thành công.
- Admin B đổi mô tả từ form đã mở trước đó và bấm Lưu.
- B nhận thông báo bàn đã được cập nhật; dữ liệu A vẫn còn nguyên.
- B tải dữ liệu mới nhất, kiểm tra lại và sửa tiếp nếu cần.

## File liên quan

- Models/BanAn.cs: Timestamp/RowVersion do database sinh.
- ViewModels/BanAnFormViewModel.cs và Queries/BanAnFormLogic.cs: truyền phiên bản vào form.
- Views/BanAn/_Form.cshtml: hidden input và liên kết tải lại.
- Controllers/BanAnController.cs: kiểm tra phiên bản và đặt OriginalValue trước SaveAsync.
- Migration AddBanAnRowVersion và snapshot: chỉ thêm cột vào BanAns.

## Database

Ngày 05/11/2026 đã sao lưu và áp dụng riêng migration AddBanAnRowVersion lên localhost/QuanLyNhaHang_VTH.
Database này vẫn có lịch sử migration cũ từ trước khi nhóm gộp migration; không đánh dấu giả rằng các migration cũ/mới khác đã được áp dụng.
Với database của nhóm có lịch sử migration đồng bộ, chạy database update như bình thường để nhận cột mới. Không chạy lại toàn bộ InitialIntegrated trên database cũ đã có bảng.

## Kiểm thử

Bộ kiểm tra Module2SqlChecks có các tình huống: hai form cùng phiên bản, giữ dữ liệu của người lưu trước, không tự làm mới token khi lỗi, token thiếu/sai, hai DbContext cạnh tranh tại SQL Server, tải lại rồi lưu thành công và POST HTTP với form cũ.
Chế độ MODULE2_CURRENT_MODEL_TESTS tạo database thử nghiệm riêng từ model; không kiểm chứng toàn bộ lịch sử migration. Database thử nghiệm được xóa sau khi chạy.
