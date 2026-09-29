using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Backend.API.Data;
using Backend.API.Models;

class Program
{
    static void Main()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=WebsiteTrangThietBiYTe;Trusted_Connection=True;TrustServerCertificate=True;");
        
        using var db = new AppDbContext(optionsBuilder.Options);
        
        var tongSanPham = db.Products.Count(p => p.TrangThai);
        Console.WriteLine($"tongSanPham: {tongSanPham}");
        
        var tongDoanhThu = db.Orders.Where(o => o.TrangThai == OrderStatus.DaGiao).Sum(o => (decimal?)o.TongTien) ?? 0;
        Console.WriteLine($"tongDoanhThu (DaGiao): {tongDoanhThu}");
        
        var allOrders = db.Orders.Count();
        Console.WriteLine($"allOrders: {allOrders}");
        
        var startDate = DateTime.Today.AddDays(-7);
        var ordersInWeek = db.Orders.Count(o => o.NgayDat >= startDate);
        Console.WriteLine($"ordersInWeek: {ordersInWeek}");
    }
}
