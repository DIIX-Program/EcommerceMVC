# EcommerceMVC 🛒

Ứng dụng thương mại điện tử được xây dựng bằng **ASP.NET Core MVC**, hỗ trợ đầy đủ các chức năng từ phía khách hàng đến quản trị hệ thống như quản lý sản phẩm, giỏ hàng, thanh toán, xác thực tài khoản và quản trị dữ liệu.

---

## Giới thiệu

EcommerceMVC là dự án mô phỏng một hệ thống bán hàng trực tuyến với kiến trúc MVC nhằm giúp triển khai và thực hành các kiến thức:

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Cookie Authentication
- AutoMapper
- Dependency Injection
- AJAX
- SMTP Email Service

Hệ thống được thiết kế theo hướng tách biệt giao diện, xử lý nghiệp vụ và dữ liệu để dễ bảo trì và mở rộng.

---

## Công nghệ sử dụng

### Backend
- C#
- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server

### Frontend
- HTML5
- CSS3
- JavaScript
- Razor View
- Bootstrap
- AJAX

### Hỗ trợ
- Cookie Authentication
- AutoMapper
- Dependency Injection
- SMTP Email Service

---

## Chức năng chính

### Người dùng

✔ Đăng ký tài khoản

✔ Xác thực Email bằng mã OTP

✔ Đăng nhập / Đăng xuất

✔ Quản lý thông tin cá nhân

✔ Upload ảnh đại diện

✔ Xem danh sách sản phẩm

✔ Lọc sản phẩm theo danh mục

✔ Xem chi tiết sản phẩm

✔ Tìm kiếm sản phẩm

✔ Thêm sản phẩm vào giỏ hàng

✔ Cập nhật số lượng bằng AJAX

✔ Danh sách yêu thích

✔ Thanh toán đơn hàng

✔ Popup khuyến mãi

---

### Quản trị viên (Admin)

✔ Quản lý sản phẩm (CRUD)

✔ Quản lý loại sản phẩm

✔ Quản lý khách hàng

✔ Quản lý nhân viên

✔ Quản lý đơn hàng

✔ Cập nhật trạng thái đơn hàng

✔ Đánh dấu sản phẩm Bestseller

✔ Sắp xếp hiển thị sản phẩm

✔ Tìm kiếm và phân trang

---

## Một số tính năng nổi bật

### Authentication & Security

- Cookie Authentication
- Role Authorization
- Mã hóa mật khẩu bằng Hash + Salt
- Xác thực Email OTP

### Shopping Cart

- Lưu giỏ hàng vào Database
- Đồng bộ theo tài khoản
- AJAX cập nhật không reload trang

### Admin Dashboard

- Dashboard quản trị riêng
- CRUD dữ liệu
- Upload hình ảnh sản phẩm
- Phân quyền Admin / Customer

### UI / UX

- Partial Views
- View Components
- Popup Sale
- Responsive Design

---

## Cấu trúc thư mục

```bash
EcommerceMVC
│
├── Controllers/
├── Data/
├── Helpers/
├── Models/
├── ViewModels/
├── Views/
├── wwwroot/
│
├── Program.cs
├── appsettings.json
└── README.md
```

---

## Luồng hoạt động

```text
User Request
      ↓
Routing
      ↓
Controller
      ↓
Business Logic
      ↓
Entity Framework Core
      ↓
SQL Server
      ↓
ViewModel
      ↓
Razor View
      ↓
Response
```

---

## Cài đặt dự án

### Clone source

```bash
git clone https://github.com/DIIX-Program/EcommerceMVC.git
```

### Di chuyển vào project

```bash
cd EcommerceMVC
```

### Cập nhật Connection String

Mở file:

```json
appsettings.json
```

Thay đổi:

```json
"ConnectionStrings": {
   "DefaultConnection":"Your_SQL_Server"
}
```

### Chạy migration

```bash
Update-Database
```

Hoặc:

```bash
dotnet ef database update
```

### Chạy ứng dụng

```bash
dotnet run
```

---

## Tài khoản test

### Admin

```text
Username: diix
Password: diix117@
```

---

## Hướng phát triển

- Tích hợp thanh toán VNPay/Momo
- Dashboard thống kê doanh thu
- Đánh giá sản phẩm
- Chat hỗ trợ khách hàng
- Gợi ý sản phẩm bằng AI
- API cho Mobile App

---

## Tác giả

DIIX

Sinh viên Công nghệ thông tin – Xây dựng dự án thực hành E-commerce bằng ASP.NET Core MVC.

---

⭐ Nếu dự án hữu ích, hãy để lại Star cho repository.
