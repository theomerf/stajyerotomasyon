using Entities.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Stajyeryotom.Models
{
    public class SectionCreateModel
    {
        [Required(ErrorMessage = "Departman gereklidir.")]
        public string? DepartmentId { get; set; }

        [RequiredListNotEmpty(ErrorMessage = "En az bir bölüm girilmelidir.")]
        public List<string> SectionNames { get; set; } = new List<string>();
    }
}
