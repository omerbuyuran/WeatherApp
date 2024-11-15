using WeatherApp.Models.Responses;
using WeatherApp.Models.Model;

namespace WeatherApp.Interfaces
{
    public interface IFavoriteService
    {
        Task<FavoriteListResponse> ListAsync();
        Task<FavoriteResponse> AddFavorite(Favorite favorite);
        Task<FavoriteResponse> UpdateFavorite(Favorite favorite,int favoriteId);
        Task<FavoriteResponse> RemoveFavorite(int favoriteId);
        Task<FavoriteResponse> GetFavoriteByIdAsync(int favoriteId);
        Task<FavoriteListResponse> GetFavoriteByUserIdAsync(int userId);
    }
}
