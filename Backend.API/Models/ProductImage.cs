namespace Backend.API.Models;
public class ProductImage
{
    public int ImageId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string Url { get; set; } = null!;
    public bool IsMain { get; set; } = true;
}