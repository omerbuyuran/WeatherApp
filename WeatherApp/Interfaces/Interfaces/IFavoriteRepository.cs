using System.Threading.Tasks;
using WeatherApp.Models.Model;

namespace WeatherApp.Interfaces
{
    public interface IFavoriteRepository
    {
        //EntityFramework kullanacağımdan dolayı işlemler async olarak kullanacağım. Dönüşler bu yüzden Task
        Task<IEnumerable<Favorite>> ListAsync();
        Task AddFavoriteAsync(Favorite favorite);
        void UpdateFavorite(Favorite favorite);
        Task RemoveFavoriteAsync(int favoriteId);
        Task<Favorite> GetFavoriteByIdAsync(int favoriteId);
        Task<IEnumerable<Favorite>> GetFavoriteByUserIdAsync(int userId);

    }
}
