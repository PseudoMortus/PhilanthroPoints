# PhilanthroPoints — User Training (Frontend)

Purpose: A clear, short guide for parents and guardians covering the core user experience: register, earn points, browse, cart and checkout, and order history.

--

## Quick Start (local user testing)

```powershell
dotnet build PhilanthroPoints.csproj
dotnet run --project PhilanthroPoints.csproj
```

Open: `https://localhost:5001` or `http://localhost:5000`.

Note: Use sample credentials in `README.md` for training/demo accounts.

--

## Register & Login
- Register with the public registration form.
- Confirm your email if the environment requires it, or use a staged account for demos.

## Browsing & Catalog
- Use categories: `Cards`, `Treats`, `Books`, `Gifts` to find items.
- Filter and sort to narrow results by points cost or availability.

## Points: Earn & Spend
- Points are awarded by configured activities; check `Services/PointsService.cs` for logic.
- Your points balance appears in your profile or user menu.

## Cart & Checkout
- Add items to cart; points required display on item tiles and in the cart.
- Proceed to checkout and confirm the order.
- Note: The cart auto-cleans stale holds — complete checkout promptly to avoid losing items.

## Order History
- View past orders and status in your profile page.

## Common Issues (user-facing)
- Forgot password: use the password reset flow or contact an admin.
- Missing points: provide details to support (transaction time, activity) to investigate.

--

## Quick Reference
- Cart page: `Pages/Parent/Cart.razor`
- Points logic: `Services/PointsService.cs`

--

If you want step-by-step screenshots or a printable quick-start for parents, I can add them.

Generated: November 20, 2025
