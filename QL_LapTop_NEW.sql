CREATE DATABASE QL_LAPTOP
GO
USE QL_LAPTOP
GO
--- BẢNG KHÁCH HÀNG (Thông tin khách hàng)
create table PHANQUYEN
( 
	MAQUYEN int  primary key not null ,
	TENQUYEN nvarchar(30), 
	MOTA nvarchar(200),
)

create table CAPQUYEN
(
	TENDN nvarchar(30)  not null unique,
	MAQUYEN int not null,
	constraint PK_CQ primary key(TenDN,MAQUYEN),
	constraint FK_CQ foreign key (MAQUYEN) references PHANQUYEN(MAQUYEN)
)

CREATE TABLE KHACHHANG
(
	MAKH int identity PRIMARY KEY NOT NULL,
	HOTEN NVARCHAR(100) NOT NULL,
	SODT VARCHAR(11) UNIQUE CHECK(SODT LIKE '0%'),
	EMAIL NVARCHAR(100) UNIQUE,
	DIACHI NVARCHAR(100),
	NGAYSINH DATE default Getdate(),
	MATKHAU VARCHAR(50) NOT NULL,
	GIOITINH NVARCHAR(5) CHECK(GIOITINH IN(N'NAM',N'NỮ'))
);

--- BẢNG NHÂN VIÊN (Nhân viên bán hàng)
CREATE TABLE NHANVIEN
(
	MANV int identity PRIMARY KEY NOT NULL,
	HOTEN NVARCHAR(100) NOT NULL,
	CHUCVU NVARCHAR(50) DEFAULT N'Nhân viên' ,  --- mặc định là nhân viên
    CHECK(CHUCVU IN (N'Giám đốc', N'Quản lý', N'Trưởng phòng', N'Nhân viên bán hàng', N'Kế toán')),
	SODT VARCHAR(11) UNIQUE CHECK(SODT LIKE '0%'),
	EMAIL NVARCHAR(50) UNIQUE,
	LUONG DECIMAL(12,2) CHECK(LUONG>0),
	MATKHAU VARCHAR(50) NOT NULL,
	NGAYVAOLAM DATE DEFAULT GETDATE(),
	TenDN nvarchar(30),
	constraint FK_NV foreign key(TenDN) references CAPQUYEN(TenDN)
);	

--- BẢNG NHÀ CUNG CẤP (Nhà cung cấp laptop)
CREATE TABLE NHACUNGCAP
(
	MANCC int identity PRIMARY KEY NOT NULL,
	TENNCC NVARCHAR(100) NOT NULL,
	SODT VARCHAR(11) UNIQUE CHECK(SODT LIKE '0%'),
	EMAIL NVARCHAR(50) UNIQUE,
	DIACHI NVARCHAR(100)
);

--- BẢNG LOẠI LAPTOP (Phân loại laptop theo hãng/dòng)
CREATE TABLE LOAI_LAPTOP
(
	MALOAI int identity PRIMARY KEY NOT NULL,
	TENLOAI NVARCHAR(100) UNIQUE NOT NULL
);

--- BẢNG LAPTOP(Danh mục sản phẩm)
CREATE TABLE LAPTOP
(
	MALAPTOP int identity PRIMARY KEY NOT NULL,
	TENLAPTOP NVARCHAR(100) NOT NULL,
	GIA DECIMAL(10,2) CHECK(GIA>0) NOT NULL, 
	CAUHINH NVARCHAR(500),
	MALOAI int NOT NULL, 
	MANCC int  NOT NULL, 
	FOREIGN KEY (MALOAI) REFERENCES LOAI_LAPTOP(MALOAI),
	FOREIGN KEY (MANCC) REFERENCES NHACUNGCAP(MANCC)
);

