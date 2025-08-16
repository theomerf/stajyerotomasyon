using Entities.Dtos;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDto>> GetAllMessagesAsync();
        Task<string> GetAllMessagesCountAsync();
        Task<MessageDto?> GetMessageByIdAsync(int messageId);
        Task<string> GetAllMessagesCountForOneUserAsync(string userId);
        Task<IEnumerable<MessageDto>> GetAllMessagesForOneUserAsync(string userId);
        Task<MessageDtoForUpdate?> GetMessageForUpdateByIdAsync(int messageId);
        Task<ResultDto> CreateMessageAsync(MessageDtoForCreation messageDto);
        Task<ResultDto> UpdateMessageAsync(MessageDtoForUpdate messageDto);
        Task<ResultDto> DeleteMessageAsync(int messageId);
    }
}
