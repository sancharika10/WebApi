using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStoreRatingsApi.Models
{
    public class Store
    {
        public int Id { get; set; }

        [Required, StringLength(60, MinimumLength = 20)]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [StringLength(400)]
        public string? Address { get; set; }

        // Owner: optional link to ApplicationUser (Store Owner)
        public string? OwnerId { get; set; }
        public ApplicationUser? Owner { get; set; }

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        [NotMapped]
        public double AverageRating => Ratings.Any() ? Math.Round(Ratings.Average(r => r.Score), 2) : 0d;
    }
}
