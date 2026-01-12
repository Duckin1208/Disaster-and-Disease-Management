USE master
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QLDBTT')
BEGIN
    ALTER DATABASE QLDBTT SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLDBTT;
END
GO

CREATE DATABASE QLDBTT
GO

USE QLDBTT
GO

--1.Quản trị hệ thống
CREATE TABLE HoSo (
    Id int primary key identity, 
    Ten nvarchar(50), 
    SDT varchar(50), 
    Email varchar(50), 
    Ext text
)
GO
INSERT INTO HoSo (Ten, SDT, Email) VALUES
    (N'Vũ Song Tùng', '0989154248', 'tung.vusong@hust.edu.vn'),
    (N'Đào Lê Thu Thảo', '0989708960', 'thao.daolethu@hust.edu.vn'),
    (N'Phạm Văn Huấn', '0342666205', 'huan.pv233421@sis.hust.edu.vn')
GO

CREATE TABLE Quyen (
    Id int primary key identity, 
    Ten nvarchar(50), 
    Ext varchar(50)
)
GO
INSERT INTO Quyen (Ten, Ext) VALUES
    (N'Lập trình viên', 'Developer'),
    (N'Quản trị hệ thống', 'Admin'),
    (N'Cán bộ nghiệp vụ', 'Staff')
GO

CREATE TABLE TaiKhoan (
    Ten varchar(50) primary key, 
    MatKhau varchar(255), 
    QuyenId int foreign key references Quyen(Id), 
    HoSoId int foreign key references HoSo(Id)
)
GO
INSERT INTO TaiKhoan (Ten, MatKhau, QuyenId, HoSoId) VALUES
    ('dev', '1234', 1, 3), 
    ('admin', '1234', 2, null),
    ('0989154248', '1234', 3, 1),
    ('0989708960', '1234', 3, 2)
GO

CREATE VIEW ViewHoSo AS
    SELECT 
        hs.Id,
        hs.Ten as HoTen, 
        hs.SDT, 
        hs.Email, 
        tk.Ten as TenDangNhap, 
        tk.MatKhau, 
        tk.QuyenId, 
        q.Ten as TenQuyen 
    FROM TaiKhoan tk
    INNER JOIN Quyen q ON tk.QuyenId = q.Id
    INNER JOIN HoSo hs ON tk.HoSoId = hs.Id
GO

--2.Nhật ký thao tác
CREATE TABLE LichSuThaoTac (
    Id int primary key identity,
    TaiKhoanTen varchar(50) not null,
    ThaoTac nvarchar(255),
    GiaTriCu nvarchar(max),
    GiaTriMoi nvarchar(max),
    ThoiGianDangNhap datetime not null default getdate(),
    constraint FK_LichSuThaoTac_TaiKhoan foreign key (TaiKhoanTen) references TaiKhoan(Ten)
);
GO

CREATE VIEW ViewLichSuThaoTac AS
SELECT
    lstt.Id,
    lstt.TaiKhoanTen,
    q.Ten as Quyen,
    hs.Ten as HoTen,
    lstt.ThaoTac,
    lstt.GiaTriCu,
    lstt.GiaTriMoi,
    lstt.ThoiGianDangNhap
FROM LichSuThaoTac lstt
LEFT JOIN TaiKhoan tk ON lstt.TaiKhoanTen = tk.Ten
LEFT JOIN Quyen q ON tk.QuyenId = q.Id
LEFT JOIN HoSo hs ON tk.HoSoId = hs.Id;
GO

--4.Danh mục chi cục thú y vùng và Hành chính
CREATE TABLE HanhChinh (
    Id int primary key identity, 
    Ten nvarchar(50), 
    TrucThuocId int foreign key references HanhChinh(Id)
)
GO
INSERT INTO HanhChinh VALUES
    ( N'Tỉnh/Thành', NULL),
    ( N'Quận/Huyện', 1),
    ( N'Phường/Xã', 2),
    ( N'Tổ/Thôn', 3)
GO

CREATE TABLE ChiCucThuy (
    Id int primary key identity, 
    Ten nvarchar(100) not null
);
GO
INSERT INTO ChiCucThuy (Ten) VALUES
    (N'Chi cục Thú y vùng I (Hà Nội)'),      -- Id = 1
    (N'Chi cục Thú y vùng II (Hải Phòng)'),  -- Id = 2
    (N'Chi cục Thú y vùng III (Nghệ An)'),
    (N'Chi cục Thú y vùng IV (Đà Nẵng)'),
    (N'Chi cục Thú y vùng V (Đắk Lắk)'),
    (N'Chi cục Thú y vùng VI (TP.HCM)'),
    (N'Chi cục Thú y vùng VII (Cần Thơ)');
GO

CREATE TABLE DonVi (
    Id int primary key identity, 
    Ten nvarchar(50), 
    HanhChinhId int foreign key references HanhChinh(Id), 
    TenHanhChinh nvarchar(50), 
    TrucThuocId int foreign key references DonVi(Id),
    ChiCucThuyId int foreign key references ChiCucThuy(Id)
)
GO

-- Khai báo biến ID cho các Vùng 
DECLARE @Vung1 INT = 1; -- Hà Nội
DECLARE @Vung2 INT = 2; -- Hải Phòng
DECLARE @Vung3 INT = 3; -- Nghệ An
DECLARE @Vung4 INT = 4; -- Đà Nẵng
DECLARE @Vung5 INT = 5; -- Đắk Lắk
DECLARE @Vung6 INT = 6; -- TP.HCM
DECLARE @Vung7 INT = 7; -- Cần Thơ

