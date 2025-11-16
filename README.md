
# PhilanthroPoints — Birthday Connections (Blazor Server, SQLite)

## How to Build, Launch & Run

1. **Build the project:**
   ```powershell
   dotnet build PhilanthroPoints.csproj
   ```

2. **Launch the application:**
   ```powershell
   dotnet run --project PhilanthroPoints.csproj
   ```

3. **Access the app:**
   - HTTPS: https://localhost:5001
   - HTTP: http://localhost:5000

## Login Credentials

### **Guest Login:**
- **Username:** `guest`
- **Password:** `Pass123!`
- **Access:** Browse and purchase items

### **Admin Login:**
- **Username:** `superadmin`
- **Password:** `SuperAdmin123!`
- **Access:** Full admin dashboard, inventory management, user management

- **Username:** `moderator` 
- **Password:** `Moderator123!`
- **Access:** Limited admin access, user management only

## Features

- **Birthday Gift Shopping:** Browse cards, treats, books, and gifts with a points-based system
- **Product Categories:** Organized into Cards, Treats, Books, and Gifts for easy navigation
- **User Management:** Separate user registration and admin user systems
- **Admin Dashboard:** Inventory management, user administration, and cart monitoring
- **Real-time Cart:** Smart cart system with automatic cleanup of abandoned items
- **Points System:** Users earn and spend points on birthday items

## Admin Access

- **Inventory Management:** Add, edit, and manage gift items
- **User Administration:** Manage user accounts and points
- **Cart Monitoring:** View active carts and abandoned items
- **Reports:** Access user and transaction data
