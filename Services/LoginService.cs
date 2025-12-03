using Microsoft.EntityFrameworkCore;
using PhilanthroPoints.Data;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services
{
    public class LoginService
    {
        private readonly ApplicationDbContext _db;
        public LoginService(ApplicationDbContext db) => _db = db;

        public async Task<Member?> AuthenticateAsync(string username, string password)
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Username == username);
            if (member == null) return null;
            return PasswordHasher.Verify(password, member.PasswordHash) ? member : null;
        }

        public async Task<Member> RegisterAsync(string username, string password, string firstName, string lastName, string email)
        {
            if (await _db.Members.AnyAsync(m => m.Username == username))
                throw new Exception("Username already exists");

            var user = new Member
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Points = 100
            };
            _db.Members.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}
