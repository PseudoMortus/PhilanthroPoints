using Microsoft.EntityFrameworkCore;
using PhilanthroPoints.Data;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services;

public class UserService
{
    private readonly ApplicationDbContext _db;
    
    public UserService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _db.Users.OrderBy(u => u.CreatedDate).ToListAsync();
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<bool> CreateUserAsync(User user)
    {
        try
        {
            // Check if username or email already exists
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);
                
            if (existingUser != null)
            {
                return false; // User already exists
            }
            
            user.CreatedDate = DateTime.Now;
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> UpdateUserAsync(User user)
    {
        try
        {
            var existingUser = await _db.Users.FindAsync(user.Id);
            if (existingUser == null) return false;
            
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.DateOfBirth = user.DateOfBirth;
            existingUser.Address = user.Address;
            existingUser.City = user.City;
            existingUser.ZipCode = user.ZipCode;
            existingUser.IsActive = user.IsActive;
            existingUser.ChildName = user.ChildName;
            existingUser.ChildAge = user.ChildAge;
            existingUser.ChildGender = user.ChildGender;
            existingUser.ChildEthnicity = user.ChildEthnicity;
            
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return false;
            
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> UpdateUserPointsAsync(int userId, int points)
    {
        try
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;
            
            user.Points = points;
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> AddPointsAsync(int userId, int pointsToAdd)
    {
        try
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return false;
            
            user.Points += pointsToAdd;
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        return !await _db.Users.AnyAsync(u => u.Username == username);
    }
    
    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        return !await _db.Users.AnyAsync(u => u.Email == email);
    }
}