--- BẢNG KHO (Quản lý tồn kho) 
CREATE TABLE KHO
(
	MAKHO int identity not null,
	MALAPTOP int, 
	SOLUONG INT CHECK(SOLUONG>=0) DEFAULT 0, -- Số lượng mặc định là 0
	constraint fk_kho primary key(MAKHO,MALAPTOP),
	FOREIGN KEY (MALAPTOP) REFERENCES LAPTOP(MALAPTOP)
);

--- BẢNG HÓA ĐƠN(Hóa đơn bán hàng) 
CREATE TABLE HOADON
(
	MAHD int identity PRIMARY KEY NOT NULL,
	NGAYLAP DATE DEFAULT GETDATE(),
	MAKH int NOT NULL,
	MANV int NOT NULL,
	TONGTIEN DECIMAL(15,2) ,  
	TRANGTHAI_THANHTOAN NVARCHAR(30) DEFAULT N'CHƯA THANH TOÁN' CHECK(TRANGTHAI_THANHTOAN IN (N'ĐÃ THANH TOÁN',N'CHƯA THANH TOÁN')),
	FOREIGN KEY (MAKH) REFERENCES KHACHHANG(MAKH),
	FOREIGN KEY (MANV) REFERENCES NHANVIEN(MANV)
);

--- BẢNG CHI TIẾT HÓA ĐƠN(Chi tiết sản phẩm trong hóa đơn)
CREATE TABLE CT_HOADON
(
	MAHD int identity NOT NULL,
	MALAPTOP int NOT NULL,
	SOLUONG INT CHECK(SOLUONG>0) NOT NULL,
	DONGIA DECIMAL(12,2) CHECK(DONGIA>0) NOT NULL,
	THANHTIEN AS (SOLUONG * DONGIA), 
	PRIMARY KEY (MAHD,MALAPTOP),
	FOREIGN KEY (MAHD) REFERENCES HOADON(MAHD),
	FOREIGN KEY (MALAPTOP) REFERENCES LAPTOP(MALAPTOP)
);

--- BẢNG BẢO HÀNH(Thông tin bảo hành)
CREATE TABLE BAOHANH
(
	MABH int identity PRIMARY KEY NOT NULL,
	MALAPTOP int NOT NULL,
	THOIHAN_THANG INT CHECK(THOIHAN_THANG>0), 
	DIEUKIEN NVARCHAR(100),
	FOREIGN KEY (MALAPTOP) REFERENCES LAPTOP(MALAPTOP)
);

--- BẢNG THANH TOÁN(Thông tin thanh toán) -- trường hợp 1 hóa đơn chỉ có 1 lần thanh toán
CREATE TABLE THANHTOAN
(
	MAHD int PRIMARY KEY NOT NULL, 
	HINHTHUC NVARCHAR(50) CHECK(HINHTHUC IN (N'TIỀN MẶT',N'CHUYỂN KHOẢN',N'THẺ TÍN DỤNG')) NOT NULL,
	SOTIEN DECIMAL(10,2) CHECK(SOTIEN>0) NOT NULL,
	NGAYTHANHTOAN DATE DEFAULT GETDATE(), 
	FOREIGN KEY (MAHD) REFERENCES HOADON(MAHD)
);



---------------------------------------------------------
-- KHÁCH HÀNG
---------------------------------------------------------
INSERT INTO KHACHHANG (HOTEN, SODT, EMAIL, DIACHI, NGAYSINH, MATKHAU, GIOITINH)
VALUES
(N'Nguyễn Văn A', '0901234567', 'anva@mail.com', N'108 Láng Hạ, Hà Nội', '1995-05-15', '123456', N'NAM'),
(N'Trần Thị B', '0987654321', 'thibtran@mail.com', N'12 Trần Phú, Đà Nẵng', '1998-08-22', '123456', N'NỮ'),
(N'Lê Minh C', '0912345678', 'minhc.le@mail.com', N'59 Nguyễn Huệ, TP.HCM', '2000-11-30', '123456', N'NAM');

