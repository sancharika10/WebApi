using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyStoreRatingsApi.DTOs;
using MyStoreRatingsApi.Models;
using MyStoreRatingsApi.Services;

namespace MyStoreRatingsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userMgr;
        private readonly SignInManager<ApplicationUser> _signInMgr;
        private readonly ITokenService _tokenService;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public AuthController(UserManager<ApplicationUser> userMgr, SignInManager<ApplicationUser> signInMgr, ITokenService tokenService, RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _signInMgr = signInMgr;
            _tokenService = tokenService;
            _roleMgr = roleMgr;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existing = await _userMgr.FindByEmailAsync(dto.Email);
            if (existing != null) return BadRequest("Email already registered.");

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                Name = dto.Name,
                Address = dto.Address
            };

            var result = await _userMgr.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.Select(e => e.Description));

            // Assign NormalUser role by default
            await _userMgr.AddToRoleAsync(user, "NormalUser");

            var token = await _tokenService.CreateTokenAsync(user);
            return Ok(new AuthResponseDto { Token = token, UserId = user.Id, Email = user.Email, Role = "NormalUser" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userMgr.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials");

            var res = await _signInMgr.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!res.Succeeded) return Unauthorized("Invalid credentials");

            var roles = await _userMgr.GetRolesAsync(user);
            var token = await _tokenService.CreateTokenAsync(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? "NormalUser"
            });
        }
    }
}
