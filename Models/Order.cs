using System.ComponentModel.DataAnnotations;

namespace PhilanthroPoints.Models;

public class Order
{
    public int Id { get; set; }
    
    [Required]
    public string HeadOfHousehold { get; set; } = string.Empty;
    
    [Required]
    [Phone]
    public string ContactPhone { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
    
    public DateTime OrderDate { get; set; } = DateTime.Now;
    
    public int TotalCost { get; set; }
    
    public int TotalItems { get; set; }
    
    public string ItemsSummary { get; set; } = string.Empty; // JSON or comma-separated list of items
    
    public bool EmailSent { get; set; } = false;
    
    public bool SmsSent { get; set; } = false;
    
    // Navigation properties
    public List<OrderItem> OrderItems { get; set; } = new();
}

public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    
    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;
    
    public string ItemName { get; set; } = string.Empty; // Store name at time of order
    
    public int ItemCost { get; set; } // Store cost at time of order
    
    public int Quantity { get; set; } = 1;
}