using Api.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;
using ViewModel.Company;
using ViewModel.Dashboard;
using ViewModel.Job;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class DashboardController : Controller
    {
        private readonly IStateService _stateService;
        private readonly IJobService _jobService;
        private readonly ISoftWareSkillsService _softWareSkillsService;
        private readonly ICompanyService _companyService;
        private readonly IUsersService _usersService;
        private readonly IJobGroupsService _jobGroupsService;
        public DashboardController(IStateService stateService,
            IJobService jobService,
            ISoftWareSkillsService softWareSkillsService,
            ICompanyService companyService,
            IUsersService usersService,
            IJobGroupsService jobGroupsService)
        {
            _stateService = stateService;
            _jobService = jobService;
            _softWareSkillsService = softWareSkillsService;
            _companyService = companyService;
            _usersService = usersService;
            _jobGroupsService = jobGroupsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = new DashboardResponse()
            {
                GroupStates = await _stateService.GetGroupStates(),
                Latest = await _jobService.Get(new JobFilter()
                {
                    PageNumber = 1,
                    PageSize = 9
                }),
                PopularTags = await _softWareSkillsService.GetPopularTags(),
                PopularCompanies = await _companyService.GetSummery(new CompanyFilter
                {
                    PageNumber = 1,
                    PageSize = 8
                }),
                UserCount = await _usersService.Count(),
                JobCount = await _jobService.Count(),
                ViewsCount = 20000,
                Groups = await _jobGroupsService.Get()
            };
            return Ok(response);
        }
    }
}
