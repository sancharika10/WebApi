using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStoreRatingsApi.Data;

namespace MyStoreRatingsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "StoreOwner")]
    public class OwnerController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public OwnerController(ApplicationDbContext db) => _db = db;

        // List users who rated owner's stores
        [HttpGet("my-raters")]
        public async Task<IActionResult> MyRaters()
        {
            var ownerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (ownerId == null) return Unauthorized();

            var stores = await _db.Stores.Include(s => s.Ratings).Where(s => s.OwnerId == ownerId).ToListAsync();
            var ratings = stores.SelectMany(s => s.Ratings.Select(r => new {
                StoreId = s.Id,
                StoreName = s.Name,
                r.UserId,
                r.Score,
                r.CreatedAt
            })).ToList();

            var avg = ratings.Any() ? Math.Round(ratings.Average(r => r.Score), 2) : 0d;

            return Ok(new { AverageRating = avg, Raters = ratings });
        }
    }
}
