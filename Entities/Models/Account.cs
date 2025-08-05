using Microsoft.AspNetCore.Identity;

namespace Entities.Models
{
    public class Account : IdentityUser
    {
        public String? FirstName { get; set; }
        public String? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? BirthDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime InternshipStartDate { get; set; } = DateTime.MinValue;
        public DateTime InternshipEndDate { get; set; } = DateTime.MinValue;
        public String? ProfilePictureUrl { get; set; }
        public int? SectionId { get; set; }
        public Section? Section { get; set; }
        public List<Note?>? Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
