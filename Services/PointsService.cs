using Microsoft.EntityFrameworkCore;
using PhilanthroPoints.Data;
using PhilanthroPoints.Models;

namespace PhilanthroPoints.Services
{
    public class PointsService
    {
        private readonly ApplicationDbContext _db;
        private readonly PointsState? _pointsState;

        public async Task<bool> RemoveMemberAsync(int memberId)
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == memberId);
            if (member == null) return false;
            _db.Members.Remove(member);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddMemberAsync(Member member)
        {
            // Check for duplicate username or email
            if (await _db.Members.AnyAsync(m => m.Username == member.Username || m.Email == member.Email))
                return false;

            _db.Members.Add(member);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMemberAsync(Member member)
        {
            var dbMember = await _db.Members.FirstOrDefaultAsync(m => m.Id == member.Id);
            if (dbMember == null) return false;
            dbMember.Username = member.Username;
            dbMember.FirstName = member.FirstName;
            dbMember.LastName = member.LastName;
            dbMember.Email = member.Email;
            dbMember.Points = member.Points;
            dbMember.Age = member.Age;
            await _db.SaveChangesAsync();
            return true;
        }

        public PointsService(ApplicationDbContext db, PointsState? pointsState = null)
        {
            _db = db;
            _pointsState = pointsState;
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            return await _db.Members.OrderBy(m => m.Username).ToListAsync();
        }

        public async Task<Member?> GetMemberAsync(int memberId)
        {
            return await _db.Members.FirstOrDefaultAsync(m => m.Id == memberId);
        }

        public async Task<bool> AddPointsAsync(int memberId, int amount)
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == memberId);
            if (member == null) return false;

            member.Points += amount;
            await _db.SaveChangesAsync();

            // Notify PointsState if this is the current user
            if (_pointsState?.CurrentUser?.Id == memberId)
            {
                await _pointsState.RefreshFromDatabaseAsync();
            }

            return true;
        }

        public async Task<bool> RemovePointsAsync(int memberId, int amount)
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == memberId);
            if (member == null) return false;

            member.Points -= amount;
            // Optional: prevent negative points
            if (member.Points < 0) member.Points = 0;

            await _db.SaveChangesAsync();

            // Notify PointsState if this is the current user
            if (_pointsState?.CurrentUser?.Id == memberId)
            {
                await _pointsState.RefreshFromDatabaseAsync();
            }

            return true;
        }

        public async Task<bool> SetPointsAsync(int memberId, int newAmount)
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.Id == memberId);
            if (member == null) return false;

            member.Points = newAmount;
            await _db.SaveChangesAsync();

            // Notify PointsState if this is the current user
            if (_pointsState?.CurrentUser?.Id == memberId)
            {
                await _pointsState.RefreshFromDatabaseAsync();
            }

            return true;
        }
    }
}
