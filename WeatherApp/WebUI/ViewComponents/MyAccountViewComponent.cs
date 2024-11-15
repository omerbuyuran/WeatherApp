using Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Models.Model;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WebUI.Services;

namespace WebUI.ViewComponents
{
    public class MyAccountViewComponent : ViewComponent
    {
        private ILoginSession _session;
        private readonly IFavoriteService _favoriteService;

        public MyAccountViewComponent(ILoginSession session,IFavoriteService favoriteService)
        {
            _session = session;
            _favoriteService = favoriteService;
        }

        public async Task<ViewViewComponentResult> InvokeAsync()
        {
            var user = _session.GetLogin();
            if (user != null)
            {
                var favCity = await _favoriteService.GetFavoriteByUserIdAsync(user.Id);
                if (favCity.Success)
                {
                    var myAccountViewModel = new MyAccountViewModel
                    {
                        Favorite = favCity.FavoriteList.ToList(),
                        User = user,
                    };
                    return View(myAccountViewModel);
                }
                else
                {
                    return View();
                }                

            }
            return View();


        }
    }
}
