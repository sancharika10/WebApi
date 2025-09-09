using System.ComponentModel.DataAnnotations;

namespace MyStoreRatingsApi.DTOs
{
    public class SubmitRatingDto
    {
        [Required]
        public int StoreId { get; set; }
        [Range(1, 5)]
        public int Score { get; set; }
    }
}
