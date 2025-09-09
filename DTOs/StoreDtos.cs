using System.ComponentModel.DataAnnotations;

namespace MyStoreRatingsApi.DTOs
{
    public class CreateStoreDto
    {
        [Required, StringLength(60, MinimumLength = 20)]
        public string Name { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [StringLength(400)]
        public string? Address { get; set; }
        public string? OwnerId { get; set; }
    }

    public class StoreListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public double AverageRating { get; set; }
    }
}
