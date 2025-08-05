using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IEventRepository : IRepositoryBase<Event>
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event?> GetEventByIdAsync(int eventId);
        Task<IEnumerable<Event>> GetAllEventsOfOneDay(DateOnly date);
        Task<IEnumerable<Event>> GetAllEventsOfOneMonth(int year, int month);
        void CreateEvent(Event @event); 
        void UpdateEvent(Event @event);
        void DeleteEvent(Event @event);

    }
}
