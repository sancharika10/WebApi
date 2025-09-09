using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyStoreRatingsApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(60, MinimumLength = 20)]
        public string Name { get; set; } = null!;

        [StringLength(400)]
        public string? Address { get; set; }
    }
}
