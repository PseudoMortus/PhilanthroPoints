using System.ComponentModel.DataAnnotations;

namespace PhilanthroPoints.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
    
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
    
    public DateTime DateOfBirth { get; set; }
    
    [StringLength(100)]
    public string? Address { get; set; }
    
    [StringLength(50)]
    public string? City { get; set; }
    
    [StringLength(10)]
    public string? ZipCode { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    public bool IsActive { get; set; } = true;
    
    // Child information for birthday planning
    [StringLength(100)]
    public string? ChildName { get; set; }
    
    public int? ChildAge { get; set; }
    
    [StringLength(20)]
    public string? ChildGender { get; set; }
    
    [StringLength(50)]
    public string? ChildEthnicity { get; set; }
    
    public int Points { get; set; } = 1000; // Starting points for new users
}