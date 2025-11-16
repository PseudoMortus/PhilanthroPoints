using System.ComponentModel.DataAnnotations;

namespace PhilanthroPoints.Models;

public class Family
{
    public int Id { get; set; }
    
    // Basic Family Information
    [Required]
    [Display(Name = "Family Name")]
    public string FamilyName { get; set; } = string.Empty;
    
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;
    
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;
    
    [Display(Name = "State")]
    public string State { get; set; } = string.Empty;
    
    [Display(Name = "ZIP Code")]
    public string Zip { get; set; } = string.Empty;
    
    // Demographics Info
    [Display(Name = "Child's Birthday")]
    [DataType(DataType.Date)]
    public DateTime? Birthday { get; set; }
    
    [Display(Name = "Child's Age")]
    public int? Age { get; set; }
    
    [Display(Name = "Impact this gift will have")]
    [StringLength(1000)]
    public string ExpectedImpact { get; set; } = string.Empty;
    
    [Display(Name = "Response you anticipate the child having")]
    [StringLength(1000)]
    public string AnticipatedChildResponse { get; set; } = string.Empty;
    
    [Display(Name = "Willingness to take a survey")]
    public bool WillingToTakeSurvey { get; set; }
    
    // Dietary Needs
    [Display(Name = "Gluten Free")]
    public bool GlutenFree { get; set; }
    
    [Display(Name = "Egg Free")]
    public bool EggFree { get; set; }
    
    [Display(Name = "Lactose Free")]
    public bool LactoseFree { get; set; }
    
    [Display(Name = "Nut Free")]
    public bool NutFree { get; set; }
    
    [Display(Name = "Other Dietary Restrictions")]
    [StringLength(500)]
    public string OtherDietaryRestrictions { get; set; } = string.Empty;
    
    // Family Demographics
    [Display(Name = "Ethnicity")]
    [StringLength(100)]
    public string Ethnicity { get; set; } = string.Empty;
    
    [Display(Name = "Head of Household")]
    [StringLength(200)]
    public string HeadOfHousehold { get; set; } = string.Empty;
    
    [Display(Name = "Preferred Gift Types")]
    [StringLength(1000)]
    public string PreferredGifts { get; set; } = string.Empty;
    
    // Additional Information
    [Display(Name = "Special Notes")]
    [StringLength(2000)]
    public string SpecialNotes { get; set; } = string.Empty;
    
    [Display(Name = "Contact Phone")]
    [Phone]
    public string ContactPhone { get; set; } = string.Empty;
    
    [Display(Name = "Contact Email")]
    [EmailAddress]
    public string ContactEmail { get; set; } = string.Empty;
}