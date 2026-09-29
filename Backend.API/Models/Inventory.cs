using System.ComponentModel.DataAnnotations.Schema;
namespace Backend.API.Models;
[Table("Inventory")]
public class Inventory
{
    public int InventoryId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int SoLuongTon { get; set; }
    public int SoLuongToiThieu { get; set; } = 5;
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;
}