public static class ShoppingSession
{
    public static string ActiveCode { get; set; }
    public static string Agency { get; set; }

    public static void Clear()
    {
        ActiveCode = null;
        Agency = null;
    }
}
