
using Microsoft.EntityFrameworkCore;
using PhilanthroPoints.Data;
using PhilanthroPoints.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure antiforgery for Blazor
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

// Authentication state provider for simple DB-backed login
builder.Services.AddAuthorizationCore();
// Register CustomAuthStateProvider as a singleton instance for both interfaces
builder.Services.AddScoped<PhilanthroPoints.Services.CustomAuthStateProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(
    sp => sp.GetRequiredService<PhilanthroPoints.Services.CustomAuthStateProvider>());

var conn = builder.Configuration.GetConnectionString("Default") ?? "Data Source=philanthro.db";
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(conn));

builder.Services.AddScoped<PointsState>();
builder.Services.AddSingleton<CartState>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<CartMonitorService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AdminUserService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<FlowState>();
builder.Services.AddScoped<PointsService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<CheckoutSessionState>();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    Seed.SeedIfEmpty(db);
    // Log member count for debugging
    var memberCount = db.Members.Count();
    Console.WriteLine($"[SEED] Total members in database: {memberCount}");
    if (memberCount > 0) {
        foreach (var m in db.Members) {
            Console.WriteLine($"[SEED] Member: {m.Username} (ID: {m.Id})");
        }
    }
    
    // Log admin users for debugging  
    var adminCount = db.AdminUsers.Count();
    Console.WriteLine($"[SEED] Total admin users in database: {adminCount}");
    if (adminCount > 0) {
        foreach (var a in db.AdminUsers) {
            Console.WriteLine($"[SEED] Admin: {a.Username} (ID: {a.Id}, IsActive: {a.IsActive})");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
