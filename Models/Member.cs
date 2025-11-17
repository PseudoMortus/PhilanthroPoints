namespace PhilanthroPoints.Models;

public class Member
{
    public int Id { get; set; }

    // Login fields
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Existing
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Points { get; set; } = 0;
}