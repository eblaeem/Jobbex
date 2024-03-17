using DataLayer.Context;
using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.UserRecommended;

namespace Services
{
    public interface IUserRecommendedService
    {
        Task<List<UserRecommendedResponse>> Get(UserRecommendedFilter filter);
    }
    public class UserRecommendedService : IUserRecommendedService
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserPriorityService _priorityService;
        private readonly IJobService _jobService;

        public UserRecommendedService(IUnitOfWork uow, IUserPriorityService userPriorityService, IJobService jobService)
        {
            _uow = uow;
            _priorityService = userPriorityService;
            _jobService = jobService;
        }

        public async Task<List<UserRecommendedResponse>> Get(UserRecommendedFilter filter)
        {
            var response = new List<UserRecommendedResponse>();
            var userPriorities = await _priorityService.Get();
            userPriorities.PageNumber = filter.PageNumber;
            userPriorities.PageSize = filter.PageSize;
            userPriorities.Sort = filter.Sort;
            var jobs = await _jobService.GetUserJobRecomended(userPriorities);
            return jobs;

        }
    }
}
