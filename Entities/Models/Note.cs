namespace Entities.Models
{
    public class Note
    {
        public int NoteId { get; set; }
        public String? NoteTitle { get; set; }
        public String? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int ApplicationId { get; set; }
        public Application? Application { get; set; }
        public String? AccountId { get; set; }
        public Account? Account { get; set; }
        public String? AccountName { get; set; }
    }
}
