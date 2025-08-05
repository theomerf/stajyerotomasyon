using Entities.Dtos;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IEventService 
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<EventDto?> GetEventByIdAsync(int eventId);
        Task<IEnumerable<EventDto>> GetAllEventsOfOneDay(DateOnly date);
        Task<IEnumerable<EventDto>> GetAllEventsOfOneMonth(int year, int month);
        Task<ResultDto> CreateEventAsync(EventDto @event, DateTime selectedDate);
        Task<ResultDto> UpdateEventAsync(EventDto @event);
        Task<ResultDto> DeleteEventAsync(int eventId);
    }
}
