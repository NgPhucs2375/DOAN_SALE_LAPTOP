CREATE DATABASE QL_SALE_LAPTOP

USE QL_SALE_LAPTOP



create table PHANQUYEN
(
    MAQUYEN int IDENTITY(1,1) primary key,
    TENQUYEN nvarchar(50) NOT NULL,
    MOTA nvarchar(200)
)
GO

CREATE TABLE KHACHHANG
(
    MAKH int identity(1,1) PRIMARY KEY,
    HOTEN NVARCHAR(100) NOT NULL,
    SODT VARCHAR(15) UNIQUE, 
    EMAIL NVARCHAR(100) UNIQUE,
    DIACHI NVARCHAR(200),
    MATKHAU VARCHAR(255),
    NGAYTAO DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE NHANVIEN
(
    MANV int identity(1,1) PRIMARY KEY,
    HOTEN NVARCHAR(100) NOT NULL,
    CHUCVU NVARCHAR(50),
    EMAIL NVARCHAR(100) UNIQUE,
    TENDN NVARCHAR(50) UNIQUE NOT NULL,
    MATKHAU VARCHAR(255) NOT NULL,
    TRANGTHAI BIT DEFAULT 1
);
GO

create table CAPQUYEN
(
    MANV int not null,
    MAQUYEN int not null,
    primary key(MANV, MAQUYEN),
    foreign key (MANV) references NHANVIEN(MANV),
    foreign key (MAQUYEN) references PHANQUYEN(MAQUYEN)
)
GO

CREATE TABLE LOAI_LAPTOP
(
    MALOAI int identity(1,1) PRIMARY KEY,
    TENLOAI NVARCHAR(100) UNIQUE NOT NULL
);
GO

CREATE TABLE HANG
(
    MAHANG int identity(1,1) PRIMARY KEY,
    TENHANG NVARCHAR(100) NOT NULL,
    DIACHI NVARCHAR(200),
    SODT VARCHAR(15)
);
GO

---------------------------------------------------------
-- 2. SẢN PHẨM & KHO
---------------------------------------------------------
CREATE TABLE LAPTOP
(
    MALAPTOP int identity(1,1) PRIMARY KEY,
    TENLAPTOP NVARCHAR(200) NOT NULL,
	MOTA NVARCHAR(MAX),
	CAUHINH NVARCHAR(200),
	GIA_GOC DECIMAL(15,2) NOT NULL CHECK(GIA_GOC >= 0),
    GIA_BAN DECIMAL(15,2) NOT NULL CHECK(GIA_BAN >= 0),
    SOLUONG_TON INT DEFAULT 0 CHECK(SOLUONG_TON >= 0),
    HINHANH0 NVARCHAR(255),
    HINHANH1 NVARCHAR(255),
    HINHANH2 NVARCHAR(255),
    HINHANH3 NVARCHAR(255),
    MALOAI int NOT NULL FOREIGN KEY REFERENCES LOAI_LAPTOP(MALOAI),
    MAHANG int NOT NULL FOREIGN KEY REFERENCES HANG(MAHANG),
    TRANGTHAI BIT DEFAULT 1
);
GO



CREATE TABLE VOUCHER
(
    MAVOUCHER VARCHAR(20) PRIMARY KEY,
    TEN_VOUCHER NVARCHAR(100),
    LOAI_GIAMGIA VARCHAR(10) CHECK(LOAI_GIAMGIA IN ('PHANTRAM', 'TIENMAT')),
    GIATRI DECIMAL(15,2) NOT NULL,
    DONHANG_TOITHIEU DECIMAL(15,2) DEFAULT 0,
    GIAM_TOIDA DECIMAL(15,2) NULL,
    NGAYBATDAU DATETIME NOT NULL,
    NGAYKETTHUC DATETIME NOT NULL,
    SOLUONG_DUNG INT DEFAULT 100,
    DA_DUNG INT DEFAULT 0
);
GO


CREATE TABLE KHACHHANG_VOUCHER
(
    MAKH int NOT NULL FOREIGN KEY REFERENCES KHACHHANG(MAKH),
    MAVOUCHER VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES VOUCHER(MAVOUCHER),
    NGAYDUNG DATETIME DEFAULT GETDATE(),
    MAHD int, -- Lưu lại dùng cho hóa đơn nào (tham chiếu sau khi tạo bảng HOADON)
    PRIMARY KEY (MAKH, MAVOUCHER) -- Khóa chính kép quan trọng
);
GO

---------------------------------------------------------
-- 4. HÓA ĐƠN & THANH TOÁN
---------------------------------------------------------
CREATE TABLE HOADON
(
    MAHD int identity(1,1) PRIMARY KEY,
    NGAYLAP DATETIME DEFAULT GETDATE(),
    MAKH int FOREIGN KEY REFERENCES KHACHHANG(MAKH),
    MANV int FOREIGN KEY REFERENCES NHANVIEN(MANV),
    TONGTIEN_HANG DECIMAL(15,2) DEFAULT 0,
    MAVOUCHER VARCHAR(20) FOREIGN KEY REFERENCES VOUCHER(MAVOUCHER),
    SOTIEN_GIAM_VOUCHER DECIMAL(15,2) DEFAULT 0,
    TONG_THANHTOAN DECIMAL(15,2) DEFAULT 0,
    TRANGTHAI NVARCHAR(50) DEFAULT N'Chờ xử lý',
    DIACHI_GIAO NVARCHAR(200),
    SDT_GIAO VARCHAR(15)
);
GO

-- Cập nhật lại tham chiếu MAHD trong bảng KHACHHANG_VOUCHER (nếu cần thiết để tra cứu ngược)
ALTER TABLE KHACHHANG_VOUCHER
ADD CONSTRAINT FK_KHV_HOADON FOREIGN KEY (MAHD) REFERENCES HOADON(MAHD);
GO

CREATE TABLE CT_HOADON
(
MAHD int NOT NULL FOREIGN KEY REFERENCES HOADON(MAHD),
MALAPTOP int NOT NULL FOREIGN KEY REFERENCES LAPTOP(MALAPTOP),
SOLUONG INT CHECK(SOLUONG>0),
DONGIA DECIMAL(15,2) NOT NULL, -- Lưu giá tại thời điểm mua (GIA_BAN)
THANHTIEN DECIMAL(15,2),       -- = SOLUONG * DONGIA
PRIMARY KEY (MAHD, MALAPTOP)
);
GO

CREATE TABLE THANHTOAN
(
    MAHD int PRIMARY KEY FOREIGN KEY REFERENCES HOADON(MAHD),
    HINHTHUC NVARCHAR(50) NOT NULL,
    NGAYTHANHTOAN DATETIME DEFAULT GETDATE(),
    SOTIEN DECIMAL(15,2),
    TRANGTHAI NVARCHAR(50) DEFAULT N'Chưa thanh toán'
);
GO


-- 1.1. PHANQUYEN
INSERT INTO PHANQUYEN (TENQUYEN, MOTA) VALUES 
(N'Admin', N'Quản trị viên hệ thống, toàn quyền'),
(N'Nhân viên bán hàng', N'Xử lý đơn hàng, chăm sóc khách hàng'),
(N'Quản lý kho', N'Nhập xuất kho, quản lý sản phẩm');
GO

-- 1.2. KHACHHANG
INSERT INTO KHACHHANG (HOTEN, SODT, EMAIL, DIACHI, MATKHAU) VALUES 
(N'Nguyễn Văn A', '0901234567', 'nguyenvana@email.com', N'123 Đường Lê Lợi, Q1, TP.HCM', 'password_hash_1'),
(N'Trần Thị B', '0912345678', 'tranthib@email.com', N'456 Đường Nguyễn Huệ, Q1, TP.HCM', 'password_hash_2'),
(N'Lê Văn C', '0987654321', 'levanc@email.com', N'789 Đường Trần Hưng Đạo, Q5, TP.HCM', 'password_hash_3');
GO

-- 1.3. NHANVIEN
INSERT INTO NHANVIEN (HOTEN, CHUCVU, EMAIL, TENDN, MATKHAU) VALUES 
(N'Admin System', N'Quản trị', 'admin@laptopstore.com', 'admin', 'admin123'), 
(N'Phạm Nhân Viên 1', N'Bán hàng', 'nv1@laptopstore.com', 'nvbanhang1', 'nv123'),
(N'Lý Thủ Kho', N'Thủ kho', 'kho@laptopstore.com', 'nvkho1', 'kho123');
GO

-- 1.4. CAPQUYEN (Admin=1, BH=2, Kho=3)
INSERT INTO CAPQUYEN (MANV, MAQUYEN) VALUES 
(1, 1), -- Admin System có quyền Admin
(2, 2), -- Phạm Nhân Viên 1 có quyền Bán hàng
(3, 3); -- Lý Thủ Kho có quyền Quản lý kho
GO

-- 1.5. LOAI_LAPTOP
INSERT INTO LOAI_LAPTOP (TENLOAI) VALUES 
(N'Laptop Gaming'),
(N'Laptop Văn phòng'),
(N'Laptop Mỏng nhẹ (Ultrabook)'),
(N'Laptop Đồ họa - Kỹ thuật'),
(N'Macbook');
GO

-- 1.6. HANG (Tạo 5 hãng ví dụ)
INSERT INTO HANG (TENHANG, DIACHI, SODT) VALUES 
(N'Dell', N'USA', '1800-555-001'),    -- MAHANG = 1
(N'Asus', N'Taiwan', '1800-555-002'), -- MAHANG = 2
(N'HP', N'USA', '1800-555-003'),      -- MAHANG = 3
(N'Lenovo', N'China', '1800-555-004'),-- MAHANG = 4
(N'Apple', N'USA', '1800-555-005'),   -- MAHANG = 5
(N'MSI', N'Taiwan', '1800-555-006');  -- MAHANG = 6
GO

-- 1.7. VOUCHER
INSERT INTO VOUCHER (MAVOUCHER, TEN_VOUCHER, LOAI_GIAMGIA, GIATRI, DONHANG_TOITHIEU, GIAM_TOIDA, NGAYBATDAU, NGAYKETTHUC, SOLUONG_DUNG) VALUES 
('SALE50K', N'Giảm 50k cho đơn từ 5 triệu', 'TIENMAT', 50000, 5000000, 50000, '2023-01-01', '2025-12-31', 1000),
('BLACKFRIDAY', N'Siêu sale 10%', 'PHANTRAM', 10, 10000000, 2000000, '2024-11-01', '2024-11-30', 500),
('NEWMEMBER', N'Chào bạn mới', 'TIENMAT', 100000, 0, 100000, '2023-01-01', '2030-12-31', 9999);
GO

-- === DELL (MAHANG = 1) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Dell Inspirion N3530 i5U165W11SLU', N'Laptop doanh nhân mỏng nhẹ cao cấp', N'Core i5 Raptor Lake - 1334U, 16GB RAM, 512GB SSD, 14" FHD+', 18990000, 15990000, 10, 3, 1, 'Dell_Inspirion_N3530_i5U165W11SLU_0.png','Dell_Inspirion_N3530_i5U165W11SLU_1.png','Dell_Inspirion_N3530_i5U165W11SLU_2.png','Dell_Inspirion_N3530_i5U165W11SLU_3.png'),
(N'Dell Gaming G15 5515', N'Laptop gaming hiệu năng cao giá tốt', N'Ryzen 7 5800H, 8GB RAM, 512GB SSD, RTX 3050, 15.6" 120Hz', 22000000, 21100000, 15, 1, 1, 'Dell_Gaming_G15_5515_0.png','Dell_Gaming_G15_5515_1.jpg','Dell_Gaming_G15_5515_2.jpg','Dell_Gaming_G15_5515_3.jpg'),
(N'Dell XPS 9350 XPS9350-U5IA165W11GR-FP', N'Laptop văn phòng cơ bản bền bỉ', N'Intel® Core™ Ultra 5 226V, 16GB RAM, 256GB SSD, 13.4" QHD+', 54990000, 50990000, 20, 2, 1, 'Dell_XPS_9350_0.png','Dell_XPS_9350_1.png','Dell_XPS_9350_2.png','Dell_XPS_9350_3.png');

-- === ASUS (MAHANG = 2) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Asus ROG Zephyrus G14', N'Chiến thần gaming mỏng nhẹ', N'Ryzen AI 9 HX, 16GB RAM, 512GB SSD, RTX 5060, 15.6" 144Hz', 52000000, 49900000, 8, 1, 2, 'Asus_ROG_Zephyrus_G14_0.png','Asus_ROG_Zephyrus_G14_1.png','Asus_ROG_Zephyrus_G14_2.png','Asus_ROG_Zephyrus_G14_3.png'),
(N'ASUS TUF Gaming F16', N'Chiến thần gaming, cấu hình khủng, giá rẻ', N'i5 12400H, 8GB RAM, 512GB SSD, RTX 3050, 14" FHD IPS', 29000000, 20500000, 12, 3, 2, 'ASUS_TUF_Gaming_F16_0.png', 'ASUS_TUF_Gaming_F16_1.png', 'ASUS_TUF_Gaming_F16_2.png', 'ASUS_TUF_Gaming_F16_3.png'),
(N'Asus VivoBook S14', N'Màn hình OLED rực rỡ cho văn phòng', N'Ryzen 5 5600H, 8GB RAM, 512GB SSD, 15.6" OLED', 17900000, 16900000, 25, 2, 2, 'ASUS_Vivobook_S14_0.png', 'ASUS_Vivobook_S14_1.png', 'ASUS_Vivobook_S14_2.png', 'ASUS_Vivobook_S14_3.png');

-- === HP (MAHANG = 3) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'HP Omen 16', N'Laptop gaming cao cấp tản nhiệt tốt', N'i7 14700H, 16GB RAM, 1TB SSD, RTX 5060Ti, 16.1" QHD 165Hz', 48000000, 45000000, 5, 1, 3, 'HP_OMEN_16_0.png', 'HP_OMEN_16_1.png', 'HP_OMEN_16_2.png', 'HP_OMEN_16_3.png'),
(N'HP VICTUS 15', N'Laptop gaming, văn phòng', N'AMD Ryzen™ 7 7445H, 8GB RAM, 512GB SSD,RTX 4050, 15.6" FHD', 31500000, 29900000, 30, 4, 3, 'HP_VICTUS_15_0.png', 'HP_VICTUS_15_1.png', 'HP_VICTUS_15_0.png', 'HP_VICTUS_15_0.png'),
(N'HP Envy x360 13', N'Xoay gập 360 độ cảm ứng', N'Ryzen 7 5800U, 8GB RAM, 512GB SSD, 13.3" FHD Touch', 22500000, 21000000, 10, 3, 3, 'HP_ENVY_13_0.png', 'HP_ENVY_13_1.png', 'HP_ENVY_13_2.png', 'HP_ENVY_13_3.png');

