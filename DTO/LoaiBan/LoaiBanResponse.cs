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

        public LoaiBanResponse()
        {
            
        }
        public LoaiBanResponse(string message, bool IsSuccess)
        {
            this.message = message;
            this.IsSuccess = IsSuccess;
        }
        public LoaiBanResponse(int maLoaiBan, string tenLoaiBan, string moTa, bool trangThai, bool isSuccess, string? message)
        {
            MaLoaiBan = maLoaiBan;
            TenLoaiBan = tenLoaiBan;
            MoTa = moTa;
            TrangThai = trangThai;
            IsSuccess = isSuccess;
            this.message = message;
        }

        public LoaiBanResponse(string message)
        {
            this.message = message;
        }

    }
}