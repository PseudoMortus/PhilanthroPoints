using System;
using System.ComponentModel.DataAnnotations;

public class AccessCode
{
    [Key]
    public int Id { get; set; }
    public string AgencyAbbrev { get; set; }
    public string Code { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsUsed { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
