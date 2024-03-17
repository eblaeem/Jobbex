using Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;
using ViewModel.JobRequest;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobRequestController : Controller
    {
        private readonly IJobRequestService _jobRequestService;
        public JobRequestController(IJobRequestService jobRequestService)
        {
            _jobRequestService = jobRequestService;
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromForm] JobRequestSave request)
        {
            var response = await _jobRequestService.Save(request);
            return Ok(response);
        }
    }
}
