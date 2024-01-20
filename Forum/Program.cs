using System.Configuration;
using Forum.Models;
using Forum.Repositorys;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Forum.Tools;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using Forum.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<JwtSecurityTokenHandler>();
builder.Services.AddScoped<IPasswordSecurity, PassWordSecurityService>();
builder.Services.AddScoped<MailMessage>();
builder.Services.AddScoped<IMailService, MailService>();
    var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection");

builder.Services.AddDbContext<UserRepository>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ThemeRepository>(options => 
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<PostsRepositroy>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<MessagesRepository>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<PasswordTokenRepository>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<LikesRepository>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:Secret"));

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:5109/",
            ValidAudience = "http://localhost:5109/",
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMvc();
app.UseSession();
app.Use(async (context, next) =>
{
    var token = context.Session.GetString("Token");
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + token);
    }
    await next();
});
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Security}/{action=Index}");

app.Run();
