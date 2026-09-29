using Backend.API.Models;
using Microsoft.EntityFrameworkCore;
namespace Backend.API.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<ProductPromotion> ProductPromotions => Set<ProductPromotion>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<UserBehavior> UserBehaviors => Set<UserBehavior>();
    public DbSet<RecommendationLog> RecommendationLogs => Set<RecommendationLog>();
    public DbSet<Tinh> Tinhs => Set<Tinh>();
    public DbSet<PhuongXa> PhuongXas => Set<PhuongXa>();
    public DbSet<Event> Events => Set<Event>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tinh>().ToTable("Tinh");
        modelBuilder.Entity<PhuongXa>().ToTable("PhuongXa");
        modelBuilder.Entity<PhuongXa>()
            .HasOne(p => p.Tinh).WithMany(t => t.PhuongXas)
            .HasForeignKey(p => p.TinhId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>()
            .HasOne(u => u.Tinh).WithMany().HasForeignKey(u => u.TinhId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>()
            .HasOne(u => u.PhuongXa).WithMany().HasForeignKey(u => u.PhuongXaId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Tinh).WithMany().HasForeignKey(o => o.TinhId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.PhuongXa).WithMany().HasForeignKey(o => o.PhuongXaId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Category>()
            .HasOne(c => c.DanhMucCha)
            .WithMany(c => c.DanhMucCon)
            .HasForeignKey(c => c.DanhMucChaId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Product>()
            .Property(p => p.Gia).HasColumnType("decimal(18,0)");
        modelBuilder.Entity<Order>()
            .Property(o => o.TongTien).HasColumnType("decimal(18,0)");
        modelBuilder.Entity<OrderDetail>()
            .Property(o => o.DonGia).HasColumnType("decimal(18,0)");
        modelBuilder.Entity<Order>()
            .HasOne(o => o.NhanVienXuLy)
            .WithMany()
            .HasForeignKey(o => o.NhanVienXuLyId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ProductPromotion>()
            .HasKey(pp => new { pp.ProductId, pp.PromotionId });
        modelBuilder.Entity<ProductImage>().HasKey(pi => pi.ImageId);
        modelBuilder.Entity<UserBehavior>().HasKey(ub => ub.BehaviorId);
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Inventory)
            .WithOne(i => i.Product)
            .HasForeignKey<Inventory>(i => i.ProductId);
    }
}