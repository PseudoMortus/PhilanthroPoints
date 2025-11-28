using PhilanthroPoints.Data;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services
{
    public class PointsService
    {
        private readonly ApplicationDbContext _db;
        private readonly PointsState? _pointsState;

        public bool RemoveMember(int memberId)
        {
            var member = _db.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null) return false;
            _db.Members.Remove(member);
            _db.SaveChanges();
            return true;
        }
        public bool AddMember(Member member)
        {
            // Check for duplicate username or email
            if (_db.Members.Any(m => m.Username == member.Username || m.Email == member.Email))
                return false;

            _db.Members.Add(member);
            _db.SaveChanges();
            return true;
        }

        public bool UpdateMember(Member member)
        {
            var dbMember = _db.Members.FirstOrDefault(m => m.Id == member.Id);
            if (dbMember == null) return false;
            dbMember.Username = member.Username;
            dbMember.FirstName = member.FirstName;
            dbMember.LastName = member.LastName;
            dbMember.Email = member.Email;
            dbMember.Points = member.Points;
            dbMember.Age = member.Age;
            _db.SaveChanges();
            return true;
        }

        public PointsService(ApplicationDbContext db, PointsState? pointsState = null)
        {
            _db = db;
            _pointsState = pointsState;
        }

        public List<Member> GetAllMembers()
        {
            return _db.Members.OrderBy(m => m.Username).ToList();
        }

        public Member? GetMember(int memberId)
        {
            return _db.Members.FirstOrDefault(m => m.Id == memberId);
        }

        public bool AddPoints(int memberId, int amount)
        {
            var member = _db.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null) return false;
            
            member.Points += amount;
            _db.SaveChanges();
            
            // Notify PointsState if this is the current user
            if (_pointsState?.CurrentUser?.Id == memberId)
            {
                _pointsState.RefreshFromDatabase();
            }
            
            return true;
        }

        public bool RemovePoints(int memberId, int amount)
        {
            var member = _db.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null) return false;
            
            member.Points -= amount;
            // Optional: prevent negative points
            if (member.Points < 0) member.Points = 0;
            
            _db.SaveChanges();
            
            // Notify PointsState if this is the current user
            if (_pointsState?.CurrentUser?.Id == memberId)
            {
                _pointsState.RefreshFromDatabase();
            }
            
            return true;
        }

        public bool SetPoints(int memberId, int newAmount)
        {
            var member = _db.Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null) return false;
            
            member.Points = newAmount;
            _db.SaveChanges();
            
            // Notify PointsState if this is the current user
            if (_pointsState?.CurrentUser?.Id == memberId)
            {
                _pointsState.RefreshFromDatabase();
            }
            
            return true;
        }
    }
}
