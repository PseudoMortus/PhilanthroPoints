using Microsoft.EntityFrameworkCore;
using PhilanthroPoints.Data;
using PhilanthroPoints.Services;
using PhilanthroPoints.Components;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
