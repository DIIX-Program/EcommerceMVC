using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceMVC.Models
{
    [Table("GioHang")]
    public partial class GioHang
    {
        [Key]
        [Column("MaKH", Order = 0)]
        [StringLength(20)]
        public string MaKh { get; set; } = null!;

        [Key]
        [Column("MaHH", Order = 1)]
        public int MaHh { get; set; }

        [Column("SoLuong")]
        public int SoLuong { get; set; }

        [ForeignKey("MaKh")]
        public virtual KhachHang MaKhNavigation { get; set; } = null!;

        [ForeignKey("MaHh")]
        public virtual HangHoa MaHhNavigation { get; set; } = null!;
    }
}
