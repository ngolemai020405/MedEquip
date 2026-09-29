namespace Backend.API.Models;

public class RecommendationLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public DateTime ThoiGian { get; set; } = DateTime.Now;
}
