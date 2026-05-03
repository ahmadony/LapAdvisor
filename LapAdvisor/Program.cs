using LapAdvisor.Bl;
using LapAdvisor.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region EF
// Registers the LapAdvisorDbContext with the dependency injection (DI) container.
// It tells the app to use SQL Server as the database provider,
// and it gets the connection string named "DefaultConnection" from appsettings.json.
builder.Services.AddDbContext<LapAdvisorDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Configure ASP.NET Identity with custom password rules and unique emails,
// then link Identity to the LapAdvisorDbContext so user accounts are stored in the database.

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<LapAdvisorDbContext>();
#endregion


#region Custom Services
// Registers the service classes with the dependency injection (DI) container using the Scoped lifetime.
// This means a new instance of each service will be created per HTTP request.
// Each interface (like ICategories) is linked to its implementation class (like ClsCategories),
// allowing the controllers to use the interfaces without depending directly on the concrete classes.
builder.Services.AddScoped<ICategories, ClsCategories>();
builder.Services.AddScoped<IItems, ClsItems>();
builder.Services.AddScoped<IOs, ClsOs>();
builder.Services.AddScoped<IItemTypes, ClsItemTypes>();
builder.Services.AddScoped<IItemImages, ClsItemImages>();
builder.Services.AddScoped<ISalesInvoice, ClsSalesInvoice>();
builder.Services.AddScoped<ISalesInvoiceItems, ClsSalesInvoiceItems>();
builder.Services.AddScoped<ISliders, ClsSliders>();
builder.Services.AddScoped<IPages, ClsPages>();
builder.Services.AddScoped<IWishlist, ClsWishlist>();
builder.Services.AddScoped<IFeedback, ClsFeedback>();
#endregion

#region Sesstion and cookies
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Users/AccessDenied";
    options.Cookie.Name = "Cookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(720);
    options.LoginPath = "/Users/Login";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
}); 
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ✅ لازم ييجي قبل UseRouting
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

// ✅ ترتيب المسارات (Areas أولاً)
#region Routing
app.MapControllerRoute(
name: "areas",
pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); 
#endregion

app.Run();
