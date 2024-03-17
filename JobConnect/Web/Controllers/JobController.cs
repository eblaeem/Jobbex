using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Services;
using ViewModel;
using ViewModel.Job;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IJobGroupsService _jobGroupsService;
        private readonly IStateService _stateService;
        private readonly ISoftWareSkillsService _softWareSkillsService;
        private readonly ICommonService _commonService;
        public JobController(IJobService jobService,
            IJobGroupsService jobGroupsService,
            IStateService stateService,
            ISoftWareSkillsService softWareSkillsService,
            ICommonService commonService)
        {
            _jobService = jobService;
            _jobGroupsService = jobGroupsService;
            _stateService = stateService;
            _softWareSkillsService = softWareSkillsService;
            _commonService = commonService;
        }

        [HttpPost("get")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] JobFilter filter)
        {
            var response = await _jobService.Get(filter);
            return Ok(response);
        }

        [HttpGet("options")]
        public async Task<IActionResult> Options()
        {
            var response = new JobResponseOption()
            {
                States = await _stateService.Get(),
                Latest = await _jobService.Get(new JobFilter()
                {
                    PageNumber = 1,
                    PageSize = 10
                }),
                Groups = await _jobGroupsService.Get(),
                PopularTags = await _softWareSkillsService.GetPopularTags(),
                ContractTypes = _commonService.GetContractTypes(),
                SalaryRequesteds = _commonService.GetSalaryRequesteds(),
                WorkExperienceYears = _commonService.GetWorkExperienceYears()
            };
            return Ok(response);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _jobService.Detail(id);
            return Ok(response);
        }
    }
}
