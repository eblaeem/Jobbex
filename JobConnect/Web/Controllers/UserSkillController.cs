using Api.Helper;
using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;
using ViewModel.UserSoftwareSkill;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserSkillController : Controller
    {
        private readonly IUserSoftWareSkillsService _userSoftWareSkillsService;
        public UserSkillController(IUserSoftWareSkillsService userSoftWareSkillsService)
        {
            _userSoftWareSkillsService = userSoftWareSkillsService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var response = await _userSoftWareSkillsService.Get();
            return Ok(response);
        }

        [HttpPost("save")]
        public async Task<ResponseBase> Save([FromBody] UserSoftwareSkillSave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var response = await _userSoftWareSkillsService.Save(request);
            return response;
        }

        [HttpPost("delete")]
        public async Task<ResponseBase> Delete([FromBody] BaseIdRequest<int> request)
        {
            var response = await _userSoftWareSkillsService.Delete(request.Id);
            return response;
        }
    }
}
