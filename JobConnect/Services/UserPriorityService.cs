using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ViewModel;
using ViewModel.UserEducation;
using ViewModel.UserPriority;

namespace Services
{
    public interface IUserPriorityService
    {
        Task<UserPriorityResponse> Get(int? userId = null);
        Task<ResponseBase> Save(UserPrioritySave request);
    }
    public class UserPriorityService : IUserPriorityService
    {

        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserPriority> _userPriorities;
        private readonly ICommonService _commonService;

        public UserPriorityService(ICommonService commonService,
            IUnitOfWork uow)
        {
            _commonService = commonService;
            _uow = uow;
            _userPriorities = _uow.Set<UserPriority>();
        }
        public async Task<UserPriorityResponse> Get(int? userId = null)
        {
            if (userId == null)
            {
                userId = _commonService.GetCurrentUserId();
            }
            if (userId == 0)
            {
                return null;
            }
            var degrees = _commonService.GetDegrees();
            var response = await _userPriorities.Where(w => w.UserId == userId).AsNoTracking().ToListAsync();
            if (response.Any() == false)
            {
                return null;
            }
            var result = new UserPriorityResponse();
            foreach (var item in response)
            {
                result.SalaryRequestedId = item.SalaryRequestedId;
                if (string.IsNullOrEmpty(item.Benefits) == false)
                {
                    result.Benefits = item.Benefits.Split(",").Select(s => Convert.ToInt32(s)).ToList();
                }
                if (string.IsNullOrEmpty(item.Cities) == false)
                {
                    result.Cities = item.Cities.Split(",").Select(s => Convert.ToInt32(s)).ToList();
                }
                if (string.IsNullOrEmpty(item.ContractTypes) == false)
                {
                    result.ContractTypes = item.ContractTypes.Split(",").Select(s => Convert.ToInt32(s)).ToList();
                }
                if (string.IsNullOrEmpty(item.Groups) == false)
                {
                    result.Groups = item.Groups.Split(",").Select(s => Convert.ToInt32(s)).ToList();
                }
            }

            return result;
        }
        public async Task<ResponseBase> Save(UserPrioritySave request)
        {
            var userId = _commonService.GetCurrentUserId();
            if (userId == 0)
            {
                return new ResponseBase(false);
            }

            if (request.Id > 0)
            {
                var find = await _userPriorities.FirstOrDefaultAsync(w => w.Id == request.Id && w.UserId == userId);
                if (find is null)
                {
                    return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
                }
                find.Cities = string.Join(",", request.Cities);
                find.Groups = string.Join(",", request.Groups);
                find.ContractTypes = string.Join(",", request.ContractTypes);
                find.Benefits = string.Join(",", request.Benefits);
                find.SalaryRequestedId = request.SalaryRequestedId;
            }
            else
            {
                _userPriorities.Add(new UserPriority
                {
                    UserId = userId,
                    Benefits = string.Join(",", request.Benefits),
                    Cities = string.Join(",", request.Cities),
                    ContractTypes = string.Join(",", request.ContractTypes),
                    Groups = string.Join(",", request.Groups),
                    SalaryRequestedId = request.SalaryRequestedId
                });
            }
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
    }
}
