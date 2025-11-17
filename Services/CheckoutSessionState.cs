namespace PhilanthroPoints.Services;

public class CheckoutSessionState
{
    public string HeadOfHousehold { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    
    public bool HasContactInfo => 
        !string.IsNullOrWhiteSpace(HeadOfHousehold) && 
        !string.IsNullOrWhiteSpace(ContactPhone) && 
        !string.IsNullOrWhiteSpace(ContactEmail);
    
    public void SetContactInfo(string headOfHousehold, string phone, string email)
    {
        HeadOfHousehold = headOfHousehold;
        ContactPhone = phone;
        ContactEmail = email;
    }
    
    public void Clear()
    {
        HeadOfHousehold = string.Empty;
        ContactPhone = string.Empty;
        ContactEmail = string.Empty;
    }
}