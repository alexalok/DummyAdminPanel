using Blazored.LocalStorage;
using DummyAdminPanel;
using DummyAdminPanel.Repositories.Posts;
using DummyAdminPanel.Repositories.Users;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var config = builder.Configuration;

services.AddOptions<AppSettings>().Bind(config);

services.AddRazorPages();
services.AddServerSideBlazor();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.ExpireTimeSpan = TimeSpan.FromDays(1);
        opt.SlidingExpiration = true;
    });

services.AddStaticAuthenticationService();
services.AddDummyApiConnector(config.GetSection("DummyApi"));
services.AddDummyApiLocalStorageConnector();

services.AddTransient<IUsersRepository, CachingUsersRepository>();
services.AddTransient<IPostsRepository, CachingPostsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

