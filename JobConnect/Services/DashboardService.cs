using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel.Dashboard;

namespace Services
{
    public interface IDashboardService
    {
        Task<CompanyDashboard> Get();
    }
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Company> _companies;
        private readonly DbSet<Job> _jobs;
        private readonly ICommonService _commonService;

        public DashboardService(IUnitOfWork uow,
            ICommonService commonService)
        {
            _uow = uow;
            _companies = _uow.Set<Company>();
            _jobs = _uow.Set<Job>();
            _commonService = commonService;
        }
        public async Task<CompanyDashboard> Get()
        {
            var currentUserId = _commonService.GetCurrentUserId();
            var company = await _companies.FirstOrDefaultAsync(w => w.UserId == currentUserId);

            var queryActiveJobs = from job in _jobs.Where(w => w.CompanyId == company.Id && w.Status == true && w.ExpiredDateTime > DateTime.Now)
                                  from jobGroup in _uow.Set<Group>().Where(w => w.Id == job.JobGroupId)
                                  let userRequestCount = _uow.Set<JobRequested>().Count(w => w.JobId == job.Id)
                                  select new DashboardJobResponse
                                  {
                                      JobId = job.Id,
                                      JobDescription = job.Description,
                                      JobTitle = job.Title,
                                      UserRequestCount = userRequestCount,
                                      JobGroupName = jobGroup.Name,
                                      DateTime = job.DateTime
                                  };
            var activeJobs = await queryActiveJobs.OrderByDescending(o => o.DateTime).Take(10).ToListAsync();
            if (activeJobs.Any())
            {
                foreach (var item in activeJobs)
                {
                    item.DateTimeString = item.DateTime.ToShortPersianDateString();
                }
            }
            var userRequest = (from job in _jobs.Where(w => w.CompanyId == company.Id)
                               from request in _uow.Set<JobRequested>().Where(w => w.JobId == job.Id)
                               select new
                               {
                                   request.Id,
                                   Date = request.DateTime.Date
                               });
            var date = DateTime.Now.Date;
            var response = new CompanyDashboard()
            {
                ActiveJobs = activeJobs,
                ActiveJobCount = await queryActiveJobs.CountAsync(),
                JobCount = _jobs.Count(w => w.CompanyId == company.Id),
                UserRequestCount = userRequest.Count(),
                UserRequestTodayCount = await userRequest.Where(w => w.Date == date).CountAsync(),
            };
            return response;
        }
    }
}