-- === LENOVO (MAHANG = 4) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Lenovo Legion 5', N'Laptop gaming quốc dân', N'i7 14700HX, 16GB RAM, 512GB SSD, RTX 5060, 15.6" 165Hz', 39000000, 38990000, 18, 1, 4, 'Lenovo_Legion_5_0.png', 'Lenovo_Legion_5_1.png', 'Lenovo_Legion_5_2.png', 'Lenovo_Legion_5_3.png'),
(N'Lenovo ThinkPad X1 Carbon Gen 9', N'Doanh nhân đẳng cấp, siêu bền', N'i7 1185G7, 16GB RAM, 1TB SSD, 14" UHD+', 55000000, 50000000, 3, 3, 4, 'Lenovo_ThinkPad_X1_0.png', 'Lenovo_ThinkPad_X1_1.png', 'Lenovo_ThinkPad_X1_2.png', 'Lenovo_ThinkPad_X1_3.png'),
(N'Lenovo IdeaPad 3', N'Giá rẻ cho sinh viên', N'i3 1115G4, 8GB RAM, 256GB SSD, 14" FHD', 15000000, 11500000, 50, 2, 4, 'Lenovo_IdeaPad_3_0.png', 'Lenovo_IdeaPad_3_1.png', 'Lenovo_IdeaPad_3_2.png', 'Lenovo_IdeaPad_3_3.png');

-- === APPLE (MAHANG = 5) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'MacBook Pro 14 M2', N'Dành cho dân đồ họa chuyên nghiệp', N'Apple M2 Pro, 18GB RAM, 512GB SSD, 14.2" Liquid Retina XDR', 51990000, 49990000, 7, 4, 5, 'Macbook_Pro_14_M2_0.png', 'Macbook_Pro_14_M2_1.png', 'Macbook_Pro_14_M2_2.png', 'Macbook_Pro_14_M2_3.png'),
(N'MacBook Air M2 2022', N'Thiết kế mới thời thượng', N'Apple M2, 16GB RAM, 512GB SSD, 13.6" Liquid Retina', 32000000, 29900000, 20, 5, 5, 'Macbook_Air_M2_0.jpg', 'Macbook_Air_M2_1.jpg', 'Macbook_Air_M2_2.jpg', 'Macbook_Air_M2_3.jpg');

-- === MSI (MAHANG = 6) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'MSI Cyborg 15', N'Gaming mỏng nhẹ giá rẻ', N'i5 11400H, 8GB RAM, 512GB SSD, GTX 3050, 15.6" 144Hz', 19000000, 17500000, 25, 1, 6, 'MSI_Cyborg_15_0.png', 'MSI_Cyborg_15_1.png', 'MSI_Cyborg_15_2.png', 'MSI_Cyborg_15_3.png'),
(N'MSI Modern 14', N'Văn phòng thời trang, nhỏ gọn', N'Ryzen 5 5500U, 8GB RAM, 512GB SSD, 14" FHD', 17000000, 14500000, 30, 2, 6,  'MSI_Modern_14_0.png', 'MSI_Modern_14_1.png', 'MSI_Modern_14_2.png', 'MSI_Modern_14_3.png'),
(N'MSI Prestige 16 AI+ Evo', N'Laptop cho nhà sáng tạo nội dung', N'Ultra 9, 32GB RAM, 1TB SSD, 16" QHD+ 120Hz Touch', 59000000, 55000000, 2, 4, 6, 'MSI_Prestige_16_0.png', 'MSI_Prestige_16_1.png', 'MSI_Prestige_16_2.png', 'MSI_Prestige_16_3.png');
GO


INSERT INTO KHACHHANG_VOUCHER (MAKH, MAVOUCHER, NGAYDUNG, MAHD) VALUES
(1, 'NEWMEMBER', NULL, NULL), -- KH 1 đã lưu nhưng chưa dùng
(2, 'BLACKFRIDAY', '2024-11-15', NULL); -- KH 2 đã dùng (sẽ cập nhật MAHD sau nếu cần)
GO



-- === THÊM DỮ LIỆU CHO DELL (MAHANG = 1) ===

