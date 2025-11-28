using System;

namespace PhilanthroPoints.Models
{
    public class FamilySurveyResponse
    {
        public int Id { get; set; }


        public string Satisfaction { get; set; } = string.Empty;
        public string ChildReaction { get; set; } = string.Empty;
        public string LikelihoodToRecommend { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;

        public string SubmittedBy { get; set; } = "Anonymous";

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
