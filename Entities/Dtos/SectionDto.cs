using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record SectionDto
    {
        public int SectionId { get; init; }
        [Required(ErrorMessage = "Bölüm adı gereklidir!")]
        public String? SectionName { get; init; }
        public int DepartmentId { get; init; }
        public ICollection<Account> Users { get; init; } = new List<Account>();
    }
}
