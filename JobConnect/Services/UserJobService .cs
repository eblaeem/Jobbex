using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ViewModel;
using ViewModel.UserJob;

namespace Services
{
    public interface IUserJobService
    {
        Task<List<UserJobResponse>> Get(int? userId = null);
        Task<ResponseBase> Save(UserJobSave request);
        Task<ResponseBase> Delete(int id);
    }
    public class UserJobService : IUserJobService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserJob> _userJobs;
        private readonly ICommonService _commonService;

        public UserJobService(ICommonService commonService,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork uow)
        {
            _commonService = commonService;
            _contextAccessor = httpContextAccessor;
            _uow = uow;
            _userJobs = uow.Set<UserJob>();
        }
        public async Task<List<UserJobResponse>> Get(int? userId = null)
        {
            if (userId == null)
            {
                userId = GetCurrentUserId();
            }
            if (userId == 0)
            {
                return new List<UserJobResponse>();
            }
            var response = (from userJob in _userJobs.Where(w => w.UserId == userId)
                            from position in _uow.Set<Position>().Where(w => w.Id == userJob.PositionId).DefaultIfEmpty()
                            from jobGroup in _uow.Set<Group>().Where(w => w.Id == userJob.JobGroupId).DefaultIfEmpty()
                            from city in _uow.Set<City>().Where(w => w.Id == userJob.CityId).DefaultIfEmpty()
                            select new { userJob, position, jobGroup, city }).ToList()
                                .Select(item => new UserJobResponse
                                {
                                    Id = item.userJob.Id,
                                    Description = item.userJob.Description,
                                    StartDate = item.userJob.StartDate.ToShortPersianDateString(),
                                    EndDate = item.userJob.EndDate.ToShortPersianDateString(),
                                    CityId = item.userJob.CityId,
                                    CompanyName = item.userJob.CompanyName,
                                    CountryId = item.userJob.CountryId,
                                    IsCurrentJob = item.userJob.IsCurrentJob,
                                    JobGroupId = item.userJob.JobGroupId,
                                    JobTitle = item.userJob.JobTitle,
                                    PositionId = item.userJob.PositionId,
                                    City = item.city?.PersianName,
                                    Position = item.position?.Name,
                                    JobGroupName = item.jobGroup?.Name
                                }).ToList();
            return response;
        }
        public async Task<ResponseBase> Save(UserJobSave request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return new ResponseBase(false);
            }

            if (request.Id > 0)
            {
                var find = await _userJobs.FirstOrDefaultAsync(w => w.Id == request.Id && w.UserId == userId);
                if (find is null)
                {
                    return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
                }
                find.CityId = request.CityId;
                find.JobTitle = request.JobTitle;
                find.Description = request.Description;
                find.StartDate = request.StartDate.ToGregorianDateTime().Value;
                find.EndDate = request.EndDate.ToGregorianDateTime().Value;
                find.CompanyName = request.CompanyName;
                find.PositionId = request.PositionId;
                find.JobGroupId = request.JobGroupId;
            }
            else
            {
                _userJobs.Add(new UserJob
                {
                    UserId = userId,
                    JobTitle = request.JobTitle,
                    Description = request.Description,
                    StartDate = request.StartDate.ToGregorianDateTime().Value,
                    EndDate = request.EndDate.ToGregorianDateTime().Value,
                    CompanyName = request.CompanyName,
                    PositionId = request.PositionId,
                    JobGroupId = request.JobGroupId,
                    CityId = request.CityId,
                    IsCurrentJob = request.IsCurrentJob
                });
            }
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
        public async Task<ResponseBase> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return new ResponseBase(false);
            }
            var find = await _userJobs.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (find is null)
            {
                return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
            }
            _userJobs.Remove(find);
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }

        private int GetCurrentUserId()
        {
            var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
            var userId = userDataClaim?.Value;
            return string.IsNullOrWhiteSpace(userId) ? 0 : int.Parse(userId);
        }

    }
}
