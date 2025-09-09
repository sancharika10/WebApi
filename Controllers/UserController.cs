using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YourApp.Models;
using YourApp.Repositories;
using YourApp.DTOs;

namespace YourApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // ✅ Register new user (Normal User / Store Owner / Admin)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Address = dto.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), // secure hashing
                Role = dto.Role
            };

            await _userRepository.AddUserAsync(user);
            return Ok(new { message = "User registered successfully" });
        }

        // ✅ Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Invalid credentials");

            // (Optional) Generate JWT Token here
            return Ok(new { message = "Login successful", user = new { user.Id, user.Name, user.Email, user.Role } });
        }

        // ✅ Get all users (Admin only)
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        // ✅ Update Password
        [HttpPut("update-password/{id}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            return Ok(new { message = "Password updated successfully" });
        }
    }
}
