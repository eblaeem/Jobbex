using Api.Helper;
using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;
using ViewModel.UserLanguage;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserLanguageController : Controller
    {
        private readonly IUserLanguageService _userLanguageService;
        public UserLanguageController(IUserLanguageService userLanguageService)
        {
            _userLanguageService = userLanguageService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var response = await _userLanguageService.Get();
            return Ok(response);
        }

        [HttpPost("save")]
        public async Task<ResponseBase> Save([FromBody] UserLanguageSave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var response = await _userLanguageService.Save(request);
            return response;
        }

        [HttpPost("delete")]
        public async Task<ResponseBase> Delete([FromBody] BaseIdRequest<int> request)
        {
            var response = await _userLanguageService.Delete(request.Id);
            return response;
        }
    }
}
