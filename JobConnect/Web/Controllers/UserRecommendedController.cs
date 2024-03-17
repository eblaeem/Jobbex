using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel.UserRecommended;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRecommendedController : Controller
    {
        private readonly IUserRecommendedService _userRecommendedService;
        public UserRecommendedController(IUserRecommendedService userRecommendedService)
        {
            _userRecommendedService = userRecommendedService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] UserRecommendedFilter filter)
        {
            var response = await _userRecommendedService.Get(filter);
            return Ok(response);
        }
    }
}
