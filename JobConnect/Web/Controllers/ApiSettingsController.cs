using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ViewModel.Setting;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiSettingsController : Controller
    {
        private readonly IOptionsSnapshot<AppSettings> _apiSettingsConfig;

        public ApiSettingsController(IOptionsSnapshot<AppSettings> apiSettingsConfig)
        {
            _apiSettingsConfig = apiSettingsConfig;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_apiSettingsConfig.Value);
        }
    }
}
