
using PhilanthroPoints.Models;
using PhilanthroPoints.Data;
using Microsoft.EntityFrameworkCore;

namespace PhilanthroPoints.Services;

public class CartState
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<int, DateTime> _itemTimestamps = new();
    private readonly Timer _cleanupTimer;
    private const int CART_TIMEOUT_MINUTES = 30; // Items abandoned for 30 minutes get restocked
    
    public List<Item> Items { get; } = new();
    public event Action? OnCartChanged;

    public CartState(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        // Run cleanup every 5 minutes
        _cleanupTimer = new Timer(CleanupAbandonedItems, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }

    public async Task<bool> AddAsync(Item item) 
    { 
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Check if item has enough stock in database
            var dbItem = await db.Items.FindAsync(item.Id);
            if (dbItem == null || dbItem.Stock <= 0)
            {
                return false; // Not enough stock
            }

            // Decrease stock in database
            dbItem.Stock--;
            await db.SaveChangesAsync();

            // Add to cart with timestamp
            Items.Add(item);
            item.Stock = dbItem.Stock; // Update local stock count
            _itemTimestamps[item.Id] = DateTime.Now;
            
            OnCartChanged?.Invoke();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveAsync(Item item) 
    { 
        try
        {
            if (Items.Remove(item))
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Restore stock in database
                var dbItem = await db.Items.FindAsync(item.Id);
                if (dbItem != null)
                {
                    dbItem.Stock++;
                    await db.SaveChangesAsync();
                    item.Stock = dbItem.Stock; // Update local stock count
                }
                
                _itemTimestamps.Remove(item.Id);
                OnCartChanged?.Invoke();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task ClearAsync(bool restoreStock = true) 
    { 
        try
        {
            if (restoreStock)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Return items to inventory before clearing
                foreach (var item in Items.ToList())
                {
                    var dbItem = await db.Items.FindAsync(item.Id);
                    if (dbItem != null)
                    {
                        dbItem.Stock++;
                        item.Stock = dbItem.Stock;
                    }
                }
                await db.SaveChangesAsync();
            }
            
            Items.Clear();
            _itemTimestamps.Clear();
            // Don't trigger OnCartChanged from background operations to avoid threading issues
        }
        catch (Exception ex)
        {
            // Log error but continue
            Console.WriteLine($"Error clearing cart: {ex.Message}");
        }
    }

    public void CompleteCheckoutAsync()
    {
        // Don't restore stock - items are purchased
        Items.Clear();
        _itemTimestamps.Clear();
        // OnCartChanged will be triggered by UI component
    }

    public async Task<(bool Success, string Message)> ProcessCheckoutAsync(int userCurrentPoints)
    {
        try
        {
            var totalCost = TotalCost();
            Console.WriteLine($"[CartState] ProcessCheckoutAsync - TotalCost: {totalCost}, UserPoints: {userCurrentPoints}, Items: {Items.Count}");
            
            // Check if user has enough points
            if (userCurrentPoints < totalCost)
            {
                Console.WriteLine($"[CartState] Insufficient points - clearing cart");
                // Restore stock for all items and clear cart
                await ClearAsync(restoreStock: true);
                return (false, $"Insufficient points! You need {totalCost} points but only have {userCurrentPoints}.");
            }
            
            // Successful checkout - don't restore stock
            Console.WriteLine($"[CartState] Successful checkout - clearing {Items.Count} items");
            Items.Clear();
            _itemTimestamps.Clear();
            // Don't call OnCartChanged here - let the UI component handle the refresh
            Console.WriteLine($"[CartState] Cart cleared, items remaining: {Items.Count}");
            
            return (true, "Checkout successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CartState] Checkout error: {ex.Message}");
            // Error occurred - restore stock
            await ClearAsync(restoreStock: true);
            return (false, $"Checkout failed: {ex.Message}");
        }
    }

    public int TotalCost() => Items.Sum(i => i.Cost);

    private async void CleanupAbandonedItems(object? state)
    {
        try
        {
            var cutoffTime = DateTime.Now.AddMinutes(-CART_TIMEOUT_MINUTES);
            var abandonedItems = _itemTimestamps
                .Where(kvp => kvp.Value < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();

            if (!abandonedItems.Any()) return;

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            foreach (var itemId in abandonedItems)
            {
                var cartItem = Items.FirstOrDefault(i => i.Id == itemId);
                if (cartItem != null)
                {
                    // Restore stock for abandoned item
                    var dbItem = await db.Items.FindAsync(itemId);
                    if (dbItem != null)
                    {
                        dbItem.Stock++;
                        cartItem.Stock = dbItem.Stock;
                    }
                    
                    Items.Remove(cartItem);
                    _itemTimestamps.Remove(itemId);
                }
            }

            await db.SaveChangesAsync();
            // Don't trigger OnCartChanged from background timer to avoid threading issues
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during cart cleanup: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Dispose();
    }
}
