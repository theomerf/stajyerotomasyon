using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EventManager : IEventService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;

        public EventManager(IRepositoryManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        public async Task<ResultDto> CreateEventAsync(EventDto @event, DateTime selectedDate)
        {
            var eventForCreate = _mapper.Map<Event>(@event);
            eventForCreate.Date = selectedDate;
            _manager.Event.CreateEvent(eventForCreate);
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Etkinlik başarıyla oluşturuldu.",
                ResultType = "success",
                LoadComponent = "Calendar"
            };
            return result;
        }

        public async Task<ResultDto> DeleteEventAsync(int eventId)
        {
            var @event = await GetEventByIdForServiceAsync(eventId);
            _manager.Event.DeleteEvent(@event!);
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Etkinlik başarıyla silindi.",
                ResultType = "success",
                LoadComponent = "Calendar"
            };
            return result;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _manager.Event.GetAllEventsAsync();
            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(events);
            return eventsDto;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsOfOneDay(DateOnly day)
        {
            var events = await _manager.Event.GetAllEventsOfOneDay(day);
            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(events);
            return eventsDto;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsOfOneMonth(int year, int month)
        {
            var events = await _manager.Event.GetAllEventsOfOneMonth(year, month);
            var eventsDto = _mapper.Map<IEnumerable<EventDto>>(events);
            return eventsDto;
        }

        public async Task<EventDto?> GetEventByIdAsync(int eventId)
        {
            var @event = await _manager.Event.GetEventByIdAsync(eventId);
            if (@event == null)
            {
                throw new KeyNotFoundException($"{eventId} id'sine sahip etkinlik bulunamadı.");
            }
            var eventDto = _mapper.Map<EventDto>(@event);
            return eventDto;
        }

        public async Task<Event?> GetEventByIdForServiceAsync(int eventId)
        {
            var @event = await _manager.Event.GetEventByIdAsync(eventId);
            if(@event == null)
            {
                throw new KeyNotFoundException($"{eventId} id'sine sahip etkinlik bulunamadı.");
            }
            return @event;
        }

        public async Task<ResultDto> UpdateEventAsync(EventDto @event)
        {
            var eventForUpdate = _mapper.Map<Event>(@event);
            _manager.Event.UpdateEvent(eventForUpdate);
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Etkinlik başarıyla güncellendi.",
                ResultType = "success",
                LoadComponent = "Calendar"
            };
            return result;
        }
    }
}
