var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<PhilanthroPoints.Services.CatalogService>();
builder.Services.AddSingleton<PhilanthroPoints.Services.CartState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<PhilanthroPoints.App>()
    .AddInteractiveServerRenderMode();

app.Run();