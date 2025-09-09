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
    public class StoreController : ControllerBase
    {
        private readonly IStoreRepository _storeRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly UserManager<ApplicationUser> _userMgr;

        public StoreController(IStoreRepository storeRepo, IRatingRepository ratingRepo, UserManager<ApplicationUser> userMgr)
        {
            _storeRepo = storeRepo;
            _ratingRepo = ratingRepo;
            _userMgr = userMgr;
        }

        // Public: list stores with filters and sorting
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? name, [FromQuery] string? address, [FromQuery] string? sortBy, [FromQuery] bool desc = false)
        {
            var stores = await _storeRepo.FilterAsync(name, address, sortBy, desc);
            var dto = stores.Select(s => new StoreListDto
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                AverageRating = s.AverageRating
            });
            return Ok(dto);
        }

        // Admin: create store (only SystemAdmin)
        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public async Task<IActionResult> CreateStore(CreateStoreDto dto)
        {
            var store = new Store
            {
                Name = dto.Name,
                Email = dto.Email,
                Address = dto.Address,
                OwnerId = dto.OwnerId
            };
            await _storeRepo.AddAsync(store);
            await _storeRepo.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = store.Id }, store);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var store = await _storeRepo.GetWithRatingsAsync(id);
            if (store == null) return NotFound();
            return Ok(new
            {
                store.Id,
                store.Name,
                store.Email,
                store.Address,
                AverageRating = store.AverageRating,
                Ratings = store.Ratings.Select(r => new { r.Id, r.Score, r.UserId, r.CreatedAt })
            });
        }
    }
}