-- Gaming (MALOAI = 1)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Dell Alienware m16 R2', N'Siêu laptop gaming thế hệ mới, tản nhiệt buồng hơi', N'i9-14900HX, 32GB RAM, 1TB SSD, RTX 4070, 16" QHD+ 240Hz', 85000000, 82990000, 10, 1, 1, 'placeholder_alienware_m16_0.png', 'placeholder_alienware_m16_1.png', 'placeholder_alienware_m16_2.png', 'placeholder_alienware_m16_3.png'),
(N'Dell G16 7630', N'Laptop gaming tầm trung, màn hình 16 inch', N'i7-13650HX, 16GB RAM, 1TB SSD, RTX 4060, 16" FHD+ 165Hz', 35000000, 32490000, 15, 1, 1, 'placeholder_dell_g16_0.png', 'placeholder_dell_g16_1.png', 'placeholder_dell_g16_2.png', 'placeholder_dell_g16_3.png');

-- Mỏng nhẹ (MALOAI = 3)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Dell XPS 13 9340', N'Thiết kế nhôm nguyên khối, viền siêu mỏng', N'Core Ultra 7 155H, 32GB RAM, 1TB SSD, 13.4" OLED 3K', 48000000, 46990000, 8, 3, 1, 'placeholder_dell_xps13_0.png', 'placeholder_dell_xps13_1.png', 'placeholder_dell_xps13_2.png', 'placeholder_dell_xps13_3.png');

-- Đồ họa (MALOAI = 4)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Dell Precision 5690', N'Máy trạm di động cho đồ họa chuyên nghiệp', N'Core Ultra 9 185H, 32GB RAM, 1TB SSD, NVIDIA RTX 5000 Ada, 16" OLED', 110000000, 105000000, 5, 4, 1, 'placeholder_dell_precision_0.png', 'placeholder_dell_precision_1.png', 'placeholder_dell_precision_2.png', 'placeholder_dell_precision_3.png');

-- Văn phòng (MALOAI = 2)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Dell Vostro 3530', N'Bền bỉ cho văn phòng, bảo mật vân tay', N'i5-1335U, 16GB RAM, 512GB SSD, 15.6" FHD 120Hz', 19500000, 17990000, 30, 2, 1, 'placeholder_dell_vostro_0.png', 'placeholder_dell_vostro_1.png', 'placeholder_dell_vostro_2.png', 'placeholder_dell_vostro_3.png');

-- === THÊM DỮ LIỆU CHO ASUS (MAHANG = 2) ===

-- Gaming (MALOAI = 1)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Asus ROG Strix SCAR 18 (2024)', N'Trùm cuối gaming, màn hình Mini LED', N'i9-14900HX, 64GB RAM, 2TB SSD, RTX 4090, 18" QHD+ 240Hz', 120000000, 115000000, 3, 1, 2, 'placeholder_asus_scar18_0.png', 'placeholder_asus_scar18_1.png', 'placeholder_asus_scar18_2.png', 'placeholder_asus_scar18_3.png'),
(N'Asus TUF Gaming A15 (2024)', N'Bền bỉ chuẩn quân đội, tản nhiệt tốt', N'Ryzen 7 7735HS, 16GB RAM, 512GB SSD, RTX 4050, 15.6" 144Hz', 26000000, 24990000, 20, 1, 2, 'placeholder_asus_tuf_a15_0.png', 'placeholder_asus_tuf_a15_1.png', 'placeholder_asus_tuf_a15_2.png', 'placeholder_asus_tuf_a15_3.png');

-- Mỏng nhẹ (MALOAI = 3)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Asus Zenbook 14 OLED UX3405', N'Siêu mỏng nhẹ, màn hình 3K 120Hz', N'Core Ultra 7 155H, 16GB RAM, 1TB SSD, 14" 3K OLED', 31000000, 29990000, 15, 3, 2, 'placeholder_asus_zenbook14_0.png', 'placeholder_asus_zenbook14_1.png', 'placeholder_asus_zenbook14_2.png', 'placeholder_asus_zenbook14_3.png');

-- Văn phòng (MALOAI = 2)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Asus ExpertBook B5', N'Dành cho doanh nhân, nhẹ 1.2kg', N'i5-1340P, 16GB RAM, 512GB SSD, 14" FHD', 23000000, 21500000, 10, 2, 2, 'placeholder_asus_expertbook_0.png', 'placeholder_asus_expertbook_1.png', 'placeholder_asus_expertbook_2.png', 'placeholder_asus_expertbook_3.png');

-- === THÊM DỮ LIỆU CHO HP (MAHANG = 3) ===

-- Gaming (MALOAI = 1)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'HP Omen Transcend 14', N'Laptop gaming mỏng nhẹ, màn OLED 2.8K', N'Core Ultra 9 185H, 32GB RAM, 1TB SSD, RTX 4070, 14" OLED 2.8K', 65000000, 62990000, 7, 1, 3, 'placeholder_hp_omen14_0.png', 'placeholder_hp_omen14_1.png', 'placeholder_hp_omen14_2.png', 'placeholder_hp_omen14_3.png');

-- Văn phòng (MALOAI = 2)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'HP Pavilion 15', N'Thiết kế thời trang, vỏ nhôm', N'Ryzen 5 7530U, 16GB RAM, 512GB SSD, 15.6" FHD', 18000000, 16500000, 25, 2, 3, 'placeholder_hp_pavilion15_0.png', 'placeholder_hp_pavilion15_1.png', 'placeholder_hp_pavilion15_2.png', 'placeholder_hp_pavilion15_3.png');

-- Đồ họa (MALOAI = 4)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'HP ZBook Firefly 16 G11', N'Máy trạm mỏng nhẹ cho AI', N'Core Ultra 7 165H, 32GB RAM, 1TB SSD, NVIDIA RTX A500, 16" WUXGA', 58000000, 55000000, 6, 4, 3, 'placeholder_hp_zbook_0.png', 'placeholder_hp_zbook_1.png', 'placeholder_hp_zbook_2.png', 'placeholder_hp_zbook_3.png');

-- === THÊM DỮ LIỆU CHO LENOVO (MAHANG = 4) ===

-- Gaming (MALOAI = 1)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Lenovo Legion Pro 7i Gen 9', N'Tản nhiệt AI, hiệu năng khủng', N'i9-14900HX, 32GB RAM, 2TB SSD, RTX 4080, 16" QHD+ 240Hz', 95000000, 92000000, 4, 1, 4, 'placeholder_lenovo_legion7i_0.png', 'placeholder_lenovo_legion7i_1.png', 'placeholder_lenovo_legion7i_2.png', 'placeholder_lenovo_legion7i_3.png');

-- Văn phòng (MALOAI = 2)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Lenovo ThinkBook 14 Gen 6', N'Bảo mật ThinkShield, build bền bỉ', N'Ryzen 5 7530U, 16GB RAM, 512GB SSD, 14" WUXGA', 20000000, 18500000, 30, 2, 4, 'placeholder_lenovo_thinkbook14_0.png', 'placeholder_lenovo_thinkbook14_1.png', 'placeholder_lenovo_thinkbook14_2.png', 'placeholder_lenovo_thinkbook14_3.png');

-- Mỏng nhẹ (MALOAI = 3)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Lenovo Yoga 9i Gen 9', N'Xoay gập 360, kèm bút cảm ứng', N'Core Ultra 7 155H, 16GB RAM, 1TB SSD, 14" 4K OLED', 45000000, 43990000, 9, 3, 4, 'placeholder_lenovo_yoga9i_0.png', 'placeholder_lenovo_yoga9i_1.png', 'placeholder_lenovo_yoga9i_2.png', 'placeholder_lenovo_yoga9i_3.png');

-- Đồ họa (MALOAI = 4)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'Lenovo ThinkPad P1 Gen 7', N'Máy trạm di động cao cấp nhất', N'Core Ultra 9 185H, 64GB RAM, 2TB SSD, RTX 3000 Ada, 16" 4K OLED', 130000000, 125000000, 2, 4, 4, 'placeholder_lenovo_p1g7_0.png', 'placeholder_lenovo_p1g7_1.png', 'placeholder_lenovo_p1g7_2.png', 'placeholder_lenovo_p1g7_3.png');

-- === THÊM DỮ LIỆU CHO APPLE (MAHANG = 5) ===

-- Macbook (MALOAI = 5)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'MacBook Pro 16 M3 Max', N'Sức mạnh tuyệt đối cho dân chuyên nghiệp', N'Apple M3 Max (16-core CPU, 40-core GPU), 64GB RAM, 2TB SSD, 16.2" Liquid Retina XDR', 95000000, 92990000, 5, 5, 5, 'placeholder_mac_pro16_0.png', 'placeholder_mac_pro16_1.png', 'placeholder_mac_pro16_2.png', 'placeholder_mac_pro16_3.png'),
(N'MacBook Pro 14 M3 Pro', N'Cân bằng hoàn hảo giữa hiệu năng và di động', N'Apple M3 Pro (12-core CPU, 18-core GPU), 36GB RAM, 1TB SSD, 14.2" Liquid Retina XDR', 70000000, 68500000, 8, 5, 5, 'placeholder_mac_pro14_0.png', 'placeholder_mac_pro14_1.png', 'placeholder_mac_pro14_2.png', 'placeholder_mac_pro14_3.png'),
(N'MacBook Air 15 M3', N'Màn hình lớn, siêu mỏng nhẹ', N'Apple M3 (8-core CPU, 10-core GPU), 16GB RAM, 512GB SSD, 15.3" Liquid Retina', 42000000, 40990000, 20, 5, 5, 'placeholder_mac_air15_0.png', 'placeholder_mac_air15_1.png', 'placeholder_mac_air15_2.png', 'placeholder_mac_air15_3.png'),
(N'MacBook Air 13 M3', N'Laptop di động hoàn hảo, thời lượng pin tốt nhất', N'Apple M3 (8-core CPU, 8-core GPU), 8GB RAM, 256GB SSD, 13.6" Liquid Retina', 30000000, 27990000, 30, 5, 5, 'placeholder_mac_air13_0.png', 'placeholder_mac_air13_1.png', 'placeholder_mac_air13_2.png', 'placeholder_mac_air13_3.png');

