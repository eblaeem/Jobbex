using Microsoft.AspNetCore.Mvc;
using Services;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CitiesController : Controller
    {
        private readonly IStateService _stateService;
        public CitiesController(IStateService stateService)
        {
            _stateService = stateService;
        }
        [HttpGet("get")]
        public async Task<IActionResult> Get(string term)
        {
            var response = await _stateService.GetGroupStates();
            if (response.Any())
            {
                response = response.Where(w => w.Label.Contains(term)).ToList();
            }
            return Ok(response);
        }
    }
}