-----------------------------------------------------------
-- CHI CỤC VÙNG I (Trụ sở Hà Nội & Các tỉnh lân cận/ĐBSH)
-----------------------------------------------------------
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung1
FROM (VALUES 
    (N'Hà Nội', N'Thành phố'),
    (N'Vĩnh Phúc', N'Tỉnh'),
    (N'Bắc Ninh', N'Tỉnh'),
    (N'Hưng Yên', N'Tỉnh'),
    (N'Hà Nam', N'Tỉnh'),
    (N'Nam Định', N'Tỉnh'),
    (N'Thái Bình', N'Tỉnh'),
    (N'Ninh Bình', N'Tỉnh'),
    (N'Hòa Bình', N'Tỉnh'),
    (N'Phú Thọ', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

-----------------------------------------------------------
-- CHI CỤC VÙNG II (Trụ sở Hải Phòng & Đông/Tây Bắc Bộ)
-----------------------------------------------------------
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung2
FROM (VALUES 
    (N'Hải Phòng', N'Thành phố'),
    (N'Hải Dương', N'Tỉnh'),
    (N'Quảng Ninh', N'Tỉnh'),
    (N'Bắc Giang', N'Tỉnh'),
    (N'Lạng Sơn', N'Tỉnh'),
    (N'Thái Nguyên', N'Tỉnh'),
    (N'Bắc Kạn', N'Tỉnh'),
    (N'Cao Bằng', N'Tỉnh'),
    (N'Tuyên Quang', N'Tỉnh'),
    (N'Hà Giang', N'Tỉnh'),
    (N'Lào Cai', N'Tỉnh'),
    (N'Yên Bái', N'Tỉnh'),
    (N'Sơn La', N'Tỉnh'),
    (N'Điện Biên', N'Tỉnh'),
    (N'Lai Châu', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

-----------------------------------------------------------
-- CHI CỤC VÙNG III (Bắc Trung Bộ)
-----------------------------------------------------------
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung3
FROM (VALUES 
    (N'Thanh Hóa', N'Tỉnh'),
    (N'Nghệ An', N'Tỉnh'),
    (N'Hà Tĩnh', N'Tỉnh'),
    (N'Quảng Bình', N'Tỉnh'),
    (N'Quảng Trị', N'Tỉnh'),
    (N'Thừa Thiên Huế', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

-----------------------------------------------------------
-- CHI CỤC VÙNG IV (Duyên hải Nam Trung Bộ)
-----------------------------------------------------------
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung4
FROM (VALUES 
    (N'Đà Nẵng', N'Thành phố'),
    (N'Quảng Nam', N'Tỉnh'),
    (N'Quảng Ngãi', N'Tỉnh'),
    (N'Bình Định', N'Tỉnh'),
    (N'Phú Yên', N'Tỉnh'),
    (N'Khánh Hòa', N'Tỉnh'),
    (N'Ninh Thuận', N'Tỉnh'),
    (N'Bình Thuận', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

-----------------------------------------------------------
-- CHI CỤC VÙNG V (Tây Nguyên)
-----------------------------------------------------------
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung5
FROM (VALUES 
    (N'Đắk Lắk', N'Tỉnh'),
    (N'Đắk Nông', N'Tỉnh'),
    (N'Gia Lai', N'Tỉnh'),
    (N'Kon Tum', N'Tỉnh'),
    (N'Lâm Đồng', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

-----------------------------------------------------------
-- CHI CỤC VÙNG VI (Đông Nam Bộ)
-----------------------------------------------------------
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung6
FROM (VALUES 
    (N'TP. Hồ Chí Minh', N'Thành phố'),
    (N'Bình Dương', N'Tỉnh'),
    (N'Bình Phước', N'Tỉnh'),
    (N'Đồng Nai', N'Tỉnh'),
    (N'Tây Ninh', N'Tỉnh'),
    (N'Bà Rịa - Vũng Tàu', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

-----------------------------------------------------------
-- CHI CỤC VÙNG VII (Đồng Bằng Sông Cửu Long)
-----------------------------------------------------------
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung7
FROM (VALUES 
    (N'Cần Thơ', N'Thành phố'),
    (N'Long An', N'Tỉnh'),
    (N'Tiền Giang', N'Tỉnh'),
    (N'Bến Tre', N'Tỉnh'),
    (N'Trà Vinh', N'Tỉnh'),
    (N'Vĩnh Long', N'Tỉnh'),
    (N'Đồng Tháp', N'Tỉnh'),
    (N'An Giang', N'Tỉnh'),
    (N'Kiên Giang', N'Tỉnh'),
    (N'Hậu Giang', N'Tỉnh'),
    (N'Sóc Trăng', N'Tỉnh'),
    (N'Bạc Liêu', N'Tỉnh'),
    (N'Cà Mau', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);
GO

-- Hà Nam
DECLARE @HaNamId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hà Nam' AND HanhChinhId = 1);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES
(N'Phủ Lý', 2, N'Thành phố', @HaNamId, 1), (N'Duy Tiên', 2, N'Thị xã', @HaNamId, 1),
(N'Kim Bảng', 2, N'Huyện', @HaNamId, 1), (N'Lý Nhân', 2, N'Huyện', @HaNamId, 1),
(N'Thanh Liêm', 2, N'Huyện', @HaNamId, 1), (N'Bình Lục', 2, N'Huyện', @HaNamId, 1);
GO

-- Nam Định
DECLARE @NamDinhId INT = (SELECT Id FROM DonVi WHERE Ten = N'Nam Định' AND HanhChinhId = 1);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES
(N'Nam Định', 2, N'Thành phố', @NamDinhId, 1), (N'Mỹ Lộc', 2, N'Huyện', @NamDinhId, 1),
(N'Vụ Bản', 2, N'Huyện', @NamDinhId, 1), (N'Ý Yên', 2, N'Huyện', @NamDinhId, 1),
(N'Nghĩa Hưng', 2, N'Huyện', @NamDinhId, 1), (N'Nam Trực', 2, N'Huyện', @NamDinhId, 1),
(N'Trực Ninh', 2, N'Huyện', @NamDinhId, 1), (N'Xuân Trường', 2, N'Huyện', @NamDinhId, 1),
(N'Giao Thủy', 2, N'Huyện', @NamDinhId, 1), (N'Hải Hậu', 2, N'Huyện', @NamDinhId, 1);
GO

-- Ninh Bình
DECLARE @NinhBinhId INT = (SELECT Id FROM DonVi WHERE Ten = N'Ninh Bình' AND HanhChinhId = 1);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES
(N'Ninh Bình', 2, N'Thành phố', @NinhBinhId, 1), (N'Tam Điệp', 2, N'Thành phố', @NinhBinhId, 1),
(N'Nho Quan', 2, N'Huyện', @NinhBinhId, 1), (N'Gia Viễn', 2, N'Huyện', @NinhBinhId, 1),
(N'Hoa Lư', 2, N'Huyện', @NinhBinhId, 1), (N'Yên Khánh', 2, N'Huyện', @NinhBinhId, 1),
(N'Kim Sơn', 2, N'Huyện', @NinhBinhId, 1), (N'Yên Mô', 2, N'Huyện', @NinhBinhId, 1);
GO

-- Hòa Bình
DECLARE @HoaBinhId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hoà Bình' AND HanhChinhId = 1);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES
(N'Hòa Bình', 2, N'Thành phố', @HoaBinhId, 1), (N'Đà Bắc', 2, N'Huyện', @HoaBinhId, 1),
(N'Lương Sơn', 2, N'Huyện', @HoaBinhId, 1), (N'Kim Bôi', 2, N'Huyện', @HoaBinhId, 1),
(N'Cao Phong', 2, N'Huyện', @HoaBinhId, 1), (N'Tân Lạc', 2, N'Huyện', @HoaBinhId, 1),
(N'Mai Châu', 2, N'Huyện', @HoaBinhId, 1), (N'Yên Thủy', 2, N'Huyện', @HoaBinhId, 1);
GO

-- Phú Thọ
DECLARE @PhuThoId INT = (SELECT Id FROM DonVi WHERE Ten = N'Phú Thọ' AND HanhChinhId = 1);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES
(N'Việt Trì', 2, N'Thành phố', @PhuThoId, 1), (N'Phú Thọ', 2, N'Thị xã', @PhuThoId, 1),
(N'Đoan Hùng', 2, N'Huyện', @PhuThoId, 1), (N'Hạ Hòa', 2, N'Huyện', @PhuThoId, 1),
(N'Thanh Ba', 2, N'Huyện', @PhuThoId, 1), (N'Phù Ninh', 2, N'Huyện', @PhuThoId, 1),
(N'Lâm Thao', 2, N'Huyện', @PhuThoId, 1), (N'Tam Nông', 2, N'Huyện', @PhuThoId, 1);
GO

-- Hải Phòng
DECLARE @HPId INT = (SELECT Id FROM DonVi WHERE Ten = N'Hải Phòng' AND HanhChinhId = 1);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES
(N'Hồng Bàng', 2, N'Quận', @HPId, 2), (N'Lê Chân', 2, N'Quận', @HPId, 2),
(N'Ngô Quyền', 2, N'Quận', @HPId, 2), (N'Kiến An', 2, N'Quận', @HPId, 2),
(N'Hải An', 2, N'Quận', @HPId, 2), (N'Đồ Sơn', 2, N'Quận', @HPId, 2),
(N'Thủy Nguyên', 2, N'Huyện', @HPId, 2), (N'An Dương', 2, N'Huyện', @HPId, 2),
(N'Tiên Lãng', 2, N'Huyện', @HPId, 2), (N'Vĩnh Bảo', 2, N'Huyện', @HPId, 2),
(N'Cát Hải', 2, N'Huyện', @HPId, 2), (N'Bạch Long Vĩ', 2, N'Huyện', @HPId, 2);
GO

-- Quảng Ninh
DECLARE @QNId INT = (SELECT Id FROM DonVi WHERE Ten = N'Quảng Ninh' AND HanhChinhId = 1);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES
(N'Hạ Long', 2, N'Thành phố', @QNId, 2), (N'Móng Cái', 2, N'Thành phố', @QNId, 2),
(N'Cẩm Phả', 2, N'Thành phố', @QNId, 2), (N'Uông Bí', 2, N'Thành phố', @QNId, 2),
(N'Đông Triều', 2, N'Thị xã', @QNId, 2), (N'Quảng Yên', 2, N'Thị xã', @QNId, 2),
(N'Vân Đồn', 2, N'Huyện', @QNId, 2), (N'Cô Tô', 2, N'Huyện', @QNId, 2);
GO

-- Bắc Ninh
DECLARE @BacNinhId INT = (SELECT Id FROM DonVi WHERE Ten = N'Bắc Ninh' AND HanhChinhId = 1);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId) VALUES
(N'Bắc Ninh', 2, N'Thành phố', @BacNinhId, 2), (N'Từ Sơn', 2, N'Thành phố', @BacNinhId, 2),
(N'Yên Phong', 2, N'Huyện', @BacNinhId, 2), (N'Quế Võ', 2, N'Huyện', @BacNinhId, 2),
(N'Tiên Du', 2, N'Huyện', @BacNinhId, 2), (N'Thuận Thành', 2, N'Thị xã', @BacNinhId, 2),
(N'Gia Bình', 2, N'Huyện', @BacNinhId, 2), (N'Lương Tài', 2, N'Huyện', @BacNinhId, 2);
GO

CREATE VIEW ViewDonVi AS
SELECT dv.Id, dv.Ten, dv.TenHanhChinh, hc.Ten as CapHanhChinh, cha.Ten as TrucThuoc, ct.Ten as TenChiCuc
FROM DonVi dv
JOIN HanhChinh hc ON dv.HanhChinhId = hc.Id
LEFT JOIN DonVi cha ON dv.TrucThuocId = cha.Id
LEFT JOIN ChiCucThuy ct ON dv.ChiCucThuyId = ct.Id;
GO

CREATE VIEW ViewChiCucThuy AS 
SELECT ct.Id, ct.Ten as TenChiCuc, dv.Ten as TinhQuanLy 
FROM ChiCucThuy ct 
LEFT JOIN DonVi dv ON dv.ChiCucThuyId = ct.Id 
WHERE dv.HanhChinhId = 1;
GO

--4.CSDL về thiên tai
CREATE TABLE LoaiThienTai (Id int primary key identity, Ten nvarchar(255) not null);
INSERT INTO LoaiThienTai VALUES (N'Trượt lở'), (N'Lũ quét');
GO

CREATE TABLE DiemThienTai (
    Id int primary key identity, 
    Ten nvarchar(255), 
    DonViId int not null, 
    LoaiThienTaiId int not null, 
    MucDo int not null,
    constraint FK_DiemThienTai_DonVi foreign key (DonViId) references DonVi(Id),
    constraint FK_DiemThienTai_LoaiThienTai foreign key (LoaiThienTaiId) references LoaiThienTai(Id)
);
GO
INSERT INTO DiemThienTai (DonViId, LoaiThienTaiId, MucDo)
SELECT dv.Id, ltt.Id, 5 FROM DonVi dv, LoaiThienTai ltt WHERE dv.Ten = N'Hà Giang' AND ltt.Ten = N'Lũ quét';
GO

CREATE VIEW ViewDiemThienTai AS
SELECT dtt.Id, dv.Ten as DonVi, hc.Ten as Cap, ltt.Ten as LoaiThienTai, dtt.MucDo
FROM DiemThienTai dtt
JOIN DonVi dv ON dtt.DonViId = dv.Id
JOIN HanhChinh hc ON dv.HanhChinhId = hc.Id
JOIN LoaiThienTai ltt ON dtt.LoaiThienTaiId = ltt.Id;
GO

--5.CSDL về dịch bệnh
CREATE TABLE LoaiDichBenh (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Ten NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(MAX)
);
INSERT INTO LoaiDichBenh (Ten) VALUES (N'Cúm gia cầm H5N1'), (N'Lở mồm long móng'), (N'Dịch tả lợn Châu Phi');
GO

CREATE TABLE ODich (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DonViId INT NOT NULL,           
    LoaiDichBenhId INT NOT NULL,    
    NgayPhatHien DATE DEFAULT GETDATE(),
    SoLuongMacBenh INT DEFAULT 0,
    SoLuongTieuHuy INT DEFAULT 0,
    TrangThai NVARCHAR(50),         
    NguyenNhan NVARCHAR(MAX),       
    DaTiemPhong BIT DEFAULT 0,      
    GhiChu NVARCHAR(MAX),
    FOREIGN KEY (DonViId) REFERENCES DonVi(Id),
    FOREIGN KEY (LoaiDichBenhId) REFERENCES LoaiDichBenh(Id)
);
GO

CREATE VIEW ViewODich AS
SELECT 
    od.Id,
    od.DonViId,
    dv.Ten AS TenDonVi,
    dv.CapHanhChinh AS CapHanhChinh,
    od.LoaiDichBenhId,
    ldb.Ten AS TenBenh,
    od.NgayPhatHien,
    od.SoLuongMacBenh,
    od.SoLuongTieuHuy,
    od.TrangThai,
    od.NguyenNhan,
    od.DaTiemPhong,
    od.GhiChu
FROM ODich od
LEFT JOIN ViewDonVi dv ON od.DonViId = dv.Id
LEFT JOIN LoaiDichBenh ldb ON od.LoaiDichBenhId = ldb.Id;
GO

--6.CSDL về chăn nuôi 
CREATE TABLE LoaiCoSo (Id int primary key identity, Ten nvarchar(255) not null);
INSERT INTO LoaiCoSo (Ten) VALUES 
(N'Đại lý thuốc thú y'),                         -- 4.3
(N'Khu tạm giữ, tiêu hủy'),                      -- 4.5
(N'Cơ sở chế biến sản phẩm chăn nuôi'),          -- 4.17
(N'Vùng chăn nuôi an toàn dịch bệnh'),           -- 4.19
(N'Tổ chức chứng nhận sự phù hợp');              -- 4.15
GO

CREATE TABLE CoSo (
    Id int primary key identity, 
    Ten nvarchar(255) not null, 
    LoaiCoSoId int not null foreign key references LoaiCoSo(Id), 
    DonViId int not null foreign key references DonVi(Id), 
    QuyMo nvarchar(100), 
    SDT varchar(20)
);
GO
INSERT INTO CoSo (Ten, LoaiCoSoId, DonViId, QuyMo, SDT)
SELECT N'Lò mổ Hưng Thịnh', lcs.Id, dv.Id, N'Lớn', '0987654321' 
FROM LoaiCoSo lcs, DonVi dv 
WHERE lcs.Ten = N'Cơ sở giết mổ' AND dv.Ten = N'Thái Thụy';
GO

CREATE TABLE GiayPhep (
    Id int primary key identity, 
    SoGiayPhep nvarchar(100), 
    NgayCap date, 
    NgayHetHan date, 
    CoSoId int not null foreign key references CoSo(Id)
);
GO

CREATE TABLE DieuKienChanNuoi (
    Id int primary key identity, 
    Ten nvarchar(255), 
    MoTa nvarchar(500), 
    GiayPhepId int not null foreign key references GiayPhep(Id)
);
GO
CREATE VIEW ViewCoSoFull AS
SELECT 
    cs.Id,
    cs.Ten,
    cs.QuyMo,
    cs.SDT,
    cs.LoaiCoSoId,
    lcs.Ten AS TenLoaiCoSo,
    cs.DonViId,
    dv.Ten AS TenDonVi,
    dv.TenHanhChinh AS CapHanhChinh 
    
FROM CoSo cs
LEFT JOIN LoaiCoSo lcs ON cs.LoaiCoSoId = lcs.Id
LEFT JOIN DonVi dv ON cs.DonViId = dv.Id;
GO

IF NOT EXISTS (SELECT * FROM LoaiCoSo WHERE Ten = N'Cá nhân chăn nuôi')
BEGIN
    INSERT INTO LoaiCoSo (Ten) VALUES (N'Cá nhân chăn nuôi');
END


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GiayPhep')
BEGIN
    CREATE TABLE GiayPhep (
        Id int primary key identity, 
        SoGiayPhep nvarchar(100), 
        NgayCap date, 
        NgayHetHan date, 
        CoSoId int not null foreign key references CoSo(Id)
    );
END
GO
---ISERT DỮ LIỆU 

--Dữ liệu về đơn vị hành chình 
-- KHAI BÁO CÁC BIẾN CỐ ĐỊNH
DECLARE @Vung1 INT = 1; -- Chi cục Vùng I (Quản lý Vĩnh Phúc)
DECLARE @Vung2 INT = 2; -- Chi cục Vùng II (Quản lý Bắc Ninh, Hải Dương)

------------------------------------------------------------------------------------
-- PHẦN 1: TỈNH VĨNH PHÚC (9 Đơn vị cấp Huyện)
------------------------------------------------------------------------------------
DECLARE @VinhPhucId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Vĩnh Phúc' AND HanhChinhId = 1);

-- 1.1. Insert các Huyện/Thành phố thuộc Vĩnh Phúc
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 2, Loai, @VinhPhucId, @Vung1
FROM (VALUES 
    (N'Vĩnh Yên', N'Thành phố'), (N'Phúc Yên', N'Thành phố'), 
    (N'Bình Xuyên', N'Huyện'), (N'Lập Thạch', N'Huyện'), 
    (N'Sông Lô', N'Huyện'), (N'Tam Dương', N'Huyện'), 
    (N'Tam Đảo', N'Huyện'), (N'Vĩnh Tường', N'Huyện'), 
    (N'Yên Lạc', N'Huyện')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @VinhPhucId);

-- 1.2. Insert Xã/Phường (Mẫu cho các đơn vị chính)

-- TP Vĩnh Yên
DECLARE @VinhYenId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Vĩnh Yên' AND TrucThuocId = @VinhPhucId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Phường', @VinhYenId, @Vung1 FROM (VALUES 
(N'Ngô Quyền'), (N'Đống Đa'), (N'Liên Bảo'), (N'Tích Sơn'), (N'Đồng Tâm'), (N'Hội Hợp'), (N'Khai Quang'), (N'Định Trung'), (N'Thanh Trù')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @VinhYenId);

-- TP Phúc Yên
DECLARE @PhucYenId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Phúc Yên' AND TrucThuocId = @VinhPhucId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Phường', @PhucYenId, @Vung1 FROM (VALUES 
(N'Trưng Trắc'), (N'Trưng Nhị'), (N'Hùng Vương'), (N'Nam Viêm'), (N'Phúc Thắng'), (N'Xuân Hòa'), (N'Đồng Xuân'), (N'Cao Minh'), (N'Ngọc Thanh'), (N'Tiền Châu')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @PhucYenId);

-- Huyện Vĩnh Tường (Huyện trọng điểm chăn nuôi)
DECLARE @VinhTuongId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Vĩnh Tường' AND TrucThuocId = @VinhPhucId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Xã', @VinhTuongId, @Vung1 FROM (VALUES 
(N'Thổ Tang'), (N'Vĩnh Tường'), (N'Tứ Trưng'), (N'Ngũ Kiên'), (N'Phú Đa'), (N'Tam Phúc'), (N'Vĩnh Ninh'), (N'Vĩnh Thịnh'), (N'Nghĩa Hưng')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @VinhTuongId);


------------------------------------------------------------------------------------
-- PHẦN 2: TỈNH BẮC NINH (8 Đơn vị cấp Huyện)
------------------------------------------------------------------------------------
DECLARE @BacNinhId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Bắc Ninh' AND HanhChinhId = 1);

-- 2.1. Insert Huyện/Thành phố
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 2, Loai, @BacNinhId, @Vung2
FROM (VALUES 
    (N'Bắc Ninh', N'Thành phố'), (N'Từ Sơn', N'Thành phố'), 
    (N'Yên Phong', N'Huyện'), (N'Quế Võ', N'Thị xã'), 
    (N'Tiên Du', N'Huyện'), (N'Thuận Thành', N'Thị xã'), 
    (N'Gia Bình', N'Huyện'), (N'Lương Tài', N'Huyện')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @BacNinhId);

-- 2.2. Insert Xã/Phường chi tiết

-- TP Bắc Ninh
DECLARE @TPBacNinhId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Bắc Ninh' AND TrucThuocId = @BacNinhId AND HanhChinhId = 2);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Phường', @TPBacNinhId, @Vung2 FROM (VALUES 
(N'Vũ Ninh'), (N'Đáp Cầu'), (N'Thị Cầu'), (N'Kinh Bắc'), (N'Vệ An'), (N'Tiền An'), (N'Đại Phúc'), (N'Ninh Xá'), (N'Suối Hoa'), (N'Võ Cường'), (N'Vạn An'), (N'Hòa Long')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TPBacNinhId);

-- TP Từ Sơn
DECLARE @TuSonId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Từ Sơn' AND TrucThuocId = @BacNinhId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Phường', @TuSonId, @Vung2 FROM (VALUES 
(N'Đông Ngàn'), (N'Đồng Kỵ'), (N'Đình Bảng'), (N'Tân Hồng'), (N'Trang Hạ'), (N'Đồng Nguyên'), (N'Châu Khê'), (N'Hương Mạc'), (N'Phù Chẩn'), (N'Phù Khê'), (N'Tam Sơn'), (N'Tương Giang')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TuSonId);

-- Huyện Yên Phong (Khu công nghiệp lớn)
DECLARE @YenPhongId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Yên Phong' AND TrucThuocId = @BacNinhId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Xã', @YenPhongId, @Vung2 FROM (VALUES 
(N'Thị trấn Chờ'), (N'Yên Phụ'), (N'Văn Môn'), (N'Đông Tiến'), (N'Tam Giang'), (N'Hòa Tiến'), (N'Tam Đa'), (N'Thụy Hòa'), (N'Yên Trung'), (N'Dũng Liệt')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @YenPhongId);


------------------------------------------------------------------------------------
-- PHẦN 3: TỈNH HẢI DƯƠNG (12 Đơn vị cấp Huyện)
------------------------------------------------------------------------------------
DECLARE @HaiDuongId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Hải Dương' AND HanhChinhId = 1);

-- 3.1. Insert Huyện/Thành phố
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 2, Loai, @HaiDuongId, @Vung2
FROM (VALUES 
    (N'Hải Dương', N'Thành phố'), (N'Chí Linh', N'Thành phố'), (N'Kinh Môn', N'Thị xã'), 
    (N'Bình Giang', N'Huyện'), (N'Cẩm Giàng', N'Huyện'), (N'Gia Lộc', N'Huyện'), 
    (N'Kim Thành', N'Huyện'), (N'Nam Sách', N'Huyện'), (N'Ninh Giang', N'Huyện'), 
    (N'Thanh Hà', N'Huyện'), (N'Thanh Miện', N'Huyện'), (N'Tứ Kỳ', N'Huyện')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @HaiDuongId);

-- 3.2. Insert Xã/Phường chi tiết

-- TP Hải Dương
DECLARE @TPHaiDuongId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Hải Dương' AND TrucThuocId = @HaiDuongId AND HanhChinhId = 2);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Phường', @TPHaiDuongId, @Vung2 FROM (VALUES 
(N'Cẩm Thượng'), (N'Bình Hàn'), (N'Ngọc Châu'), (N'Nhị Châu'), (N'Quang Trung'), (N'Nguyễn Trãi'), (N'Phạm Ngũ Lão'), (N'Trần Hưng Đạo'), (N'Trần Phú'), (N'Thanh Bình'), (N'Tân Bình'), (N'Lê Thanh Nghị'), (N'Hải Tân'), (N'Tứ Minh'), (N'Việt Hòa')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @TPHaiDuongId);

-- TP Chí Linh
DECLARE @ChiLinhId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Chí Linh' AND TrucThuocId = @HaiDuongId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Phường', @ChiLinhId, @Vung2 FROM (VALUES 
(N'Sao Đỏ'), (N'Phả Lại'), (N'Bến Tắm'), (N'Hoàng Tân'), (N'Cộng Hòa'), (N'Văn An'), (N'Chí Minh'), (N'Thái Học'), (N'Đỏ Lạc'), (N'An Lạc'), (N'Tân Dân'), (N'Đồng Lạc')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @ChiLinhId);

-- Huyện Cẩm Giàng
DECLARE @CamGiangId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Cẩm Giàng' AND TrucThuocId = @HaiDuongId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Xã', @CamGiangId, @Vung2 FROM (VALUES 
(N'Thị trấn Lai Cách'), (N'Thị trấn Cẩm Giang'), (N'Cẩm Hoàng'), (N'Cẩm Định'), (N'Kim Giang'), (N'Lương Điền'), (N'Ngọc Liên'), (N'Tân Trường'), (N'Cao An'), (N'Định Sơn')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @CamGiangId);

-- Huyện Gia Lộc
DECLARE @GiaLocId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Gia Lộc' AND TrucThuocId = @HaiDuongId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 3, N'Xã', @GiaLocId, @Vung2 FROM (VALUES 
(N'Thị trấn Gia Lộc'), (N'Thống Nhất'), (N'Yết Kiêu'), (N'Gia Tân'), (N'Gia Khánh'), (N'Gia Lương'), (N'Lê Lợi'), (N'Toàn Thắng'), (N'Hoàng Diệu'), (N'Hồng Hưng')
) AS S(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = S.Ten AND TrucThuocId = @GiaLocId);
GO


-- HÀ NỘI (Chi tiết)
-- 1. KHAI BÁO BIẾN CƠ BẢN
DECLARE @HanoiId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Hà Nội' AND HanhChinhId = 1);
DECLARE @Vung1 INT = 1; -- Chi cục Thú y vùng I (Quản lý Hà Nội)
DECLARE @Quan INT = 2; -- Cấp Quận/Huyện
DECLARE @Phuong INT = 3; -- Cấp Phường/Xã

-- Nếu chưa có Hà Nội thì báo lỗi (Vì script trước đã thêm rồi)
IF @HanoiId IS NULL
BEGIN
    PRINT N'Lỗi: Chưa có dữ liệu Tỉnh/Thành phố Hà Nội trong bảng DonVi.';
    RETURN;
END

-- 2. INSERT 30 QUẬN/HUYỆN/THỊ XÃ CỦA HÀ NỘI
-- Sử dụng bảng tạm để insert gọn gàng
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Quan, Loai, @HanoiId, @Vung1
FROM (VALUES 
    -- 12 Quận
    (N'Ba Đình', N'Quận'), (N'Hoàn Kiếm', N'Quận'), (N'Tây Hồ', N'Quận'), (N'Long Biên', N'Quận'),
    (N'Cầu Giấy', N'Quận'), (N'Đống Đa', N'Quận'), (N'Hai Bà Trưng', N'Quận'), (N'Hoàng Mai', N'Quận'),
    (N'Thanh Xuân', N'Quận'), (N'Nam Từ Liêm', N'Quận'), (N'Bắc Từ Liêm', N'Quận'), (N'Hà Đông', N'Quận'),
    -- 1 Thị xã
    (N'Sơn Tây', N'Thị xã'),
    -- 17 Huyện
    (N'Ba Vì', N'Huyện'), (N'Chương Mỹ', N'Huyện'), (N'Đan Phượng', N'Huyện'), (N'Đông Anh', N'Huyện'),
    (N'Gia Lâm', N'Huyện'), (N'Hoài Đức', N'Huyện'), (N'Mê Linh', N'Huyện'), (N'Mỹ Đức', N'Huyện'),
    (N'Phú Xuyên', N'Huyện'), (N'Phúc Thọ', N'Huyện'), (N'Quốc Oai', N'Huyện'), (N'Sóc Sơn', N'Huyện'),
    (N'Thạch Thất', N'Huyện'), (N'Thanh Oai', N'Huyện'), (N'Thanh Trì', N'Huyện'), (N'Thường Tín', N'Huyện'),
    (N'Ứng Hòa', N'Huyện')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @HanoiId);

-----------------------------------------------------------------------------------
-- 3. INSERT CHI TIẾT PHƯỜNG/XÃ CHO CÁC QUẬN NỘI THÀNH (DỮ LIỆU ĐẦY ĐỦ)
-----------------------------------------------------------------------------------

-- QUẬN BA ĐÌNH
DECLARE @BaDinh INT = (SELECT Id FROM DonVi WHERE Ten = N'Ba Đình' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @BaDinh, @Vung1
FROM (VALUES 
(N'Phúc Xá'), (N'Trúc Bạch'), (N'Vĩnh Phúc'), (N'Cống Vị'), (N'Liễu Giai'), 
(N'Nguyễn Trung Trực'), (N'Quán Thánh'), (N'Ngọc Hà'), (N'Điện Biên'), 
(N'Đội Cấn'), (N'Ngọc Khánh'), (N'Kim Mã'), (N'Giảng Võ'), (N'Thành Công')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @BaDinh);

-- QUẬN HOÀN KIẾM
DECLARE @HoanKiem INT = (SELECT Id FROM DonVi WHERE Ten = N'Hoàn Kiếm' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @HoanKiem, @Vung1
FROM (VALUES 
(N'Phúc Tân'), (N'Đồng Xuân'), (N'Hàng Mã'), (N'Hàng Buồm'), (N'Hàng Đào'), 
(N'Hàng Bồ'), (N'Cửa Đông'), (N'Lý Thái Tổ'), (N'Hàng Bạc'), (N'Hàng Gai'), 
(N'Chương Dương'), (N'Hàng Trống'), (N'Cửa Nam'), (N'Hàng Bông'), (N'Tràng Tiền'), 
(N'Trần Hưng Đạo'), (N'Phan Chu Trinh'), (N'Hàng Bài')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @HoanKiem);

-- QUẬN TÂY HỒ
DECLARE @TayHo INT = (SELECT Id FROM DonVi WHERE Ten = N'Tây Hồ' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @TayHo, @Vung1
FROM (VALUES 
(N'Phú Thượng'), (N'Nhật Tân'), (N'Tứ Liên'), (N'Quảng An'), 
(N'Xuân La'), (N'Yên Phụ'), (N'Bưởi'), (N'Thụy Khuê')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @TayHo);

-- QUẬN CẦU GIẤY
DECLARE @CauGiay INT = (SELECT Id FROM DonVi WHERE Ten = N'Cầu Giấy' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @CauGiay, @Vung1
FROM (VALUES 
(N'Nghĩa Đô'), (N'Nghĩa Tân'), (N'Mai Dịch'), (N'Dịch Vọng'), 
(N'Dịch Vọng Hậu'), (N'Quan Hoa'), (N'Yên Hòa'), (N'Trung Hòa')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @CauGiay);

-- QUẬN ĐỐNG ĐA
DECLARE @DongDa INT = (SELECT Id FROM DonVi WHERE Ten = N'Đống Đa' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @DongDa, @Vung1
FROM (VALUES 
(N'Cát Linh'), (N'Văn Miếu'), (N'Quốc Tử Giám'), (N'Láng Thượng'), (N'Ô Chợ Dừa'), 
(N'Văn Chương'), (N'Hàng Bột'), (N'Láng Hạ'), (N'Khâm Thiên'), (N'Thổ Quan'), 
(N'Nam Đồng'), (N'Trung Phụng'), (N'Quang Trung'), (N'Trung Liệt'), (N'Phương Liên'), 
(N'Thịnh Quang'), (N'Trung Tự'), (N'Kim Liên'), (N'Phương Mai'), (N'Ngã Tư Sở'), (N'Khương Thượng')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @DongDa);

-- QUẬN HAI BÀ TRƯNG
DECLARE @HaiBaTrung INT = (SELECT Id FROM DonVi WHERE Ten = N'Hai Bà Trưng' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @HaiBaTrung, @Vung1
FROM (VALUES 
(N'Nguyễn Du'), (N'Bạch Đằng'), (N'Phạm Đình Hổ'), (N'Lê Đại Hành'), (N'Đồng Nhân'), 
(N'Phố Huế'), (N'Đống Mác'), (N'Thanh Lương'), (N'Thanh Nhàn'), (N'Cầu Dền'), 
(N'Bách Khoa'), (N'Đồng Tâm'), (N'Vĩnh Tuy'), (N'Bạch Mai'), (N'Quỳnh Mai'), 
(N'Quỳnh Lôi'), (N'Minh Khai'), (N'Trương Định')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @HaiBaTrung);

-- QUẬN HOÀNG MAI
DECLARE @HoangMai INT = (SELECT Id FROM DonVi WHERE Ten = N'Hoàng Mai' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @HoangMai, @Vung1
FROM (VALUES 
(N'Thanh Trì'), (N'Vĩnh Hưng'), (N'Định Công'), (N'Mai Động'), (N'Tương Mai'), 
(N'Đại Kim'), (N'Tân Mai'), (N'Hoàng Văn Thụ'), (N'Giáp Bát'), (N'Lĩnh Nam'), 
(N'Thịnh Liệt'), (N'Trần Phú'), (N'Hoàng Liệt'), (N'Yên Sở')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @HoangMai);

-- QUẬN THANH XUÂN
DECLARE @ThanhXuan INT = (SELECT Id FROM DonVi WHERE Ten = N'Thanh Xuân' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @ThanhXuan, @Vung1
FROM (VALUES 
(N'Nhân Chính'), (N'Thượng Đình'), (N'Khương Trung'), (N'Khương Mai'), 
(N'Thanh Xuân Trung'), (N'Phương Liệt'), (N'Hạ Đình'), (N'Khương Đình'), 
(N'Thanh Xuân Bắc'), (N'Thanh Xuân Nam'), (N'Kim Giang')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @ThanhXuan);

-- QUẬN LONG BIÊN
DECLARE @LongBien INT = (SELECT Id FROM DonVi WHERE Ten = N'Long Biên' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @LongBien, @Vung1
FROM (VALUES 
(N'Thượng Thanh'), (N'Ngọc Thụy'), (N'Giang Biên'), (N'Đức Giang'), (N'Việt Hưng'), 
(N'Gia Thụy'), (N'Ngọc Lâm'), (N'Phúc Lợi'), (N'Bồ Đề'), (N'Sài Đồng'), 
(N'Long Biên'), (N'Thạch Bàn'), (N'Phúc Đồng'), (N'Cự Khối')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @LongBien);

-- QUẬN HÀ ĐÔNG
DECLARE @HaDong INT = (SELECT Id FROM DonVi WHERE Ten = N'Hà Đông' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, N'Phường', @HaDong, @Vung1
FROM (VALUES 
(N'Nguyễn Trãi'), (N'Mộ Lao'), (N'Văn Quán'), (N'Vạn Phúc'), (N'Yết Kiêu'), 
(N'Quang Trung'), (N'La Khê'), (N'Phú La'), (N'Phúc La'), (N'Hà Cầu'), 
(N'Yên Nghĩa'), (N'Kiến Hưng'), (N'Phú Lương'), (N'Phú Lãm'), (N'Dương Nội'), 
(N'Biên Giang'), (N'Đồng Mai')
) AS Source(Ten) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @HaDong);

-----------------------------------------------------------------------------------
-- 4. INSERT MỘT SỐ HUYỆN NGOẠI THÀNH TIÊU BIỂU (VÌ QUÁ DÀI NÊN INSERT ĐẠI DIỆN)
-----------------------------------------------------------------------------------

-- HUYỆN ĐÔNG ANH
DECLARE @DongAnh INT = (SELECT Id FROM DonVi WHERE Ten = N'Đông Anh' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, Loai, @DongAnh, @Vung1
FROM (VALUES 
(N'Thị trấn Đông Anh', N'Thị trấn'), (N'Bắc Hồng', N'Xã'), (N'Nam Hồng', N'Xã'), 
(N'Tiên Dương', N'Xã'), (N'Vân Nội', N'Xã'), (N'Uy Nỗ', N'Xã'), (N'Cổ Loa', N'Xã'), 
(N'Hải Bối', N'Xã'), (N'Kim Chung', N'Xã'), (N'Võng La', N'Xã'), (N'Đông Hội', N'Xã')
) AS Source(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @DongAnh);

-- HUYỆN GIA LÂM
DECLARE @GiaLam INT = (SELECT Id FROM DonVi WHERE Ten = N'Gia Lâm' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, Loai, @GiaLam, @Vung1
FROM (VALUES 
(N'Thị trấn Trâu Quỳ', N'Thị trấn'), (N'Yên Viên', N'Thị trấn'), (N'Bát Tràng', N'Xã'), 
(N'Kiêu Kỵ', N'Xã'), (N'Đa Tốn', N'Xã'), (N'Dương Xá', N'Xã'), (N'Cổ Bi', N'Xã'), 
(N'Đặng Xá', N'Xã'), (N'Phú Thị', N'Xã'), (N'Ninh Hiệp', N'Xã')
) AS Source(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @GiaLam);

-- HUYỆN THANH TRÌ
DECLARE @ThanhTri INT = (SELECT Id FROM DonVi WHERE Ten = N'Thanh Trì' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, Loai, @ThanhTri, @Vung1
FROM (VALUES 
(N'Thị trấn Văn Điển', N'Thị trấn'), (N'Tân Triều', N'Xã'), (N'Thanh Liệt', N'Xã'), 
(N'Tả Thanh Oai', N'Xã'), (N'Hữu Hòa', N'Xã'), (N'Tam Hiệp', N'Xã'), (N'Vĩnh Quỳnh', N'Xã'), 
(N'Ngũ Hiệp', N'Xã'), (N'Ngọc Hồi', N'Xã')
) AS Source(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @ThanhTri);

-- HUYỆN SÓC SƠN
DECLARE @SocSon INT = (SELECT Id FROM DonVi WHERE Ten = N'Sóc Sơn' AND TrucThuocId = @HanoiId);
INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, @Phuong, Loai, @SocSon, @Vung1
FROM (VALUES 
(N'Thị trấn Sóc Sơn', N'Thị trấn'), (N'Phú Minh', N'Xã'), (N'Phù Lỗ', N'Xã'), 
(N'Phù Linh', N'Xã'), (N'Bắc Sơn', N'Xã'), (N'Nam Sơn', N'Xã'), (N'Hồng Kỳ', N'Xã'), 
(N'Trung Giã', N'Xã'), (N'Minh Phú', N'Xã'), (N'Minh Trí', N'Xã')
) AS Source(Ten, Loai) WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten AND TrucThuocId = @SocSon);
GO

--Vùng chăn nuôi an toàn 
IF NOT EXISTS (SELECT 1 FROM LoaiCoSo WHERE Ten = N'Vùng chăn nuôi an toàn dịch bệnh')
BEGIN
    INSERT INTO LoaiCoSo (Ten) VALUES (N'Vùng chăn nuôi an toàn dịch bệnh');
END
GO
DECLARE @LoaiAnToanId INT = (SELECT Id FROM LoaiCoSo WHERE Ten = N'Vùng chăn nuôi an toàn dịch bệnh');

DECLARE @DongNaiId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Đồng Nai');
DECLARE @BinhDuongId INT = (SELECT Top 1 Id FROM DonVi WHERE Ten = N'Bình Dương');

IF @DongNaiId IS NOT NULL
BEGIN
    INSERT INTO CoSo (Ten, LoaiCoSoId, DonViId, QuyMo, SDT) VALUES 
    (N'Vùng an toàn dịch Cúm gia cầm Thống Nhất', @LoaiAnToanId, @DongNaiId, N'Quy mô Huyện', '02513888999'),
    (N'Vùng chăn nuôi heo an toàn Xuân Lộc', @LoaiAnToanId, @DongNaiId, N'Quy mô Huyện', '02513777888');
END

IF @BinhDuongId IS NOT NULL
BEGIN
    INSERT INTO CoSo (Ten, LoaiCoSoId, DonViId, QuyMo, SDT) VALUES 
    (N'Vùng gà an toàn Phú Giáo', @LoaiAnToanId, @BinhDuongId, N'Quy mô Huyện', '02743555666');
END
GO

-- 3. Cấp Giấy chứng nhận (Cái thì còn hạn, cái thì hết hạn để test hiển thị)
DECLARE @CoSo1 INT = (SELECT Top 1 Id FROM CoSo WHERE Ten = N'Vùng an toàn dịch Cúm gia cầm Thống Nhất');
DECLARE @CoSo2 INT = (SELECT Top 1 Id FROM CoSo WHERE Ten = N'Vùng gà an toàn Phú Giáo');

IF @CoSo1 IS NOT NULL
BEGIN
    -- Còn hạn
    INSERT INTO GiayPhep (SoGiayPhep, NgayCap, NgayHetHan, CoSoId) 
    VALUES (N'GCN-ATDB-2025-001', '2025-01-01', '2030-01-01', @CoSo1);
END

IF @CoSo2 IS NOT NULL
BEGIN
    -- Đã hết hạn (Test màu đỏ)
    INSERT INTO GiayPhep (SoGiayPhep, NgayCap, NgayHetHan, CoSoId) 
    VALUES (N'GCN-ATDB-2018-999', '2018-01-01', '2023-01-01', @CoSo2);
END
GO
