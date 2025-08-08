using System.Text.Json.Serialization;

namespace Entities.Models
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.OnWait;
        public String? Title { get; set; }
        public String? Description { get; set; }
        public String? ApplicantFirstName { get; set; }
        public String? ApplicantLastName { get; set; }
        public String? ApplicantEmail { get; set; }
        public String? ApplicantPhoneNumber { get; set; }
        public String? CvUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public DateTime? SeenDate { get; set; }
        public DateTime? InterviewedDate { get; set; }
        public DateTime? DeniedDate { get; set; }
        public bool IsExported { get; set; } = false;
        public List<Note>? Notes { get; set; }
        public int? SectionId { get; set; }
        public Section? Section { get; set; }
    
    }

    public enum ApplicationStatus
    {
        OnWait = 0,
        Interview = 1,
        Denied = 2,
        Approved = 3,
    }
}
