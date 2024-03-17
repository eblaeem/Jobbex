using Api.Helper;
using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;
using ViewModel.UserEducation;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserEducationController : Controller
    {
        private readonly IUserEducationService _userEducationService;
        public UserEducationController(IUserEducationService userEducationService)
        {
            _userEducationService = userEducationService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var response = await _userEducationService.Get();
            return Ok(response);
        }

        [HttpPost("save")]
        public async Task<ResponseBase> Save([FromBody] UserEducationSave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var response = await _userEducationService.Save(request);
            return response;
        }

        [HttpPost("delete")]
        public async Task<ResponseBase> Delete([FromBody] BaseIdRequest<int> request)
        {
            var response = await _userEducationService.Delete(request.Id);
            return response;
        }
    }
}
