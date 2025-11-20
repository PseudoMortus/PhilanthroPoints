using PhilanthroPoints.Models;
using PhilanthroPoints.Data;
using Microsoft.EntityFrameworkCore;

namespace PhilanthroPoints.Services;

public class CartMonitorService
{
    private readonly ApplicationDbContext _db;
    public event Action? OnStockChanged;

    public CartMonitorService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<Item>> GetLowStockItemsAsync(int threshold = 5)
    {
        return await _db.Items
            .Include(i => i.Category)
            .Where(i => i.Stock <= threshold)
            .OrderBy(i => i.Stock)
            .ToListAsync();
    }

    public async Task<List<Item>> GetOutOfStockItemsAsync()
    {
        return await _db.Items
            .Include(i => i.Category)
            .Where(i => i.Stock == 0)
            .OrderBy(i => i.Category!.Name)
            .ThenBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<Dictionary<string, int>> GetStockSummaryAsync()
    {
        var summary = await _db.Items
            .Include(i => i.Category)
            .GroupBy(i => i.Category!.Name)
            .Select(g => new { Category = g.Key, TotalStock = g.Sum(i => i.Stock) })
            .ToDictionaryAsync(x => x.Category, x => x.TotalStock);

        return summary;
    }

    public void NotifyStockChanged()
    {
        OnStockChanged?.Invoke();
    }
}