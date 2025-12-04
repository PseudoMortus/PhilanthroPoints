
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PhilanthroPoints.Data;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services;

public class PointsState
{
    private readonly IJSRuntime _js;
    private readonly ApplicationDbContext _db;

    public PointsState(IJSRuntime js, ApplicationDbContext db) { _js = js; _db = db; }

    public event Action? OnPointsChanged;

    public int Points { get; private set; } = 0;
    public Member? CurrentUser { get; private set; }
    public string CurrentUserDisplay => CurrentUser is null ? "Not signed in" : $"{CurrentUser.FirstName} ({Points} pts)";

    public Task InitializeAsync()
    {
        // Only initialize local state, don't call JS during prerendering
        Points = 0;
        CurrentUser = null;
        return Task.CompletedTask;
    }

    public async Task InitializeJavaScriptAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("import", "/points.js");
            var saved = await _js.InvokeAsync<LocalState>("pointsStore.load");
            Points = saved.value;
            if (!string.IsNullOrWhiteSpace(saved.name))
                CurrentUser = await _db.Members.FirstOrDefaultAsync(m => (m.FirstName + " " + m.LastName) == saved.name);
            OnPointsChanged?.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JavaScript initialization failed: {ex.Message}");
            // Fallback to default state
            Points = 0;
            CurrentUser = null;
        }
    }

    public async Task SetUserAsync(Member member) { CurrentUser = member; Points = member.Points; await PersistAsync(); OnPointsChanged?.Invoke(); }
    public async Task<bool> SpendAsync(int amount) { if (Points < amount) return false; Points -= amount; await PersistAsync(); OnPointsChanged?.Invoke(); return true; }
    public async Task AddAsync(int amount) { Points += amount; await PersistAsync(); OnPointsChanged?.Invoke(); }
    public async Task ResetAsync() { Points = 0; CurrentUser = null; await PersistAsync(); OnPointsChanged?.Invoke(); }
    
    public async Task RefreshFromDatabaseAsync()
    {
        if (CurrentUser == null) return;

        // Reload the user from the database to get the latest points value
        var refreshedUser = await _db.Members.FirstOrDefaultAsync(m => m.Id == CurrentUser.Id);
        if (refreshedUser != null)
        {
            CurrentUser = refreshedUser;
            Points = refreshedUser.Points;
            await PersistAsync();
            OnPointsChanged?.Invoke();
        }
    }

    private async Task PersistAsync()
    {
        try
        {
            var name = CurrentUser is null ? null : ($"{CurrentUser.FirstName} {CurrentUser.LastName}");
            await _js.InvokeVoidAsync("pointsStore.save", new LocalState{ value = Points, name = name });
            if(CurrentUser != null) { CurrentUser.Points = Points; await _db.SaveChangesAsync(); }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Persist failed: {ex.Message}");
            // Still update database even if JS fails
            if(CurrentUser != null) { CurrentUser.Points = Points; await _db.SaveChangesAsync(); }
        }
    }

    private class LocalState { public int value { get; set; } public string? name { get; set; } }
}
