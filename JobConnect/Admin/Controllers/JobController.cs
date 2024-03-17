using Admin.Helper;
using Entities;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Services;
using ViewModel;
using ViewModel.Company;
using ViewModel.Job;
using ViewModel.User;
using ViewModel.UserJob;

namespace Admin.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ICommonService _commonService;
        private readonly ICompanyService _companyService;
        private readonly IStateService _stateService;
        private readonly IJobGroupsService _jobGroupsService;
        public JobController(IJobService jobService,
            ICommonService commonService,
            ICompanyService companyService,
            IStateService stateService,
            IJobGroupsService jobGroupsService)
        {
            _jobService = jobService;
            _commonService = commonService;
            _companyService = companyService;
            _stateService = stateService;
            _jobGroupsService = jobGroupsService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _companyService.GetJobs(new CompanyJobFilter
            {
                UserId = _commonService.GetCurrentUserId(),
                PageNumber = 1,
                PageSize = 7
            });
            var model = new CompanyJobFilter()
            {
                ContractTypes = _commonService.GetContractTypes(),
                Cities = await _stateService.GetGroupStates(),
                JobGroups = await _jobGroupsService.Get(),
                WorkExperienceYears = _commonService.GetWorkExperienceYears(),
                SalaryRequesteds = _commonService.GetSalaryRequesteds(),
                Latest = response,
                PageLengths = _commonService.GetPageLengths(),
                TotalRowNumber = response.FirstOrDefault()?.TotalRowCount,
                PageSize = 5
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CompanyJobFilter filter)
        {
            var response = await _companyService.GetJobs(filter);
            return PartialView("_Get", response);
        }

        [HttpGet]
        public async Task<IActionResult> Save(int id)
        {
            var currentUserId = _commonService.GetCurrentUserId();
            var model = new JobSave()
            {
                ContractTypes = _commonService.GetContractTypes(),
                Cities = await _stateService.GetGroupStates(),
                JobGroups = await _jobGroupsService.Get(),
                WorkExperienceYears = _commonService.GetWorkExperienceYears(),
                SalaryTypes = _commonService.GetSalaryRequesteds(),
                Genders = _commonService.GetGenders(),
                EducationLevels = _commonService.GetDegrees(),
                MilitaryStates = _commonService.GetMilitaryStatus(),
            };
            if (id == 0)
            {
                model.Status = true;
                return View(model);
            }
            var response = await _jobService.Detail(id);
            if (response != null)
            {
                model.Id = response.Id;
                model.CityId = response.CityId;
                model.ContractTypeId = response.ContractTypeId;
                model.EducationLevelRequired = response.EducationLevelRequiredId;
                model.GenderRequired = response.GenderRequiredId;
                model.JobGroupId = response.JobGroupId;
                model.MilitaryStateRequired = response.MilitaryStateRequiredId;
                model.WorkExperienceYearId = response.WorkExperienceYearId;
                model.Title = response.JobTitle;
                model.Description = response.Description;
                model.SalaryTypeId = response.SalaryTypeId;
                model.WorkingDays = response.WorkingDays;

                model.Status = response.Status;
                model.RequiredLoginToSite = response.RequiredLoginToSite;
                model.RequiredInformation = response.RequiredInformation;
                model.RequiredEducation = response.RequiredEducation;
                model.RequiredJob = response.RequiredJob;
                model.RequiredLanguage = response.RequiredLanguage;
                model.RequiredSkills = response.RequiredSkills;
                model.RequiredPriorities = response.RequiredPriorities;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] JobSave request)
        {
            request.CompanyId = _commonService.GetCurrentCompanyId();
            if (!ModelState.IsValid)
            {
                return Ok(new ResponseBase(false, HelperExtension.Errors(ModelState)));
            }
            request.UserId = _commonService.GetCurrentUserId();
            var response = await _jobService.Save(request);
            return Ok(response);
        }
    }
}
