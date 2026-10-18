using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.DTO;

public class MonAnResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public MonAn? Data { get; set; }
}