-- === THÊM DỮ LIỆU CHO MSI (MAHANG = 6) ===

-- Gaming (MALOAI = 1)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'MSI Titan 18 HX', N'Thay thế máy bàn, trang bị khủng nhất', N'i9-14900HX, 128GB RAM, 4TB SSD, RTX 4090, 18" 4K 120Hz Mini LED', 150000000, 149000000, 2, 1, 6, 'placeholder_msi_titan18_0.png', 'placeholder_msi_titan18_1.png', 'placeholder_msi_titan18_2.png', 'placeholder_msi_titan18_3.png'),
(N'MSI Katana 15 B13V', N'Thiết kế gaming mới, bàn phím RGB', N'i7-13620H, 16GB RAM, 1TB SSD, RTX 4060, 15.6" 144Hz', 32000000, 29990000, 22, 1, 6, 'placeholder_msi_katana15_0.png', 'placeholder_msi_katana15_1.png', 'placeholder_msi_katana15_2.png', 'placeholder_msi_katana15_3.png');

-- Mỏng nhẹ (MALOAI = 3)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'MSI Stealth 14 AI Studio', N'Mỏng nhẹ tích hợp AI, vỏ Magie', N'Core Ultra 7 155H, 32GB RAM, 1TB SSD, RTX 4050, 14" QHD+', 43000000, 40990000, 10, 3, 6, 'placeholder_msi_stealth14_0.png', 'placeholder_msi_stealth14_1.png', 'placeholder_msi_stealth14_2.png', 'placeholder_msi_stealth14_3.png');

-- Đồ họa (MALOAI = 4)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'MSI Creator Z17 HX Studio', N'Màn hình cảm ứng tỷ lệ vàng 16:10', N'i9-13980HX, 64GB RAM, 2TB SSD, RTX 4070, 17" QHD+ 165Hz Touch', 85000000, 82000000, 3, 4, 6, 'placeholder_msi_creator_z17_0.png', 'placeholder_msi_creator_z17_1.png', 'placeholder_msi_creator_z17_2.png', 'placeholder_msi_creator_z17_3.png');

-- Văn phòng (MALOAI = 2)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'MSI Modern 15 H', N'Laptop văn phòng hiệu năng cao', N'Core i7-1355U, 16GB RAM, 512GB SSD, 15.6" FHD', 21000000, 19990000, 20, 2, 6, 'placeholder_msi_modern15_0.png', 'placeholder_msi_modern15_1.png', 'placeholder_msi_modern15_2.png', 'placeholder_msi_modern15_3.png');

-- Thêm 2 sản phẩm nữa cho đa dạng
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, HINHANH1, HINHANH2, HINHANH3) VALUES
(N'HP Envy 16 (2024)', N'Laptop 2-in-1 cao cấp, màn hình OLED', N'Core Ultra 7 155H, 16GB RAM, 1TB SSD, 16" 2.8K OLED Touch', 40000000, 38500000, 12, 3, 3, 'placeholder_hp_envy16_0.png', 'placeholder_hp_envy16_1.png', 'placeholder_hp_envy16_2.png', 'placeholder_hp_envy16_3.png'),
(N'Acer Predator Helios 18', N'Tản nhiệt 3 quạt, màn hình Mini LED', N'i9-14900HX, 32GB RAM, 2TB SSD, RTX 4080, 18" QHD+ 240Hz', 75000000, 72990000, 7, 1, 2, 'placeholder_acer_helios18_0.png', 'placeholder_acer_helios18_1.png', 'placeholder_acer_helios18_2.png', 'placeholder_acer_helios18_3.png');

GO


UPDATE LAPTOP
SET 
    HINHANH0 = 'MSI_Prestige_16_0.png',
    HINHANH1 = 'MSI_Prestige_16_1.png',
    HINHANH2 = 'MSI_Prestige_16_2.png',
	HINHANH3 = 'MSI_Prestige_16_3.png'

WHERE 
    TENLAPTOP = 'Dell Alienware m16 R2';


	UPDATE LAPTOP
SET 
    HINHANH0 = 'Dell_G16_7jpg_0.jpg',
    HINHANH1 = 'Dell_G16_7jpg_1.jpg',
    HINHANH2 = 'Dell_G16_7jpg_2.jpg',
	HINHANH3 = 'Dell_G16_7jpg_3.jpg'

WHERE 
    TENLAPTOP = 'Dell G16 7630';

		UPDATE LAPTOP
SET 
    HINHANH0 = 'Dell_XPS_13_9340_0.jpg',
    HINHANH1 = 'Dell_XPS_13_9340_1.jpg',
    HINHANH2 = 'Dell_XPS_13_9340_2.jpg',
	HINHANH3 = 'Dell_XPS_13_9340_3.jpg'

WHERE 
    TENLAPTOP = 'Dell XPS 13 9340';

			UPDATE LAPTOP
SET 
    HINHANH0 = 'Dell_Precision_5690_0.jpg',
    HINHANH1 = 'Dell_Precision_5690_1.jpg',
    HINHANH2 = 'Dell_Precision_5690_2.jpg',
	HINHANH3 = 'Dell_Precision_5690_3.jpg'

WHERE 
    TENLAPTOP = 'Dell Precision 5690';

			UPDATE LAPTOP
SET 
    HINHANH0 = 'Dell_Vostro_3530_0.jpg',
    HINHANH1 = 'Dell_Vostro_3530_1.jpg',
    HINHANH2 = 'Dell_Vostro_3530_2.jpg',
	HINHANH3 = 'Dell_Vostro_3530_3.jpg'

WHERE 
    TENLAPTOP = 'Dell Vostro 3530';

			UPDATE LAPTOP
SET 
    HINHANH0 = 'Asus_ROG_Strix_SCAR_18_(2024)_0.jpg',
    HINHANH1 = 'Asus_ROG_Strix_SCAR_18_(2024)_1.jpg',
    HINHANH2 = 'Asus_ROG_Strix_SCAR_18_(2024)_2.jpg',
	HINHANH3 = 'Asus_ROG_Strix_SCAR_18_(2024)_3.jpg'

WHERE 
    TENLAPTOP = 'Asus ROG Strix SCAR 18 (2024)';


			UPDATE LAPTOP
SET 
    HINHANH0 = 'Asus_TUF_Gaming_A15_(2024)_0.jpg',
    HINHANH1 = 'Asus_TUF_Gaming_A15_(2024)_1.jpg',
    HINHANH2 = 'Asus_TUF_Gaming_A15_(2024)_2.jpg',
	HINHANH3 = 'Asus_TUF_Gaming_A15_(2024)_3.jpg'

WHERE 
    TENLAPTOP = 'Asus TUF Gaming A15 (2024)';


			UPDATE LAPTOP
SET 
    HINHANH0 = 'Asus_Zenbook_14_OLED_UX3405_0.jpg',
    HINHANH1 = 'Asus_Zenbook_14_OLED_UX3405_1.jpg',
    HINHANH2 = 'Asus_Zenbook_14_OLED_UX3405_2.jpg',
	HINHANH3 = 'Asus_Zenbook_14_OLED_UX3405_3.jpg'

WHERE 
    TENLAPTOP = 'Asus Zenbook 14 OLED UX3405';


			UPDATE LAPTOP
SET 
    HINHANH0 = 'Asus_ExpertBook_B5_0.jpg',
    HINHANH1 = 'Asus_ExpertBook_B5_1.jpg',
    HINHANH2 = 'Asus_ExpertBook_B5_2.jpg',
	HINHANH3 = 'Asus_ExpertBook_B5_3.jpg'

WHERE 
    TENLAPTOP = 'Asus ExpertBook B5';


			UPDATE LAPTOP
SET 
    HINHANH0 = 'HP_Omen_Transcend_14_0.jpg',
    HINHANH1 = 'HP_Omen_Transcend_14_1.jpg',
    HINHANH2 = 'HP_Omen_Transcend_14_2.jpg',
	HINHANH3 = 'HP_Omen_Transcend_14_3.jpg'

WHERE 
    TENLAPTOP = 'HP Omen Transcend 14';

				UPDATE LAPTOP
SET 
    HINHANH0 = 'HP_Pavilion_15_0.jpg',
    HINHANH1 = 'HP_Pavilion_15_1.jpg',
    HINHANH2 = 'HP_Pavilion_15_2.jpg',
	HINHANH3 = 'HP_Pavilion_15_3.jpg'

WHERE 
    TENLAPTOP = 'HP Pavilion 15';

				UPDATE LAPTOP
SET 
    HINHANH0 = 'HP_ZBook_Firefly_16_G11_0.jpg',
    HINHANH1 = 'HP_ZBook_Firefly_16_G11_1.jpg',
    HINHANH2 = 'HP_ZBook_Firefly_16_G11_2.jpg',
	HINHANH3 = 'HP_ZBook_Firefly_16_G11_3.jpg'

WHERE 
    TENLAPTOP = 'HP ZBook Firefly 16 G11';

				UPDATE LAPTOP
SET 
    HINHANH0 = 'Lenovo_Legion_Pro_7i_Gen_9_0.jpg',
    HINHANH1 = 'Lenovo_Legion_Pro_7i_Gen_9_1.jpg',
    HINHANH2 = 'Lenovo_Legion_Pro_7i_Gen_9_2.jpg',
	HINHANH3 = 'Lenovo_Legion_Pro_7i_Gen_9_4.jpg'

