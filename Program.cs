using EcommerceMVC.Data;
using EcommerceMVC.Helpers;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Add the database context to the services container
var myConnectionString = builder.Configuration.GetConnectionString("MyConnectString");
builder.Services.AddDbContext<Hshop2023Context>(option => option.UseSqlServer(myConnectionString));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<EcommerceMVC.Helpers.AutoMapperProfile>());

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSingleton<IVNMailService, VNMailService>();



builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/KhachHang/DangNhap";
    options.AccessDeniedPath = "/AccessDenied";
});


var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<Hshop2023Context>();
        
        // Add columns to HangHoa if they don't exist
        var sql = @"
            IF COL_LENGTH('HangHoa', 'IsBestseller') IS NULL
            BEGIN
                ALTER TABLE HangHoa ADD IsBestseller BIT NOT NULL DEFAULT 0;
            END

            IF COL_LENGTH('HangHoa', 'SortOrder') IS NULL
            BEGIN
                ALTER TABLE HangHoa ADD SortOrder INT NOT NULL DEFAULT 0;
            END
        ";
        context.Database.ExecuteSqlRaw(sql);

        // Seed Admin user
        var adminUser = context.KhachHangs.SingleOrDefault(k => k.MaKh == "diix");
        if (adminUser == null)
        {
            var randomKey = EcommerceMVC.Helpers.MyUtil.GenerateRandomKey();
            adminUser = new EcommerceMVC.Models.KhachHang
            {
                MaKh = "diix",
                MatKhau = "diix117@".ToMd5Hash(randomKey),
                HoTen = "Admin",
                GioiTinh = true,
                NgaySinh = DateTime.Now,
                DiaChi = "Admin Address",
                DienThoai = "0999999999",
                Email = "admin@diix.com",
                Hinh = "default.png",
                HieuLuc = true,
                VaiTro = 1, // Admin role
                RandomKey = randomKey
            };
            context.KhachHangs.Add(adminUser);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred seeding the DB: " + ex.Message);
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable serving static files from the "wwwroot" folder
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "trangchu",
    pattern: "trangchu",
    defaults: new { controller = "HangHoa", action = "Index" });

app.MapControllerRoute(
    name: "hanghoa",
    pattern: "hanghoa",
    defaults: new { controller = "HangHoa", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HangHoa}/{action=Index}/{id?}");

app.Run();
