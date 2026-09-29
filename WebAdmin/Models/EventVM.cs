using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebAdmin.Models
{
    public class EventVM
    {
        public int EventId { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập tên sự kiện")]
        [Display(Name = "Tên sự kiện")]
        public string TenSuKien { get; set; } = null!;
        
        public string? HinhAnh { get; set; }
        
        [Display(Name = "Tải ảnh lên")]
        public IFormFile? HinhAnhUpload { get; set; }
        
        [Display(Name = "Đường dẫn liên kết")]
        public string? DuongDan { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; } = DateTime.Now.AddDays(7);
        
        [Display(Name = "Trạng thái hiển thị")]
        public bool TrangThai { get; set; } = true;
    }
}
