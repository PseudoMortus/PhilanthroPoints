# PhilanthroPoints — Admin Training (Frontend & Backend)

Purpose: Detailed, role-focused instructions for administrators covering both the web UI (frontend) and operational/developer tasks (backend).

--

## Quick Start (local)

```powershell
dotnet build PhilanthroPoints.csproj
dotnet run --project PhilanthroPoints.csproj
```

Open: `https://localhost:5001` or `http://localhost:5000`.

--

## Frontend (UI) — daily admin workflows

- Sign in & verify permissions
  - Use an admin account (see `README.md` sample credentials for training).
  - Confirm admin menu items are visible after login.

- Inventory management
  - Path: `Admin -> Inventory`.
  - Add: `New` → fill `Title`, `Category`, `Points`, `Quantity`, `Description`, upload image (optional) → set `Active` → Save.
  - Edit: find item → `Edit` → update fields → Save → confirm in public catalog.

- Users management
  - Path: `Admin -> Users`.
  - View profile: check points, order history, and active carts.
  - Adjust points: use the Points control; always add an audit note.
  - Lock/unlock or deactivate accounts as needed.

- Orders & fulfillment
  - Path: `Admin -> Orders`.
  - Review order details and update status (Processing → Shipped/Completed).
  - Add fulfillment notes and confirm customer notifications.

- Cart monitoring
  - Path: `Admin -> Cart Monitor`.
  - Identify abandoned carts and contact users or clear stale entries.

- Notifications
  - Compose in `Admin -> Notifications` and test in staging.

Frontend Admin Checklist (daily)
- Review pending orders and any low-stock alerts.
- Inspect the cart monitor and recent activity.
- Spot-check the public store for visibility/typos.

--

## Backend (operations & developer-facing)

- Configuration & environment
  - Files: `appsettings.json`, `appsettings.Development.json`.
  - Confirm DB path/connection for SQLite in local dev.

- Database & migrations
  - DbContext: `Data/ApplicationDbContext.cs`.
  - Create/apply migrations when changing models:

```powershell
dotnet ef migrations add <Name> ; dotnet ef database update
```

- Key services
  - `Services/AdminUserService.cs` — admin account utilities
  - `Services/PointsService.cs` — earning/spending logic
  - `Services/CartMonitorService.cs` — background cleanup and notifications

- Background jobs & monitoring
  - Ensure hosted services run in production; check logs for exceptions.

- Backups & recovery
  - For SQLite, copy the DB file to a secure backup location; follow hosting provider guidance in production.

Backend Checklist (weekly)
- Review logs for errors and exception spikes.
- Validate background job runs and scheduled tasks.
- Run migration tests in staging after model changes.

--

## Troubleshooting (admin-focused)

- App won't start:
  - Ports busy: stop conflicting processes or change ports.
  - Check console output for startup exceptions.

- DB/migration problems:
  - Confirm DB file exists and migrations are up to date.

- Auth/permission issues:
  - Inspect `AdminUser` table and `CustomAuthStateProvider.cs`.

- Background services failing:
  - Check `CartMonitorService` and hosted service logs; restart service if needed.

--

## Files & commands (admin/dev quick reference)
- Build: `dotnet build PhilanthroPoints.csproj`
- Run: `dotnet run --project PhilanthroPoints.csproj`
- DbContext: `Data/ApplicationDbContext.cs`
- Services: `Services/AdminUserService.cs`, `Services/PointsService.cs`, `Services/CartMonitorService.cs`
- Admin pages: `Pages/Admin/Inventory.razor`, `Pages/Admin/Orders.razor`

--

If you'd like, I can add screenshots or step-by-step UI screenshots to this doc.

Generated: November 20, 2025
