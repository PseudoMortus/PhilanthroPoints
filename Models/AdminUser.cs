using System.ComponentModel.DataAnnotations;

namespace PhilanthroPoints.Models;

public class AdminUser
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [StringLength(15)]
    public string? PhoneNumber { get; set; }
    
    [StringLength(100)]
    public string Role { get; set; } = "Admin";
    
    [StringLength(200)]
    public string? Department { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public DateTime LastLoginDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public bool CanManageInventory { get; set; } = true;
    
    public bool CanManageUsers { get; set; } = true;
    
    public bool CanViewReports { get; set; } = true;
    
    [StringLength(500)]
    public string? Notes { get; set; }
}