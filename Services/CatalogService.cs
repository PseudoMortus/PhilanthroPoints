
using Microsoft.EntityFrameworkCore;
using PhilanthroPoints.Data;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services;

public class CatalogService
{
    private readonly ApplicationDbContext _db;
    public CatalogService(ApplicationDbContext db) => _db = db;

    public Task<List<Item>> ByCategoryAsync(string categoryName) =>
        _db.Items.Include(i=>i.Category).Where(i=>i.Category!.Name == categoryName).ToListAsync();

    public Task<List<Item>> AllAsync() => _db.Items.Include(i=>i.Category).ToListAsync();
    public Task<List<Category>> CategoriesAsync() => _db.Categories.ToListAsync();
    
    public async Task<Item?> GetItemAsync(int id) => 
        await _db.Items.Include(i=>i.Category).FirstOrDefaultAsync(i => i.Id == id);

    public async Task<bool> AddItemAsync(Item item)
    {
        try
        {
            _db.Items.Add(item);
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateItemAsync(Item item)
    {
        try
        {
            // Find the existing item
            var existingItem = await _db.Items.FindAsync(item.Id);
            if (existingItem == null)
            {
                return false;
            }

            // Update the properties
            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.ImageUrl = item.ImageUrl;
            existingItem.Cost = item.Cost;
            existingItem.Stock = item.Stock;
            existingItem.CategoryId = item.CategoryId;

            // Save changes
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"UpdateItemAsync Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        try
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null) return false;
            
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Item>> SearchItemsAsync(string search, string? categoryFilter = null)
    {
        var query = _db.Items.Include(i => i.Category).AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => i.Name.Contains(search) || 
                                   (i.Description != null && i.Description.Contains(search)));
        }
        
        if (!string.IsNullOrWhiteSpace(categoryFilter))
        {
            query = query.Where(i => i.Category != null && i.Category.Name == categoryFilter);
        }
        
        return await query.OrderBy(i => i.Category!.Name).ThenBy(i => i.Name).ToListAsync();
    }

    public async Task<Category?> GetCategoryAsync(int id) =>
        await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Category?> GetCategoryByNameAsync(string name) =>
        await _db.Categories.FirstOrDefaultAsync(c => c.Name == name);
}
