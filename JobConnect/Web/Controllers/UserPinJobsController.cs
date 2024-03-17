using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserPinJobsController : Controller
    {
        private readonly IUserPinJobsService _userPinJobsService;

        public UserPinJobsController(IUserPinJobsService userPinJobsService)
        {
            _userPinJobsService = userPinJobsService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get([FromQuery] PagingModel model,CancellationToken cancellationToken)
        {
            var result =await _userPinJobsService.Get(model, cancellationToken);
            return Ok(result);
        }

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] BaseIdRequest<int> req, CancellationToken cancellationToken)
        {
            var response = await _userPinJobsService.Save(req.Id, cancellationToken);
            return Ok(response);
        }
        
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery] int id, CancellationToken cancellationToken)
        {
            var response = await _userPinJobsService.Delete(id, cancellationToken);
            return Ok(response);
        }
    }
}