---------------------------------------------------------
-- NHÂN VIÊN
---------------------------------------------------------
INSERT INTO NHANVIEN (HOTEN, CHUCVU, SODT, EMAIL, LUONG, MATKHAU)
VALUES
(N'Nguyễn Văn Quản', N'Quản lý', '0909123456', 'quan.nguyen@mail.com', 15000000, '123456'),
(N'Trần Thị Bán', N'Nhân viên bán hàng', '0911223344', 'ban.tran@mail.com', 10000000, '123456'),
(N'Lê Minh Kho', N'Kế toán', '0922334455', 'kho.le@mail.com', 12000000, '123456');

---------------------------------------------------------
-- NHÀ CUNG CẤP
---------------------------------------------------------
INSERT INTO NHACUNGCAP (TENNCC, SODT, EMAIL, DIACHI)
VALUES
(N'Công ty ASUS Việt Nam', '0281234567', 'support@asus.vn', N'12 Nguyễn Văn Linh, TP.HCM'),
(N'Công ty Dell Việt Nam', '0289876543', 'contact@dell.vn', N'23 Nguyễn Thị Minh Khai, Hà Nội'),
(N'Công ty Acer Việt Nam', '0289988776', 'sales@acer.vn', N'45 Lý Thường Kiệt, Đà Nẵng');

---------------------------------------------------------
-- LOẠI LAPTOP
---------------------------------------------------------
INSERT INTO LOAI_LAPTOP (TENLOAI)
VALUES
(N'Văn phòng'),
(N'Gaming'),
(N'Đồ họa'),
(N'Cao cấp');

---------------------------------------------------------
-- LAPTOP
---------------------------------------------------------
INSERT INTO LAPTOP (TENLAPTOP, GIA, CAUHINH, MALOAI, MANCC)
VALUES
(N'ASUS Vivobook 15', 15500000, N'CPU i5, RAM 8GB, SSD 512GB', 1, 1),
(N'Dell Inspiron 14', 16500000, N'i5, RAM 16GB, SSD 512GB', 1, 2),
(N'Acer Nitro 5', 25000000, N'RTX 3050, i5, RAM 16GB', 2, 3),
(N'ASUS ROG Strix', 42000000, N'RTX 4070, i9, RAM 32GB', 2, 1),
(N'MacBook Air M2', 32000000, N'Chip M2, SSD 512GB, pin 18 tiếng', 4, 1);

---------------------------------------------------------
-- KHO
---------------------------------------------------------
INSERT INTO KHO (MALAPTOP, SOLUONG)
VALUES
(1, 15),
(2, 8),
(3, 5),
(4, 3),
(5, 4);

---------------------------------------------------------
-- HÓA ĐƠN
---------------------------------------------------------
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN, TRANGTHAI_THANHTOAN)
VALUES
('2024-03-10', 1, 2, 15500000, N'ĐÃ THANH TOÁN'),
('2024-04-15', 2, 2, 41500000, N'CHƯA THANH TOÁN'),
('2024-06-01', 3, 1, 32000000, N'ĐÃ THANH TOÁN');

---------------------------------------------------------
-- CHI TIẾT HÓA ĐƠN
---------------------------------------------------------
INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA)
VALUES
(1, 1, 1, 15500000),
(2, 3, 1, 25000000),
(2, 4, 1, 16500000),
(3, 5, 1, 32000000);

---------------------------------------------------------
-- BẢO HÀNH
---------------------------------------------------------
INSERT INTO BAOHANH (MALAPTOP, THOIHAN_THANG, DIEUKIEN)
VALUES
(1, 12, N'Bảo hành chính hãng 1 năm'),
(2, 24, N'Bảo hành phần cứng 2 năm'),
(3, 12, N'Bảo hành 1 năm cho linh kiện'),
(4, 36, N'Bảo hành 3 năm chính hãng'),
(5, 24, N'Bảo hành 2 năm Apple');

