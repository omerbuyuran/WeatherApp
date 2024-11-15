using Business;
using Interfaces.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;
using WeatherApp.Business;
using WeatherApp.Entities;
using WeatherApp.Interfaces;
using WeatherApp.Models.UnitOfWork;
using WeatherApp.Repositories;
using WebUI.Middlewares;
using WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(Log.Logger);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDbContext<WeatherAppDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnectionString"]);
});
// Add services to the container.
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IFavoriteService, FavoriteService>();
builder.Services.AddTransient<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddTransient<IWeatherService, WeatherService>();
builder.Services.AddTransient<IWeatherRepository, WeatherRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ILoginSession, LoginSession>();
builder.Services.AddTransient<ILoginService, LoginService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<WeatherAPIClient.Model.APIConfig>(builder.Configuration.GetSection("APIConfig"));
builder.Services.Configure<WeatherStackClient.Model.StackConfig>(builder.Configuration.GetSection("StackConfig"));
builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpClient<WeatherAPIClient.Service.IAPIService, WeatherAPIClient.Service.APIService>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    var config = serviceProvider.GetRequiredService<IOptions<WeatherAPIClient.Model.APIConfig>>().Value;
                    client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
                });

builder.Services.AddHttpClient<WeatherStackClient.Service.IStackService, WeatherStackClient.Service.StackService>()
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    var config = serviceProvider.GetRequiredService<IOptions<WeatherStackClient.Model.StackConfig>>().Value;
                    client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
                });

// IAPIService ve APIService'ý ekle
builder.Services.AddScoped<WeatherAPIClient.Service.IAPIService, WeatherAPIClient.Service.APIService>();
builder.Services.AddScoped<WeatherStackClient.Service.IStackService, WeatherStackClient.Service.StackService>();

// Redis baðlantýsýný yapýlandýrma
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379,abortConnect=false", true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

var app = builder.Build();

app.UseSerilogRequestLogging(); // HTTP istek loglarýný Serilog ile kaydetmek için

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseFileServer();
app.UseNodeModules(app.Environment.ContentRootPath);
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
Log.CloseAndFlush();