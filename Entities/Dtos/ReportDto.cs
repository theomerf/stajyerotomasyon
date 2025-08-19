using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record ReportDto
    {
        public int ReportId { get; init; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Rapor adı gerekli.")]
        public String? ReportTitle { get; init; }
        public String? DepartmentName { get; init; }
        public String? SectionName { get; init; }
        public String? Status { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.Now;
        public String? AccountProfilePictureUrl { get; init; }
        public String? AccountFirstName { get; init; }
        public String? AccountLastName { get; init; }
        public String? WorkName{ get; init; }
    }
}
