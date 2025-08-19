using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record ReportDtoForCreation : ReportDto
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Rapor içeriği gerekli.")]
        public String? ReportContent { get; init; }
        public List<String>? ImageUrls { get; set; }
        public ReportType ReportType { get; set; }
        public Account? Account { get; init; }
        public String? AccountId { get; set; }
        public Work? Work { get; set; }
        public int? WorkId { get; set; }
    }
}
