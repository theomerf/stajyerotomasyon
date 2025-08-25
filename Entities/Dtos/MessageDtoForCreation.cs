using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record MessageDtoForCreation
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Mesajı başlığı gerekli.")]
        public String? MessageTitle { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Mesaj içeriği gerekli.")]
        public String? MessageBody { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Gönderme seçimi gerekli.")]
        public String? BroadcastType { get; init; }
        public int? DepartmentId { get; init; }
        public int? SectionId { get; init; }
        public List<String>? InternsId { get; init; }
    }
}