WHERE 
    TENLAPTOP = 'Lenovo Legion Pro 7i Gen 9';

				UPDATE LAPTOP
SET 
    HINHANH0 = 'Lenovo_ThinkBook_14_Gen_6_0.jpg',
    HINHANH1 = 'Lenovo_ThinkBook_14_Gen_6_1.jpg',
    HINHANH2 = 'Lenovo_ThinkBook_14_Gen_6_2.jpg',
	HINHANH3 = 'Lenovo_ThinkBook_14_Gen_6_3.jpg'

WHERE 
    TENLAPTOP = 'Lenovo ThinkBook 14 Gen 6';


				UPDATE LAPTOP
SET 
    HINHANH0 = 'Lenovo_Yoga_9i_Gen_9_0.jpg',
    HINHANH1 = 'Lenovo_Yoga_9i_Gen_9_1.jpg',
    HINHANH2 = 'Lenovo_Yoga_9i_Gen_9_2.jpg',
	HINHANH3 = 'Lenovo_Yoga_9i_Gen_9_3.jpg'

WHERE 
    TENLAPTOP = 'Lenovo Yoga 9i Gen 9';


				UPDATE LAPTOP
SET 
    HINHANH0 = 'Lenovo_ThinkPad_P1_Gen_7_0.jpg',
    HINHANH1 = 'Lenovo_ThinkPad_P1_Gen_7_1.jpg',
    HINHANH2 = 'Lenovo_ThinkPad_P1_Gen_7_2.jpg',
	HINHANH3 = 'Lenovo_ThinkPad_P1_Gen_7_3.jpg'

WHERE 
    TENLAPTOP = 'Lenovo ThinkPad P1 Gen 7';


				UPDATE LAPTOP
SET 
    HINHANH0 = 'MacBook_Pro_16_M3_Max_0.jpg',
    HINHANH1 = 'MacBook_Pro_16_M3_Max_1.jpg',
    HINHANH2 = 'MacBook_Pro_16_M3_Max_2.jpg',
	HINHANH3 = 'MacBook_Pro_16_M3_Max_3.jpg'

WHERE 
    TENLAPTOP = 'MacBook Pro 16 M3 Max';

				UPDATE LAPTOP
SET 
    HINHANH0 = 'MacBook_Pro_14_M3_Pro_0.jpg',
    HINHANH1 = 'MacBook_Pro_14_M3_Pro_1.jpg',
    HINHANH2 = 'MacBook_Pro_14_M3_Pro_2.jpg',
	HINHANH3 = 'MacBook_Pro_14_M3_Pro_3.jpg'

WHERE 
    TENLAPTOP = 'MacBook Pro 14 M3 Pro';

				UPDATE LAPTOP
SET 
    HINHANH0 = 'MacBook_Air_15_M3_0.jpg',
    HINHANH1 = 'MacBook_Air_15_M3_1.jpg',
    HINHANH2 = 'MacBook_Air_15_M3_2.jpg',
	HINHANH3 = 'MacBook_Air_15_M3_3.jpg'

WHERE 
    TENLAPTOP = 'MacBook Air 15 M3';

					UPDATE LAPTOP
SET 
    HINHANH0 = 'MacBook_Pro_14_M3_Pro_0.jpg',
    HINHANH1 = 'MacBook_Pro_14_M3_Pro_1.jpg',
    HINHANH2 = 'MacBook_Pro_14_M3_Pro_2.jpg',
	HINHANH3 = 'MacBook_Pro_14_M3_Pro_3.jpg'

WHERE 
    TENLAPTOP = 'MacBook Air 13 M3';

					UPDATE LAPTOP
SET 
    HINHANH0 = 'MSI_Titan_18_HX_0.jpg',
    HINHANH1 = 'MSI_Titan_18_HX_1.jpg',
    HINHANH2 = 'MSI_Titan_18_HX_3.jpg',
	HINHANH3 = 'Acer_Predator_Helios_18_1.jpg'

WHERE 
    TENLAPTOP = 'MSI Titan 18 HX';

					UPDATE LAPTOP
SET 
    HINHANH0 = 'Lenovo_ThinkBook_14_Gen_6_0.jpg',
    HINHANH1 = 'Lenovo_ThinkBook_14_Gen_6_1.jpg',
    HINHANH2 = 'Lenovo_ThinkBook_14_Gen_6_2.jpg',
	HINHANH3 = 'Lenovo_ThinkBook_14_Gen_6_3.jpg'

WHERE 
    TENLAPTOP = 'MSI Katana 15 B13V';

					UPDATE LAPTOP
SET 
    HINHANH0 = 'MSI_Prestige_16_0.png',
    HINHANH1 = 'MSI_Prestige_16_1.png',
    HINHANH2 = 'MSI_Prestige_16_2.png',
	HINHANH3 = 'MSI_Prestige_16_3.png'

WHERE 
    TENLAPTOP = 'MSI Stealth 14 AI Studio';

					UPDATE LAPTOP
SET 
    HINHANH0 = 'Asus_ROG_Strix_SCAR_18_(2024)_0.jpg',
    HINHANH1 = 'Asus_ROG_Strix_SCAR_18_(2024)_1.jpg',
    HINHANH2 = 'Asus_ROG_Strix_SCAR_18_(2024)_2.jpg',
	HINHANH3 = 'Asus_ROG_Strix_SCAR_18_(2024)_3.jpg'

WHERE 
    TENLAPTOP = 'HP Envy 16 (2024)';

					UPDATE LAPTOP
SET 
    HINHANH0 = 'Lenovo_Yoga_9i_Gen_9_0.jpg',
    HINHANH1 = 'Lenovo_Yoga_9i_Gen_9_1.jpg',
    HINHANH2 = 'Lenovo_Yoga_9i_Gen_9_2.jpg',
	HINHANH3 = 'Lenovo_Yoga_9i_Gen_9_3.jpg'

WHERE 
    TENLAPTOP = 'MSI Modern 15 H';


					UPDATE LAPTOP
SET 
    HINHANH0 = 'MSI_Creator_Z17_HX_Studio_0.jpg',
    HINHANH1 = 'MSI_Creator_Z17_HX_Studio_1.jpg',
    HINHANH2 = 'MSI_Creator_Z17_HX_Studio_2.jpg',
	HINHANH3 = 'MSI_Creator_Z17_HX_Studio_3.jpg'

WHERE 
    TENLAPTOP = 'MSI Creator Z17 HX Studio';

					UPDATE LAPTOP
SET 
    HINHANH0 = 'Acer_Predator_Helios_18_0.jpg',
    HINHANH1 = 'Acer_Predator_Helios_18_1.jpg',
    HINHANH2 = 'Acer_Predator_Helios_18_2.jpg',
	HINHANH3 = 'Acer_Predator_Helios_18_3.jpg'

WHERE 
    TENLAPTOP = 'Acer Predator Helios 18';





------------------------------------------------------------------
-- BỔ SUNG: KHÁCH HÀNG & NHÂN VIÊN MẪU (nâng tổng KH lên ~15 người)
------------------------------------------------------------------
INSERT INTO KHACHHANG (HOTEN, SODT, EMAIL, DIACHI, MATKHAU)
VALUES
(N'Phan Quang Huy', '0902000001', 'huy.pq@example.com', N'12 Nguyễn Trãi, Q5, TP.HCM', 'hash_pw_01'),
(N'Ngô Thị Mai', '0902000002', 'mai.ngo@example.com', N'34 Cách Mạng, Q3, TP.HCM', 'hash_pw_02'),
(N'Bùi Văn Long', '0902000003', 'long.bv@example.com', N'56 Trần Phú, Q6, TP.HCM', 'hash_pw_03'),
(N'Hoàng Thị Lan', '0902000004', 'lan.ht@example.com', N'78 Lê Duẩn, Q1, TP.HCM', 'hash_pw_04'),
(N'Vũ Minh Khang', '0902000005', 'khang.vm@example.com', N'90 Phạm Ngũ Lão, Q1, TP.HCM', 'hash_pw_05'),
(N'Lê Thị Hạnh', '0902000006', 'hanh.lt@example.com', N'101 Bến Thành, Q1, TP.HCM', 'hash_pw_06'),
(N'Đinh Công Trí', '0902000007', 'tri.dc@example.com', N'202 Võ Văn Tần, Q3, TP.HCM', 'hash_pw_07'),
(N'Phạm Văn Nam', '0902000008', 'nam.pv@example.com', N'303 Nguyễn Văn Cừ, Q5, TP.HCM', 'hash_pw_08'),
(N'Nguyễn Thị Thanh', '0902000009', 'thanh.nt@example.com', N'404 Huỳnh Thúc Kháng, Q1, TP.HCM', 'hash_pw_09'),
(N'Phan Minh Đức', '0902000010', 'duc.pm@example.com', N'505 Trần Quốc Toản, Q3, TP.HCM', 'hash_pw_10');
GO

-- Thêm 2 NV để có lịch sử bán hàng đa dạng
INSERT INTO NHANVIEN (HOTEN, CHUCVU, EMAIL, TENDN, MATKHAU, TRANGTHAI)
VALUES
(N'Nguyễn Minh Sales', N'Bán hàng', 'sales1@laptopstore.com', 'sales1', 'pw_sales1', 1),
(N'Trần Văn Ship', N'Giao nhận', 'ship1@laptopstore.com', 'ship1', 'pw_ship1', 1);
GO

