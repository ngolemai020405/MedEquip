namespace Backend.API.DTOs;
public class ReviewCreateDto
{
    public int ProductId { get; set; }
    public int SoSao { get; set; }
    public string? NoiDung { get; set; }
}