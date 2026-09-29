namespace Backend.API.Models;
public class ProductPromotion
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; } = null!;
}