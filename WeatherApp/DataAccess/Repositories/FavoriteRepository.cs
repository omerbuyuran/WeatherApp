using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WeatherApp.Entities;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Repositories;

namespace WeatherApp.Repositories
{
    public class FavoriteRepository : BaseRepository, IFavoriteRepository
    {
        public FavoriteRepository(WeatherAppDBContext context) : base(context)
        {
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
           await context.Favorites.AddAsync(favorite);
        }

        public async Task<Favorite> GetFavoriteByIdAsync(int favoriteId)
        {
            return await context.Favorites.FindAsync(favoriteId);
        }

        public async Task<IEnumerable<Favorite>> GetFavoriteByUserIdAsync(int userId)
        {
            return await context.Favorites
                        .Where(x => x.UserId == userId)
                        .ToListAsync();
        }

        public async Task<IEnumerable<Favorite>> ListAsync()
        {
            return await context.Favorites.ToListAsync();
        }

        public async Task RemoveFavoriteAsync(int favoriteId)
        {
            //await callback mekanizması, metoddan cevap gelene kadar alt metoda geçmesini engelliyor
            var favorite = await GetFavoriteByIdAsync(favoriteId);
            context.Favorites.Remove(favorite);
        }

        public void UpdateFavorite(Favorite favorite)
        {
            context.Favorites.Update(favorite);
        }
    }
}
