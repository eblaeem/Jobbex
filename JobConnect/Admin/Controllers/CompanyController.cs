using Admin.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.ComponentModel.DataAnnotations;
using ViewModel.Company;
using ViewModel.User;

namespace Admin.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly ICommonService _commonService;
        private readonly IStateService _stateService;
        private readonly IJobGroupsService _jobGroupsService;

        public CompanyController(ICompanyService companyService,
            ICommonService commonService,
            IStateService stateService,
            IJobGroupsService jobGroupsService)
        {
            _companyService = companyService;
            _commonService = commonService;
            _stateService = stateService;
            _jobGroupsService = jobGroupsService;
        }

        [HttpGet]
        public async Task<IActionResult> Save()
        {
            var currentUserId = _commonService.GetCurrentUserId();
            var response = await _companyService.GetByUserId(currentUserId);
            response.Cities = await _stateService.GetGroupStates();
            response.JobGroups = await _jobGroupsService.Get();

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] CompanySave request)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new CreateUserResponse(false, HelperExtension.Errors(ModelState)));
            }
            request.Id = _commonService.GetCurrentCompanyId();
            var response = await _companyService.Save(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] int type, IFormFile file)
        {
            if (file != null)
            {
                var response = await _companyService.Upload(type, file);
                return Ok(response);
            }
            return NotFound();
        }
    }
}
