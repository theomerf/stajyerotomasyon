using Entities.Dtos;

namespace Stajyeryotom.Models
{
    public class CalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<CalendarDay> Days { get; set; } = new();
        public DateTime CurrentDate { get; set; }
        public DateTime Today { get; set; } = DateTime.Today;
        public EventDto? Event { get; set; }
    }

    public class CalendarDay
    {
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public bool IsOtherMonth { get; set; }
        public bool IsToday { get; set; }
        public List<EventDto> Events { get; set; } = new();
        public bool HasMoreEvents => Events.Count > 3;
        public int MoreEventsCount => Math.Max(0, Events.Count - 3);
    }
}
