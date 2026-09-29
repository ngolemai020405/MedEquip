using System.ComponentModel.DataAnnotations;

namespace Backend.API.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string TenSuKien { get; set; } = null!;
        
        [Required]
        public string HinhAnh { get; set; } = null!;
        
        public string? DuongDan { get; set; }
        
        public DateTime NgayBatDau { get; set; }
        
        public DateTime NgayKetThuc { get; set; }
        
        public bool TrangThai { get; set; } = true;
    }
}
