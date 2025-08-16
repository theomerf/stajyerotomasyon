using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record WorkViewDto : WorkDto
    {
        public ICollection<Report>? Reports { get; set; }
        public ICollection<String>? InternsName { get; set; }
    }
}
