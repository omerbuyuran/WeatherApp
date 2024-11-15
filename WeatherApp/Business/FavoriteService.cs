using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Models.Responses;

namespace Business
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository favoriteRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger logger;
        public FavoriteService(IFavoriteRepository favoriteRepository, IUnitOfWork unitOfWork, ILogger logger)
        {
            this.favoriteRepository = favoriteRepository;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<FavoriteResponse> AddFavorite(Favorite favorite)
        {
            try
            {
                await favoriteRepository.AddFavoriteAsync(favorite);
                await unitOfWork.CompleteAsync();
                logger.Information("Successfully added a new favorite with ID: {FavoriteId}", favorite.Id);
                return new FavoriteResponse(favorite);
            }
            catch (Exception ex)
            {
                return new FavoriteResponse(ex.Message);
            }
        }

        public async Task<FavoriteResponse> GetFavoriteByIdAsync(int favoriteId)
        {
            try
            {
                logger.Information("Fetching favorite with ID: {FavoriteId}", favoriteId);
                Favorite favorite = await favoriteRepository.GetFavoriteByIdAsync(favoriteId);
                if (favorite == null)
                {
                    logger.Warning("Favorite with ID: {FavoriteId} not found", favoriteId);
                    return new FavoriteResponse("Favori bulunamadı");
                }
                else
                {
                    logger.Information("Successfully fetched favorite with ID: {FavoriteId}", favoriteId);
                    return new FavoriteResponse(favorite);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while fetching favorite with ID: {FavoriteId}", favoriteId);
                return new FavoriteResponse(ex.Message);
            }
        }

        public async Task<FavoriteListResponse> GetFavoriteByUserIdAsync(int userId)
        {
            try
            {
                logger.Information("Fetching favorites for User ID: {UserId}", userId);
                IEnumerable<Favorite> favorites = await favoriteRepository.GetFavoriteByUserIdAsync(userId);
                logger.Information("Successfully fetched {Count} favorites for User ID: {UserId}", favorites.Count(), userId);
                return new FavoriteListResponse(favorites);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while fetching favorites for User ID: {UserId}", userId);
                return new FavoriteListResponse(ex.Message);
            }
        }

        public async Task<FavoriteListResponse> ListAsync()
        {
            try
            {
                IEnumerable<Favorite> favorites = await favoriteRepository.ListAsync();
                logger.Information("Successfully listed {Count} favorites", favorites.Count());
                return new FavoriteListResponse(favorites);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while listing all favorites");
                return new FavoriteListResponse(ex.Message);
            }
        }

        public async Task<FavoriteResponse> RemoveFavorite(int favoriteId)
        {
            try
            {
                logger.Information("Removing favorite with ID: {FavoriteId}", favoriteId);
                Favorite favorite = await favoriteRepository.GetFavoriteByIdAsync(favoriteId);
                if (favorite == null)
                {
                    logger.Warning("Favorite with ID: {FavoriteId} not found", favoriteId);
                    return new FavoriteResponse("Favori bulunamadı");
                }
                else
                {
                    await favoriteRepository.RemoveFavoriteAsync(favoriteId);
                    await unitOfWork.CompleteAsync();
                    logger.Information("Successfully removed favorite with ID: {FavoriteId}", favoriteId);
                    return new FavoriteResponse(favorite);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while removing favorite with ID: {FavoriteId}", favoriteId);
                return new FavoriteResponse(ex.Message);
            }
        }

        public async Task<FavoriteResponse> UpdateFavorite(Favorite favorite, int favoriteId)
        {
            try
            {
                logger.Information("Updating favorite with ID: {FavoriteId}", favoriteId);
                var firstFavorite = await favoriteRepository.GetFavoriteByIdAsync(favoriteId);
                if (firstFavorite == null)
                {
                    logger.Warning("Favorite with ID: {FavoriteId} not found", favoriteId);
                    return new FavoriteResponse("Favori bulunamadı");
                }
                else
                {
                    firstFavorite.CityName = favorite.CityName;
                    firstFavorite.UserId = favorite.UserId;
                    favoriteRepository.UpdateFavorite(firstFavorite);
                    await unitOfWork.CompleteAsync();

                    logger.Information("Successfully updated favorite with ID: {FavoriteId}", favoriteId);
                    return new FavoriteResponse(firstFavorite);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while updating favorite with ID: {FavoriteId}", favoriteId);
                return new FavoriteResponse(ex.Message);
            }
        }
    }
}
