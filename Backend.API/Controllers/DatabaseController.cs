using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.API.Data;
using Microsoft.Data.SqlClient;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DatabaseController : ControllerBase
{
    private readonly AppDbContext _db;

    public DatabaseController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("backup")]
    public async Task<IActionResult> Backup()
    {
        try
        {
            string dbName = _db.Database.GetDbConnection().Database;
            string backupDir = @"C:\Users\Public\MedEquipBackups";
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            string backupFileName = $"{dbName}_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            string backupFilePath = Path.Combine(backupDir, backupFileName);

            string sqlCommand = $"BACKUP DATABASE [{dbName}] TO DISK = '{backupFilePath}' WITH FORMAT;";

            await _db.Database.ExecuteSqlRawAsync(sqlCommand);

            if (!System.IO.File.Exists(backupFilePath))
            {
                return StatusCode(500, new { message = "Sao lưu thất bại. Không tìm thấy file backup." });
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(backupFilePath);
            
            System.IO.File.Delete(backupFilePath);

            return File(fileBytes, "application/octet-stream", backupFileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi sao lưu dữ liệu: " + ex.Message });
        }
    }
}
