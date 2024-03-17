using Api.Helper;
using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;
using ViewModel.UserPriority;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserPriorityController : Controller
    {
        private readonly IUserPriorityService _userPriorityService;
        public UserPriorityController(IUserPriorityService userPriorityService)
        {
            _userPriorityService = userPriorityService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var response = await _userPriorityService.Get();
            return Ok(response);
        }

        [HttpPost("save")]
        public async Task<ResponseBase> Save([FromBody] UserPrioritySave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var response = await _userPriorityService.Save(request);
            return response;
        }
    }
}
