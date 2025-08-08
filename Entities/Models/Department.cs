using System.Text.Json.Serialization;

namespace Entities.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public String? DepartmentName { get; set; }
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
