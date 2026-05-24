using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EcommerceMVC.ViewModels
{
    public class CapNhatThongTinVM
    {
        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string HoTen { get; set; }

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 kí tự")]
        public string? DiaChi { get; set; }

        [Display(Name = "Điện thoại")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 kí tự")]
        public string? DienThoai { get; set; }
        
        public string? HinhMacDinh { get; set; }

        [Display(Name = "Hình đại diện")]
        public IFormFile? Hinh { get; set; }
    }
}
