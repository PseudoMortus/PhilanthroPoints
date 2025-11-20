using Microsoft.EntityFrameworkCore;
using PhilanthroPoints.Data;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services;

public class AdminUserService
{
    private readonly ApplicationDbContext _db;
    
    public AdminUserService(ApplicationDbContext db)
    {
        _db = db;
    }
    
    public async Task<List<AdminUser>> GetAllAdminUsersAsync()
    {
        return await _db.AdminUsers.OrderBy(a => a.CreatedDate).ToListAsync();
    }
    
    public async Task<AdminUser?> GetAdminUserByIdAsync(int id)
    {
        return await _db.AdminUsers.FindAsync(id);
    }
    
    public async Task<AdminUser?> GetAdminUserByUsernameAsync(string username)
    {
        return await _db.AdminUsers.FirstOrDefaultAsync(a => a.Username == username);
    }
    
    public async Task<AdminUser?> GetAdminUserByEmailAsync(string email)
    {
        return await _db.AdminUsers.FirstOrDefaultAsync(a => a.Email == email);
    }
    
    public async Task<bool> CreateAdminUserAsync(AdminUser adminUser)
    {
        try
        {
            // Check if username or email already exists
            var existingAdmin = await _db.AdminUsers
                .FirstOrDefaultAsync(a => a.Username == adminUser.Username || a.Email == adminUser.Email);
                
            if (existingAdmin != null)
            {
                return false; // Admin already exists
            }
            
            adminUser.CreatedDate = DateTime.Now;
            adminUser.LastLoginDate = DateTime.Now;
            _db.AdminUsers.Add(adminUser);
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> UpdateAdminUserAsync(AdminUser adminUser)
    {
        try
        {
            var existingAdmin = await _db.AdminUsers.FindAsync(adminUser.Id);
            if (existingAdmin == null) return false;
            
            existingAdmin.FirstName = adminUser.FirstName;
            existingAdmin.LastName = adminUser.LastName;
            existingAdmin.Email = adminUser.Email;
            existingAdmin.PhoneNumber = adminUser.PhoneNumber;
            existingAdmin.Role = adminUser.Role;
            existingAdmin.Department = adminUser.Department;
            existingAdmin.IsActive = adminUser.IsActive;
            existingAdmin.CanManageInventory = adminUser.CanManageInventory;
            existingAdmin.CanManageUsers = adminUser.CanManageUsers;
            existingAdmin.CanViewReports = adminUser.CanViewReports;
            existingAdmin.Notes = adminUser.Notes;
            
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> UpdatePasswordAsync(int adminId, string newPasswordHash)
    {
        try
        {
            var admin = await _db.AdminUsers.FindAsync(adminId);
            if (admin == null) return false;
            
            admin.PasswordHash = newPasswordHash;
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> UpdateLastLoginAsync(int adminId)
    {
        try
        {
            var admin = await _db.AdminUsers.FindAsync(adminId);
            if (admin == null) return false;
            
            admin.LastLoginDate = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> DeleteAdminUserAsync(int id)
    {
        try
        {
            var admin = await _db.AdminUsers.FindAsync(id);
            if (admin == null) return false;
            
            _db.AdminUsers.Remove(admin);
            await _db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> ValidateAdminLoginAsync(string username, string password)
    {
        try
        {
            var admin = await GetAdminUserByUsernameAsync(username);
            if (admin == null || !admin.IsActive) return false;
            
            bool isValidPassword = PasswordHasher.Verify(password, admin.PasswordHash);
            if (isValidPassword)
            {
                await UpdateLastLoginAsync(admin.Id);
            }
            
            return isValidPassword;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        return !await _db.AdminUsers.AnyAsync(a => a.Username == username);
    }
    
    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        return !await _db.AdminUsers.AnyAsync(a => a.Email == email);
    }
}