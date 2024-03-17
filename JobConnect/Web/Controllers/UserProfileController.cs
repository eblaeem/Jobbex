using Api.Helper;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Mvc;
using Services;
using ViewModel;
using ViewModel.User;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ICommonService _commonService;


        public UserProfileController(IUserProfileService userProfileService,
                ICommonService commonService)
        {
            _userProfileService = userProfileService;
            _commonService = commonService;
        }

        [HttpGet("get")]
        public async Task<UserProfile> Get()
        {
            var response = await _userProfileService.Get();
            return response;
        }


        [HttpGet("getResumeh")]
        public async Task<UserProfile> GetResumeh()
        {
            var response = await Get();
            if (response != null)
            {
                response.Cites = null;
                response.SalaryRequesteds = null;
                response.Genders = null;
                response.MilitaryStatus = null;
                response.MaritalStatus = null;
                response.Degrees = null;
                response.Languages = null;
                response.LanguageLevels = null;
                response.JobPositions = null;
                response.JobGroups = null;
                response.Skills = null;
            }
            return response;
        }
        [HttpPost("save")]
        public async Task<ResponseBase> Save([FromBody] UserProfile request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            request.Age = request.BirthDate.ToGregorianDateTime().Value.GetAge();
            var response = await _userProfileService.Save(request);
            return response;
        }

        [HttpPost("uploadProfile")]
        public async Task<ResponseBase> UploadProfile(IFormFile file, [FromForm] int? userProfileImageId)
        {
            var response = await _userProfileService.UploadProfile(userProfileImageId, file);
            return response;
        }


    }
}
