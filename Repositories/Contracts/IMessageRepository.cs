using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IMessageRepository : IRepositoryBase<Message>
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task<int> GetAllMessagesCountAsync();
        Task<Message?> GetMessageByIdAsync(int messageId);
        Task<Stats> GetUserMessagesStatsAsync(string userId);
        Task<int> GetAllMessagesCountForOneUserAsync(string userId);
        Task<IEnumerable<Message>> GetAllMessagesForOneUserAsync(string userId);
        Task<Message?> GetMessageForUpdateByIdAsync(int workId);
        void CreateMessage(Message message);
        void UpdateMessage(Message message);
        void DeleteMessage(Message message);
    }
}
