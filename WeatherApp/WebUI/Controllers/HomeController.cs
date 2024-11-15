using AutoMapper;
using Interfaces.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Model;
using Models.Request;
using System.Diagnostics;
using WeatherApp.Interfaces;
using WeatherApp.Models.Model;
using WeatherApp.Models.Request;
using WeatherApp.Models.Responses;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IFavoriteService _favoriteService;
        private readonly IMapper _mapper;
        private readonly ILoginSession _loginSession;
        private readonly IWeatherService _weatherService;
        private readonly IRedisService _redisService;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IFavoriteService favoriteService, IMapper mapper, ILoginSession loginSession, IWeatherService weatherService, IRedisService redisService)
        {
            _logger = logger;
            _userService = userService;
            _favoriteService = favoriteService;
            _mapper = mapper;
            _loginSession = loginSession;
            _weatherService = weatherService;
            _redisService = redisService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            //UserRequest userRequest = new UserRequest();
            //userRequest.Name = "Elif";
            //userRequest.Surname = "Yesil Buyuran";
            //userRequest.Type = "1";
            //User user = _mapper.Map<UserRequest, User>(userRequest);
            //await _userService.AddUser(user);
            var list = await _userService.ListAsync();
            UserListViewModel model = new UserListViewModel
            {
                Users = list.UserList.ToList()
            };
            return View(model);
        }

        public async Task<IActionResult> PrivacyAsync()
        {
            var user = _loginSession.GetLogin();
            if(user != null) 
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

        public async Task<IActionResult> DeleteFavorite(int favoriteId)
        {
            var user = _loginSession.GetLogin();
            if (user != null)
            {
                var favResult = await _favoriteService.RemoveFavorite(favoriteId);
                if (favResult.Success)
                {
                    var favCity = await _favoriteService.ListAsync();
                    var myAccountViewModel = new MyAccountViewModel
                    {
                        Favorite = favCity.FavoriteList.ToList(),
                    };
                    TempData.Add("message", String.Format("Successfully deleted city"));
                    return RedirectToAction("Privacy", "Home");
                }
                else
                {
                    TempData.Add("message", String.Format("Failed to deleted"));
                    return RedirectToAction("Privacy", "Home");
                }

            }

            return View();
        }

        public async Task<IActionResult> AddToFavorite(string cityName)
        {
            var user = _loginSession.GetLogin();
            if (user != null)
            {
                Favorite favorite = new Favorite
                {
                    CityName = cityName,
                    UserId = user.Id
                };
                var favResult = await _favoriteService.AddFavorite(favorite);
                if (favResult.Success)
                {
                    var favCity = await _favoriteService.ListAsync();
                    var myAccountViewModel = new MyAccountViewModel
                    {
                        Favorite = favCity.FavoriteList.ToList(),
                    };
                    TempData.Add("message", String.Format("Successfully added favorite for {0}", cityName));
                    return RedirectToAction("Privacy", "Home");
                }
                else
                {
                    TempData.Add("message", String.Format("Failed to add favorite for {0}", cityName));
                    return RedirectToAction("Privacy", "Home");
                }

            }

            return View();
        }

        public async Task<IActionResult> LoginAsync(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            //var login = _loginSession.GetLogin();
            _loginSession.SetLogin(user.User);

            TempData.Add("message", String.Format("Login successfully for {0}", user.User.Name));
            return RedirectToAction("Privacy", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Complete(RequestApiViewModel req)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var requestApiViewModel = new RequestApiViewModel
            {
                Request = req.Request
            };

            var temp = await _redisService.GetValueAsync(req.Request.q.ToLowerInvariant());
            
            if (String.IsNullOrEmpty(temp))
            {
                var averageTemp = await _weatherService.GetAverageWeatherFromApisAsync(req.Request);
                if(averageTemp != null && averageTemp.Success && averageTemp.WeatherInfo != null)
                {
                    await _redisService.SetValueAsync(req.Request.q.ToLowerInvariant(), averageTemp.WeatherInfo.Temperature.ToString());
                }

                var completeViewModel = new CompleteViewModel
                {
                    WeatherResponse = averageTemp
                };

                return View(completeViewModel);
            }
            else
            {
                WeatherInfo weatherInfo = new WeatherInfo
                {
                    City = req.Request.q,
                    Date = DateTime.Now,
                    Temperature = Convert.ToDecimal(temp),
                };
                WeatherResponse response = new WeatherResponse(weatherInfo);

                var completeViewModel = new CompleteViewModel
                {
                    WeatherResponse = response
                };


                return View(completeViewModel);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}