using ChromiumHtmlToPdfLib;
using ChromiumHtmlToPdfLib.Enums;
using ChromiumHtmlToPdfLib.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services;
using ViewModel;
using ViewModel.Attachment;
using ViewModel.Company;
using ViewModel.JobRequest;
using ViewModel.Setting;

namespace Admin.Controllers
{
    public class JobRequestController : Controller
    {
        private readonly IJobRequestService _jobRequestService;
        private readonly IJobService _jobService;
        private readonly ICommonService _commonService;
        private readonly ICompanyService _companyService;
        private readonly IStateService _stateService;
        private readonly IJobGroupsService _jobGroupsService;
        private readonly IAttachmentService _attachmentService;
        private readonly IUserProfileService _userProfileService;
        private readonly IViewRenderService _viewRenderService;
        private readonly AppSettings _appSettings;
        public JobRequestController(IJobService jobService,
            ICommonService commonService,
            ICompanyService companyService,
            IStateService stateService,
            IJobGroupsService jobGroupsService,
            IJobRequestService jobRequestService,
            IAttachmentService attachmentService,
            IUserProfileService userProfileService,
            IViewRenderService viewRenderService,
            IOptionsSnapshot<AppSettings> options)
        {
            _jobService = jobService;
            _commonService = commonService;
            _companyService = companyService;
            _stateService = stateService;
            _jobGroupsService = jobGroupsService;
            _jobRequestService = jobRequestService;
            _attachmentService = attachmentService;
            _userProfileService = userProfileService;
            _viewRenderService = viewRenderService;
            _appSettings = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _jobRequestService.Get(new JobRequestFilter
            {
                PageNumber = 1,
                PageSize = 5,
                JobId = id
            });
            var model = new JobRequestFilter()
            {
                ContractTypes = _commonService.GetContractTypes(),
                Cities = await _stateService.GetGroupStates(),
                JobGroups = await _jobGroupsService.Get(),
                WorkExperienceYears = _commonService.GetWorkExperienceYears(),
                SalaryRequesteds = _commonService.GetSalaryRequesteds(),
                Latest = response,
                PageLengths = _commonService.GetPageLengths(),
                TotalRowNumber = response.FirstOrDefault()?.TotalRowCount,
                PageSize = 5,
                JobId = id
            };
            var jobs = await _companyService.GetJobs(new CompanyJobFilter
            {
                JobId = id
            });
            if (jobs.Any())
            {
                model.Job = jobs.FirstOrDefault();
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] JobRequestFilter filter)
        {
            var response = await _jobRequestService.Get(filter);
            return PartialView("_Get", response);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeStatus(int id, string modalId)
        {
            ViewBag.ModalId = modalId;
            var model = new JobRequestChangeStatus();
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus([FromBody] JobRequestChangeStatusSave request)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DownloadResumeh([FromBody] AttachmentDownloadRequest request)
        {
            var response = await _attachmentService.Download(request);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult DownloadSiteResumeh([FromBody] BaseIdRequest<int> request)
        {
            using (var converter = new Converter())
            using (var stream = new MemoryStream())
            {
                var url = $"{_appSettings.ApplicationAdminUrl}/UserProfile/Index?userId={request.Id}";
                converter.ConvertToPdf(new ConvertUri(url), stream, new PageSettings(PaperFormat.A4)
                {
                    DisplayHeaderFooter = false,
                    PrintBackground = true
                });

                return Ok(new AttachmentResponse
                {
                    FileData = Convert.ToBase64String(stream.ToArray()),
                    FileName = "Report.pdf",
                    IsValid = true
                });
            }
        }
    }
}