-- Cấp quyền cho NV mới (nếu PHANQUYEN đã có Admin=1,BH=2,Kho=3)
INSERT INTO CAPQUYEN (MANV, MAQUYEN)
VALUES
((SELECT MANV FROM NHANVIEN WHERE TENDN='sales1'), 2),
((SELECT MANV FROM NHANVIEN WHERE TENDN='ship1'), 2);
GO

------------------------------------------------------------------
-- BỔ SUNG: MỘT SỐ KHÁCH HÀNG ĐÃ LƯU VOUCHER (một vài đã dùng)
------------------------------------------------------------------
INSERT INTO KHACHHANG_VOUCHER (MAKH, MAVOUCHER, NGAYDUNG, MAHD)
VALUES
((SELECT MAKH FROM KHACHHANG WHERE EMAIL='huy.pq@example.com'), 'NEWMEMBER', NULL, NULL),
((SELECT MAKH FROM KHACHHANG WHERE EMAIL='mai.ngo@example.com'), 'SALE50K', NULL, NULL),
((SELECT MAKH FROM KHACHHANG WHERE EMAIL='long.bv@example.com'), 'SALE50K', '2024-06-15', NULL),
((SELECT MAKH FROM KHACHHANG WHERE EMAIL='lan.ht@example.com'), 'GIAM500', '2024-12-24', NULL);
GO

------------------------------------------------------------------
-- TẠO LỊCH SỬ ĐƠN HÀNG MẪU (đa dạng ngày, voucher, trạng thái)
-- Mỗi block: INSERT HOADON -> lấy @mahd -> INSERT CT_HOADON -> UPDATE kho -> INSERT THANHTOAN
------------------------------------------------------------------

-- ĐƠN 1: KH tối giản, dùng voucher 'SALE50K', hoàn thành (ngày 2024-03-05)
BEGIN TRAN;
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, MAVOUCHER, SOTIEN_GIAM_VOUCHER, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
VALUES ('2024-03-05', (SELECT MAKH FROM KHACHHANG WHERE EMAIL='huy.pq@example.com'), (SELECT MANV FROM NHANVIEN WHERE TENDN='nvbanhang1'),
        (SELECT TOP 1 GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Dell G15%'), 'SALE50K', 50000,
        (SELECT TOP 1 GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Dell G15%') - 50000, N'Hoàn thành', N'12 Nguyễn Trãi', '0902000001');
DECLARE @mahd INT = SCOPE_IDENTITY();

INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd, MALAPTOP, 1, GIA_BAN, GIA_BAN*1
FROM LAPTOP WHERE TENLAPTOP LIKE '%Dell G15%';

UPDATE LAPTOP
SET SOLUONG_TON = SOLUONG_TON - 1
WHERE MALAPTOP = (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP LIKE '%Dell G15%');

INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
VALUES (@mahd, N'MoMo', '2024-03-05', (SELECT TONG_THANHTOAN FROM HOADON WHERE MAHD = @mahd), N'Đã thanh toán');
COMMIT;
GO

-- ĐƠN 2: Mua 2 sp (MacBook Air M1 + RAM upgrade giả lập), trả tiền mặt, ngày 2024-04-10
BEGIN TRAN;
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, MAVOUCHER, SOTIEN_GIAM_VOUCHER, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
VALUES ('2024-04-10', (SELECT MAKH FROM KHACHHANG WHERE EMAIL='mai.ngo@example.com'), (SELECT MANV FROM NHANVIEN WHERE TENDN='nvbanhang1'),
        ((SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%MacBook Air M1%') + (SELECT ISNULL((SELECT TOP 1 GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo IdeaPad 3%'),0))),
        NULL, 0,
        ((SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%MacBook Air M1%') + (SELECT ISNULL((SELECT TOP 1 GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo IdeaPad 3%'),0))),
        N'Hoàn thành', N'34 Cách Mạng', '0902000002');
DECLARE @mahd2 INT = SCOPE_IDENTITY();

-- CT
INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd2, MALAPTOP, 1, GIA_BAN, GIA_BAN*1 FROM LAPTOP WHERE TENLAPTOP LIKE '%MacBook Air M1%';
INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd2, MALAPTOP, 1, GIA_BAN, GIA_BAN*1 FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo IdeaPad 3%';

