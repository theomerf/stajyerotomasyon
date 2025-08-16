using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MessageManager : IMessageService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public MessageManager(IRepositoryManager manager, IMapper mapper, IMemoryCache cache)
        {
            _manager = manager;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResultDto> CreateMessageAsync(MessageDtoForCreation messageDto)
        {
            var message = _mapper.Map<Message>(messageDto);

            if (message.BroadcastType == "All")
            {
                var internsId = await _manager.Account.GetAllInternsId();
                var interns = internsId.Select(id => new Account { Id = id }).ToList();
                _manager.Account.AttachRange(interns);
                message.Interns = interns;
            }
            else if (messageDto.BroadcastType == "Users")
            {
                var interns = messageDto.InternsId!.Select(id => new Account { Id = id }).ToList();
                _manager.Account.AttachRange(interns);
                message.Interns = interns;
            }
            else if (messageDto.BroadcastType == "Department")
            {
                var internsId = await _manager.Account.GelAllInternsOfDepartment(messageDto.DepartmentId!.Value);
                var interns = internsId.Select(id => new Account { Id = id }).ToList();
                _manager.Account.AttachRange(interns);
                message.Interns = interns;
            }
            else if (messageDto.BroadcastType == "Section")
            {
                var internsId = await _manager.Account.GelAllInternsOfSection(messageDto.SectionId!.Value);
                var interns = internsId.Select(id => new Account { Id = id! }).ToList();
                _manager.Account.AttachRange(interns);
                message.Interns = interns;
            }

            _manager.Message.CreateMessage(message);
            await _manager.SaveAsync();

            _cache.Remove("messagesCount");

            var result = new ResultDto()
            {
                Success = true,
                Message = "Mesaj başarıyla oluşturuldu.",
                ResultType = "success",
                LoadComponent = "Messages"
            };
            return result;
        }

        public async Task<ResultDto> DeleteMessageAsync(int messageId)
        {
            var message = await GetMessageByIdForServiceAsync(messageId);

            _manager.Message.DeleteMessage(message);
            await _manager.SaveAsync();

            _cache.Remove("messagesCount");

            var result = new ResultDto()
            {
                Success = true,
                Message = "Mesaj başarıyla silindi.",
                ResultType = "success",
                LoadComponent = "Messages"
            };
            return result;
        }

        private async Task<Message> GetMessageByIdForServiceAsync(int messageId)
        {
            var message = await _manager.Message.GetMessageByIdAsync(messageId);

            if (message == null)
            {
                throw new KeyNotFoundException($"{messageId} id'li mesaj bulunamadı.");
            }

            return message;
        }

        public async Task<IEnumerable<MessageDto>> GetAllMessagesAsync()
        {
            var messages = await _manager.Message.GetAllMessagesAsync();
            var messagesDto = _mapper.Map<IEnumerable<MessageDto>>(messages);

            return messagesDto;
        }

        public async Task<IEnumerable<MessageDto>> GetAllMessagesForOneUserAsync(string userId)
        {
            var messages = await _manager.Message.GetAllMessagesForOneUserAsync(userId);
            var messagesDto = _mapper.Map<IEnumerable<MessageDto>>(messages);

            return messagesDto;
        }

        public async Task<string> GetAllMessagesCountForOneUserAsync(string userId)
        {
            var count = await _manager.Message.GetAllMessagesCountForOneUserAsync(userId);

            return count.ToString();
        }

        public async Task<MessageDto?> GetMessageByIdAsync(int messageId)
        {
            var message = await GetMessageByIdForServiceAsync(messageId);
            var messageDto = _mapper.Map<MessageDto>(message);

            return messageDto;
        }

        public async Task<MessageDtoForUpdate?> GetMessageForUpdateByIdAsync(int messageId)
        {
            var message = await _manager.Message.GetMessageForUpdateByIdAsync(messageId);
            if (message == null)
            {
                throw new KeyNotFoundException($"{messageId} id'sine sahip mesaj bulunamadı.");
            }

            var messageDto = _mapper.Map<MessageDtoForUpdate>(message);
            return messageDto;
        }

        public async Task<ResultDto> UpdateMessageAsync(MessageDtoForUpdate messageDto)
        {
            var existingMessage = await _manager.Message.GetMessageForUpdateByIdAsync(messageDto.MessageId);

            _mapper.Map(messageDto, existingMessage);

            if (existingMessage == null)
                throw new KeyNotFoundException($"{messageDto.MessageId} id'sine sahip görev bulunamadı.");

            if (messageDto.BroadcastType == "All")
            {
                var internsId = await _manager.Account.GetAllInternsId();

                foreach (var id in internsId)
                {
                    if (existingMessage.Interns!.Where(i => i.Id == id).Any())
                    {
                        continue;
                    }
                    var intern = new Account { Id = id };
                    _manager.Account.Attach(intern);
                    existingMessage.Interns?.Add(intern);
                }
            }
            else if (messageDto.BroadcastType == "Users")
            {
                if (messageDto.UpdatedInternsId != null && messageDto.UpdatedInternsId.Any())
                {
                    foreach (var intern in existingMessage.Interns!)
                    {
                        if (!messageDto.UpdatedInternsId.Contains(intern.Id))
                        {
                            existingMessage.Interns?.Remove(intern);
                        }
                    }
                    foreach (var id2 in messageDto.UpdatedInternsId)
                    {
                        if (existingMessage.Interns!.Where(i => i.Id == id2).Any())
                        {
                            continue;
                        }
                        var intern = new Account { Id = id2 };
                        _manager.Account.Attach(intern);
                        existingMessage.Interns?.Add(intern);
                    }
                }
            }
            else if (messageDto.BroadcastType == "Department")
            {
                var internsId = await _manager.Account.GelAllInternsOfDepartment(messageDto.DepartmentId!.Value);

                foreach (var intern2 in existingMessage.Interns!)
                {
                    if (!internsId.Contains(intern2.Id))
                    {
                        existingMessage.Interns?.Remove(intern2);
                    }
                }

                foreach (var id in internsId)
                {
                    if (existingMessage.Interns!.Where(i => i.Id == id).Any())
                    {
                        continue;
                    }
                    var intern = new Account { Id = id };
                    _manager.Account.Attach(intern);
                    existingMessage.Interns?.Add(intern);
                }
            }
            else if (messageDto.BroadcastType == "Section")
            {
                var internsId = await _manager.Account.GelAllInternsOfSection(messageDto.SectionId!.Value);

                foreach (var intern2 in existingMessage.Interns!)
                {
                    if (!internsId.Contains(intern2.Id))
                    {
                        existingMessage.Interns?.Remove(intern2);
                    }
                }

                foreach (var id in internsId)
                {
                    if (existingMessage.Interns!.Where(i => i.Id == id).Any())
                    {
                        continue;
                    }
                    var intern = new Account { Id = id! };
                    _manager.Account.Attach(intern);
                    existingMessage.Interns?.Add(intern);
                }
            }

            _manager.Message.UpdateMessage(existingMessage);
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Mesaj başarıyla güncellendi.",
                ResultType = "success",
                LoadComponent = "Messages"
            };
            return result;
        }

        public async Task<string> GetAllMessagesCountAsync()
        {
            string cacheKey = "messagesCount";

            if (_cache.TryGetValue(cacheKey, out int? cachedData))
            {
                return cachedData!.ToString() ?? "";
            }

            var count = await _manager.Message.GetAllMessagesCountAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, count, cacheOptions);

            return count.ToString();
        }
    }
}
