using System.ComponentModel.DataAnnotations;

namespace MyStoreRatingsApi.DTOs
{
    public class RegisterDto
    {
        [Required, StringLength(60, MinimumLength = 20)]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [StringLength(400)]
        public string? Address { get; set; }

        [Required, StringLength(16, MinimumLength = 8)]
        public string Password { get; set; } = null!;
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