---------------------------------------------------------
-- THANH TOÁN
---------------------------------------------------------
INSERT INTO THANHTOAN (MAHD, HINHTHUC, SOTIEN)
VALUES
(1, N'TIỀN MẶT', 15500000),
(2, N'CHUYỂN KHOẢN', 41500000),
(3, N'THẺ TÍN DỤNG', 32000000);

---------------------------------------------------------
-- PHÂN QUYỀN
---------------------------------------------------------
INSERT INTO PhanQuyen (TenQuyen)
VALUES
(N'Thêm sản phẩm'),
(N'Xóa sản phẩm'),
(N'Sửa sản phẩm'),
(N'Xem hóa đơn'),
(N'Quản lý tài khoản');

---------------------------------------------------------
-- CẤP QUYỀN
---------------------------------------------------------
INSERT INTO CapQuyen (TenDN, MaQuyen)
VALUES
(N'admin', 1),
(N'admin', 2),
(N'admin', 3),
(N'admin', 4),
(N'nhanvien', 4);




ALTER TABLE LAPTOP
ADD HINHANH NVARCHAR(255);

UPDATE LAPTOP
SET HINHANH = 'HP360.jpg'
WHERE MALAPTOP = '1';

UPDATE LAPTOP
SET HINHANH = 'DELL13.jpg'
WHERE MALAPTOP = '2';

UPDATE LAPTOP
SET HINHANH = 'ASUS14.jpg'
WHERE MALAPTOP = '3';

UPDATE LAPTOP
SET HINHANH = 'SS4.jpg'
WHERE MALAPTOP = '4';

UPDATE LAPTOP
SET HINHANH = 'HELIOS.jpg'
WHERE MALAPTOP = '5';

UPDATE LAPTOP
SET HINHANH = 'X1.jpg'
WHERE MALAPTOP = '6';

UPDATE LAPTOP
SET HINHANH = 'MSI15.jpg'
WHERE MALAPTOP = '7';

UPDATE LAPTOP
SET HINHANH = 'M3.jpg'
WHERE MALAPTOP = '8';

UPDATE LAPTOP
SET HINHANH = 'SURFACE6.jpg'
WHERE MALAPTOP = '9';

UPDATE LAPTOP
SET HINHANH = 'U9311.jpg'
WHERE MALAPTOP = '10';


SELECT * FROM KHACHHANG
SELECT * FROM NHANVIEN
SELECT * FROM NHACUNGCAP
SELECT * FROM LOAI_LAPTOP
SELECT * FROM LAPTOP
SELECT * FROM KHO
SELECT * FROM HOADON
SELECT * FROM CT_HOADON
SELECT * FROM BAOHANH
SELECT * FROM THANHTOAN


--- 1. Truy vấn cơ bản (SELECT, WHERE, ORDER BY)
-- Câu 1.1: Xem tất cả thông tin khách hàng nữ sinh sau năm 2000.
SELECT *
FROM KHACHHANG
WHERE GIOITINH = N'NỮ' AND YEAR(NGAYSINH)>2000;
-- Câu 1.2: Lấy thông tin Laptop của hãng Dell và sắp xếp theo giá giảm dần.
SELECT LT.TENLAPTOP, LT.GIA, NCC.TENNCC, LL.TENLOAI
FROM LAPTOP LT
JOIN NHACUNGCAP NCC ON LT.MANCC = NCC.MANCC
JOIN LOAI_LAPTOP LL ON LT.MALOAI = LL.MALOAI
WHERE NCC.TENNCC LIKE N'%Dell%'
ORDER BY LT.GIA DESC;
-- Câu 1.3: Xem các nhân viên không phải là 'Nhân viên bán hàng' và có lương trên 10 triệu.
SELECT MANV, HOTEN, CHUCVU, LUONG
FROM NHANVIEN
WHERE CHUCVU <> N'Nhân viên bán hàng' AND LUONG > 10000000.00
ORDER BY LUONG DESC;

