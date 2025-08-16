using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record WorkDtoForCreation
    {
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Görev adı gerekli.")]
        public String? WorkName { get; init; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Görev içeriği gerekli.")]
        public String? WorkDescription { get; init; }
        [Required(ErrorMessage = "Görev verilecek kişiler gerekli.")]
        public List<String>? InternsId { get; init; }
        public List<String>? ImageUrls { get; set; }
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Görev başlangıç tarihi gerekli.")]
        public DateTime WorkStartDate { get; init; }
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Görev bitiş tarihi gerekli.")]
        public DateTime WorkEndDate { get; init; }
        public String? TaskMasterId { get; set; }
        public String? BroadcastType { get; init; }
        public int? DepartmentId { get; init; }
        public int? SectionId { get; init; }
    }
}
