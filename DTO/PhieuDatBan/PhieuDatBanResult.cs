namespace QuanLyNhaHang.DTO;

public class PhieuDatBanResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? MaPhieu { get; set; }
    public Dictionary<string, string[]> Errors { get; set; } = new();
}
