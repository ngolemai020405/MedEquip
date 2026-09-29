using Backend.API.Data;
using Backend.API.Models;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Services;
public class RecommendationService
{
    private readonly AppDbContext _db;
    public RecommendationService(AppDbContext db) => _db = db;
    public async Task<List<Product>> GetTopKForUserAsync(int userId, int k = 10)
    {
        var behaviors = await _db.UserBehaviors
            .Where(b => b.UserId == userId)
            .Include(b => b.Product)
            .ToListAsync();
        if (behaviors.Count == 0)
        {
            return await _db.Products
                .Where(p => p.TrangThai)
                .OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.SoSao) : 0)
                .Take(k)
                .ToListAsync();
        }
        var diemTheoDanhMuc = behaviors
            .GroupBy(b => b.Product.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(b => b.GiaTri));
        var sanPhamDaTuongTac = behaviors.Select(b => b.ProductId).ToHashSet();
        var candidates = await _db.Products
            .Where(p => p.TrangThai && !sanPhamDaTuongTac.Contains(p.ProductId))
            .Include(p => p.Reviews)
            .ToListAsync();
        var ketQua = candidates
            .Select(p => new
            {
                Product = p,
                Diem = (diemTheoDanhMuc.TryGetValue(p.CategoryId, out var d) ? d : 0)
                       + (p.Reviews.Any() ? p.Reviews.Average(r => r.SoSao) : 0)
            })
            .OrderByDescending(x => x.Diem)
            .Take(k)
            .Select(x => x.Product)
            .ToList();
        return ketQua;
    }
}