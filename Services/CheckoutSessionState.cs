namespace PhilanthroPoints.Services;

public class CheckoutSessionState
{
    public string HeadOfHousehold { get; set; } = string.Empty;
    
    public bool HasContactInfo => 
        !string.IsNullOrWhiteSpace(HeadOfHousehold);
    
    public void SetContactInfo(string headOfHousehold)
    {
        HeadOfHousehold = headOfHousehold;
    }
    
    public void Clear()
    {
        HeadOfHousehold = string.Empty;
    }
}