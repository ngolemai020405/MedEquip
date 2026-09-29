# HƯỚNG DẪN CHẠY PROJECT TỔNG HỢP - HỆ SINH THÁI MEDEQUIP (KLCN106)

Hệ thống bao gồm 4 thành phần chính, tất cả nằm chung trong Solution `MedEquip.sln`:
1. **Backend.API** — ASP.NET Core Web API (.NET 8) xử lý logic và DB — chạy cổng `https://localhost:7001`
2. **WebAdmin** — Website quản trị (dành cho Admin, Nhân viên)
3. **WebClient** — Website bán hàng (dành cho Khách hàng)
4. **MedEquipMobile** — Ứng dụng điện thoại (.NET MAUI) (dành cho Khách hàng)

---

## PHẦN 1: CÀI ĐẶT MÔI TRƯỜNG & DATABASE

### Bước 1: Công cụ cần thiết
1. **Visual Studio 2022** (cài sẵn workload *ASP.NET and web development* và *.NET Multi-platform App UI development*).
2. **SQL Server** (Express hoặc Developer) và SQL Server Management Studio (SSMS).
3. Máy ảo Android (Cài qua *Android Device Manager* trong Visual Studio).

### Bước 2: Tạo Database
1. Mở SSMS, kết nối vào SQL Server (thường là `.\SQLEXPRESS` hoặc `localhost`).
2. Mở file `Database/CreateDatabase.sql` bằng SSMS và nhấn **F5 (Execute)**.
3. Script sẽ tự tạo database tên `WebsiteTrangThietBiYTe`, tạo 13 bảng và chèn sẵn dữ liệu mẫu (38 sản phẩm, danh mục, đơn hàng, người dùng...).

### Bước 3: Cấu hình kết nối Database trong Project
1. Mở file `MedEquip.sln` bằng Visual Studio 2022.
2. Mở file `Backend.API/appsettings.json`, sửa dòng `DefaultConnection` cho khớp với SQL Server của bạn. 
   - *Ví dụ:* Nếu dùng bản Express thì để nguyên `Server=.\SQLEXPRESS;...`
   - *Ví dụ:* Nếu máy báo lỗi kết nối, đổi thành `Server=.;Database=WebsiteTrangThietBiYTe;Trusted_Connection=True;TrustServerCertificate=True;`

---

## PHẦN 2: HƯỚNG DẪN CHẠY TOÀN BỘ HỆ THỐNG (WEB + APP)

Để test đồng bộ toàn bộ luồng (Ví dụ: Khách mua hàng trên App Mobile -> Quản lý vào Web Admin thấy đơn hàng mới), hãy chạy theo cách tối ưu nhất sau đây:

### Bước 1: Chạy 3 hệ thống Web (Chạy ngầm)
Bạn cần chạy API lên trước để các hệ thống khác có dữ liệu.
1. Ở cột **Solution Explorer** bên phải, nhấp chuột phải vào project **`Backend.API`** -> chọn **Debug** -> **Start Without Debugging** (hoặc nhấn `Ctrl + F5`). (Trang API Swagger sẽ mở ra, cứ để đó).
2. Làm tương tự với **`WebAdmin`**: Chuột phải -> **Debug** -> **Start Without Debugging**.
3. Làm tương tự với **`WebClient`**: Chuột phải -> **Debug** -> **Start Without Debugging**.
*(Lúc này, toàn bộ Backend và 2 trang Web đã chạy sẵn sàng mà không làm nặng máy)*

### Bước 2: Chạy App Mobile (.NET MAUI)
1. Chuột phải vào project **`MedEquipMobile`** -> Chọn **Set as Startup Project** (để tên project in đậm).
2. Trên thanh công cụ ở trên cùng, xổ mục thiết bị chạy (Run dropdown) -> Chọn **Android Emulators** -> Chọn máy ảo Android của bạn (VD: `Pixel 5 - API 34`).
3. Nhấn **F5** (hoặc nút Run màu xanh). 
4. Đợi Visual Studio cài app vào máy ảo và mở lên (lần chạy đầu tiên sẽ mất 1-3 phút).

---

## PHẦN 3: HƯỚNG DẪN TEST & TÀI KHOẢN DEMO

**Mật khẩu dùng chung cho mọi tài khoản là:** `123456`

### 1. Luồng App Mobile & Web Bán Hàng (Khách hàng)
- **Tài khoản Demo:** `khachhang1@gmail.com` / `123456`
- **Các bước Test:**
  1. Đăng nhập vào App hoặc WebClient.
  2. Xem các sản phẩm gợi ý tự động, xem chi tiết cấu hình sản phẩm.
  3. Bấm **Mua/Thêm vào giỏ hàng** -> Chuyển sang Tab Giỏ Hàng -> Xác nhận đặt hàng.
  4. Vào phần **Đơn hàng** để xem lại đơn vừa đặt.

### 2. Luồng Web Quản Trị (Admin)
- **Tài khoản Demo:** `admin1@medequip.vn` / `123456` (Hoặc nhân viên kho: `nhanvienkho1@medequip.vn`)
- **Các bước Test:**
  1. Đăng nhập vào hệ thống quản trị WebAdmin.
  2. Vào phần **Đơn hàng**: Sẽ thấy đơn hàng bạn vừa đặt từ App Mobile / WebClient xuất hiện. Tiến hành **Duyệt đơn**.
  3. Vào phần **Sản phẩm**: Thử sửa mô tả, thêm mới sản phẩm. Ngay lập tức App/WebClient sẽ cập nhật.
  4. Xem bảng điều khiển (Dashboard) hiển thị tổng doanh thu và sản phẩm bán chạy.

---

## CÁC LỖI THƯỜNG GẶP KHI TEST

1. **Lỗi `0xe0434352` khi tạo máy ảo Android:** Tắt Visual Studio, nhấp chuột phải vào icon Visual Studio chọn **Run as Administrator**, sau đó vào Tools -> Android Device Manager để tạo lại máy ảo.
2. **App Mobile không gọi được API / Không đăng nhập được:** Đảm bảo `Backend.API` ĐÃ ĐƯỢC CHẠY (Ctrl+F5) TRƯỚC KHI mở App. App được cấu hình kết nối tự động tới `https://10.0.2.2:7001` (trỏ vào localhost của máy tính).
3. **Lỗi Database khi chạy API:** Kiểm tra lại kĩ chuỗi kết nối trong `appsettings.json`, nếu vẫn lỗi hãy mở SSMS lên kiểm tra xem database `WebsiteTrangThietBiYTe` đã có đủ 38 sản phẩm trong bảng Products chưa.

*Chúc nhóm kiểm thử và bảo vệ Khóa Luận thành công!*
