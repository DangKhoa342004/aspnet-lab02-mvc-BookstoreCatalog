# Bookstore Catalog MVC - Cửa hàng bán sách

Dự án quản lý danh mục sách được xây dựng bằng ASP.NET Core MVC. Hệ thống hỗ trợ quản lý sách (CRUD), điều chỉnh tồn kho, kiểm soát quyền truy cập và lưu trữ nhật ký hoạt động (Audit Logs).

## Thông tin người thực hiện
- **Sinh viên thực hiện:** Nguyễn Đăng Khoa
- **Mã số sinh viên:** 22110084
- **Môn học/Hệ thống:** Web ASP.NET Core

---

## ✨ Tính năng nổi bật

Dự án tích hợp đầy đủ các tính năng của một hệ thống quản lý kho chuyên nghiệp:

*   **Quản lý Sản phẩm:** Thêm, sửa, xóa (xóa mềm), xem chi tiết sách.
*   **Thùng rác & Khôi phục:** Xóa sản phẩm không làm mất dữ liệu vĩnh viễn, hỗ trợ khôi phục dễ dàng từ giao diện Thùng rác.
*   **Kiểm soát tồn kho:** Điều chỉnh số lượng tồn kho với cơ chế xử lý đồng thời (RowVersion) để tránh xung đột dữ liệu.
*   **Audit Logs (Lịch sử hoạt động):** Tự động Ghi lại lịch sử hoạt động của người dùng (tên user, hành động, thời gian, địa chỉ IP).
*   **Phân quyền (RBAC):** 
    *   **Admin:** Toàn quyền hệ thống.
    *   **Staff:** Quản lý sản phẩm và tồn kho.
    *   **User:** Chỉ xem danh mục công khai (booklist và có thể mua sản phẩm).
    *   **Anonymous:** Chỉ đăng nhập

---

## Công nghệ sử dụng

*   **Framework:** .NET 10 / ASP.NET Core MVC
*   **Database:** Entity Framework Core, SQL Server
*   **Giao diện:** HBootstrap, Razor Views
*   **Kiến trúc:** Model-View-Controller (MVC)
*   **Logging:** NLog/Serilog (hoặc ILogger tích hợp)

---

## Cấu trúc thư mục (Tóm tắt)

*   **GET /health/live-json:** Trả về trạng thái hoạt động hiện tại (uptime, bộ nhớ, môi trường) dưới dạng JSON.
*   **GET /health/ready-json:** Trả về trạng thái sẵn sàng của ứng dụng và kết nối cơ sở dữ liệu dưới dạng JSON.

---

## Tài khoản demo

*   **Admin:** admin@bookstore.test / Admin@123
*   **Staff:** staff@bookstore.test / Staff@123
*   **User:** user@bookstore.test / User@123


---

## Hướng dẫn cài đặt và chạy sản phẩm

**Bước 1: Clone dự án về máy**
`git clone https://github.com/DangKhoa342004/aspnet-lab02-mvc-BookstoreCatalog.git`
`cd sporthub-inventory`

**Bước 2: Cấu hình Database**
Mở file appsettings.json và cập nhật Connection String trỏ tới cơ sở dữ liệu của bạn.

**Bước 3: Cập nhật Database**
Mở Terminal/Package Manager Console và chạy lệnh: `dotnet ef database update`

**Bước 4: Chạy ứng dụng**
`dotnet run`

---

## Cấu trúc thư mục (Tóm tắt)

*   **/Controllers:** Xử lý luồng yêu cầu (ví dụ: BooksController.cs).
*   **/Models:** Chứa các Entity của Database (ví dụ: Book.cs, AuditLog.cs).
*   **/Services:** Chứa Business Logic (ví dụ: BookService.cs, AuditLogService.cs).
*   **/ViewModels:** Chứa các model phục vụ hiển thị dữ liệu.
*   **/wwwroot/uploads/books:** Nơi lưu trữ file ảnh bìa sách.


Truy cập vào trình duyệt với địa chỉ hiển thị trong terminal.

*Dự án đã hoàn thiện - Nguyễn Đăng Khoa.*