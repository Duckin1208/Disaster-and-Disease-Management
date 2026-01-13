USE QLDBTT
GO

-- ================================================================
-- PHẦN 1: HÀ NỘI - TẠO TỈNH VÀ CÁC QUẬN/HUYỆN
-- ================================================================
DECLARE @HanoiId INT;
DECLARE @Vung1 INT = 1; 
DECLARE @CapQuan INT = 2; 

-- 1. Đảm bảo Hà Nội tồn tại
IF NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'Hà Nội' AND HanhChinhId = 1)
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    VALUES (N'Hà Nội', 1, N'Thành phố', NULL, @Vung1);
END

-- Lấy ID Hà Nội
SELECT @HanoiId = Id FROM DonVi WHERE Ten = N'Hà Nội' AND HanhChinhId = 1;

-- 2. Insert Quận/Huyện
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @CapQuan, Loai, @HanoiId, @Vung1
FROM (VALUES 
    (N'Ba Đình', N'Quận'), (N'Hoàn Kiếm', N'Quận'), (N'Tây Hồ', N'Quận'), (N'Long Biên', N'Quận'),
    (N'Cầu Giấy', N'Quận'), (N'Đống Đa', N'Quận'), (N'Hai Bà Trưng', N'Quận'), (N'Hoàng Mai', N'Quận'),
    (N'Thanh Xuân', N'Quận'), (N'Sóc Sơn', N'Huyện'), (N'Đông Anh', N'Huyện'), (N'Gia Lâm', N'Huyện'),
    (N'Nam Từ Liêm', N'Quận'), (N'Thanh Trì', N'Huyện'), (N'Bắc Từ Liêm', N'Quận'), (N'Mê Linh', N'Huyện'),
    (N'Hà Đông', N'Quận'), (N'Sơn Tây', N'Thị xã'), (N'Ba Vì', N'Huyện'), (N'Phúc Thọ', N'Huyện'),
    (N'Đan Phượng', N'Huyện'), (N'Hoài Đức', N'Huyện'), (N'Quốc Oai', N'Huyện'), (N'Thạch Thất', N'Huyện'),
    (N'Chương Mỹ', N'Huyện'), (N'Thanh Oai', N'Huyện'), (N'Thường Tín', N'Huyện'), (N'Phú Xuyên', N'Huyện'),
    (N'Ứng Hòa', N'Huyện'), (N'Mỹ Đức', N'Huyện')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @HanoiId);
GO

-- ================================================================
-- PHẦN 2: HÀ NỘI - CHI TIẾT PHƯỜNG/XÃ (KHỐI 1)
-- ================================================================
DECLARE @HanoiId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hà Nội' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;
DECLARE @CapXa INT = 3;

-- 1. Ba Đình
DECLARE @BaDinh INT = (SELECT Id FROM DonVi WHERE Ten = N'Ba Đình' AND TrucThuocId = @HanoiId);
IF @BaDinh IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @BaDinh, @Vung1 FROM (VALUES 
    (N'Phúc Xá'),(N'Trúc Bạch'),(N'Vĩnh Phúc'),(N'Cống Vị'),(N'Liễu Giai'),(N'Nguyễn Trung Trực'),(N'Quán Thánh'),
    (N'Ngọc Hà'),(N'Điện Biên'),(N'Đội Cấn'),(N'Ngọc Khánh'),(N'Kim Mã'),(N'Giảng Võ'),(N'Thành Công')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @BaDinh);
END

-- 2. Hoàn Kiếm
DECLARE @HoanKiem INT = (SELECT Id FROM DonVi WHERE Ten = N'Hoàn Kiếm' AND TrucThuocId = @HanoiId);
IF @HoanKiem IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @HoanKiem, @Vung1 FROM (VALUES 
    (N'Phúc Tân'),(N'Đồng Xuân'),(N'Hàng Mã'),(N'Hàng Buồm'),(N'Hàng Đào'),(N'Hàng Bồ'),(N'Cửa Đông'),(N'Lý Thái Tổ'),
    (N'Hàng Bạc'),(N'Hàng Gai'),(N'Chương Dương'),(N'Hàng Trống'),(N'Cửa Nam'),(N'Hàng Bông'),(N'Tràng Tiền'),
    (N'Trần Hưng Đạo'),(N'Phan Chu Trinh'),(N'Hàng Bài')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @HoanKiem);
END

-- 3. Tây Hồ
DECLARE @TayHo INT = (SELECT Id FROM DonVi WHERE Ten = N'Tây Hồ' AND TrucThuocId = @HanoiId);
IF @TayHo IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @TayHo, @Vung1 FROM (VALUES 
    (N'Phú Thượng'),(N'Nhật Tân'),(N'Tứ Liên'),(N'Quảng An'),(N'Xuân La'),(N'Yên Phụ'),(N'Bưởi'),(N'Thụy Khuê')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TayHo);
END

-- 4. Cầu Giấy
DECLARE @CauGiay INT = (SELECT Id FROM DonVi WHERE Ten = N'Cầu Giấy' AND TrucThuocId = @HanoiId);
IF @CauGiay IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @CauGiay, @Vung1 FROM (VALUES 
    (N'Nghĩa Đô'),(N'Nghĩa Tân'),(N'Mai Dịch'),(N'Dịch Vọng'),(N'Dịch Vọng Hậu'),(N'Quan Hoa'),(N'Yên Hòa'),(N'Trung Hòa')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @CauGiay);
END
GO

-- ================================================================
-- PHẦN 2: HÀ NỘI - CHI TIẾT PHƯỜNG/XÃ (KHỐI 2 - Đống Đa, HBT, HM, TX)
-- ================================================================
DECLARE @HanoiId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hà Nội' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;
DECLARE @CapXa INT = 3;

-- 5. Đống Đa
DECLARE @DongDa INT = (SELECT Id FROM DonVi WHERE Ten = N'Đống Đa' AND TrucThuocId = @HanoiId);
IF @DongDa IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @DongDa, @Vung1 FROM (VALUES 
    (N'Cát Linh'),(N'Văn Miếu'),(N'Quốc Tử Giám'),(N'Láng Thượng'),(N'Ô Chợ Dừa'),(N'Văn Chương'),(N'Hàng Bột'),
    (N'Láng Hạ'),(N'Khâm Thiên'),(N'Thổ Quan'),(N'Nam Đồng'),(N'Trung Phụng'),(N'Quang Trung'),(N'Trung Liệt'),
    (N'Phương Liên'),(N'Thịnh Quang'),(N'Trung Tự'),(N'Kim Liên'),(N'Phương Mai'),(N'Ngã Tư Sở'),(N'Khương Thượng')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @DongDa);
END

-- 6. Hai Bà Trưng
DECLARE @HaiBaTrung INT = (SELECT Id FROM DonVi WHERE Ten = N'Hai Bà Trưng' AND TrucThuocId = @HanoiId);
IF @HaiBaTrung IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @HaiBaTrung, @Vung1 FROM (VALUES 
    (N'Nguyễn Du'),(N'Bạch Đằng'),(N'Phạm Đình Hổ'),(N'Lê Đại Hành'),(N'Đồng Nhân'),(N'Phố Huế'),(N'Đống Mác'),
    (N'Thanh Lương'),(N'Thanh Nhàn'),(N'Cầu Dền'),(N'Bách Khoa'),(N'Đồng Tâm'),(N'Vĩnh Tuy'),(N'Bạch Mai'),
    (N'Quỳnh Mai'),(N'Quỳnh Lôi'),(N'Minh Khai'),(N'Trương Định')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @HaiBaTrung);
END

-- 7. Hoàng Mai
DECLARE @HoangMai INT = (SELECT Id FROM DonVi WHERE Ten = N'Hoàng Mai' AND TrucThuocId = @HanoiId);
IF @HoangMai IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @HoangMai, @Vung1 FROM (VALUES 
    (N'Thanh Trì'),(N'Vĩnh Hưng'),(N'Định Công'),(N'Mai Động'),(N'Tương Mai'),(N'Đại Kim'),(N'Tân Mai'),
    (N'Hoàng Văn Thụ'),(N'Giáp Bát'),(N'Lĩnh Nam'),(N'Thịnh Liệt'),(N'Trần Phú'),(N'Hoàng Liệt'),(N'Yên Sở')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @HoangMai);
END

-- 8. Thanh Xuân
DECLARE @ThanhXuan INT = (SELECT Id FROM DonVi WHERE Ten = N'Thanh Xuân' AND TrucThuocId = @HanoiId);
IF @ThanhXuan IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @ThanhXuan, @Vung1 FROM (VALUES 
    (N'Nhân Chính'),(N'Thượng Đình'),(N'Khương Trung'),(N'Khương Mai'),(N'Thanh Xuân Trung'),(N'Phương Liệt'),
    (N'Hạ Đình'),(N'Khương Đình'),(N'Thanh Xuân Bắc'),(N'Thanh Xuân Nam'),(N'Kim Giang')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @ThanhXuan);
