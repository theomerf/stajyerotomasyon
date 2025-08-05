using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record ResultDto
    {
        public required bool Success { get; init; }
        public required String Message { get; init; }
        public String? LoadComponent { get; init; }
        public required String ResultType { get; init; }
    }
}
