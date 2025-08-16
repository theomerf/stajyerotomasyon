using Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Stajyeryotom.Infrastructure.Extensions;
using Stajyeryotom.Models;

namespace Stajyeryotom.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly IServiceManager _manager;

        public CalendarController(IServiceManager manager)
        {
            _manager = manager;
        }

        public async Task<IActionResult> Index(int? year, int? month)
        {
            var currentDate = DateTime.Today;
            var targetYear = year ?? currentDate.Year;
            var targetMonth = month ?? currentDate.Month;

            var model = await BuildCalendarViewModel(targetYear, targetMonth);

            if (year != null || month != null)
            {
                ViewBag.Month = targetMonth;
                ViewBag.Year = targetYear;
                return PartialView("Small/_CalendarGrid", model);
            }
            return PartialView("_Index", model);
        }

        private async Task<CalendarViewModel> BuildCalendarViewModel(int year, int month)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var events = await _manager.EventService.GetAllEventsOfOneMonth(year, month);
            var eventsLookup = events.GroupBy(e => e.Date.Date).ToDictionary(g => g.Key, g => g.ToList());

            var model = new CalendarViewModel
            {
                Year = year,
                Month = month,
                CurrentDate = firstDayOfMonth,
                Days = GenerateCalendarDays(firstDayOfMonth, eventsLookup)
            };

            return model;
        }

        private List<CalendarDay> GenerateCalendarDays(DateTime firstDayOfMonth, Dictionary<DateTime, List<EventDto>> eventsLookup)
        {
            var days = new List<CalendarDay>();
            var today = DateTime.Today;

            var startDay = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

            var prevMonth = firstDayOfMonth.AddMonths(-1);
            var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            for (int i = startDay - 1; i >= 0; i--)
            {
                var day = daysInPrevMonth - i;
                var date = new DateTime(prevMonth.Year, prevMonth.Month, day);
                days.Add(new CalendarDay
                {
                    Day = day,
                    Date = date,
                    IsOtherMonth = true,
                    IsToday = false,
                    Events = eventsLookup.GetValueOrDefault(date, new List<EventDto>())
                });
            }

            var daysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, day);
                days.Add(new CalendarDay
                {
                    Day = day,
                    Date = date,
                    IsOtherMonth = false,
                    IsToday = date.Date == today.Date,
                    Events = eventsLookup.GetValueOrDefault(date, new List<EventDto>())
                });
            }

            var totalDays = days.Count;
            var remainingDays = 35 - totalDays;
            var nextMonth = firstDayOfMonth.AddMonths(1);

            for (int day = 1; day <= remainingDays; day++)
            {
                var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
                days.Add(new CalendarDay
                {
                    Day = day,
                    Date = date,
                    IsOtherMonth = true,
                    IsToday = false,
                    Events = eventsLookup.GetValueOrDefault(date, new List<EventDto>())
                });
            }

            return days;
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddEvent([Bind(Prefix = "Event")] EventDto eventDto, DateTime selectedDate)
        {
            var result = await _manager.EventService.CreateEventAsync(eventDto, selectedDate);

            if (!ModelState.IsValid)
            {
                var html = await this.RenderViewAsync("_Index", eventDto, true);
                return Json(new { success = false, html = $"<div id='content' hx-swap-oob='true'>{html}</div>", message = "Etkinlik oluşturulurken form hatası oluştu.", type = "warning" });
            }
            else
            {
                var model = await BuildCalendarViewModel(selectedDate.Year, selectedDate.Month);
                var html = await this.RenderViewAsync("Small/_CalendarGrid", model, true);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    html = $"<div id='calendarContainer' hx-swap-oob='true'>{html}</div>",
                    type = result.ResultType,
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RemoveEvent([FromQuery] int eventId, [FromQuery] string selectedDateForRemove)
        {
            DateTime.TryParse(selectedDateForRemove, out DateTime selectedDate);
            var result = await _manager.EventService.DeleteEventAsync(eventId);
            var model = await BuildCalendarViewModel(selectedDate.Year, selectedDate.Month);
            var html = await this.RenderViewAsync("Small/_CalendarGrid", model, true);
            return Json(new
            {
                success = result.Success,
                message = result.Message,
                html = $"<div id='calendarContainer' hx-swap-oob='true'>{html}</div>",
                type = result.ResultType,
            });
        }
    }
}
