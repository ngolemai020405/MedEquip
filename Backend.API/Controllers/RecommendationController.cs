using System.Security.Claims;
using Backend.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Backend.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly RecommendationService _service;
    public RecommendationController(RecommendationService service) => _service = service;
    [HttpGet("top-k")]
    [Authorize]
    public async Task<IActionResult> TopK([FromQuery] int k = 10)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _service.GetTopKForUserAsync(userId, k);
        return Ok(result.Select(p => new
        {
            p.ProductId,
            p.TenSP,
            p.Gia,
            p.HinhAnh
        }));
    }
}