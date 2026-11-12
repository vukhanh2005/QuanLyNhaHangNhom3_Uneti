-- Chon dung database cua ung dung trong SSMS truoc khi chay.
-- Cau hinh Development hien tai: QuanLyNhaHang_VTH.
-- Them 5 loai ban (2..6 cho), moi loai 2 ban; khong xoa du lieu cu.
-- Ma tu tang va RowVersion do SQL Server sinh, khong gan thu cong.
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Loai TABLE (SoCho int PRIMARY KEY, TenLoaiBan nvarchar(100));
    INSERT INTO @Loai (SoCho, TenLoaiBan)
    VALUES (2, N'Bàn 2 chỗ'), (3, N'Bàn 3 chỗ'), (4, N'Bàn 4 chỗ'),
           (5, N'Bàn 5 chỗ'), (6, N'Bàn 6 chỗ');

    INSERT INTO dbo.LoaiBans (TenLoaiBan, MoTa, TrangThai)
    SELECT l.TenLoaiBan, CONCAT(N'Loại bàn dành cho ', l.SoCho, N' người.'), 1
    FROM @Loai AS l
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.LoaiBans WITH (UPDLOCK, HOLDLOCK)
        WHERE TenLoaiBan = l.TenLoaiBan
    );
    DECLARE @LoaiDaThem int = @@ROWCOUNT;

    -- Dung ma loai thuc te, khong gia dinh MaLoaiBan bang so cho.
    DECLARE @Ban TABLE (
        TenBan nvarchar(100), MaLoaiBan int, SoChoNgoi int, ViTri nvarchar(100)
    );
    INSERT INTO @Ban (TenBan, MaLoaiBan, SoChoNgoi, ViTri)
    SELECT CONCAT(N'Bàn ', l.SoCho, N' chỗ - 0', n.STT),
           existing.MaLoaiBan, l.SoCho,
           CASE n.STT WHEN 1 THEN N'Tầng 1' ELSE N'Tầng 2' END
    FROM @Loai AS l
    CROSS APPLY (
        SELECT TOP (1) MaLoaiBan
        FROM dbo.LoaiBans
        WHERE TenLoaiBan = l.TenLoaiBan
        ORDER BY MaLoaiBan
    ) AS existing
    CROSS JOIN (VALUES (1), (2)) AS n(STT);

    -- Neu ten mau da duoc dung cho ban khac, dung lai thay vi sua du lieu cu.
    IF EXISTS (
        SELECT 1 FROM @Ban AS s
        JOIN dbo.BanAns AS b WITH (UPDLOCK, HOLDLOCK) ON b.TenBan = s.TenBan
        WHERE b.MaLoaiBan <> s.MaLoaiBan OR b.SoChoNgoi <> s.SoChoNgoi
    )
        THROW 50001, N'Tên bàn mẫu đã tồn tại nhưng khác loại hoặc số chỗ. Hãy kiểm tra dữ liệu trước khi seed.', 1;

    INSERT INTO dbo.BanAns (TenBan, MaLoaiBan, SoChoNgoi, ViTri, MoTa, TrangThai)
    SELECT s.TenBan, s.MaLoaiBan, s.SoChoNgoi, s.ViTri,
           N'Bàn mẫu phục vụ demo Module 2.', N'Sẵn sàng'
    FROM @Ban AS s
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.BanAns WITH (UPDLOCK, HOLDLOCK)
        WHERE TenBan = s.TenBan
    );
    DECLARE @BanDaThem int = @@ROWCOUNT;

    COMMIT TRANSACTION;

    SELECT DB_NAME() AS DatabaseDaSeed,
           @LoaiDaThem AS SoLoaiBanDaThem, @BanDaThem AS SoBanAnDaThem;
    SELECT b.MaBan, b.TenBan, l.TenLoaiBan, b.SoChoNgoi, b.ViTri, b.TrangThai
    FROM dbo.BanAns AS b
    JOIN dbo.LoaiBans AS l ON l.MaLoaiBan = b.MaLoaiBan
    JOIN @Ban AS s ON s.TenBan = b.TenBan
    ORDER BY b.SoChoNgoi, b.TenBan;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
