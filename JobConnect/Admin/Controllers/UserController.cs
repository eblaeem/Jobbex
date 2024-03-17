using Admin.Helper;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;
using ViewModel;
using ViewModel.Company;
using ViewModel.User;

namespace Admin.Controllers
{
    public class UserController : Controller
    {
        public readonly IUsersService _usersService;
        private readonly ICaptchaService _captchaService;
        public UserController(IUsersService usersService,
            ICaptchaService captchaService)
        {
            _usersService = usersService;
            _captchaService = captchaService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            var model = new CreateLoginAdmin();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] CreateLoginAdmin request)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new LoginResponse(false, HelperExtension.Errors(ModelState)));
            }

            var captcha = new Captcha
            {
                CaptchaCode = request.Captcha.ToPersianNumbers(),
                ClientGuid = request.ClientGuid
            };
            if (_captchaService.Validate(captcha) == false)
            {
                return Ok(new LoginResponse(false, "کد امنیتی را به درستی وارد نمایید"));
            }

            var response = await _usersService.FindUserCompany(request.UserName, request.Password);
            if (response.IsValid == false)
            {
                return Ok(response);
            }

            SignIn(response);

            return Ok(new ResponseBase(true));
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new CreateUserAdmin();
            return View(model);
        }

        public async Task<IActionResult> Register([FromBody] CreateUserAdmin request)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new CreateUserResponse(false, HelperExtension.Errors(ModelState)));
            }
            if (request.UserName.ContainsFarsi(false))
            {
                return Ok(new CreateUserResponse(false, "نام کاربری نمی تواند شامل حروف فارسی باشد"));
            }
            var response = await _usersService.CreateCompanyUser(request);
            return Ok(response);
        }

        [HttpPost()]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        [AllowAnonymous]
        public ResponseBase Captcha([FromBody] Captcha request)
        {
            var result = _captchaService.GetImage(request.ClientGuid);
            return string.IsNullOrEmpty(result) == false
                ? new ResponseBase(true, result)
                : new ResponseBase(false, "کد امنیتی ایجاد نشد!");
        }


        private void SignIn(UserCompanyResponse response)
        {
            var claims = new List<Claim>
                {
                      new Claim(ClaimTypes.NameIdentifier, response.UserName.ToString()),
                      new Claim(ClaimTypes.Name, response.DisplayName),
                      new Claim("CompanyId", response.CompanyId.ToString()),
                      new Claim("UserId", response.UserId.ToString()),
                      new Claim(ClaimTypes.SerialNumber, response.SerialNumber),
                      new Claim(ClaimTypes.UserData, response.UserId.ToString()),
                };
            var cookieClaims = new ClaimsPrincipal(new ClaimsIdentity(claims, "Login1"));
            var expiresUtchour = 7.5;
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cookieClaims,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddHours(expiresUtchour),
                    AllowRefresh = true
                }).ConfigureAwait(false);
        }
    }
}