--- 2. Truy vấn thống kê và tổng hợp (GROUP BY, SUM, COUNT, HAVING)
-- Câu 2.1: Đếm số lượng laptop tồn kho của mỗi loại (hãng/dòng).
SELECT LL.TENLOAI, SUM(K.SOLUONG) AS TONG_TONKHO
FROM KHO K
JOIN LAPTOP LT ON K.MALAPTOP = LT.MALAPTOP
JOIN LOAI_LAPTOP LL ON LT.MALOAI = LL.MALOAI
GROUP BY LL.TENLOAI
ORDER BY TONG_TONKHO DESC;
-- Câu 2.2: Tính tổng doanh thu (Tổng Tiền) của mỗi nhân viên trong tháng 9 năm 2024.
SELECT NV.HOTEN, SUM(HD.TONGTIEN) AS TONG_DOANHTHU
FROM HOADON HD
JOIN NHANVIEN NV ON HD.MANV = NV.MANV
WHERE YEAR(HD.NGAYLAP) = 2024 AND MONTH(HD.NGAYLAP) = 9
GROUP BY NV.HOTEN
ORDER BY TONG_DOANHTHU DESC;
-- Câu 2.3: Tìm khách hàng đã mua hàng với tổng giá trị hóa đơn lớn hơn 50 triệu.
SELECT KH.HOTEN, COUNT(HD.MAHD) AS SO_LUONG_HOA_DON, SUM(HD.TONGTIEN) AS TONG_GIA_TRI_MUA
FROM HOADON HD
JOIN KHACHHANG KH ON HD.MAKH = KH.MAKH
GROUP BY KH.HOTEN
HAVING SUM(HD.TONGTIEN) > 50000000.00
ORDER BY TONG_GIA_TRI_MUA DESC;

--- 3. Truy vấn phức tạp hơn (JOIN, Subquery)
-- Câu 3.1: Liệt kê chi tiết các sản phẩm (tên laptop, số lượng, đơn giá) trong hóa đơn HD002.
SELECT HD.MAHD, LT.TENLAPTOP, CT.SOLUONG, CT.DONGIA, CT.THANHTIEN
FROM CT_HOADON CT
JOIN HOADON HD ON CT.MAHD = HD.MAHD
JOIN LAPTOP LT ON CT.MALAPTOP = LT.MALAPTOP
WHERE HD.MAHD = 'HD002';
-- Câu 3.2: Tìm các Laptop chưa từng xuất hiện trong bất kỳ Hóa đơn nào (Dùng NOT IN hoặc LEFT JOIN).

-- Sử dụng NOT IN
SELECT TENLAPTOP
FROM LAPTOP
WHERE MALAPTOP NOT IN (SELECT DISTINCT MALAPTOP FROM CT_HOADON);
-- Câu 3.3: Lấy thông tin các hóa đơn đã được thanh toán bằng hình thức 'TIỀN MẶT'.
SELECT HD.MAHD, HD.NGAYLAP, KH.HOTEN AS KHACH_HANG, NV.HOTEN AS NHAN_VIEN_BAN, TT.SOTIEN, TT.HINHTHUC
FROM HOADON HD
JOIN THANHTOAN TT ON HD.MAHD = TT.MAHD
JOIN KHACHHANG KH ON HD.MAKH = KH.MAKH
JOIN NHANVIEN NV ON HD.MANV = NV.MANV
WHERE TT.HINHTHUC = N'TIỀN MẶT';
-- Câu 3.4: Liệt kê các Laptop có thời hạn bảo hành (BAOHANH) dài hơn thời hạn bảo hành trung bình của tất cả Laptop.
SELECT LT.TENLAPTOP, BH.THOIHAN_THANG
FROM BAOHANH BH
JOIN LAPTOP LT ON BH.MALAPTOP = LT.MALAPTOP
WHERE BH.THOIHAN_THANG > (SELECT AVG(THOIHAN_THANG) FROM BAOHANH);



