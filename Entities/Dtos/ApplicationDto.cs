using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record ApplicationDto
    {
        public int ApplicationId { get; init; }
        public String? Status { get; init; }
        public String? Title { get; init; }
        public String? Description { get; init; }
        public String? ApplicantFirstName { get; init; }
        public String? ApplicantLastName { get; init; }
        public String? ApplicantPhoneNumber { get; init; }
        public String? CvUrl { get; init; }
        public String? ApplicantEmail { get; init; }
        public DateTime CreatedDate { get; init; } = DateTime.Now;
        public DateTime UpdatedDate { get; init; } = DateTime.Now;
        public DateTime? SeenDate { get; init; }
        public DateTime? InterviewedDate { get; init; }
        public DateTime? DeniedDate { get; init; }
        public List<Note>? Notes { get; init; }
        public bool IsExported { get; init; } = false;
        public int DepartmentId { get; init; }
        public String? DepartmentName { get; init; }
        public int? SectionId { get; init; }
        public String? SectionName { get; init; }
    }
}
