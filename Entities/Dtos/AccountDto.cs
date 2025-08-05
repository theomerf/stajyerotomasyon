using Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public record AccountDto
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Kullanıcı no gerekli.")]
        public String? UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "E-Posta gerekli.")]
        public String? Email { get; init; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Telefon numarası gerekli.")]

        public String? PhoneNumber { get; init; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Ad gerekli.")]
        public String? FirstName { get; init; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Soyad gerekli.")]
        public String? LastName { get; init; }
        public DateTime? LastLoginDate { get; init; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Staj bitiş tarihi gerekli.")]
        public DateTime InternshipStartDate { get; init; } = new DateTime(DateTime.Now.Year, 1, 1);

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Staj başlangıç tarihi gerekli.")]
        public DateTime InternshipEndDate { get; init; } = new DateTime(DateTime.Now.Year, 1, 1);
        public bool EmailConfirmed { get; init; } = true;
        public DateTime? BirthDate { get; init; }
        public String? ProfilePictureUrl { get; init; }

        [Required(ErrorMessage = "Bölüm seçimi gerekli.")]
        public int? SectionId { get; init; }
        public String? SectionName { get; init; }
        [Required(ErrorMessage = "Bölüm seçimi gerekli.")]
        public int? DepartmentId { get; init; }
        public List<Note>? Notes { get; init; }
        public ICollection<Work>? Works { get; init; }
        public ICollection<Work>? SupervisedWorks { get; init; }
        public String? DepartmentName { get; init; }
        public HashSet<String?> Roles { get; set; } = new HashSet<String?>();
        public bool IsActive { get; init; } = true;
    }
}
