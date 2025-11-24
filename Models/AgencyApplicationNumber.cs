using System;
using System.ComponentModel.DataAnnotations;

namespace PhilanthroPoints.Models
{
    public class AgencyApplicationNumber
    {
        [Key]
        public int Id { get; set; }
        public string AgencyAbbrev { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Month { get; set; }
        public int LastNumber { get; set; }
    }
}
