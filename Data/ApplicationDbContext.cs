
using Microsoft.EntityFrameworkCore;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Family> Families => Set<Family>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });
        
        // Configure AdminUser entity
        modelBuilder.Entity<AdminUser>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}

public static class Seed
{
    public static void SeedIfEmpty(ApplicationDbContext db)
    {
         Console.WriteLine("[SEED] Starting SeedIfEmpty check...");
        var categoriesExist = db.Categories.Any();
        Console.WriteLine($"[SEED] Categories exist: {categoriesExist}");

        if(!categoriesExist)
        {
            Console.WriteLine("[SEED] Seeding categories...");
            var gifts = new Category { Name = "Gifts" };
            var treats = new Category { Name = "Treats" };
            var cards  = new Category { Name = "Cards"  };
            var books = new Category { Name = "Books" };
            db.Categories.AddRange(gifts, treats, cards, books);
            db.SaveChanges();
            Console.WriteLine("[SEED] Categories saved.");

            Console.WriteLine("[SEED] Seeding items...");
            db.Items.AddRange(
                new Item{ Name="LEGO Set", Description="Starter kit", Cost=50, Stock=5, CategoryId=gifts.Id },
                new Item{ Name="Toy Car", Description="Racing car toy", Cost=25, Stock=8, CategoryId=gifts.Id },
                new Item{ Name="Chocolate Cake", Cost=30, Stock=7, CategoryId=treats.Id },
                new Item{ Name="Cupcake Dozen", Cost=18, Stock=12, CategoryId=treats.Id },
                new Item{ Name="Unicorn Card", Cost=5, Stock=30, CategoryId=cards.Id },
                new Item{ Name="Birthday Card", Description="Happy Birthday greeting", Cost=3, Stock=25, CategoryId=cards.Id },
                new Item{ Name="Adventure Stories", Description="Exciting tales for young readers", Cost=15, Stock=12, CategoryId=books.Id },
                new Item{ Name="Picture Book", Description="Colorful illustrated book", Cost=12, Stock=15, CategoryId=books.Id },
                new Item{ Name="Educational Workbook", Description="Fun learning activities", Cost=18, Stock=10, CategoryId=books.Id }
            );

            Console.WriteLine("[SEED] Seeding members...");
            db.Members.AddRange(
                new Member{ 
                    Username = "guest",
                    PasswordHash = PhilanthroPoints.Services.PasswordHasher.Hash("Pass123!"),
                    FirstName="Guest", 
                    LastName="User", 
                    Email="guest@example.com", 
                    Points=1200 
                },
                new Member{
                    Username = "admin",
                    PasswordHash = PhilanthroPoints.Services.PasswordHasher.Hash("Admin123!"),
                    FirstName = "Site",
                    LastName = "Admin",
                    Email = "admin@example.com",
                    Points = 5000
                }
            );

            Console.WriteLine("[SEED] Seeding sample users...");
            db.Users.AddRange(
                new User
                {
                    Username = "sampleuser",
                    FirstName = "Sample",
                    LastName = "User",
                    Email = "sample@example.com",
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    Points = 500,
                    ChildName = "Little One",
                    ChildAge = 5,
                    ChildGender = "Other",
                    ChildEthnicity = "Mixed"
                }
            );

            Console.WriteLine("[SEED] Seeding admin users...");
            db.AdminUsers.AddRange(
                new AdminUser
                {
                    Username = "superadmin",
                    PasswordHash = PhilanthroPoints.Services.PasswordHasher.Hash("SuperAdmin123!"),
                    FirstName = "Super",
                    LastName = "Administrator",
                    Email = "superadmin@philanthropoints.com",
                    Role = "Super Admin",
                    Department = "IT",
                    CanManageInventory = true,
                    CanManageUsers = true,
                    CanViewReports = true,
                    LastLoginDate = DateTime.Now
                },
                new AdminUser
                {
                    Username = "moderator",
                    PasswordHash = PhilanthroPoints.Services.PasswordHasher.Hash("Moderator123!"),
                    FirstName = "Site",
                    LastName = "Moderator",
                    Email = "moderator@philanthropoints.com",
                    Role = "Moderator",
                    Department = "Customer Service",
                    CanManageInventory = false,
                    CanManageUsers = true,
                    CanViewReports = false,
                    LastLoginDate = DateTime.Now
                }
            );

            Console.WriteLine("[SEED] Calling SaveChanges for items and members...");
            db.SaveChanges();
            Console.WriteLine("[SEED] Seed complete!");
        }
        else
        {
            Console.WriteLine("[SEED] Database already seeded, skipping.");
        }
        
        // Always check and seed AdminUsers separately
        var adminUsersExist = db.AdminUsers.Any();
        Console.WriteLine($"[SEED] AdminUsers exist: {adminUsersExist}");
        
        // Check if Mike's admin account exists specifically
        var mikeExists = db.AdminUsers.Any(a => a.Username == "mike");
        Console.WriteLine($"[SEED] Mike admin exists: {mikeExists}");
        
        if (!mikeExists)
        {
            Console.WriteLine("[SEED] Adding Mike admin account...");
            db.AdminUsers.Add(new AdminUser
            {
                Username = "mike",
                PasswordHash = PhilanthroPoints.Services.PasswordHasher.Hash("Mike123!"),
                FirstName = "Mike",
                LastName = "Criswell",
                Email = "mike@philanthropoints.com",
                Role = "Super Admin",
                Department = "Owner",
                CanManageInventory = true,
                CanManageUsers = true,
                CanViewReports = true,
                IsActive = true,
                CreatedDate = DateTime.Now,
                LastLoginDate = DateTime.Now
            });
            
            Console.WriteLine("[SEED] Calling SaveChanges for Mike admin...");
            db.SaveChanges();
            Console.WriteLine("[SEED] Mike admin added successfully!");
        }
        else
        {
            Console.WriteLine("[SEED] Mike admin already exists.");
        }
        
        if (!adminUsersExist)
        {
            Console.WriteLine("[SEED] Seeding other admin users...");
            db.AdminUsers.AddRange(
                new AdminUser
                {
                    Username = "superadmin",
                    PasswordHash = PhilanthroPoints.Services.PasswordHasher.Hash("SuperAdmin123!"),
                    FirstName = "Super",
                    LastName = "Administrator",
                    Email = "superadmin@philanthropoints.com",
                    Role = "Super Admin",
                    Department = "IT",
                    CanManageInventory = true,
                    CanManageUsers = true,
                    CanViewReports = true,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    LastLoginDate = DateTime.Now
                },
                new AdminUser
                {
                    Username = "moderator",
                    PasswordHash = PhilanthroPoints.Services.PasswordHasher.Hash("Moderator123!"),
                    FirstName = "Site",
                    LastName = "Moderator",
                    Email = "moderator@philanthropoints.com",
                    Role = "Moderator",
                    Department = "Customer Service",
                    CanManageInventory = false,
                    CanManageUsers = true,
                    CanViewReports = false,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    LastLoginDate = DateTime.Now
                }
            );
            
            Console.WriteLine("[SEED] Calling SaveChanges for other AdminUsers...");
            db.SaveChanges();
            Console.WriteLine("[SEED] Other AdminUsers seeded successfully!");
        }
        else
        {
            Console.WriteLine("[SEED] Other AdminUsers already exist, skipping seeding.");
        }
        
        // Always check and seed Users separately
        var usersExist = db.Users.Any();
        Console.WriteLine($"[SEED] Users exist: {usersExist}");
        
        if (!usersExist)
        {
            Console.WriteLine("[SEED] Seeding sample users (separate check)...");
            db.Users.AddRange(
                new User
                {
                    Username = "sampleuser",
                    FirstName = "Sample",
                    LastName = "User",
                    Email = "sample@example.com",
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    Points = 500,
                    ChildName = "Little One",
                    ChildAge = 5,
                    ChildGender = "Other",
                    ChildEthnicity = "Mixed"
                }
            );
            
            Console.WriteLine("[SEED] Calling SaveChanges for Users...");
            db.SaveChanges();
            Console.WriteLine("[SEED] Users seeded successfully!");
        }
        else
        {
            Console.WriteLine("[SEED] Users already exist, skipping Users seeding.");
        }
    }
}
