using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ViewModel;
using ViewModel.UserEducation;

namespace Services
{
    public interface IUserEducationService
    {
        Task<List<UserEducationResponse>> Get(int? userId = null);
        Task<ResponseBase> Save(UserEducationSave request);
        Task<ResponseBase> Delete(int id);
    }
    public class UserEducationService : IUserEducationService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserEducation> _userEducations;
        private readonly ICommonService _commonService;

        public UserEducationService(ICommonService commonService,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork uow)
        {
            _commonService = commonService;
            _contextAccessor = httpContextAccessor;
            _uow = uow;
            _userEducations = uow.Set<UserEducation>();
        }
        public async Task<List<UserEducationResponse>> Get(int? userId = null)
        {
            if (userId == null)
            {
                userId = GetCurrentUserId();
            }
            if (userId == 0)
            {
                return new List<UserEducationResponse>();
            }
            var degrees = _commonService.GetDegrees();
            var response = _userEducations.Where(w => w.UserId == userId).ToList()
                                .Select(item => new UserEducationResponse
                                {
                                    Id = item.Id,
                                    DegreeId = item.DegreeId,
                                    Description = item.Description,
                                    StartDate = item.StartDate.ToShortPersianDateString(),
                                    EndDate = item.EndDate.ToShortPersianDateString(),
                                    Field = item.Field,
                                    Score = item.Score,
                                    UniversityName = item.UniversityName,
                                    Degree = degrees.FirstOrDefault(w => w.Id == item.DegreeId)?.Label
                                }).ToList();
            return response;
        }
        public async Task<ResponseBase> Save(UserEducationSave request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return new ResponseBase(false);
            }

            if (request.Id > 0)
            {
                var find = await _userEducations.FirstOrDefaultAsync(w => w.Id == request.Id && w.UserId == userId);
                if (find is null)
                {
                    return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
                }
                find.DegreeId = request.DegreeId;
                find.UniversityName = request.UniversityName;
                find.Description = request.Description;
                find.StartDate = request.StartDate.ToGregorianDateTime().Value;
                find.EndDate = request.EndDate.ToGregorianDateTime().Value;
                find.Score = request.Score;
            }
            else
            {
                _userEducations.Add(new UserEducation
                {
                    UserId = userId,
                    DegreeId = request.DegreeId,
                    Description = request.Description,
                    StartDate = request.StartDate.ToGregorianDateTime().Value,
                    EndDate = request.EndDate.ToGregorianDateTime().Value,
                    Score = request.Score,
                    Field = request.Field,
                    UniversityName = request.UniversityName
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
            var find = await _userEducations.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (find is null)
            {
                return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
            }
            _userEducations.Remove(find);
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
