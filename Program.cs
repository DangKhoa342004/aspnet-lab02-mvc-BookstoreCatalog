using BookstoreCatalog.Mvc.Services;
using BookstoreCatalog.Mvc.Data;
using BookstoreCatalog.Mvc.Models;
using BookstoreCatalog.Mvc.Options;
using BookstoreCatalog.Mvc.Repositories;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/bookstoreCatalog-.txt", rollingInterval: RollingInterval.Day));

builder.Services.AddControllersWithViews();

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//==============================================================

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanViewBook", p => p.RequireRole("Admin", "Staff"));
    options.AddPolicy("CanManageBook", p => p.RequireRole("Admin"));
    options.AddPolicy("CanAdjustStock", p => p.RequireRole("Admin", "Staff"));
    options.AddPolicy("CanViewAuditLog", p => p.RequireRole("Admin"));
    options.AddPolicy("CanUploadBookImage", p => p.RequireRole("Admin"));
    options.AddPolicy("CanViewSale", p => p.RequireRole("Admin", "Staff", "User"));
    options.AddPolicy("CanManageSale", p => p.RequireRole("Admin", "Staff", "User"));
    options.AddPolicy("CanViewDataHealth", p => p.RequireRole("Admin"));
});

//==============================================================

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running."), tags: new[] { "live" })
    .AddDbContextCheck<AppDbContext>("database", tags: new[] { "ready" });

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
    };
});

//==============================================================

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>(); 
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

//==============================================================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await AccountDb.SeedIdentityAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.Run();