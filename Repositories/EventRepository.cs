using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class EventRepository : RepositoryBase<Event>, IEventRepository
    {
        public EventRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            var events = await FindAll(false)
                .ToListAsync();
            
            return events;
        }

        public async Task<IEnumerable<Event>> GetAllEventsOfOneDay(DateOnly date)
        {
            var events = await FindAllByCondition(e => DateOnly.FromDateTime(e.Date!.Value) == date,false)
                .ToListAsync();

            return events;
        }

        public async Task<IEnumerable<Event>> GetAllEventsOfOneMonth(int year, int month)
        {
            var events = await FindAllByCondition(e => DateOnly.FromDateTime(e.Date!.Value).Month == month && DateOnly.FromDateTime(e.Date.Value).Year == year, false)
                .ToListAsync();

            return events;
        }

        public async Task<Event?> GetEventByIdAsync(int eventId)
        {
            var @event = await FindByCondition(e => e.EventId == eventId,false)
                .FirstOrDefaultAsync();

            return @event;
        }

        public void UpdateEvent(Event @event)
        {
            Update(@event);
        }

        public void CreateEvent(Event @event)
        {
            Create(@event);
        }

        public void DeleteEvent(Event @event)
        {
            Remove(@event);
        }
    }
}
