using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Hubs;
using PlateformeRHCollaborative.Web.Repositories;
using PlateformeRHCollaborative.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<ITeleworkRepository, TeleworkRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<LeaveService>();
builder.Services.AddScoped<TeleworkService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddHostedService<RoleInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Routes spécifiques pour chaque rôle - DOIT être avant la route par défaut
app.MapControllerRoute(
    name: "rh-home",
    pattern: "RH/Home",
    defaults: new { controller = "Home", action = "Index", area = "" });

app.MapControllerRoute(
    name: "manager-home",
    pattern: "Manager/Home",
    defaults: new { controller = "Home", action = "Index", area = "" });

app.MapControllerRoute(
    name: "employe-home",
    pattern: "Employe/Home",
    defaults: new { controller = "Home", action = "Index", area = "" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.MapHub<NotificationsHub>("/hubs/notifications");

app.Run();