END
GO

-- ================================================================
-- PHẦN 2: HÀ NỘI - KHỐI 3 (Long Biên, Hà Đông, Từ Liêm)
-- ================================================================
DECLARE @HanoiId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hà Nội' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;
DECLARE @CapXa INT = 3;

-- 9. Long Biên
DECLARE @LongBien INT = (SELECT Id FROM DonVi WHERE Ten = N'Long Biên' AND TrucThuocId = @HanoiId);
IF @LongBien IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @LongBien, @Vung1 FROM (VALUES 
    (N'Thượng Thanh'),(N'Ngọc Thụy'),(N'Giang Biên'),(N'Đức Giang'),(N'Việt Hưng'),(N'Gia Thụy'),(N'Ngọc Lâm'),
    (N'Phúc Lợi'),(N'Bồ Đề'),(N'Sài Đồng'),(N'Long Biên'),(N'Thạch Bàn'),(N'Phúc Đồng'),(N'Cự Khối')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @LongBien);
END

-- 10. Hà Đông
DECLARE @HaDong INT = (SELECT Id FROM DonVi WHERE Ten = N'Hà Đông' AND TrucThuocId = @HanoiId);
IF @HaDong IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @HaDong, @Vung1 FROM (VALUES 
    (N'Nguyễn Trãi'),(N'Mộ Lao'),(N'Văn Quán'),(N'Vạn Phúc'),(N'Yết Kiêu'),(N'Quang Trung'),(N'La Khê'),(N'Phú La'),
    (N'Phúc La'),(N'Hà Cầu'),(N'Yên Nghĩa'),(N'Kiến Hưng'),(N'Phú Lương'),(N'Phú Lãm'),(N'Dương Nội'),(N'Biên Giang'),(N'Đồng Mai')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @HaDong);
END

-- 11. Nam Từ Liêm
DECLARE @NamTuLiem INT = (SELECT Id FROM DonVi WHERE Ten = N'Nam Từ Liêm' AND TrucThuocId = @HanoiId);
IF @NamTuLiem IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @NamTuLiem, @Vung1 FROM (VALUES 
    (N'Cầu Diễn'),(N'Mỹ Đình 1'),(N'Mỹ Đình 2'),(N'Phú Đô'),(N'Mễ Trì'),(N'Trung Văn'),(N'Đại Mỗ'),(N'Tây Mỗ'),(N'Phương Canh'),(N'Xuân Phương')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @NamTuLiem);
END

-- 12. Bắc Từ Liêm
DECLARE @BacTuLiem INT = (SELECT Id FROM DonVi WHERE Ten = N'Bắc Từ Liêm' AND TrucThuocId = @HanoiId);
IF @BacTuLiem IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, N'Phường', @BacTuLiem, @Vung1 FROM (VALUES 
    (N'Thượng Cát'),(N'Liên Mạc'),(N'Thụy Phương'),(N'Minh Khai'),(N'Tây Tựu'),(N'Đông Ngạc'),(N'Đức Thắng'),
    (N'Xuân Đỉnh'),(N'Xuân Tảo'),(N'Cổ Nhuế 1'),(N'Cổ Nhuế 2'),(N'Phú Diễn'),(N'Phúc Diễn')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @BacTuLiem);
END
GO

-- ================================================================
-- PHẦN 2: HÀ NỘI - KHỐI 4 (Đông Anh, Sóc Sơn, Gia Lâm, Thanh Trì)
-- ================================================================
DECLARE @HanoiId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hà Nội' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;
DECLARE @CapXa INT = 3;