-- Update stock
UPDATE LAPTOP SET SOLUONG_TON = SOLUONG_TON - 1 WHERE MALAPTOP IN (
    (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP LIKE '%MacBook Air M1%'),
    (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo IdeaPad 3%')
);

INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
VALUES (@mahd2, N'Tiền mặt', '2024-04-10', (SELECT TONG_THANHTOAN FROM HOADON WHERE MAHD = @mahd2), N'Đã thanh toán');
COMMIT;
GO

-- ĐƠN 3: Đơn lớn có voucher 'BLACKFRIDAY' (giả lập ngày 2024-11-15), trạng thái Hoàn thành
BEGIN TRAN;
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, MAVOUCHER, SOTIEN_GIAM_VOUCHER, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
VALUES ('2024-11-15', (SELECT MAKH FROM KHACHHANG WHERE EMAIL='long.bv@example.com'), (SELECT MANV FROM NHANVIEN WHERE TENDN='nvbanhang1'),
        (SELECT SUM(GIA_BAN) FROM LAPTOP WHERE TENLAPTOP IN ('MacBook Pro 14 M2','MSI Titan 18 HX')), 'BLACKFRIDAY',  (SELECT SUM(GIA_BAN)*0.10 FROM LAPTOP WHERE TENLAPTOP IN ('MacBook Pro 14 M2','MSI Titan 18 HX')),
        (SELECT SUM(GIA_BAN) - SUM(GIA_BAN)*0.10 FROM LAPTOP WHERE TENLAPTOP IN ('MacBook Pro 14 M2','MSI Titan 18 HX')), N'Hoàn thành', N'56 Trần Phú', '0902000003');
DECLARE @mahd3 INT = SCOPE_IDENTITY();

-- CT: 1 x MacBook Pro 14 M2, 1 x MSI Titan 18 HX
INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd3, MALAPTOP, 1, GIA_BAN, GIA_BAN FROM LAPTOP WHERE TENLAPTOP = 'MacBook Pro 14 M2';

INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd3, MALAPTOP, 1, GIA_BAN, GIA_BAN FROM LAPTOP WHERE TENLAPTOP = 'MSI Titan 18 HX';

UPDATE LAPTOP SET SOLUONG_TON = SOLUONG_TON - 1 WHERE MALAPTOP IN (
    (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP = 'MacBook Pro 14 M2'),
    (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP = 'MSI Titan 18 HX')
);

INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
VALUES (@mahd3, N'Chuyển khoản', '2024-11-15', (SELECT TONG_THANHTOAN FROM HOADON WHERE MAHD = @mahd3), N'Đã thanh toán');
COMMIT;
GO

-- ĐƠN 4: Đơn chờ xử lý, chưa thanh toán (2025-02-02)
BEGIN TRAN;
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, MAVOUCHER, SOTIEN_GIAM_VOUCHER, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
VALUES ('2025-02-02', (SELECT MAKH FROM KHACHHANG WHERE EMAIL='lan.ht@example.com'), (SELECT MANV FROM NHANVIEN WHERE TENDN='sales1'),
        (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo Legion 5%'), NULL, 0,
        (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo Legion 5%'), N'Chờ xử lý', N'78 Lê Duẩn', '0902000004');
DECLARE @mahd4 INT = SCOPE_IDENTITY();

INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd4, MALAPTOP, 1, GIA_BAN, GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo Legion 5%';

-- chưa trừ kho (giữ tồn khi chưa xác nhận giao)
INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
VALUES (@mahd4, N'MoMo', NULL, 0, N'Chưa thanh toán');
COMMIT;
GO

-- ĐƠN 5: Nhiều đơn nhỏ rải rác (thêm 4 đơn nhanh để số liệu dày)
-- (a)
BEGIN TRAN;
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
VALUES ('2024-07-01', (SELECT MAKH FROM KHACHHANG WHERE EMAIL='khang.vm@example.com'), (SELECT MANV FROM NHANVIEN WHERE TENDN='nvkho1'),
        (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo IdeaPad 3%'), (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo IdeaPad 3%'), N'Hoàn thành', N'90 Phạm Ngũ Lão', '0902000005');
DECLARE @mahd5 INT = SCOPE_IDENTITY();
INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd5, MALAPTOP, 1, GIA_BAN, GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo IdeaPad 3%';
UPDATE LAPTOP SET SOLUONG_TON = SOLUONG_TON - 1 WHERE MALAPTOP = (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP LIKE '%Lenovo IdeaPad 3%');
INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
VALUES (@mahd5, N'Tiền mặt', '2024-07-01', (SELECT TONG_THANHTOAN FROM HOADON WHERE MAHD=@mahd5), N'Đã thanh toán');
COMMIT;
GO

-- (b)
BEGIN TRAN;
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
VALUES ('2024-08-15', (SELECT MAKH FROM KHACHHANG WHERE EMAIL='hanh.lt@example.com'), (SELECT MANV FROM NHANVIEN WHERE TENDN='nvbanhang1'),
        (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Asus Vivobook 15%'), (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Asus Vivobook 15%'), N'Hoàn thành', N'101 Bến Thành', '0902000006');
DECLARE @mahd6 INT = SCOPE_IDENTITY();
INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd6, MALAPTOP, 1, GIA_BAN, GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%Asus Vivobook 15%';
UPDATE LAPTOP SET SOLUONG_TON = SOLUONG_TON - 1 WHERE MALAPTOP = (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP LIKE '%Asus Vivobook 15%');
INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
VALUES (@mahd6, N'Tiền mặt', '2024-08-15', (SELECT TONG_THANHTOAN FROM HOADON WHERE MAHD=@mahd6), N'Đã thanh toán');
COMMIT;
GO

-- (c)
BEGIN TRAN;
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
VALUES ('2024-09-30', (SELECT MAKH FROM KHACHHANG WHERE EMAIL='tri.dc@example.com'), (SELECT MANV FROM NHANVIEN WHERE TENDN='sales1'),
        (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%MSI Modern 14%'), (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%MSI Modern 14%'), N'Hoàn thành', N'202 Võ Văn Tần', '0902000007');
DECLARE @mahd7 INT = SCOPE_IDENTITY();
INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd7, MALAPTOP, 1, GIA_BAN, GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%MSI Modern 14%';
UPDATE LAPTOP SET SOLUONG_TON = SOLUONG_TON - 1 WHERE MALAPTOP = (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP LIKE '%MSI Modern 14%');
INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
VALUES (@mahd7, N'Chuyển khoản', '2024-09-30', (SELECT TONG_THANHTOAN FROM HOADON WHERE MAHD=@mahd7), N'Đã thanh toán');
COMMIT;
GO

-- (d)
BEGIN TRAN;
INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
VALUES ('2025-01-05', (SELECT MAKH FROM KHACHHANG WHERE EMAIL='nam.pv@example.com'), (SELECT MANV FROM NHANVIEN WHERE TENDN='nvbanhang1'),
        (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%HP Omen 16%'), (SELECT GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%HP Omen 16%'), N'Hoàn thành', N'303 Nguyễn Văn Cừ', '0902000008');
DECLARE @mahd8 INT = SCOPE_IDENTITY();
INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
SELECT @mahd8, MALAPTOP, 1, GIA_BAN, GIA_BAN FROM LAPTOP WHERE TENLAPTOP LIKE '%HP Omen 16%';
UPDATE LAPTOP SET SOLUONG_TON = SOLUONG_TON - 1 WHERE MALAPTOP = (SELECT MALAPTOP FROM LAPTOP WHERE TENLAPTOP LIKE '%HP Omen 16%');
INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
VALUES (@mahd8, N'MoMo', '2025-01-05', (SELECT TONG_THANHTOAN FROM HOADON WHERE MAHD=@mahd8), N'Đã thanh toán');
COMMIT;
GO

------------------------------------------------------------------
-- TĂNG DẦN SỐ LIỆU THỐNG KÊ: TẠO THÊM NHIỀU ĐƠN NGẪU NHIÊN (COPY/PASTE nhanh)
-- Dùng vòng lặp để tạo các đơn nhỏ giúp đồ thị dày hơn
------------------------------------------------------------------
DECLARE @i INT = 1;
WHILE @i <= 15
BEGIN
    BEGIN TRAN;
    INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, TONG_THANHTOAN, TRANGTHAI)
    VALUES (DATEADD(day, -(@i*5), GETDATE()),
            (SELECT TOP 1 MAKH FROM KHACHHANG ORDER BY NEWID()),
            (SELECT TOP 1 MANV FROM NHANVIEN ORDER BY NEWID()),
            (SELECT TOP 1 GIA_BAN FROM LAPTOP ORDER BY NEWID()),
            (SELECT TOP 1 GIA_BAN FROM LAPTOP ORDER BY NEWID()),
            N'Hoàn thành');
    DECLARE @mid INT = SCOPE_IDENTITY();
    INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
    SELECT @mid, MALAPTOP, 1, GIA_BAN, GIA_BAN FROM (
        SELECT TOP 1 MALAPTOP, GIA_BAN FROM LAPTOP ORDER BY NEWID()
    ) t;
    -- Trừ kho 1 đơn mỗi lần (nếu đủ)
    UPDATE LAPTOP SET SOLUONG_TON = CASE WHEN SOLUONG_TON > 0 THEN SOLUONG_TON - 1 ELSE 0 END
    WHERE MALAPTOP = (SELECT TOP 1 MALAPTOP FROM LAPTOP ORDER BY NEWID());
    INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
    VALUES (@mid, N'Online', GETDATE(), (SELECT TONG_THANHTOAN FROM HOADON WHERE MAHD=@mid), N'Đã thanh toán');
    COMMIT;
    SET @i = @i + 1;
END
GO

------------------------------------------------------------------
-- TINH CHỈ SỐ: CẬP NHẬT TRẠNG THÁI VOUCHER ĐÃ DÙNG (ví dụ mark 1 voucher đã dùng)
------------------------------------------------------------------
UPDATE VOUCHER SET DA_DUNG = DA_DUNG + 1 WHERE MAVOUCHER IN ('NEWMEMBER','SALE50K') -- tăng 1 cho ví dụ
GO

------------------------------------------------------------------
-- TỐI CUỐI: Một vài view/report mẫu (giúp test BI / đồ thị)
------------------------------------------------------------------
-- 1) Tổng đơn theo ngày (dùng để vẽ chart)
CREATE OR ALTER VIEW V_REPORT_DON_THEO_NGAY AS
SELECT CAST(NGAYLAP AS DATE) AS [NGAY], COUNT(*) AS SO_DON, SUM(ISNULL(TONG_THANHTOAN,0)) AS DOANHTHU
FROM HOADON
GROUP BY CAST(NGAYLAP AS DATE);
GO

-- 2) Top 10 sản phẩm bán chạy
CREATE OR ALTER VIEW V_TOP_SANPHAM AS
SELECT TOP 10 L.MALAPTOP, L.TENLAPTOP, SUM(C.SOLUONG) AS SO_LUONG_BAN
FROM CT_HOADON C
JOIN LAPTOP L ON C.MALAPTOP = L.MALAPTOP
GROUP BY L.MALAPTOP, L.TENLAPTOP
ORDER BY SO_LUONG_BAN DESC;
GO


UPDATE LAPTOP
SET SOLUONG_TON = SOLUONG_TON + 100;
GO

USE QL_SALE_LAPTOP;
GO

-- 1. TĂNG TỒN KHO TOÀN BỘ (Hàng lúc nào cũng sẵn)
UPDATE LAPTOP SET SOLUONG_TON = SOLUONG_TON + 150 WHERE SOLUONG_TON < 100;
GO

-- 2. THÊM DỮ LIỆU SẢN PHẨM MỚI (Cao cấp & Đa dạng)

-- === ACER (MAHANG = 7 - Nếu chưa có thì thêm hãng này trước) ===
IF NOT EXISTS (SELECT * FROM HANG WHERE TENHANG = 'Acer')
BEGIN
    INSERT INTO HANG (TENHANG, DIACHI, SODT) VALUES (N'Acer', N'Taiwan', '1800-9999');
END
DECLARE @MaHangAcer int = (SELECT MAHANG FROM HANG WHERE TENHANG = 'Acer');

-- Gaming (MALOAI = 1)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, TRANGTHAI) VALUES
(N'Acer Predator Helios Neo 16', N'Chiến binh hiệu năng cao, tản nhiệt AeroBlade 3D', N'i7-13700HX, 16GB RAM, 512GB SSD, RTX 4060, 16" WQXGA 165Hz', 38000000, 35990000, 120, 1, @MaHangAcer, 'acer_predator_neo16.png', 1),
(N'Acer Nitro V 15 2024', N'Laptop gaming quốc dân phiên bản mới', N'i5-13420H, 8GB RAM, 512GB SSD, RTX 4050, 15.6" FHD 144Hz', 24000000, 21990000, 200, 1, @MaHangAcer, 'acer_nitro_v15.png', 1);

-- Mỏng nhẹ (MALOAI = 3)
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, TRANGTHAI) VALUES
(N'Acer Swift Go 14 AI', N'Laptop AI mỏng nhẹ, màn hình OLED rực rỡ', N'Core Ultra 5 125H, 16GB RAM, 512GB SSD, 14" 2.8K OLED', 26000000, 24490000, 150, 3, @MaHangAcer, 'acer_swift_go14.png', 1);


-- === DELL (MAHANG = 1) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, TRANGTHAI) VALUES
(N'Dell XPS 16 9640 (2024)', N'Tuyệt tác công nghệ, thiết kế tương lai', N'Core Ultra 9 185H, 32GB RAM, 1TB SSD, RTX 4070, 16.3" 4K+ OLED Touch', 95000000, 92990000, 105, 4, 1, 'dell_xps_16_9640.png', 1),
(N'Dell Inspiron 14 7440 2-in-1', N'Xoay gập linh hoạt, làm việc đa năng', N'Core 5 120U, 16GB RAM, 512GB SSD, 14" FHD+ Touch', 21000000, 19500000, 180, 2, 1, 'dell_inspiron_7440.png', 1);


-- === ASUS (MAHANG = 2) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, TRANGTHAI) VALUES
(N'Asus ROG Zephyrus G16 (2024)', N'Laptop gaming mỏng nhất thế giới, màn OLED', N'Core Ultra 9 185H, 32GB RAM, 1TB SSD, RTX 4080, 16" 2.5K OLED 240Hz', 80000000, 78990000, 110, 1, 2, 'asus_rog_g16_2024.png', 1),
(N'Asus Zenbook Duo (2024)', N'Laptop 2 màn hình OLED cảm ứng độc đáo', N'Core Ultra 9 185H, 32GB RAM, 1TB SSD, 2x 14" 3K OLED 120Hz', 65000000, 63990000, 105, 4, 2, 'asus_zenbook_duo_2024.png', 1),
(N'Asus Vivobook Go 15', N'Giá rẻ, mỏng nhẹ cho sinh viên', N'Ryzen 5 7520U, 8GB RAM, 512GB SSD, 15.6" FHD OLED', 13000000, 11490000, 300, 2, 2, 'asus_vivobook_go15.png', 1);


