using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record ReportViewDto : ReportDto
    {
        public String? ReportContent { get; init; }
        public List<String>? ImageUrls { get; init; }
    }
}
