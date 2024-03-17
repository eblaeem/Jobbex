using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Table.PivotTable;
using Services;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonController : Controller
    {
        private readonly ICommonService _commonService;
        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet("setting")]
        public async Task<IActionResult> Setting()
        {
            return Json(new
            {
                PageLengths = _commonService.GetPageLengths(),
                SortingTypes = _commonService.GetSortingTypes()
            });
        }
    }
}
