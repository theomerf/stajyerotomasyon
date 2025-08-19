using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        public MessageRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateMessage(Message message)
        {
            Create(message);
        }

        public void DeleteMessage(Message message)
        {
            Remove(message);
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            var messages = await FindAll(false)
                .Include(m => m.Interns)
                .ToListAsync();

            return messages;
        }

        public async Task<int> GetAllMessagesCountAsync()
        {
            var count = await FindAll(false)
                .CountAsync();

            return count;
        }

        public async Task<Message?> GetMessageByIdAsync(int messageId)
        {
            var message = await FindByCondition(m => m.MessageId == messageId, false)
                .FirstOrDefaultAsync();

            return message;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesForOneUserAsync(string userId)
        {
            var message = await FindByCondition(m => m.Interns!.Any(i => i.Id == userId), false)
                .ToListAsync();

            return message;
        }

        public async Task<int> GetAllMessagesCountForOneUserAsync(string userId)
        {
            var count = await FindByCondition(m => m.Interns!.Any(i => i.Id == userId), false)
                .CountAsync();

            return count;
        }
        public async Task<Stats> GetUserMessagesStatsAsync(string userId)
        {
            var stats = await FindAllByCondition(w => w.Interns!.Any(i => i.Id == userId), false)
                .GroupBy(_ => 1)
                .Select(g => new Stats()
                {
                    Key = g.Key.ToString(),
                    TotalCount = g.Count(),
                    ThisMonthsCount = 0,
                    LastMonthsCount = 0,
                })
                .FirstOrDefaultAsync();

            return stats ?? new Stats();
        }

        public async Task<Message?> GetMessageForUpdateByIdAsync(int workId)
        {
            var message = await FindByCondition(w => w.MessageId == workId, true)
                .Include(w => w.Interns!)
                .FirstOrDefaultAsync();

            return message;
        }

        public void UpdateMessage(Message message)
        {
            Update(message);
        }
    }
}
