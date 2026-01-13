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
    (N'Chi cục Thú y vùng I (Hà Nội)'),
    (N'Chi cục Thú y vùng II (Hải Phòng)'),
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

DECLARE @Vung1 INT = 1;
DECLARE @Vung2 INT = 2;
DECLARE @Vung3 INT = 3;
DECLARE @Vung4 INT = 4;
DECLARE @Vung5 INT = 5;
DECLARE @Vung6 INT = 6;
DECLARE @Vung7 INT = 7;

INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung1
FROM (VALUES 
    (N'Hà Nội', N'Thành phố'), (N'Vĩnh Phúc', N'Tỉnh'), (N'Bắc Ninh', N'Tỉnh'), 
    (N'Hưng Yên', N'Tỉnh'), (N'Hà Nam', N'Tỉnh'), (N'Nam Định', N'Tỉnh'), 
    (N'Thái Bình', N'Tỉnh'), (N'Ninh Bình', N'Tỉnh'), (N'Hòa Bình', N'Tỉnh'), (N'Phú Thọ', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung2
FROM (VALUES 
    (N'Hải Phòng', N'Thành phố'), (N'Hải Dương', N'Tỉnh'), (N'Quảng Ninh', N'Tỉnh'), 
    (N'Bắc Giang', N'Tỉnh'), (N'Lạng Sơn', N'Tỉnh'), (N'Thái Nguyên', N'Tỉnh'), 
    (N'Bắc Kạn', N'Tỉnh'), (N'Cao Bằng', N'Tỉnh'), (N'Tuyên Quang', N'Tỉnh'), 
    (N'Hà Giang', N'Tỉnh'), (N'Lào Cai', N'Tỉnh'), (N'Yên Bái', N'Tỉnh'), 
    (N'Sơn La', N'Tỉnh'), (N'Điện Biên', N'Tỉnh'), (N'Lai Châu', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung3
FROM (VALUES 
    (N'Thanh Hóa', N'Tỉnh'), (N'Nghệ An', N'Tỉnh'), (N'Hà Tĩnh', N'Tỉnh'), 
    (N'Quảng Bình', N'Tỉnh'), (N'Quảng Trị', N'Tỉnh'), (N'Thừa Thiên Huế', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung4
FROM (VALUES 
    (N'Đà Nẵng', N'Thành phố'), (N'Quảng Nam', N'Tỉnh'), (N'Quảng Ngãi', N'Tỉnh'), 
    (N'Bình Định', N'Tỉnh'), (N'Phú Yên', N'Tỉnh'), (N'Khánh Hòa', N'Tỉnh'), 
    (N'Ninh Thuận', N'Tỉnh'), (N'Bình Thuận', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung5
FROM (VALUES 
    (N'Đắk Lắk', N'Tỉnh'), (N'Đắk Nông', N'Tỉnh'), (N'Gia Lai', N'Tỉnh'), 
    (N'Kon Tum', N'Tỉnh'), (N'Lâm Đồng', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung6
FROM (VALUES 
    (N'TP. Hồ Chí Minh', N'Thành phố'), (N'Bình Dương', N'Tỉnh'), (N'Bình Phước', N'Tỉnh'), 
    (N'Đồng Nai', N'Tỉnh'), (N'Tây Ninh', N'Tỉnh'), (N'Bà Rịa - Vũng Tàu', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);

INSERT INTO DonVi (Ten, HanhChinhId, TenHanhChinh, TrucThuocId, ChiCucThuyId)
SELECT Ten, 1, Loai, NULL, @Vung7
FROM (VALUES 
    (N'Cần Thơ', N'Thành phố'), (N'Long An', N'Tỉnh'), (N'Tiền Giang', N'Tỉnh'), 
    (N'Bến Tre', N'Tỉnh'), (N'Trà Vinh', N'Tỉnh'), (N'Vĩnh Long', N'Tỉnh'), 
    (N'Đồng Tháp', N'Tỉnh'), (N'An Giang', N'Tỉnh'), (N'Kiên Giang', N'Tỉnh'), 
    (N'Hậu Giang', N'Tỉnh'), (N'Sóc Trăng', N'Tỉnh'), (N'Bạc Liêu', N'Tỉnh'), (N'Cà Mau', N'Tỉnh')
) AS Source(Ten, Loai)
WHERE NOT EXISTS (SELECT 1 FROM DonVi WHERE Ten = Source.Ten);
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

CREATE VIEW ViewDiemThienTai AS
SELECT dtt.Id, dv.Ten as DonVi, hc.Ten as Cap, ltt.Ten as LoaiThienTai, dtt.MucDo
FROM DiemThienTai dtt
JOIN DonVi dv ON dtt.DonViId = dv.Id
JOIN HanhChinh hc ON dv.HanhChinhId = hc.Id
JOIN LoaiThienTai ltt ON dtt.LoaiThienTaiId = ltt.Id;
GO

CREATE TABLE LoaiDichBenh (
    Id INT PRIMARY KEY IDENTITY,
    Ten NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(MAX)
);
INSERT INTO LoaiDichBenh (Ten) VALUES (N'Cúm gia cầm H5N1'), (N'Lở mồm long móng'), (N'Dịch tả lợn Châu Phi');
GO

CREATE TABLE ODich (
    Id INT PRIMARY KEY IDENTITY,
    TenODich NVARCHAR(255),
    DonViId INT NOT NULL,            
    LoaiDichBenhId INT NOT NULL,     
    NgayPhatHien DATE DEFAULT GETDATE(),
    SoLuongMacBenh INT DEFAULT 0,
    SoLuongTieuHuy INT DEFAULT 0,
    TrangThai NVARCHAR(50),          
    NguyenNhan NVARCHAR(MAX),        
    ChanDoan NVARCHAR(MAX),
    DaTiemPhong BIT DEFAULT 0,       
    GhiChu NVARCHAR(MAX),
    FOREIGN KEY (DonViId) REFERENCES DonVi(Id),
    FOREIGN KEY (LoaiDichBenhId) REFERENCES LoaiDichBenh(Id)
);
GO

CREATE VIEW ViewODich AS
SELECT 
    od.Id,
    od.TenODich,
    od.DonViId,
    dv.Ten AS TenDonVi,
    dv.TenHanhChinh AS CapHanhChinh,
    od.LoaiDichBenhId,
    ldb.Ten AS TenBenh,
    od.NgayPhatHien,
    od.SoLuongMacBenh,
    od.SoLuongTieuHuy,
    od.TrangThai,
    od.NguyenNhan,
    od.ChanDoan,
    od.DaTiemPhong,
    od.GhiChu
FROM ODich od
LEFT JOIN DonVi dv ON od.DonViId = dv.Id
LEFT JOIN LoaiDichBenh ldb ON od.LoaiDichBenhId = ldb.Id;
GO

CREATE TABLE TiemPhong (
    Id INT PRIMARY KEY IDENTITY,
    TenDotTiem NVARCHAR(255) NOT NULL, 
    LoaiDichBenhId INT NOT NULL,        
    ODichId INT NOT NULL,
    NgayTiem DATE DEFAULT GETDATE(),    
    LoaiVaccine NVARCHAR(255),          
    SoLuong INT DEFAULT 0,              
    NguoiThucHien NVARCHAR(255),       
    GhiChu NVARCHAR(MAX),
    FOREIGN KEY (LoaiDichBenhId) REFERENCES LoaiDichBenh(Id),
    FOREIGN KEY (ODichId) REFERENCES ODich(Id) ON DELETE CASCADE
);
GO

CREATE VIEW ViewTiemPhong AS
SELECT 
    tp.Id,
    tp.TenDotTiem,
    tp.NgayTiem,
    tp.LoaiVaccine,
    tp.SoLuong,
    tp.NguoiThucHien,
    tp.GhiChu,
    tp.ODichId,
    tp.LoaiDichBenhId,
    od.TenODich,
    ldb.Ten AS TenBenh
FROM TiemPhong tp
LEFT JOIN LoaiDichBenh ldb ON tp.LoaiDichBenhId = ldb.Id
LEFT JOIN ODich od ON tp.ODichId = od.Id;
GO

CREATE TABLE LoaiCoSo (Id int primary key identity, Ten nvarchar(255) not null);
INSERT INTO LoaiCoSo (Ten) VALUES 
(N'Đại lý thuốc thú y'),
(N'Khu tạm giữ, tiêu hủy'),
(N'Cơ sở chế biến sản phẩm chăn nuôi'),
(N'Vùng chăn nuôi an toàn dịch bệnh'),
(N'Tổ chức chứng nhận sự phù hợp');
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

IF NOT EXISTS (SELECT 1 FROM LoaiCoSo WHERE Ten = N'Vùng chăn nuôi an toàn dịch bệnh')
BEGIN
    INSERT INTO LoaiCoSo (Ten) VALUES (N'Vùng chăn nuôi an toàn dịch bệnh');
END
GO