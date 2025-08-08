using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class ApplicationManager : IApplicationService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ApplicationManager(IRepositoryManager manager, IMapper mapper, IMemoryCache cache)
        {
            _manager = manager;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResultDto> DeleteApplicationAsync(int applicationId)
        {
            var application = await GetOneApplicationForService(applicationId);

            if (application.Status.ToString() == "OnWait")
            {
                _cache.Remove("applicationsOnWaitStats");
            }
            _cache.Remove("applicationsStats");
            _cache.Remove("applicationsMonthlyCount");
            _cache.Remove("applicationsCount");

            _manager.Application.DeleteApplication(application);
            await _manager.SaveAsync();
            var result = new ResultDto()
            {
                Success = true,
                Message = "Başvuru başarıyla silindi.",
                ResultType = "success",
                LoadComponent = "Applications"
            };
            return result;
        }

        public async Task<ApplicationDto> ChangeApplicationSeenAsync(int applicationId)
        {
            var application = await GetOneApplicationForService(applicationId);
            if (application.SeenDate == null && application.Status.ToString() == "OnWait")
            {
                application.SeenDate = DateTime.Now;
                _manager.Application.Update(application);
                await _manager.SaveAsync();
            }

            var applicationDto = _mapper.Map<ApplicationDto>(application);
            return applicationDto;
        }

        public async Task<Application> GetOneApplicationForService(int applicationId)
        {
            var application = await _manager.Application.GetApplicationByIdAsync(applicationId);
            if (application != null)
            {
                return application;
            }
            throw new KeyNotFoundException($"{applicationId}'ye sahip başvuru bulunamadı.");
        }

        public async Task<Dictionary<string, int>> GetMonthlyApplicationCountsAsync()
        {
            string cacheKey = "applicationsMonthlyCount";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, int>? cachedData))
            {
                return cachedData ?? new Dictionary<string, int>();
            }
            var stats = await _manager.Application.GetMonthlyApplicationCountsAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, stats, cacheOptions);
            return stats;
        }

        public async Task<ResultDto> AddNoteToApplicationAsync(int applicationId, string note, string userId)
        {
            var application = await GetOneApplicationForService(applicationId);
            var name = await _manager.Account.GetOneUsersNameAsync(userId);

            var notes = application.Notes;

            if (notes == null)
            {
                notes = new List<Note>();
            }

            if (name != null)
            {
                notes.Add(new Note()
                {
                    AccountId = userId,
                    AccountName = name,
                    Content = note,
                });

                application.Notes = notes;
                application.UpdatedDate = DateTime.Now;

                _manager.Application.Update(application);
                await _manager.SaveAsync();
            }

            var result = new ResultDto()
            {
                Success = true,
                Message = "Not eklendi.",
                ResultType = "success",
                LoadComponent = "Applications"
            };
            return result;
        }

        public async Task<ResultDto> RemoveNoteFromApplicationAsync(int applicationId, int noteId)
        {
            var application = await GetOneApplicationForService(applicationId);

            var note = await _manager.Note.GetNoteByIdAsync(noteId);
            if (note == null)
            {
                throw new KeyNotFoundException($"{noteId}'ye sahip not bulunamadı.");
            }

            application.Notes!.Remove(note);
            _manager.Note.DeleteNote(note);
            application.UpdatedDate = DateTime.Now;
            _manager.Application.Update(application);

            await _manager.SaveAsync();

            return new ResultDto
            {
                Success = true,
                Message = "Not silindi.",
                ResultType = "success",
                LoadComponent = "Applications"
            };
        }


        public async Task<List<Note>> GetAllNotesForOneApplicationAsync(int applicationId)
        {
            var notes = await _manager.Application.GetAllNotesForOneApplicationAsync(applicationId);

            return notes;
        }

        public async Task ExportToInternsAsync(int applicationId)
        {
            var application = await GetOneApplicationForService(applicationId);

            application.IsExported = true;

            _manager.Application.Update(application);
            await _manager.SaveAsync();
        }

        public async Task<ResultDto> ChangeApplicationStatusAsync(int applicationId, string status)
        {
            var application = await GetOneApplicationForService(applicationId);
            if (status == "Approved")
            {
                if ((application.Status.ToString() != "Approved") && (application.Status.ToString() != "Denied"))
                {
                    if (application.Status.ToString() == "OnWait")
                    {
                        _cache.Remove("applicationsOnWaitStats");
                    }
                    _cache.Remove("applicationsStats");

                    application.Status = ApplicationStatus.Approved;
                    application.UpdatedDate = DateTime.Now;
                    _manager.Application.Update(application);
                    await _manager.SaveAsync();

                    var result = new ResultDto()
                    {
                        Success = true,
                        Message = "Başvuru onaylandı.",
                        ResultType = "success",
                    };
                    return result;
                }
                else
                {
                    var result = new ResultDto()
                    {
                        Success = false,
                        Message = "Hatalı işlem.",
                        ResultType = "danger",
                        LoadComponent = "Applications"
                    };
                    return result;
                }
            }
            else if (status == "Interview")
            {
                if ((application.Status.ToString() != "Interview") && (application.Status.ToString() != "Denied") && (application.Status.ToString() != "Approved"))
                {
                    if (application.Status.ToString() == "OnWait")
                    {
                        _cache.Remove("applicationsOnWaitStats");
                    }
                    _cache.Remove("applicationsStats");

                    application.Status = ApplicationStatus.Interview;
                    application.InterviewedDate = DateTime.Now;
                    application.UpdatedDate = DateTime.Now;
                    _manager.Application.Update(application);
                    await _manager.SaveAsync();

                    var result = new ResultDto()
                    {
                        Success = true,
                        Message = "Başarıyla mülakat oluşturuldu.",
                        ResultType = "success",
                    };
                    return result;
                }
                else
                {
                    var result = new ResultDto()
                    {
                        Success = false,
                        Message = "Hatalı işlem.",
                        ResultType = "danger",
                        LoadComponent = "Applications"
                    };
                    return result;
                }
            }
            else if (status == "Denied")
            {
                if ((application.Status.ToString() != "Denied") && (application.Status.ToString() != "Approved"))
                {
                    if (application.Status.ToString() == "OnWait")
                    {
                        _cache.Remove("applicationsOnWaitStats");
                    }
                    _cache.Remove("applicationsStats");

                    application.Status = ApplicationStatus.Denied;
                    application.DeniedDate = DateTime.Now;
                    application.UpdatedDate = DateTime.Now;
                    _manager.Application.Update(application);
                    await _manager.SaveAsync();

                    var result = new ResultDto()
                    {
                        Success = true,
                        Message = "Başvuru reddedildi.",
                        ResultType = "success",
                    };
                    return result;
                }
                else
                {
                    var result = new ResultDto()
                    {
                        Success = false,
                        Message = "Hatalı işlem.",
                        ResultType = "danger",
                        LoadComponent = "Applications"
                    };
                    return result;
                }
            }
            else
            {
                var result = new ResultDto()
                {
                    Success = false,
                    Message = "Hatalı işlem.",
                    ResultType = "danger",
                    LoadComponent = "Applications"
                };
                return result;
            }
        }

        public async Task<IEnumerable<StatsDto>> GetApplicationsStatusStatsAsync()
        {
            string cacheKey = "applicationsStats";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<StatsDto>? cachedData))
            {
                return cachedData!;
            }

            var stats = await _manager.Application.GetApplicationsStatusStatsAsync();
            var statsDto = _mapper.Map<IEnumerable<StatsDto>>(stats);

            var cacheOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, statsDto, cacheOptions);
            return statsDto;
        }

        public async Task<StatsDto> GetApplicationsOnWaitStatsAsync()
        {
            string cacheKey = "applicationsOnWaitStats";

            if (_cache.TryGetValue(cacheKey, out StatsDto? cachedData))
            {
                return cachedData!;
            }

            var stats = await _manager.Application.GetApplicationsOnWaitStatsAsync();
            var statsDto = _mapper.Map<StatsDto>(stats);

            var cacheOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, statsDto, cacheOptions);
            return statsDto;
        }

        public async Task<IEnumerable<ApplicationDto>> GetAllApplicationsAsync(ApplicationRequestParameters p)
        {
            var applications = await _manager.Application.GetAllApplicationsAsync(p);
            var applicationsDto = _mapper.Map<IEnumerable<ApplicationDto>>(applications);
            return applicationsDto;
        }

        public async Task<int> GetApplicationsCountAsync(ApplicationRequestParameters p)
        {
            var count = await _manager.Application.GetApplicationsCountAsync(p);
            return count;
        }
        public async Task<int> GetAllApplicationsCountAsync()
        {
            string cacheKey = "applicationsCount";

            if (_cache.TryGetValue(cacheKey, out int cachedData))
            {
                return cachedData;
            }
            var count = await _manager.Application.GetAllApplicationsCountAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, count, cacheOptions);

            return count;
        }

        public async Task<ApplicationDto?> GetApplicationByIdAsync(int applicationId)
        {
            var application = await GetOneApplicationForService(applicationId);
            var applicationDto = _mapper.Map<ApplicationDto>(application);
            return applicationDto;
        }

        public async Task<AccountDtoForCreation> GetApplicationForExportAsync(int applicationId, string userNo)
        {
            var application = await GetOneApplicationForService(applicationId);

            var accountDto = new AccountDtoForCreation()
            {
                UserName = userNo,
                FirstName = application.ApplicantFirstName,
                LastName = application.ApplicantLastName,
                Email = application.ApplicantEmail,
                PhoneNumber = application.ApplicantPhoneNumber,
                SectionId = application.SectionId!.Value,
                DepartmentId = application.Section?.DepartmentId
            };
            return accountDto;
        }
    }
}
