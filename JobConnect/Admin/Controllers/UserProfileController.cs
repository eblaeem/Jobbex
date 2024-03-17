using Azure;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Admin.Controllers
{
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        public async Task<IActionResult> Index(int userId)
        {
            var response = await _userProfileService.GetByUserId(userId);
            return View("_Get",response);
        }
    }
}
