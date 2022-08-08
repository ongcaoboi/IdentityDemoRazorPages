using IdentityDemo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using webapp_examble.Mail;

var builder = WebApplication.CreateBuilder(args);

string connectstring = builder.Configuration.GetConnectionString("IdentityDemoDbContext");
builder.Services.AddDbContext<IdentityDemoDbContext>(options => options.UseSqlServer(connectstring));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityDemoDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập cấu hình mặc định cho password
    options.Password.RequireDigit = false; // Bắt phải có số
    options.Password.RequiredLength = 3; // Độ dài tối thiểu
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt
    options.Password.RequireUppercase = false; // Ký tự viết hoa
    options.Password.RequireNonAlphanumeric = false; // Ký tự đặc biệt
    options.Password.RequireLowercase = false; // Ký tự thường

    // Cấu hình khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5p
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần là khóa
    options.Lockout.AllowedForNewUsers = true; 

    // Cấu hình về user
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true; // Email phải là duy nhất

    //Cấu hình đăng nhập
    options.SignIn.RequireConfirmedEmail = true; // Phải xác thực email
    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true; // Phải xác nhận tk mới được đăng nhập
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Login";
    options.LogoutPath = "/Logout";
    options.AccessDeniedPath = "/AccessDeniedModel";
    options.SlidingExpiration = true;
});

var mailSettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);

builder.Services.AddTransient<IEmailSender, SendMailService>();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();

app.Run();