-- 13. Đông Anh
DECLARE @DongAnh INT = (SELECT Id FROM DonVi WHERE Ten = N'Đông Anh' AND TrucThuocId = @HanoiId);
IF @DongAnh IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, Loai, @DongAnh, @Vung1 FROM (VALUES 
    (N'Thị trấn Đông Anh', N'Thị trấn'),(N'Bắc Hồng', N'Xã'),(N'Nam Hồng', N'Xã'),(N'Tiên Dương', N'Xã'),(N'Vân Nội', N'Xã'),
    (N'Uy Nỗ', N'Xã'),(N'Cổ Loa', N'Xã'),(N'Hải Bối', N'Xã'),(N'Kim Chung', N'Xã'),(N'Võng La', N'Xã'),(N'Đông Hội', N'Xã'),
    (N'Kim Nỗ', N'Xã'),(N'Thụy Lâm', N'Xã'),(N'Mai Lâm', N'Xã'),(N'Liên Hà', N'Xã'),(N'Dục Tú', N'Xã'),(N'Đại Mạch', N'Xã'),
    (N'Việt Hùng', N'Xã'),(N'Xuân Nộn', N'Xã'),(N'Nguyên Khê', N'Xã'),(N'Tàm Xá', N'Xã'),(N'Vĩnh Ngọc', N'Xã'),(N'Xuân Canh', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @DongAnh);
END

-- 14. Sóc Sơn
DECLARE @SocSon INT = (SELECT Id FROM DonVi WHERE Ten = N'Sóc Sơn' AND TrucThuocId = @HanoiId);
IF @SocSon IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, Loai, @SocSon, @Vung1 FROM (VALUES 
    (N'Thị trấn Sóc Sơn', N'Thị trấn'),(N'Phù Linh', N'Xã'),(N'Bắc Sơn', N'Xã'),(N'Nam Sơn', N'Xã'),(N'Hồng Kỳ', N'Xã'),
    (N'Phù Lỗ', N'Xã'),(N'Minh Phú', N'Xã'),(N'Minh Trí', N'Xã'),(N'Trung Giã', N'Xã'),(N'Tân Minh', N'Xã'),
    (N'Quang Tiến', N'Xã'),(N'Hiền Ninh', N'Xã'),(N'Thanh Xuân', N'Xã'),(N'Tiên Dược', N'Xã'),(N'Mai Đình', N'Xã'),
    (N'Đức Hòa', N'Xã'),(N'Đông Xuân', N'Xã'),(N'Kim Lũ', N'Xã'),(N'Phú Cường', N'Xã'),(N'Phú Minh', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @SocSon);
END

-- 15. Gia Lâm
DECLARE @GiaLam INT = (SELECT Id FROM DonVi WHERE Ten = N'Gia Lâm' AND TrucThuocId = @HanoiId);
IF @GiaLam IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, Loai, @GiaLam, @Vung1 FROM (VALUES 
    (N'Thị trấn Trâu Quỳ', N'Thị trấn'),(N'Yên Viên', N'Thị trấn'),(N'Ninh Hiệp', N'Xã'),(N'Bát Tràng', N'Xã'),(N'Kiêu Kỵ', N'Xã'),
    (N'Đa Tốn', N'Xã'),(N'Dương Xá', N'Xã'),(N'Cổ Bi', N'Xã'),(N'Đặng Xá', N'Xã'),(N'Phú Thị', N'Xã'),
    (N'Kim Lan', N'Xã'),(N'Văn Đức', N'Xã'),(N'Dương Quang', N'Xã'),(N'Dương Hà', N'Xã'),(N'Yên Thường', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @GiaLam);
END

-- 16. Thanh Trì
DECLARE @ThanhTri INT = (SELECT Id FROM DonVi WHERE Ten = N'Thanh Trì' AND TrucThuocId = @HanoiId);
IF @ThanhTri IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, @CapXa, Loai, @ThanhTri, @Vung1 FROM (VALUES 
    (N'Thị trấn Văn Điển', N'Thị trấn'),(N'Tân Triều', N'Xã'),(N'Thanh Liệt', N'Xã'),(N'Tả Thanh Oai', N'Xã'),(N'Hữu Hòa', N'Xã'),
    (N'Tam Hiệp', N'Xã'),(N'Vĩnh Quỳnh', N'Xã'),(N'Ngũ Hiệp', N'Xã'),(N'Ngọc Hồi', N'Xã'),(N'Tứ Hiệp', N'Xã'),
    (N'Vạn Phúc', N'Xã'),(N'Duyên Hà', N'Xã'),(N'Yên Mỹ', N'Xã'),(N'Đông Mỹ', N'Xã'),(N'Liên Ninh', N'Xã'),(N'Đại Áng', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @ThanhTri);
END
GO

-- ================================================================
-- PHẦN 2: HÀ NỘI - KHỐI 5 (Sơn Tây, Ba Vì, Thạch Thất...)
-- ================================================================
DECLARE @HanoiId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hà Nội' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;

-- Sơn Tây
DECLARE @SonTay INT = (SELECT Id FROM DonVi WHERE Ten = N'Sơn Tây' AND TrucThuocId = @HanoiId);
IF @SonTay IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @SonTay, @Vung1 FROM (VALUES 
    (N'Lê Lợi', N'Phường'), (N'Sơn Lộc', N'Phường'), (N'Đường Lâm', N'Xã'), (N'Cổ Đông', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @SonTay);
END

-- Ba Vì
DECLARE @BaVi INT = (SELECT Id FROM DonVi WHERE Ten = N'Ba Vì' AND TrucThuocId = @HanoiId);
IF @BaVi IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @BaVi, @Vung1 FROM (VALUES 
    (N'Thị trấn Tây Đằng', N'Thị trấn'), (N'Vân Hòa', N'Xã'), (N'Ba Trại', N'Xã'), (N'Tản Lĩnh', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @BaVi);
END

-- Đan Phượng
DECLARE @DanPhuong INT = (SELECT Id FROM DonVi WHERE Ten = N'Đan Phượng' AND TrucThuocId = @HanoiId);
IF @DanPhuong IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @DanPhuong, @Vung1 FROM (VALUES 
    (N'Thị trấn Phùng', N'Thị trấn'), (N'Tân Hội', N'Xã'), (N'Tân Lập', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @DanPhuong);
END

-- Hoài Đức
DECLARE @HoaiDuc INT = (SELECT Id FROM DonVi WHERE Ten = N'Hoài Đức' AND TrucThuocId = @HanoiId);
IF @HoaiDuc IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @HoaiDuc, @Vung1 FROM (VALUES 
    (N'Thị trấn Trạm Trôi', N'Thị trấn'), (N'An Khánh', N'Xã'), (N'Vân Canh', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @HoaiDuc);
END

-- Thạch Thất
DECLARE @ThachThat INT = (SELECT Id FROM DonVi WHERE Ten = N'Thạch Thất' AND TrucThuocId = @HanoiId);
IF @ThachThat IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @ThachThat, @Vung1 FROM (VALUES 
    (N'Thị trấn Liên Quan', N'Thị trấn'), (N'Thạch Hòa', N'Xã'), (N'Phùng Xá', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @ThachThat);
END

-- Mê Linh
DECLARE @MeLinh INT = (SELECT Id FROM DonVi WHERE Ten = N'Mê Linh' AND TrucThuocId = @HanoiId);
IF @MeLinh IS NOT NULL
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @MeLinh, @Vung1 FROM (VALUES 
    (N'Thị trấn Quang Minh', N'Thị trấn'), (N'Thị trấn Chi Đông', N'Thị trấn'), (N'Mê Linh', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @MeLinh);
END
GO

-- ================================================================
-- PHẦN 3: VĨNH PHÚC - TỈNH, HUYỆN VÀ XÃ
-- ================================================================
DECLARE @VinhPhucId INT;
DECLARE @Vung1 INT = 1; 
DECLARE @CapHuyen INT = 2;

-- 1. Đảm bảo Vĩnh Phúc tồn tại
IF NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'Vĩnh Phúc' AND HanhChinhId = 1)
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    VALUES (N'Vĩnh Phúc', 1, N'Tỉnh', NULL, @Vung1);
END
SELECT @VinhPhucId = Id FROM DonVi WHERE Ten = N'Vĩnh Phúc' AND HanhChinhId = 1;

-- 2. Insert Huyện
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @CapHuyen, Loai, @VinhPhucId, @Vung1
FROM (VALUES 
    (N'Vĩnh Yên', N'Thành phố'), (N'Phúc Yên', N'Thành phố'), (N'Bình Xuyên', N'Huyện'),
    (N'Lập Thạch', N'Huyện'), (N'Sông Lô', N'Huyện'), (N'Tam Dương', N'Huyện'),
    (N'Tam Đảo', N'Huyện'), (N'Vĩnh Tường', N'Huyện'), (N'Yên Lạc', N'Huyện')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @VinhPhucId);
GO

-- 3. Insert Xã (Block riêng để reset biến)
DECLARE @VinhPhucId INT = (SELECT Id FROM DonVi WHERE Ten = N'Vĩnh Phúc' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;

-- TP Vĩnh Yên
DECLARE @VinhYen INT = (SELECT Id FROM DonVi WHERE Ten = N'Vĩnh Yên' AND TrucThuocId = @VinhPhucId);
IF @VinhYen IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @VinhYen, @Vung1 FROM (VALUES 
    (N'Ngô Quyền', N'Phường'), (N'Đống Đa', N'Phường'), (N'Liên Bảo', N'Phường'), 
    (N'Tích Sơn', N'Phường'), (N'Đồng Tâm', N'Phường'), (N'Hội Hợp', N'Phường'), 
    (N'Khai Quang', N'Phường'), (N'Định Trung', N'Phường'), (N'Thanh Trù', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @VinhYen);

-- TP Phúc Yên
DECLARE @PhucYen INT = (SELECT Id FROM DonVi WHERE Ten = N'Phúc Yên' AND TrucThuocId = @VinhPhucId);
IF @PhucYen IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @PhucYen, @Vung1 FROM (VALUES 
    (N'Trưng Trắc', N'Phường'), (N'Trưng Nhị', N'Phường'), (N'Hùng Vương', N'Phường'), 
    (N'Nam Viêm', N'Phường'), (N'Phúc Thắng', N'Phường'), (N'Xuân Hòa', N'Phường'), 
    (N'Đồng Xuân', N'Phường'), (N'Tiền Châu', N'Phường'), (N'Cao Minh', N'Xã'), (N'Ngọc Thanh', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @PhucYen);

-- Huyện Bình Xuyên
DECLARE @BinhXuyen INT = (SELECT Id FROM DonVi WHERE Ten = N'Bình Xuyên' AND TrucThuocId = @VinhPhucId);
IF @BinhXuyen IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @BinhXuyen, @Vung1 FROM (VALUES 
    (N'Hương Canh', N'Thị trấn'), (N'Bá Hiến', N'Thị trấn'), (N'Đạo Đức', N'Thị trấn'), 
    (N'Gia Khánh', N'Thị trấn'), (N'Thanh Lãng', N'Thị trấn'), 
    (N'Quất Lưu', N'Xã'), (N'Tam Hợp', N'Xã'), (N'Thiện Kế', N'Xã'), (N'Trung Mỹ', N'Xã'), 
    (N'Phú Xuân', N'Xã'), (N'Tân Phong', N'Xã'), (N'Sơn Lôi', N'Xã'), (N'Hương Sơn', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @BinhXuyen);

-- Huyện Yên Lạc
DECLARE @YenLac INT = (SELECT Id FROM DonVi WHERE Ten = N'Yên Lạc' AND TrucThuocId = @VinhPhucId);
IF @YenLac IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @YenLac, @Vung1 FROM (VALUES 
    (N'Yên Lạc', N'Thị trấn'), (N'Tam Hồng', N'Thị trấn'), 
    (N'Đồng Văn', N'Xã'), (N'Tề Lỗ', N'Xã'), (N'Trung Nguyên', N'Xã'), (N'Đồng Cương', N'Xã'), 
    (N'Nguyệt Đức', N'Xã'), (N'Yên Đồng', N'Xã'), (N'Minh Tân', N'Xã'), (N'Bình Định', N'Xã'), 
    (N'Trung Kiên', N'Xã'), (N'Hồng Phương', N'Xã'), (N'Trung Hà', N'Xã'), (N'Liên Châu', N'Xã'), 
    (N'Đại Tự', N'Xã'), (N'Hồng Châu', N'Xã'), (N'Yên Phương', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @YenLac);

-- Huyện Vĩnh Tường
DECLARE @VinhTuong INT = (SELECT Id FROM DonVi WHERE Ten = N'Vĩnh Tường' AND TrucThuocId = @VinhPhucId);
IF @VinhTuong IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @VinhTuong, @Vung1 FROM (VALUES 
    (N'Vĩnh Tường', N'Thị trấn'), (N'Thổ Tang', N'Thị trấn'), (N'Tứ Trưng', N'Thị trấn'), 
    (N'Nghĩa Hưng', N'Xã'), (N'Kim Xá', N'Xã'), (N'Yên Bình', N'Xã'), (N'Chấn Hưng', N'Xã'), 
    (N'Đại Đồng', N'Xã'), (N'Tân Tiến', N'Xã'), (N'Lũng Hòa', N'Xã'), (N'Cao Đại', N'Xã'), 
    (N'Vĩnh Sơn', N'Xã'), (N'Bình Dương', N'Xã'), (N'Vĩnh Thịnh', N'Xã'), (N'Vũ Di', N'Xã'), 
    (N'Thượng Trưng', N'Xã'), (N'Tân Phú', N'Xã'), (N'Việt Xuân', N'Xã'), (N'Bồ Sao', N'Xã'), 
    (N'Tuân Chính', N'Xã'), (N'Tam Phúc', N'Xã'), (N'Phú Đa', N'Xã'), (N'Ngũ Kiên', N'Xã'), 
    (N'Vĩnh Ninh', N'Xã'), (N'An Tường', N'Xã'), (N'Lý Nhân', N'Xã'), (N'Yên Lập', N'Xã'), 
    (N'Tân Cương', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @VinhTuong);

-- Huyện Tam Dương
DECLARE @TamDuong INT = (SELECT Id FROM DonVi WHERE Ten = N'Tam Dương' AND TrucThuocId = @VinhPhucId);
IF @TamDuong IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @TamDuong, @Vung1 FROM (VALUES 
    (N'Hợp Hòa', N'Thị trấn'), (N'Kim Long', N'Thị trấn'), (N'Thanh Vân', N'Thị trấn'), 
    (N'Hoàng Hoa', N'Xã'), (N'Đồng Tĩnh', N'Xã'), (N'Hướng Đạo', N'Xã'), (N'Đạo Tú', N'Xã'), 
    (N'Duy Phiên', N'Xã'), (N'Hoàng Đan', N'Xã'), (N'Hoàng Lâu', N'Xã'), (N'Vân Hội', N'Xã'), 
    (N'Hợp Thịnh', N'Xã'), (N'An Hòa', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TamDuong);

-- Huyện Tam Đảo
DECLARE @TamDao INT = (SELECT Id FROM DonVi WHERE Ten = N'Tam Đảo' AND TrucThuocId = @VinhPhucId);
IF @TamDao IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @TamDao, @Vung1 FROM (VALUES 
    (N'Hợp Châu', N'Thị trấn'), (N'Tam Đảo', N'Thị trấn'), (N'Đại Đình', N'Thị trấn'), 
    (N'Hồ Sơn', N'Xã'), (N'Tam Quan', N'Xã'), (N'Yên Dương', N'Xã'), (N'Đạo Trù', N'Xã'), 
    (N'Bồ Lý', N'Xã'), (N'Minh Quang', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TamDao);

-- Huyện Lập Thạch
DECLARE @LapThach INT = (SELECT Id FROM DonVi WHERE Ten = N'Lập Thạch' AND TrucThuocId = @VinhPhucId);
IF @LapThach IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @LapThach, @Vung1 FROM (VALUES 
    (N'Lập Thạch', N'Thị trấn'), (N'Hoa Sơn', N'Thị trấn'), (N'Thị trấn Văn Quán', N'Thị trấn'),
    (N'Sơn Đông', N'Xã'), (N'Triệu Đề', N'Xã'), (N'Đình Chu', N'Xã'), (N'Xuân Lôi', N'Xã'), 
    (N'Văn Quán', N'Xã'), (N'Tiên Lữ', N'Xã'), (N'Đồng Ích', N'Xã'), (N'Bàn Giản', N'Xã'), 
    (N'Tử Du', N'Xã'), (N'Liên Hòa', N'Xã'), (N'Ngọc Mỹ', N'Xã'), (N'Xuân Hòa', N'Xã'), 
    (N'Hợp Lý', N'Xã'), (N'Bắc Bình', N'Xã'), (N'Thái Hòa', N'Xã'), (N'Liễn Sơn', N'Xã'), 
    (N'Quang Sơn', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @LapThach);

-- Huyện Sông Lô
DECLARE @SongLo INT = (SELECT Id FROM DonVi WHERE Ten = N'Sông Lô' AND TrucThuocId = @VinhPhucId);
IF @SongLo IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @SongLo, @Vung1 FROM (VALUES 
    (N'Tam Sơn', N'Thị trấn'), 
    (N'Lãng Công', N'Xã'), (N'Quang Yên', N'Xã'), (N'Bạch Lưu', N'Xã'), (N'Hải Lựu', N'Xã'), 
    (N'Đôn Nhân', N'Xã'), (N'Nhân Đạo', N'Xã'), (N'Phương Khoan', N'Xã'), (N'Đồng Quế', N'Xã'), 
    (N'Nhạo Sơn', N'Xã'), (N'Như Thụy', N'Xã'), (N'Yên Thạch', N'Xã'), (N'Tân Lập', N'Xã'), 
    (N'Tứ Yên', N'Xã'), (N'Đồng Thịnh', N'Xã'), (N'Đức Bác', N'Xã'), (N'Cao Phong', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @SongLo);
GO

-- ================================================================
-- PHẦN 4: HẢI DƯƠNG - TỈNH, HUYỆN VÀ XÃ
-- ================================================================
DECLARE @HaiDuongId INT;
DECLARE @Vung2 INT = 2; -- Chi cục Thú y vùng II
DECLARE @CapHuyen INT = 2;

-- 1. Đảm bảo Hải Dương tồn tại
IF NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'Hải Dương' AND HanhChinhId = 1)
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    VALUES (N'Hải Dương', 1, N'Tỉnh', NULL, @Vung2);
END
SELECT @HaiDuongId = Id FROM DonVi WHERE Ten = N'Hải Dương' AND HanhChinhId = 1;

-- 2. Insert Huyện
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @CapHuyen, Loai, @HaiDuongId, @Vung2
FROM (VALUES 
    (N'Hải Dương', N'Thành phố'), (N'Chí Linh', N'Thành phố'), (N'Kinh Môn', N'Thị xã'),
    (N'Bình Giang', N'Huyện'), (N'Cẩm Giàng', N'Huyện'), (N'Gia Lộc', N'Huyện'),
    (N'Kim Thành', N'Huyện'), (N'Nam Sách', N'Huyện'), (N'Ninh Giang', N'Huyện'),
    (N'Thanh Hà', N'Huyện'), (N'Thanh Miện', N'Huyện'), (N'Tứ Kỳ', N'Huyện')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @HaiDuongId);
GO

-- 3. Insert Xã (Block 1: TP và Thị Xã)
DECLARE @HaiDuongId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hải Dương' AND HanhChinhId = 1);
DECLARE @Vung2 INT = 2;

-- TP Hải Dương
DECLARE @TPHaiDuong INT = (SELECT Id FROM DonVi WHERE Ten = N'Hải Dương' AND TrucThuocId = @HaiDuongId AND HanhChinhId = 2);
IF @TPHaiDuong IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @TPHaiDuong, @Vung2 FROM (VALUES 
    (N'Bình Hàn', N'Phường'), (N'Cẩm Thượng', N'Phường'), (N'Hải Tân', N'Phường'), (N'Lê Thanh Nghị', N'Phường'),
    (N'Nam Đồng', N'Phường'), (N'Ngọc Châu', N'Phường'), (N'Nguyễn Trãi', N'Phường'), (N'Nhị Châu', N'Phường'),
    (N'Phạm Ngũ Lão', N'Phường'), (N'Quang Trung', N'Phường'), (N'Tân Bình', N'Phường'), (N'Thạch Khôi', N'Phường'),
    (N'Thanh Bình', N'Phường'), (N'Trần Hưng Đạo', N'Phường'), (N'Trần Phú', N'Phường'), (N'Tứ Minh', N'Phường'),
    (N'Việt Hòa', N'Phường'), (N'Ái Quốc', N'Phường'), (N'An Thượng', N'Xã'), (N'Gia Xuyên', N'Xã'),
    (N'Liên Hồng', N'Xã'), (N'Ngọc Sơn', N'Xã'), (N'Quyết Thắng', N'Xã'), (N'Tiền Tiến', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TPHaiDuong);

-- TP Chí Linh
DECLARE @ChiLinh INT = (SELECT Id FROM DonVi WHERE Ten = N'Chí Linh' AND TrucThuocId = @HaiDuongId);
IF @ChiLinh IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @ChiLinh, @Vung2 FROM (VALUES 
    (N'Bến Tắm', N'Phường'), (N'Chí Minh', N'Phường'), (N'Cổ Thành', N'Phường'), (N'Cộng Hòa', N'Phường'),
    (N'Đồng Lạc', N'Phường'), (N'Hoàng Tân', N'Phường'), (N'Hoàng Tiến', N'Phường'), (N'Phả Lại', N'Phường'),
    (N'Sao Đỏ', N'Phường'), (N'Tân Dân', N'Phường'), (N'Thái Học', N'Phường'), (N'Văn An', N'Phường'),
    (N'Văn Đức', N'Phường'), (N'An Lạc', N'Phường'), (N'Bắc An', N'Xã'), (N'Hoàng Hoa Thám', N'Xã'),
    (N'Hưng Đạo', N'Xã'), (N'Lê Lợi', N'Xã'), (N'Nhân Huệ', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @ChiLinh);

-- TX Kinh Môn
DECLARE @KinhMon INT = (SELECT Id FROM DonVi WHERE Ten = N'Kinh Môn' AND TrucThuocId = @HaiDuongId);
IF @KinhMon IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @KinhMon, @Vung2 FROM (VALUES 
    (N'An Lưu', N'Phường'), (N'An Phụ', N'Phường'), (N'An Sinh', N'Phường'), (N'Duy Tân', N'Phường'),
    (N'Hiến Thành', N'Phường'), (N'Hiệp An', N'Phường'), (N'Hiệp Sơn', N'Phường'), (N'Long Xuyên', N'Phường'),
    (N'Minh Tân', N'Phường'), (N'Phạm Thái', N'Phường'), (N'Phú Thứ', N'Phường'), (N'Tân Dân', N'Phường'),
    (N'Thái Thịnh', N'Phường'), (N'Thất Hùng', N'Phường'), (N'Bạch Đằng', N'Xã'), (N'Hiệp Hòa', N'Xã'),
    (N'Hoành Sơn', N'Xã'), (N'Lạc Long', N'Xã'), (N'Lê Ninh', N'Xã'), (N'Minh Hòa', N'Xã'),
    (N'Quang Thành', N'Xã'), (N'Thăng Long', N'Xã'), (N'Thượng Quận', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @KinhMon);
GO

-- 4. Insert Xã (Block 2: Các Huyện)
DECLARE @HaiDuongId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hải Dương' AND HanhChinhId = 1);
DECLARE @Vung2 INT = 2;

-- Cẩm Giàng
DECLARE @CamGiang INT = (SELECT Id FROM DonVi WHERE Ten = N'Cẩm Giàng' AND TrucThuocId = @HaiDuongId);
IF @CamGiang IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @CamGiang, @Vung2 FROM (VALUES 
    (N'Lai Cách', N'Thị trấn'), (N'Cẩm Giang', N'Thị trấn'), 
    (N'Cẩm Điền', N'Xã'), (N'Cẩm Đoài', N'Xã'), (N'Cẩm Đông', N'Xã'), (N'Cẩm Hoàng', N'Xã'), 
    (N'Cẩm Hưng', N'Xã'), (N'Cẩm Phúc', N'Xã'), (N'Cẩm Văn', N'Xã'), (N'Cẩm Vũ', N'Xã'), 
    (N'Cao An', N'Xã'), (N'Định Sơn', N'Xã'), (N'Đức Chính', N'Xã'), (N'Lương Điền', N'Xã'), 
    (N'Ngọc Liên', N'Xã'), (N'Tân Trường', N'Xã'), (N'Thạch Lỗi', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @CamGiang);

-- Bình Giang
DECLARE @BinhGiang INT = (SELECT Id FROM DonVi WHERE Ten = N'Bình Giang' AND TrucThuocId = @HaiDuongId);
IF @BinhGiang IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @BinhGiang, @Vung2 FROM (VALUES 
    (N'Kẻ Sặt', N'Thị trấn'), 
    (N'Bình Minh', N'Xã'), (N'Bình Xuyên', N'Xã'), (N'Cổ Bì', N'Xã'), (N'Hồng Khê', N'Xã'), 
    (N'Hùng Thắng', N'Xã'), (N'Long Xuyên', N'Xã'), (N'Nhân Quyền', N'Xã'), (N'Tân Hồng', N'Xã'), 
    (N'Tân Việt', N'Xã'), (N'Thái Dương', N'Xã'), (N'Thái Học', N'Xã'), (N'Thái Hòa', N'Xã'), 
    (N'Thúc Kháng', N'Xã'), (N'Vĩnh Hồng', N'Xã'), (N'Vĩnh Hưng', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @BinhGiang);

-- Gia Lộc
DECLARE @GiaLoc INT = (SELECT Id FROM DonVi WHERE Ten = N'Gia Lộc' AND TrucThuocId = @HaiDuongId);
IF @GiaLoc IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @GiaLoc, @Vung2 FROM (VALUES 
    (N'Gia Lộc', N'Thị trấn'), 
    (N'Đoàn Thượng', N'Xã'), (N'Đồng Quang', N'Xã'), (N'Đức Xương', N'Xã'), (N'Gia Khánh', N'Xã'), 
    (N'Gia Lương', N'Xã'), (N'Gia Tân', N'Xã'), (N'Gia Xuyên', N'Xã'), (N'Hoàng Diệu', N'Xã'), 
    (N'Hồng Hưng', N'Xã'), (N'Lê Lợi', N'Xã'), (N'Nhật Tân', N'Xã'), (N'Phạm Trấn', N'Xã'), 
    (N'Quang Minh', N'Xã'), (N'Tân Tiến', N'Xã'), (N'Thống Kênh', N'Xã'), (N'Thống Nhất', N'Xã'), 
    (N'Toàn Thắng', N'Xã'), (N'Yết Kiêu', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @GiaLoc);

-- Thanh Miện
DECLARE @ThanhMien INT = (SELECT Id FROM DonVi WHERE Ten = N'Thanh Miện' AND TrucThuocId = @HaiDuongId);
IF @ThanhMien IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @ThanhMien, @Vung2 FROM (VALUES 
    (N'Thanh Miện', N'Thị trấn'), 
    (N'Cao Thắng', N'Xã'), (N'Chi Lăng Bắc', N'Xã'), (N'Chi Lăng Nam', N'Xã'), (N'Đoàn Kết', N'Xã'), 
    (N'Đoàn Tùng', N'Xã'), (N'Hồng Phong', N'Xã'), (N'Hồng Quang', N'Xã'), (N'Lam Sơn', N'Xã'), 
    (N'Lê Hồng', N'Xã'), (N'Ngô Quyền', N'Xã'), (N'Ngũ Hùng', N'Xã'), (N'Phạm Kha', N'Xã'), 
    (N'Tân Trào', N'Xã'), (N'Thanh Giang', N'Xã'), (N'Thanh Tùng', N'Xã'), (N'Tứ Cường', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @ThanhMien);

-- Nam Sách
DECLARE @NamSach INT = (SELECT Id FROM DonVi WHERE Ten = N'Nam Sách' AND TrucThuocId = @HaiDuongId);
IF @NamSach IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @NamSach, @Vung2 FROM (VALUES 
    (N'Nam Sách', N'Thị trấn'), 
    (N'An Bình', N'Xã'), (N'An Lâm', N'Xã'), (N'An Sơn', N'Xã'), (N'Cộng Hòa', N'Xã'), 
    (N'Đồng Lạc', N'Xã'), (N'Hiệp Cát', N'Xã'), (N'Hồng Phong', N'Xã'), (N'Hợp Tiến', N'Xã'), 
    (N'Minh Tân', N'Xã'), (N'Nam Chính', N'Xã'), (N'Nam Hồng', N'Xã'), (N'Nam Hưng', N'Xã'), 
    (N'Nam Tân', N'Xã'), (N'Nam Trung', N'Xã'), (N'Phú Điền', N'Xã'), (N'Quốc Tuấn', N'Xã'), 
    (N'Thái Tân', N'Xã'), (N'Thanh Quang', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @NamSach);

-- Thanh Hà
DECLARE @ThanhHa INT = (SELECT Id FROM DonVi WHERE Ten = N'Thanh Hà' AND TrucThuocId = @HaiDuongId);
IF @ThanhHa IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @ThanhHa, @Vung2 FROM (VALUES 
    (N'Thanh Hà', N'Thị trấn'), 
    (N'An Phượng', N'Xã'), (N'Cẩm Chế', N'Xã'), (N'Thanh An', N'Xã'), (N'Thanh Cường', N'Xã'), 
    (N'Thanh Hải', N'Xã'), (N'Thanh Hồng', N'Xã'), (N'Thanh Khê', N'Xã'), (N'Thanh Lang', N'Xã'), 
    (N'Thanh Quang', N'Xã'), (N'Thanh Sơn', N'Xã'), (N'Thanh Thủy', N'Xã'), (N'Thanh Xá', N'Xã'), 
    (N'Thanh Xuân', N'Xã'), (N'Việt Hồng', N'Xã'), (N'Vĩnh Lập', N'Xã'), (N'Quyết Thắng', N'Xã'), 
    (N'Tân An', N'Xã'), (N'Tân Việt', N'Xã'), (N'Liên Mạc', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @ThanhHa);

-- Kim Thành
DECLARE @KimThanh INT = (SELECT Id FROM DonVi WHERE Ten = N'Kim Thành' AND TrucThuocId = @HaiDuongId);
IF @KimThanh IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @KimThanh, @Vung2 FROM (VALUES 
    (N'Phú Thái', N'Thị trấn'), 
    (N'Bình Dân', N'Xã'), (N'Cổ Dũng', N'Xã'), (N'Cộng Hòa', N'Xã'), (N'Đại Đức', N'Xã'), 
    (N'Đồng Cẩm', N'Xã'), (N'Kim Anh', N'Xã'), (N'Kim Đính', N'Xã'), (N'Kim Liên', N'Xã'), 
    (N'Kim Tân', N'Xã'), (N'Kim Xuyên', N'Xã'), (N'Lai Vu', N'Xã'), (N'Liên Hòa', N'Xã'), 
    (N'Ngũ Phúc', N'Xã'), (N'Phúc Thành', N'Xã'), (N'Tam Kỳ', N'Xã'), (N'Thượng Vũ', N'Xã'), 
    (N'Tuấn Việt', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @KimThanh);

-- Tứ Kỳ
DECLARE @TuKy INT = (SELECT Id FROM DonVi WHERE Ten = N'Tứ Kỳ' AND TrucThuocId = @HaiDuongId);
IF @TuKy IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @TuKy, @Vung2 FROM (VALUES 
    (N'Tứ Kỳ', N'Thị trấn'), 
    (N'An Thanh', N'Xã'), (N'Bình Lãng', N'Xã'), (N'Chí Minh', N'Xã'), (N'Cộng Lạc', N'Xã'), 
    (N'Đại Hợp', N'Xã'), (N'Đại Sơn', N'Xã'), (N'Dân Chủ', N'Xã'), (N'Hà Kỳ', N'Xã'), 
    (N'Hà Thanh', N'Xã'), (N'Hưng Đạo', N'Xã'), (N'Minh Đức', N'Xã'), (N'Ngọc Kỳ', N'Xã'), 
    (N'Nguyên Giáp', N'Xã'), (N'Phượng Kỳ', N'Xã'), (N'Quang Khải', N'Xã'), (N'Quang Phục', N'Xã'), 
    (N'Quang Trung', N'Xã'), (N'Tái Sơn', N'Xã'), (N'Tân Kỳ', N'Xã'), (N'Tiên Động', N'Xã'), 
    (N'Văn Tố', N'Xã'), (N'Vạn Điểm', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TuKy);

-- Ninh Giang
DECLARE @NinhGiang INT = (SELECT Id FROM DonVi WHERE Ten = N'Ninh Giang' AND TrucThuocId = @HaiDuongId);
IF @NinhGiang IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @NinhGiang, @Vung2 FROM (VALUES 
    (N'Ninh Giang', N'Thị trấn'), 
    (N'An Đức', N'Xã'), (N'Đồng Tâm', N'Xã'), (N'Đông Xuyên', N'Xã'), (N'Hiệp Lực', N'Xã'), 
    (N'Hồng Dụ', N'Xã'), (N'Hồng Đức', N'Xã'), (N'Hồng Phong', N'Xã'), (N'Hồng Thái', N'Xã'), 
    (N'Kiến Quốc', N'Xã'), (N'Nghĩa An', N'Xã'), (N'Ninh Hải', N'Xã'), (N'Ninh Thành', N'Xã'), 
    (N'Tân Hương', N'Xã'), (N'Tân Phong', N'Xã'), (N'Tân Quang', N'Xã'), (N'Ứng Hòe', N'Xã'), 
    (N'Vạn Phúc', N'Xã'), (N'Vĩnh Hòa', N'Xã'), (N'Quyết Thắng', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @NinhGiang);
GO

-- ================================================================
-- PHẦN 5: BẮC NINH - TỈNH, HUYỆN VÀ XÃ
-- ================================================================
DECLARE @BacNinhId INT;
DECLARE @Vung1 INT = 1;
DECLARE @CapHuyen INT = 2;

-- 1. Đảm bảo Bắc Ninh tồn tại
IF NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'Bắc Ninh' AND HanhChinhId = 1)
BEGIN
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    VALUES (N'Bắc Ninh', 1, N'Tỉnh', NULL, @Vung1);
END
SELECT @BacNinhId = Id FROM DonVi WHERE Ten = N'Bắc Ninh' AND HanhChinhId = 1;

-- 2. Insert Huyện
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @CapHuyen, Loai, @BacNinhId, @Vung1
FROM (VALUES 
    (N'Bắc Ninh', N'Thành phố'), (N'Từ Sơn', N'Thành phố'), (N'Thuận Thành', N'Thị xã'),
    (N'Quế Võ', N'Thị xã'), (N'Yên Phong', N'Huyện'), (N'Tiên Du', N'Huyện'),
    (N'Gia Bình', N'Huyện'), (N'Lương Tài', N'Huyện')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @BacNinhId);
GO

-- 3. Insert Xã (TP Bắc Ninh, Từ Sơn)
DECLARE @BacNinhId INT = (SELECT Id FROM DonVi WHERE Ten = N'Bắc Ninh' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;

-- TP Bắc Ninh
DECLARE @TPBacNinh INT = (SELECT Id FROM DonVi WHERE Ten = N'Bắc Ninh' AND TrucThuocId = @BacNinhId AND HanhChinhId = 2);
IF @TPBacNinh IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, N'Phường', @TPBacNinh, @Vung1 FROM (VALUES 
    (N'Đại Phúc'), (N'Đáp Cầu'), (N'Hạp Lĩnh'), (N'Hòa Long'), (N'Khắc Niệm'), 
    (N'Khúc Xuyên'), (N'Kinh Bắc'), (N'Kim Chân'), (N'Nam Sơn'), (N'Ninh Xá'), 
    (N'Phong Khê'), (N'Suối Hoa'), (N'Thị Cầu'), (N'Tiền An'), (N'Vạn An'), 
    (N'Vân Dương'), (N'Vệ An'), (N'Võ Cường'), (N'Vũ Ninh')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TPBacNinh);

-- TP Từ Sơn
DECLARE @TPTuSon INT = (SELECT Id FROM DonVi WHERE Ten = N'Từ Sơn' AND TrucThuocId = @BacNinhId);
IF @TPTuSon IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, N'Phường', @TPTuSon, @Vung1 FROM (VALUES 
    (N'Châu Khê'), (N'Đình Bảng'), (N'Đông Ngàn'), (N'Đồng Kỵ'), (N'Đồng Nguyên'), 
    (N'Hương Mạc'), (N'Phù Chẩn'), (N'Phù Khê'), (N'Tam Sơn'), (N'Tân Hồng'), 
    (N'Trang Hạ'), (N'Tương Giang')
    ) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TPTuSon);
GO

-- 4. Insert Xã (Thuận Thành, Quế Võ)
DECLARE @BacNinhId INT = (SELECT Id FROM DonVi WHERE Ten = N'Bắc Ninh' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;

-- TX Thuận Thành
DECLARE @ThuanThanh INT = (SELECT Id FROM DonVi WHERE Ten = N'Thuận Thành' AND TrucThuocId = @BacNinhId);
IF @ThuanThanh IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @ThuanThanh, @Vung1 FROM (VALUES 
    (N'Hồ', N'Phường'), (N'An Bình', N'Phường'), (N'Gia Đông', N'Phường'), (N'Hà Mãn', N'Phường'), 
    (N'Khắc Niệm', N'Phường'), 
    (N'Ninh Xá', N'Phường'), (N'Song Hồ', N'Phường'), (N'Thanh Khương', N'Phường'), 
    (N'Trạm Lộ', N'Phường'), (N'Trí Quả', N'Phường'), (N'Xuân Lâm', N'Phường'),
    (N'Đại Đồng Thành', N'Xã'), (N'Đình Tổ', N'Xã'), (N'Hoài Thượng', N'Xã'), 
    (N'Mão Điền', N'Xã'), (N'Nghĩa Đạo', N'Xã'), (N'Ngũ Thái', N'Xã'), (N'Nguyệt Đức', N'Xã'), (N'Song Liễu', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @ThuanThanh);

-- TX Quế Võ
DECLARE @QueVo INT = (SELECT Id FROM DonVi WHERE Ten = N'Quế Võ' AND TrucThuocId = @BacNinhId);
IF @QueVo IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @QueVo, @Vung1 FROM (VALUES 
    (N'Phố Mới', N'Phường'), (N'Bằng An', N'Phường'), (N'Bồng Lai', N'Phường'), (N'Cách Bi', N'Phường'), 
    (N'Đại Xuân', N'Phường'), (N'Nhân Hòa', N'Phường'), (N'Phù Lương', N'Phường'), (N'Phương Liễu', N'Phường'), 
    (N'Phượng Mao', N'Phường'), (N'Quế Tân', N'Phường'), (N'Việt Hùng', N'Phường'),
    (N'Châu Phong', N'Xã'), (N'Chi Lăng', N'Xã'), (N'Đào Viên', N'Xã'), (N'Đức Long', N'Xã'), 
    (N'Hán Quảng', N'Xã'), (N'Mộ Đạo', N'Xã'), (N'Ngọc Xá', N'Xã'), (N'Phù Lãng', N'Xã'), 
    (N'Việt Thống', N'Xã'), (N'Yên Giả', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @QueVo);
GO

-- 5. Insert Xã (Các huyện còn lại)
DECLARE @BacNinhId INT = (SELECT Id FROM DonVi WHERE Ten = N'Bắc Ninh' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1;

-- Yên Phong
DECLARE @YenPhong INT = (SELECT Id FROM DonVi WHERE Ten = N'Yên Phong' AND TrucThuocId = @BacNinhId);
IF @YenPhong IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @YenPhong, @Vung1 FROM (VALUES 
    (N'Chờ', N'Thị trấn'), 
    (N'Dũng Liệt', N'Xã'), (N'Đông Phong', N'Xã'), (N'Đông Thọ', N'Xã'), (N'Đông Tiến', N'Xã'), 
    (N'Hòa Tiến', N'Xã'), (N'Long Châu', N'Xã'), (N'Tam Đa', N'Xã'), (N'Tam Giang', N'Xã'), 
    (N'Thụy Hòa', N'Xã'), (N'Trung Nghĩa', N'Xã'), (N'Văn Môn', N'Xã'), (N'Yên Phụ', N'Xã'), 
    (N'Yên Trung', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @YenPhong);

-- Tiên Du
DECLARE @TienDu INT = (SELECT Id FROM DonVi WHERE Ten = N'Tiên Du' AND TrucThuocId = @BacNinhId);
IF @TienDu IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @TienDu, @Vung1 FROM (VALUES 
    (N'Lim', N'Thị trấn'), 
    (N'Cảnh Hưng', N'Xã'), (N'Đại Đồng', N'Xã'), (N'Hiên Vân', N'Xã'), (N'Hoàn Sơn', N'Xã'), 
    (N'Lạc Vệ', N'Xã'), (N'Liên Bão', N'Xã'), (N'Minh Đạo', N'Xã'), (N'Nội Duệ', N'Xã'), 
    (N'Phật Tích', N'Xã'), (N'Phú Lâm', N'Xã'), (N'Tân Chi', N'Xã'), (N'Tri Phương', N'Xã'), 
    (N'Việt Đoàn', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TienDu);

-- Gia Bình
DECLARE @GiaBinh INT = (SELECT Id FROM DonVi WHERE Ten = N'Gia Bình' AND TrucThuocId = @BacNinhId);
IF @GiaBinh IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @GiaBinh, @Vung1 FROM (VALUES 
    (N'Gia Bình', N'Thị trấn'), 
    (N'Bình Dương', N'Xã'), (N'Cao Đức', N'Xã'), (N'Đại Bái', N'Xã'), (N'Đại Lai', N'Xã'), 
    (N'Đông Cứu', N'Xã'), (N'Giang Sơn', N'Xã'), (N'Lãng Ngâm', N'Xã'), (N'Nhân Thắng', N'Xã'), 
    (N'Quỳnh Phú', N'Xã'), (N'Song Giang', N'Xã'), (N'Thái Bảo', N'Xã'), (N'Vạn Ninh', N'Xã'), 
    (N'Xuân Lai', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @GiaBinh);

-- Lương Tài
DECLARE @LuongTai INT = (SELECT Id FROM DonVi WHERE Ten = N'Lương Tài' AND TrucThuocId = @BacNinhId);
IF @LuongTai IS NOT NULL
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
    SELECT Ten, 3, Loai, @LuongTai, @Vung1 FROM (VALUES 
    (N'Thứa', N'Thị trấn'), 
    (N'An Thịnh', N'Xã'), (N'Bình Định', N'Xã'), (N'Lai Hạ', N'Xã'), (N'Lâm Thao', N'Xã'), 
    (N'Minh Tân', N'Xã'), (N'Mỹ Hương', N'Xã'), (N'Phú Hòa', N'Xã'), (N'Phú Lương', N'Xã'), 
    (N'Quảng Phú', N'Xã'), (N'Tân Lãng', N'Xã'), (N'Trung Chính', N'Xã'), (N'Trung Kênh', N'Xã'), 
    (N'Trừng Xá', N'Xã')
    ) AS S(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @LuongTai);
GO

-- ================================================================
-- PHẦN 6: DỮ LIỆU MẪU (THIÊN TAI & CƠ SỞ)
-- ================================================================

-- Lấy ID các tỉnh/thành nếu có (Không insert mới tỉnh ngoài phạm vi đề bài để an toàn)
DECLARE @YenBaiId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Yên Bái' AND HanhChinhId = 1);
DECLARE @HaGiangId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Hà Giang' AND HanhChinhId = 1);
DECLARE @DongNaiId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Đồng Nai' AND HanhChinhId = 1);
DECLARE @ThanhHoaId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Thanh Hóa' AND HanhChinhId = 1);
DECLARE @HCMId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'TP. Hồ Chí Minh' AND HanhChinhId = 1);

-- Chỉ insert huyện mẫu nếu tỉnh đã tồn tại
IF @YenBaiId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'Mù Cang Chải')
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES (N'Mù Cang Chải', 2, N'Huyện', @YenBaiId, 2);

IF @HaGiangId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'Bắc Mê')
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES (N'Bắc Mê', 2, N'Huyện', @HaGiangId, 2);

IF @DongNaiId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'Thống Nhất')
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES (N'Thống Nhất', 2, N'Huyện', @DongNaiId, 6);

IF @HCMId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'Củ Chi')
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES (N'Củ Chi', 2, N'Huyện', @HCMId, 6);

IF @ThanhHoaId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = N'TP. Thanh Hóa' AND TrucThuocId = @ThanhHoaId)
    INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES (N'TP. Thanh Hóa', 2, N'Thành phố', @ThanhHoaId, 3);

-- Điểm thiên tai
DECLARE @LuQuetId INT = (SELECT TOP 1 Id FROM LoaiThienTai WHERE Ten = N'Lũ quét');
DECLARE @SatLoId INT = (SELECT TOP 1 Id FROM LoaiThienTai WHERE Ten = N'Trượt lở');

DECLARE @MuCangChaiId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Mù Cang Chải');
DECLARE @BacMeId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Bắc Mê');

IF @MuCangChaiId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM DiemThienTai WHERE Ten = N'Điểm sạt lở đèo Khau Phạ')
        INSERT INTO DiemThienTai (Ten, DonViId, LoaiThienTaiId, MucDo)
        VALUES (N'Điểm sạt lở đèo Khau Phạ', @MuCangChaiId, @SatLoId, 4);
    
    IF NOT EXISTS (SELECT 1 FROM DiemThienTai WHERE Ten = N'Suối Nậm Kim')
        INSERT INTO DiemThienTai (Ten, DonViId, LoaiThienTaiId, MucDo)
        VALUES (N'Suối Nậm Kim', @MuCangChaiId, @LuQuetId, 5);
END

IF @BacMeId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM DiemThienTai WHERE Ten = N'Khu vực đường 34')
        INSERT INTO DiemThienTai (Ten, DonViId, LoaiThienTaiId, MucDo)
        VALUES (N'Khu vực đường 34', @BacMeId, @SatLoId, 3);
END

-- Cơ sở chăn nuôi
DECLARE @LoaiAnToan INT = (SELECT TOP 1 Id FROM LoaiCoSo WHERE Ten = N'Vùng chăn nuôi an toàn dịch bệnh');
DECLARE @LoaiDaiLy INT = (SELECT TOP 1 Id FROM LoaiCoSo WHERE Ten = N'Đại lý thuốc thú y');
DECLARE @LoaiCaNhan INT = (SELECT TOP 1 Id FROM LoaiCoSo WHERE Ten = N'Cá nhân chăn nuôi');

DECLARE @ThongNhatId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Thống Nhất');
DECLARE @TPThanhHoaId2 INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'TP. Thanh Hóa');
DECLARE @CuChiId INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Củ Chi');

IF @ThongNhatId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM CoSo WHERE Ten = N'Vùng chăn nuôi gà đồi Kiệm Tân')
    BEGIN
        INSERT INTO CoSo (Ten, LoaiCoSoId, DonViId, QuyMo, SDT)
        VALUES (N'Vùng chăn nuôi gà đồi Kiệm Tân', @LoaiAnToan, @ThongNhatId, N'Quy mô Huyện', '02513999888');
        
        DECLARE @CoSoId INT = (SELECT Id FROM CoSo WHERE Ten = N'Vùng chăn nuôi gà đồi Kiệm Tân');
        INSERT INTO GiayPhep (SoGiayPhep, NgayCap, NgayHetHan, CoSoId)
        VALUES (N'GCN-ATDB-2026-001', '2026-01-01', '2031-01-01', @CoSoId);
    END
END

IF @TPThanhHoaId2 IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM CoSo WHERE Ten = N'Đại lý thuốc thú y Lam Sơn')
    BEGIN
        INSERT INTO CoSo (Ten, LoaiCoSoId, DonViId, QuyMo, SDT)
        VALUES (N'Đại lý thuốc thú y Lam Sơn', @LoaiDaiLy, @TPThanhHoaId2, N'Cấp 1', '0912345678');
    END
END

IF @CuChiId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM CoSo WHERE Ten = N'Trại bò sữa hộ ông Nguyễn Văn Ba')
    BEGIN
        INSERT INTO CoSo (Ten, LoaiCoSoId, DonViId, QuyMo, SDT)
        VALUES (N'Trại bò sữa hộ ông Nguyễn Văn Ba', @LoaiCaNhan, @CuChiId, N'50 con bò sữa', '0988777666');
    END
END
GO
INSERT INTO LoaiDichBenh (Ten, MoTa)
SELECT Ten, MoTa
FROM (VALUES 
    (N'Tai xanh (PRRS)', N'Hội chứng rối loạn hô hấp và sinh sản ở lợn.'),
    (N'Viêm da nổi cục', N'Bệnh truyền nhiễm do virus gây ra trên trâu, bò.'),
    (N'Bệnh Dại (Rabies)', N'Bệnh truyền nhiễm cấp tính lây từ động vật sang người.'),
    (N'Tụ huyết trùng', N'Bệnh truyền nhiễm cấp tính ở gia súc, gia cầm.')
) AS Source(Ten, MoTa)
WHERE NOT EXISTS (SELECT 1 FROM LoaiDichBenh WHERE Ten = Source.Ten);
GO

-- ================================================================
-- PHẦN 2: INSERT DỮ LIỆU Ổ DỊCH (ODich)
-- ================================================================

-- --- BƯỚC 1: KHAI BÁO BIẾN ĐỂ LẤY ID (Lookup ID) ---
-- Khai báo biến ID Dịch bệnh
DECLARE @Id_H5N1 INT = (SELECT TOP 1 Id FROM LoaiDichBenh WHERE Ten LIKE N'%H5N1%');
DECLARE @Id_LMLM INT = (SELECT TOP 1 Id FROM LoaiDichBenh WHERE Ten LIKE N'%Lở mồm%');
DECLARE @Id_ASF INT = (SELECT TOP 1 Id FROM LoaiDichBenh WHERE Ten LIKE N'%Châu Phi%'); -- Tả lợn
DECLARE @Id_TaiXanh INT = (SELECT TOP 1 Id FROM LoaiDichBenh WHERE Ten LIKE N'%Tai xanh%');
DECLARE @Id_ViemDa INT = (SELECT TOP 1 Id FROM LoaiDichBenh WHERE Ten LIKE N'%Viêm da%');

-- Khai báo biến ID Đơn vị (Xã/Phường) - Dựa trên dữ liệu địa chính đã tạo
DECLARE @XaUyNo INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Uy Nỗ'); -- Đông Anh, Hà Nội
DECLARE @XaThoTang INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Thổ Tang'); -- Vĩnh Tường, Vĩnh Phúc
DECLARE @XaThanhCuong INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Thanh Cường'); -- Thanh Hà, Hải Dương
DECLARE @XaMinhTri INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Minh Trí'); -- Sóc Sơn, Hà Nội
DECLARE @XaNgocXa INT = (SELECT TOP 1 Id FROM DonVi WHERE Ten = N'Ngọc Xá'); -- Quế Võ, Bắc Ninh

-- --- BƯỚC 2: THỰC HIỆN INSERT ---

-- 1. Ổ dịch Tả lợn Châu Phi tại Uy Nỗ (Đông Anh)
IF @XaUyNo IS NOT NULL AND @Id_ASF IS NOT NULL
BEGIN
    INSERT INTO ODich (TenODich, DonViId, LoaiDichBenhId, NgayPhatHien, SoLuongMacBenh, SoLuongTieuHuy, TrangThai, NguyenNhan, ChanDoan, DaTiemPhong, GhiChu)
    VALUES (
        N'Ổ dịch Tả lợn hộ ông Nguyễn Văn A', 
        @XaUyNo, 
        @Id_ASF, 
        '2026-01-05', 
        55, -- Mắc bệnh
        55, -- Tiêu hủy (ASF thường hủy hết)
        N'Đang xử lý', 
        N'Nhập con giống trôi nổi không kiểm dịch', 
        N'Xét nghiệm PCR dương tính virus ASF', 
        0, -- Chưa tiêm phòng (ASF chưa phổ biến vắc xin đại trà)
        N'Đã rắc vôi bột và phun khử trùng chuồng trại.'
    );
END

-- 2. Ổ dịch Lở mồm long móng tại Thổ Tang (Vĩnh Tường - Chợ đầu mối)
IF @XaThoTang IS NOT NULL AND @Id_LMLM IS NOT NULL
BEGIN
    INSERT INTO ODich (TenODich, DonViId, LoaiDichBenhId, NgayPhatHien, SoLuongMacBenh, SoLuongTieuHuy, TrangThai, NguyenNhan, ChanDoan, DaTiemPhong, GhiChu)
    VALUES (
        N'Ổ dịch LMLM khu vực chợ gia súc', 
        @XaThoTang, 
        @Id_LMLM, 
        '2025-12-20', 
        120, -- Bò mắc bệnh
        0, -- LMLM thường chữa được, ít khi hủy trừ khi quá nặng
        N'Đã qua 21 ngày', 
        N'Lây lan từ xe vận chuyển gia súc ngoại tỉnh', 
        N'Triệu chứng lâm sàng: Loét miệng, móng', 
        1, -- Đã tiêm
        N'Đã tiêm phòng bao vây bán kính 3km.'
    );
END

-- 3. Ổ dịch Cúm H5N1 tại Thanh Cường (Hải Dương)
IF @XaThanhCuong IS NOT NULL AND @Id_H5N1 IS NOT NULL
BEGIN
    INSERT INTO ODich (TenODich, DonViId, LoaiDichBenhId, NgayPhatHien, SoLuongMacBenh, SoLuongTieuHuy, TrangThai, NguyenNhan, ChanDoan, DaTiemPhong, GhiChu)
    VALUES (
        N'Ổ dịch Cúm gia cầm đàn vịt chạy đồng', 
        @XaThanhCuong, 
        @Id_H5N1, 
        GETDATE(), -- Mới phát hiện hôm nay
        2000, 
        2000, -- Cúm gia cầm tiêu hủy toàn bộ
        N'Mới phát sinh', 
        N'Tiếp xúc với chim hoang dã', 
        N'Test nhanh dương tính H5', 
        0, 
        N'Khoanh vùng khẩn cấp, lập chốt kiểm dịch.'
    );
END

-- 4. Ổ dịch Tai xanh tại Ngọc Xá (Bắc Ninh)
IF @XaNgocXa IS NOT NULL AND @Id_TaiXanh IS NOT NULL
BEGIN
    INSERT INTO ODich (TenODich, DonViId, LoaiDichBenhId, NgayPhatHien, SoLuongMacBenh, SoLuongTieuHuy, TrangThai, NguyenNhan, ChanDoan, DaTiemPhong, GhiChu)
    VALUES (
        N'Ổ dịch Tai xanh (PRRS) HTX Chăn nuôi', 
        @XaNgocXa, 
        @Id_TaiXanh, 
        '2026-01-10', 
        300, 
        50, 
        N'Đang điều trị', 
        N'Thời tiết thay đổi, sức đề kháng giảm', 
        N'Sốt cao, tím tái vùng tai', 
        1, -- Đã tiêm nhưng vẫn bị (vắc xin hiệu quả không 100%)
        N'Đang điều trị hạ sốt, kháng viêm.'
    );
END

-- 5. Ổ dịch Viêm da nổi cục tại Minh Trí (Sóc Sơn)
IF @XaMinhTri IS NOT NULL AND @Id_ViemDa IS NOT NULL
BEGIN
    INSERT INTO ODich (TenODich, DonViId, LoaiDichBenhId, NgayPhatHien, SoLuongMacBenh, SoLuongTieuHuy, TrangThai, NguyenNhan, ChanDoan, DaTiemPhong, GhiChu)
    VALUES (
        N'Ổ dịch Viêm da nổi cục trên bò', 
        @XaMinhTri, 
        @Id_ViemDa, 
        '2025-11-15', 
        10, 
        2, 
        N'Đã công bố hết dịch', 
        N'Côn trùng đốt (ruồi, muỗi, ve)', 
        N'Nổi u cục trên da, sốt', 
        0, 
        N'Đã dập dịch thành công.'
    );
END
GO