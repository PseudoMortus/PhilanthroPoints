
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
                CurrentUser = _db.Members.FirstOrDefault(m => (m.FirstName + " " + m.LastName) == saved.name);
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

    public void SetUser(Member member) { CurrentUser = member; Points = member.Points; Persist(); OnPointsChanged?.Invoke(); }
    public bool Spend(int amount) { if (Points < amount) return false; Points -= amount; Persist(); OnPointsChanged?.Invoke(); return true; }
    public void Add(int amount) { Points += amount; Persist(); OnPointsChanged?.Invoke(); }
    public void Reset() { Points = 0; CurrentUser = null; Persist(); OnPointsChanged?.Invoke(); }
    
    public void RefreshFromDatabase()
    {
        if (CurrentUser == null) return;
        
        // Reload the user from the database to get the latest points value
        var refreshedUser = _db.Members.FirstOrDefault(m => m.Id == CurrentUser.Id);
        if (refreshedUser != null)
        {
            CurrentUser = refreshedUser;
            Points = refreshedUser.Points;
            Persist();
            OnPointsChanged?.Invoke();
        }
    }

    private void Persist()
    {
        try
        {
            var name = CurrentUser is null ? null : ($"{CurrentUser.FirstName} {CurrentUser.LastName}");
            _js.InvokeVoidAsync("pointsStore.save", new LocalState{ value = Points, name = name });
            if(CurrentUser != null) { CurrentUser.Points = Points; _db.SaveChanges(); }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Persist failed: {ex.Message}");
            // Still update database even if JS fails
            if(CurrentUser != null) { CurrentUser.Points = Points; _db.SaveChanges(); }
        }
    }

    private class LocalState { public int value { get; set; } public string? name { get; set; } }
}
