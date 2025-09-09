using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStoreRatingsApi.Data;
using MyStoreRatingsApi.Models;
using WebApi.Models;

namespace MyStoreRatingsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SystemAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userMgr;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userMgr)
        {
            _db = db;
            _userMgr = userMgr;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var totalUsers = await _userMgr.Users.CountAsync();
            var totalStores = await _db.Stores.CountAsync();
            var totalRatings = await _db.Ratings.CountAsync();
            return Ok(new { totalUsers, totalStores, totalRatings });
        }

        [HttpGet("stores")]
        public async Task<IActionResult> Stores(string? name, string? email, string? address, string? sortBy, bool desc = false)
        {
            var q = _db.Stores.Include(s => s.Ratings).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name)) q = q.Where(s => s.Name.Contains(name));
            if (!string.IsNullOrWhiteSpace(email)) q = q.Where(s => s.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(address)) q = q.Where(s => s.Address != null && s.Address.Contains(address));

            q = sortBy?.ToLower() switch
            {
                "name" => desc ? q.OrderByDescending(s => s.Name) : q.OrderBy(s => s.Name),
                "email" => desc ? q.OrderByDescending(s => s.Email) : q.OrderBy(s => s.Email),
                _ => q.OrderBy(s => s.Id)
            };

            var list = await q.Select(s => new {
                s.Id,
                s.Name,
                s.Email,
                s.Address,
                Rating = s.Ratings.Any() ? Math.Round(s.Ratings.Average(r => r.Score), 2) : 0
            }).ToListAsync();

            return Ok(list);
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users(string? name, string? email, string? address, string? role, string? sortBy, bool desc = false)
        {
            var q = _userMgr.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name)) q = q.Where(u => u.Name.Contains(name));
            if (!string.IsNullOrWhiteSpace(email)) q = q.Where(u => u.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(address)) q = q.Where(u => u.Address != null && u.Address.Contains(address));

            var list = await q.ToListAsync();

            // Fetch roles for each user
            var results = new List<object>();
            foreach (var u in list)
            {
                var roles = await _userMgr.GetRolesAsync(u);
                if (!string.IsNullOrWhiteSpace(role) && !roles.Contains(role)) continue;

                var rating = 0d;
                if (roles.Contains("StoreOwner"))
                {
                    // average rating for stores owned by this user
                    var ownerStores = await _db.Stores.Include(s => s.Ratings).Where(s => s.OwnerId == u.Id).ToListAsync();
                    rating = ownerStores.SelectMany(s => s.Ratings).Any() ? Math.Round(ownerStores.SelectMany(s => s.Ratings).Average(r => r.Score), 2) : 0d;
                }

                results.Add(new { u.Id, u.Name, u.Email, u.Address, Roles = roles, Rating = rating });
            }

            // Sorting
            results = sortBy?.ToLower() switch
            {
                "name" => desc ? results.OrderByDescending(r => ((dynamic)r).Name).ToList() : results.OrderBy(r => ((dynamic)r).Name).ToList(),
                "email" => desc ? results.OrderByDescending(r => ((dynamic)r).Email).ToList() : results.OrderBy(r => ((dynamic)r).Email).ToList(),
                _ => results
            };

            return Ok(results);
        }

        // Get details of a user
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            var u = await _userMgr.FindByIdAsync(id);
            if (u == null) return NotFound();

            var roles = await _userMgr.GetRolesAsync(u);
            double rating = 0;
            if (roles.Contains("StoreOwner"))
            {
                var ownerStores = await _db.Stores.Include(s => s.Ratings).Where(s => s.OwnerId == u.Id).ToListAsync();
                rating = ownerStores.SelectMany(s => s.Ratings).Any() ? Math.Round(ownerStores.SelectMany(s => s.Ratings).Average(r => r.Score), 2) : 0d;
            }

            return Ok(new { u.Id, u.Name, u.Email, u.Address, Roles = roles, Rating = rating });
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterAdminDto dto)
        {
            // Create a user with specified role (SystemAdmin or NormalUser or StoreOwner)
            // DTO defined inline below or in DTOs folder
            var existing = await _userMgr.FindByEmailAsync(dto.Email);
            if (existing != null) return BadRequest("Email already exists.");

            var u = new ApplicationUser { UserName = dto.Email, Email = dto.Email, Name = dto.Name, Address = dto.Address };
            var res = await _userMgr.CreateAsync(u, dto.Password);
            if (!res.Succeeded) return BadRequest(res.Errors.Select(e => e.Description));

            // add role
            if (!await _userMgr.IsInRoleAsync(u, dto.Role))
                await _userMgr.AddToRoleAsync(u, dto.Role);

            return Ok();
        }

        public class RegisterAdminDto
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
            public string? Address { get; set; }
            public string Role { get; set; } = "NormalUser";
        }
    }
}
