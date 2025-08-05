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
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Rapor içeriği gerekli.")]
        public String? ReportContent { get; init; }
        public String? Status { get; init; }
        public List<String>? ImageUrls { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.Now;
        public Account? Account { get; init; }
        public String? AccountId { get; init; }
        public Work? Work { get; init; }
        public int? WorkId { get; init; }
    }
}
