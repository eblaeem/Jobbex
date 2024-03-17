using Api.Helper;
using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;
using ViewModel.UserJob;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserJobController : Controller
    {
        private readonly IUserJobService _userJobService;
        public UserJobController(IUserJobService userJobService)
        {
            _userJobService = userJobService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var response = await _userJobService.Get();
            return Ok(response);
        }

        [HttpPost("save")]
        public async Task<ResponseBase> Save([FromBody] UserJobSave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var response = await _userJobService.Save(request);
            return response;
        }

        [HttpPost("delete")]
        public async Task<ResponseBase> Delete([FromBody] BaseIdRequest<int> request)
        {
            var response = await _userJobService.Delete(request.Id);
            return response;
        }
    }
}
