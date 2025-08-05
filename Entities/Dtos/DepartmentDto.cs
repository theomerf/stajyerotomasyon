using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record DepartmentDto
    {
        public int DepartmentId { get; init; }
        [Required(ErrorMessage = "Departman adı gereklidir!")]
        public string? DepartmentName { get; init; }
        public ICollection<Section> Sections { get; init; } = new List<Section>();
    }
}
