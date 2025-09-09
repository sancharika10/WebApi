using System.ComponentModel.DataAnnotations;

namespace MyStoreRatingsApi.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Required]
        public int StoreId { get; set; }
        public Store? Store { get; set; }

        [Required]
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
