namespace PhilanthroPoints.Services;

public class CheckoutSessionState
{
    public string HeadOfHousehold { get; set; } = string.Empty;
    /* public string ContactPhone { get; set; } = string.Empty; */

    public bool HasContactInfo => 
        !string.IsNullOrWhiteSpace(HeadOfHousehold)/* && !string.IsNullOrWhiteSpace(ContactPhone)*/;

    public void SetContactInfo(string headOfHousehold/*, string phone */)
    {
        HeadOfHousehold = headOfHousehold;
        /* ContactPhone = phone; */
    }

    public void Clear()
    {
        HeadOfHousehold = string.Empty;
        /* ContactPhone = string.Empty; */
    }
}