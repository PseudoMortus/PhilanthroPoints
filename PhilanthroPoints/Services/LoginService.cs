using PhilanthroPoints.Data;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services
{
    public class LoginService
    {
        private readonly ApplicationDbContext _db;
        public LoginService(ApplicationDbContext db) => _db = db;

        public Member? Authenticate(string username, string password)
        {
            var member = _db.Members.FirstOrDefault(m => m.Username == username);
            if (member == null) return null;
            return PasswordHasher.Verify(password, member.PasswordHash) ? member : null;
        }

        public Member Register(string username, string password, string firstName, string lastName, string email)
        {
            if (_db.Members.Any(m => m.Username == username))
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
            _db.SaveChanges();
            return user;
        }
    }
}
