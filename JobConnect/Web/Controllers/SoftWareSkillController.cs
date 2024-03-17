using Microsoft.AspNetCore.Mvc;
using Services;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SoftWareSkillController : Controller
    {
        private readonly ISoftWareSkillsService _softWareSkillsService;
        public SoftWareSkillController(ISoftWareSkillsService softWareSkillsService)
        {
            _softWareSkillsService = softWareSkillsService;
        }

        [HttpGet("getPopularTags")]
        public async Task<IActionResult> GetPopularTags()
        {
            var response = await _softWareSkillsService.GetPopularTags();
            return Ok(response);
        }
    }
}
