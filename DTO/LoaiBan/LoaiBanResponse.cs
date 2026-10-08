namespace QuanLyNhaHang.DTO
{
    public class LoaiBanResponse
    {
        public int MaLoaiBan { get; set; }
        public string TenLoaiBan { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; }
        public bool IsSuccess {get; set;}
        public string? message {get; set;}
    }
}