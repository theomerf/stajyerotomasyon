using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record EventDto
    {
        public int EventId { get; init; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Etkinlik adı gerekli.")]
        public String? Title { get; init; }
        [DataType(DataType.Time)]
        [Required(ErrorMessage = "Etkinlik saati gerekli.")]
        public TimeSpan Time { get; init; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Etkinlik türü gerekli.")]
        public String? Type { get; init; }
        public String? Description { get; init; }
        public DateTime Date { get; init; }
    }
}
