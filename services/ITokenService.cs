using MyStoreRatingsApi.Models;

namespace MyStoreRatingsApi.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
