using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyStoreRatingsApi.DTOs;
using MyStoreRatingsApi.Models;
using MyStoreRatingsApi.Repositories;
using System.Security.Claims;
using WebApi.Models;

namespace MyStoreRatingsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepo;
        private readonly IStoreRepository _storeRepo;
        private readonly UserManager<ApplicationUser> _userMgr;

        public RatingController(IRatingRepository ratingRepo, IStoreRepository storeRepo, UserManager<ApplicationUser> userMgr)
        {
            _ratingRepo = ratingRepo;
            _storeRepo = storeRepo;
            _userMgr = userMgr;
        }

        // Submit or update rating
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitRating(SubmitRatingDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();

            var store = await _storeRepo.GetByIdAsync(dto.StoreId);
            if (store == null) return NotFound("Store not found");

            var existing = await _ratingRepo.GetByUserAndStoreAsync(userId, dto.StoreId);
            if (existing != null)
            {
                existing.Score = dto.Score;
                existing.CreatedAt = DateTime.UtcNow;
                _ratingRepo.Update(existing);
            }
            else
            {
                var r = new Rating
                {
                    StoreId = dto.StoreId,
                    UserId = userId,
                    Score = dto.Score
                };
                await _ratingRepo.AddAsync(r);
            }

            await _ratingRepo.SaveChangesAsync();
            return Ok();
        }

        // Get logged in user's submitted rating for a store (for UI)
        [HttpGet("mine/{storeId}")]
        public async Task<IActionResult> MyRating(int storeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userId == null) return Unauthorized();
            var rating = await _ratingRepo.GetByUserAndStoreAsync(userId, storeId);
            if (rating == null) return NotFound();
            return Ok(new { rating.Id, rating.Score, rating.CreatedAt });
        }
    }
}
