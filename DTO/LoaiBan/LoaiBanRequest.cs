using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.DTO
{
    public class LoaiBanRequest
    {
        public string TenLoaiBan { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }

        public LoaiBan ConvertToLoaiBan()
        {
            LoaiBan rs = new LoaiBan();
            rs.TenLoaiBan = TenLoaiBan;
            rs.MoTa = MoTa;
            rs.TrangThai = TrangThai;
            return rs;
        }
    }
}