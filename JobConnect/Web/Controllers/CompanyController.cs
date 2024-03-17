using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel.Company;
using ViewModel.Job;
using ViewModel.User;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost("get")]
        public async Task<IActionResult> Get([FromBody] CompanyFilter filter)
        {
            var response = await _companyService.Get(filter);
            return Ok(response);
        }

        [HttpGet("groupsCompany")]
        public async Task<IActionResult> GroupsCompany()
        {
            var response = await _companyService.GetGroupsCompany();
            return Ok(response);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _companyService.Detail(id);
            return Ok(response);
        }
    }
}