-- === HP (MAHANG = 3) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, TRANGTHAI) VALUES
(N'HP Spectre x360 14 (2024)', N'Laptop doanh nhân xoay gập tốt nhất thế giới', N'Core Ultra 7 155H, 32GB RAM, 1TB SSD, 14" 2.8K OLED 120Hz', 45000000, 42990000, 130, 3, 3, 'hp_spectre_x360_14.png', 1),
(N'HP Victus 16 (2024)', N'Gaming tầm trung, hiệu năng ổn định', N'i5-14500HX, 16GB RAM, 512GB SSD, RTX 4060, 16.1" FHD 144Hz', 29000000, 27490000, 160, 1, 3, 'hp_victus_16_2024.png', 1);


-- === LENOVO (MAHANG = 4) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, TRANGTHAI) VALUES
(N'Lenovo Legion 9i Gen 9', N'Siêu phẩm gaming tản nhiệt nước tích hợp', N'i9-14900HX, 64GB RAM, 2TB SSD, RTX 4090, 16" 3.2K Mini-LED 165Hz', 130000000, 125990000, 102, 1, 4, 'lenovo_legion_9i.png', 1),
(N'Lenovo ThinkPad X1 Carbon Gen 12', N'Biểu tượng doanh nhân, siêu nhẹ, siêu bền', N'Core Ultra 7 155U, 32GB RAM, 1TB SSD, 14" 2.8K OLED', 62000000, 59990000, 120, 2, 4, 'lenovo_thinkpad_x1_g12.png', 1),
(N'Lenovo LOQ 15IAX9', N'Gaming giá rẻ thay thế dòng IdeaPad Gaming', N'i5-12450HX, 12GB RAM, 512GB SSD, RTX 2050, 15.6" FHD 144Hz', 18000000, 16490000, 250, 1, 4, 'lenovo_loq_15.png', 1);


-- === APPLE (MAHANG = 5) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, TRANGTHAI) VALUES
(N'MacBook Air 13 M3 (Midnight)', N'Chip M3 mạnh mẽ, màu đêm xanh huyền bí', N'Apple M3, 16GB RAM, 512GB SSD, 13.6" Liquid Retina', 35000000, 32990000, 180, 5, 5, 'macbook_air_m3_midnight.png', 1),
(N'MacBook Pro 14 M3 (Space Black)', N'Màu đen không gian mới, chip M3 Pro', N'Apple M3 Pro, 18GB RAM, 1TB SSD, 14.2" XDR 120Hz', 60000000, 57990000, 140, 5, 5, 'macbook_pro_m3_black.png', 1);


-- === MSI (MAHANG = 6) ===
INSERT INTO LAPTOP (TENLAPTOP, MOTA, CAUHINH, GIA_GOC, GIA_BAN, SOLUONG_TON, MALOAI, MAHANG, HINHANH0, TRANGTHAI) VALUES
(N'MSI Raider GE78 HX', N'Dải đèn Matrix LED cực chất, hiệu năng đỉnh', N'i9-14900HX, 64GB RAM, 2TB SSD, RTX 4080, 17" QHD+ 240Hz', 90000000, 87990000, 110, 1, 6, 'msi_raider_ge78.png', 1),
(N'MSI Claw A1M', N'Máy chơi game cầm tay (Handheld PC) đầu tiên của MSI', N'Core Ultra 7 155H, 16GB RAM, 512GB SSD, 7" FHD 120Hz Touch', 21000000, 19990000, 150, 1, 6, 'msi_claw_a1m.png', 1);

GO


UPDATE LAPTOP 
SET SOLUONG_TON = SOLUONG_TON + 200 
WHERE SOLUONG_TON < 200;
GO

-- ============================================================
-- PHẦN 2: TỰ ĐỘNG SINH 50 KHÁCH HÀNG MỚI (TRÁNH TRÙNG LẶP)
-- Sử dụng vòng lặp và check NOT EXISTS để an toàn tuyệt đối
-- ============================================================
PRINT N'Dang tao khach hang gia lap...';
DECLARE @k INT = 1;
DECLARE @fakePhone VARCHAR(15);
DECLARE @fakeEmail NVARCHAR(100);

WHILE @k <= 50
BEGIN
    -- Tạo SĐT và Email ngẫu nhiên theo index
    SET @fakePhone = '099' + RIGHT('000000' + CAST(@k AS VARCHAR(10)), 7);
    SET @fakeEmail = 'user.auto.' + CAST(@k AS VARCHAR(10)) + '@gmail.com';

    -- Chỉ thêm nếu chưa tồn tại (Tránh lỗi Unique Key)
    IF NOT EXISTS (SELECT 1 FROM KHACHHANG WHERE EMAIL = @fakeEmail OR SODT = @fakePhone)
    BEGIN
        INSERT INTO KHACHHANG (HOTEN, SODT, EMAIL, DIACHI, MATKHAU, NGAYTAO)
        VALUES (
            N'Khách Hàng Ảo ' + CAST(@k AS NVARCHAR(10)), 
            @fakePhone, 
            @fakeEmail, 
            N'Địa chỉ ngẫu nhiên số ' + CAST(@k AS NVARCHAR(10)) + N', TP.HCM', 
            '123456', -- Mật khẩu giả
            DATEADD(DAY, -CAST(RAND()*365 AS INT), GETDATE()) -- Ngày tạo ngẫu nhiên trong 1 năm qua
        );
    END
    SET @k = @k + 1;
END
GO

-- ============================================================
-- PHẦN 3: CỖ MÁY THỜI GIAN - TẠO 300 ĐƠN HÀNG QUÁ KHỨ
-- Giúp biểu đồ Doanh thu vẽ đẹp (có dữ liệu 12 tháng)
-- ============================================================
PRINT N'Dang chay co may thoi gian tao don hang...';

DECLARE @i INT = 1;
DECLARE @TotalOrders INT = 300; -- Số lượng đơn giả lập muốn tạo

-- Biến tạm
DECLARE @RandomDate DATETIME;
DECLARE @RandomKhach INT;
DECLARE @RandomProduct INT;
DECLARE @ProductPrice DECIMAL(15,2);
DECLARE @RandomQty INT;
DECLARE @RandomStatus NVARCHAR(50);
DECLARE @NewOrderID INT;

WHILE @i <= @TotalOrders
BEGIN
    -- 1. Random Ngày: Trong vòng 365 ngày trở lại đây
    SET @RandomDate = DATEADD(DAY, -CAST(RAND()*365 AS INT), GETDATE());
    
    -- 2. Random Khách: Lấy ngẫu nhiên 1 khách
    SELECT TOP 1 @RandomKhach = MAKH FROM KHACHHANG ORDER BY NEWID();
    
    -- 3. Random Sản phẩm: Lấy ngẫu nhiên 1 laptop
    SELECT TOP 1 @RandomProduct = MALAPTOP, @ProductPrice = GIA_BAN 
    FROM LAPTOP ORDER BY NEWID();
    
    -- 4. Random Số lượng: 1 đến 3 chiếc
    SET @RandomQty = CAST(RAND()*2 AS INT) + 1; 

    -- 5. Random Trạng thái (80% Hoàn thành, 10% Hủy, 10% Chờ)
    DECLARE @dice FLOAT = RAND();
    IF @dice > 0.2 
        SET @RandomStatus = N'Hoàn thành';
    ELSE IF @dice > 0.1
        SET @RandomStatus = N'Đã hủy';
    ELSE
        SET @RandomStatus = N'Chờ xử lý';

    BEGIN TRAN;
        -- A. Tạo Hóa Đơn
		INSERT INTO HOADON (NGAYLAP, MAKH, MANV, TONGTIEN_HANG, TONG_THANHTOAN, TRANGTHAI, DIACHI_GIAO, SDT_GIAO)
        VALUES (
            @RandomDate, 
            @RandomKhach, 
            1, -- Gán tạm cho Admin (MANV=1)
            @ProductPrice * @RandomQty, 
            @ProductPrice * @RandomQty, -- Tạm thời không tính voucher cho đơn giản
            @RandomStatus, 
            N'Địa chỉ giao hàng tự động', 
            '0900000000'
        );
        
        SET @NewOrderID = SCOPE_IDENTITY();

        -- B. Tạo Chi Tiết Hóa Đơn
        INSERT INTO CT_HOADON (MAHD, MALAPTOP, SOLUONG, DONGIA, THANHTIEN)
        VALUES (@NewOrderID, @RandomProduct, @RandomQty, @ProductPrice, @ProductPrice * @RandomQty);

        -- C. Trừ Tồn Kho (Chỉ trừ nếu đơn không hủy)
        IF @RandomStatus != N'Đã hủy'
        BEGIN
            UPDATE LAPTOP 
            SET SOLUONG_TON = CASE WHEN SOLUONG_TON >= @RandomQty THEN SOLUONG_TON - @RandomQty ELSE 0 END
            WHERE MALAPTOP = @RandomProduct;
        END

        -- D. Tạo Thanh Toán (Nếu hoàn thành)
        IF @RandomStatus = N'Hoàn thành'
        BEGIN
            INSERT INTO THANHTOAN (MAHD, HINHTHUC, NGAYTHANHTOAN, SOTIEN, TRANGTHAI)
            VALUES (
                @NewOrderID, 
                CASE WHEN RAND() > 0.5 THEN N'MoMo' ELSE N'Tiền mặt' END, -- 50/50 MoMo hoặc Tiền mặt
                @RandomDate, 
                @ProductPrice * @RandomQty, 
                N'Đã thanh toán'
            );
        END
    COMMIT;

    SET @i = @i + 1;
END
GO