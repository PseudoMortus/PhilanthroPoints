using System;

namespace BirthdayConnections.Data
{
    public class FamilyLoginCode
    {
        public int Id { get; set; }

        // First 3 letters of agency, e.g. "ICS"
        public string AgencyPrefix { get; set; } = default!;

        // Full generated login code, e.g. ICS11250803
        public string Code { get; set; } = default!;

        // Family info

        // For monthly sequence tracking
        public int Year { get; set; }
        public int Month { get; set; }
        public int Sequence { get; set; }   // 1,2,3... up to 99 per month per agency

        // Optional: mark if code has been used already
        public bool IsUsed { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
