using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using todolist_server.Data;
using todolist_server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToDoListDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<Email>();
builder.Services.AddTransient<JwtToken>();

//signIn manager no need to add extra coding in Services.AddAuthenication() 
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ToDoListDbContext>() //used to create users and validate password
    .AddDefaultTokenProviders(); //used for password reset, email change and two factor authentication

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; //prevent cross-site scripting that cookie cannot be accessed in .js
    options.LoginPath = "/Web/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(3);

    options.Cookie.MaxAge = options.ExpireTimeSpan; //persistent cookie that will not be deleted after closing broswer
    options.SlidingExpiration = true; //reissue a new cookie on every login
});

//solve the issue persistent cookie is still valid but redirect user to login page
//https://stackoverflow.com/questions/46318461/asp-net-core-remember-me-persistent-cookie-not-works-after-deploy
var environment = builder.Services.BuildServiceProvider().GetRequiredService<IWebHostEnvironment>();
builder.Services.AddDataProtection()
    .SetApplicationName($"my-app-{environment.EnvironmentName}")
    .PersistKeysToFileSystem(new DirectoryInfo($@"{environment.ContentRootPath}\keys"));

//set the requirement of the password and login process
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
});

builder.Services.Configure<DataProtectionTokenProviderOptions>
    (options => options.TokenLifespan = TimeSpan.FromMinutes(30)
);
//only valid for GeneratePasswordResetTokenAsync that use DataProtectorTokenProvider
//PhoneNumberTokenProvider use TotpSecurityStampBasedTokenProvider with 3 minutes of expiration time and variance of 9 minutes.

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(20);
});

//implement the use of jwtToken for authenication in controller and signalr
builder.Services.AddAuthentication()
    .AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateLifetime = false,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ChatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });



var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //for the use of jwt token
app.UseAuthorization();

app.MapControllerRoute("Default", "{controller=Web}/{action=Task}");
app.MapHub<ChatHub>("/chatHub");

app.Run();

