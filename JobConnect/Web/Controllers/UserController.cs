using Api.Helper;
using DataLayer.Context;
using DNTCommon.Web.Core;
using DNTPersianUtils.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services;
using System.Security.Claims;
using ViewModel;
using ViewModel.Attachment;
using ViewModel.Setting;
using ViewModel.Sms;
using ViewModel.User;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IUnitOfWork _uow;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly ICaptchaService _captchaService;
        private readonly AppSettings _appSettings;
        private readonly IEmailSender _emailSender;
        private readonly ISmsService _smsSender;
        private readonly EmailConfig _emailConfig;
        private readonly IAttachmentService _attachmentService;
        private readonly ICommonService _commonService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserController(
                IUsersService usersService,
                ITokenStoreService tokenStoreService,
                ITokenFactoryService tokenFactoryService,
                IUnitOfWork uow,
                ICaptchaService captchaService,
                IOptionsSnapshot<AppSettings> appSetting,
                IOptionsSnapshot<EmailConfig> emailOptions,
                IEmailSender emailSender,
                ISmsService smsSender,
                IAttachmentService attachmentService,
                ICommonService commonService,
                IHttpContextAccessor httpContextAccessor)
        {
            _usersService = usersService;
            _tokenStoreService = tokenStoreService;
            _uow = uow;
            _tokenFactoryService = tokenFactoryService;
            _captchaService = captchaService;
            _appSettings = appSetting.Value;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _emailConfig = emailOptions.Value;
            _attachmentService = attachmentService;
            _commonService = commonService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        [BlockByIPAddress(Name = "Login", Seconds = 2)]
        [AllowAnonymous]
        public async Task<LoginResponse> Login([FromBody] Login request)
        {
            if (!ModelState.IsValid)
            {
                return new LoginResponse(false, HelperExtension.Errors(ModelState));
            }

            var captcha = new Captcha
            {
                CaptchaCode = request.Captcha.ToPersianNumbers(),
                ClientGuid = request.ClientGuid
            };
            if (_captchaService.Validate(captcha) == false)
            {
                return new LoginResponse(false, "کد امنیتی را به درستی وارد نمایید");
            }

            var user = await _usersService.FindUser(request.UserName, request.Password);
            if (user is null)
            {
                var message = "نام کاربری یا رمز عبور اشتباه می باشد";
                return new LoginResponse(false, message);
            }

            if (user.IsActive == false)
            {
                var message = "اکانت شما غیر فعال می باشد";
                return new LoginResponse(false, message);
            }

            if (user.EnableTwoStepVerification)
            {
                var rand = new Random();
                var token = new string(Enumerable.Repeat("123456789", 6).Select(s => s[rand.Next(s.Length)]).ToArray());

                await _emailSender.Send(new EmailSenderModel()
                {
                    Email = user.Email,
                    Title = "کد جدید اعتبارسنجی دو مرحله‌ای",
                    ViewPath = "~/Views/Shared/_TwoFactorSendCode.cshtml",
                    Data = new TwoFactorSendCode
                    {
                        Token = token,
                        EmailSignature = _emailConfig.FromAddress,
                        MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                    },
                });

                if (string.IsNullOrEmpty(user.PhoneNumber) == false)
                {
                    if (_appSettings.EnableTwoStepSmsVerification)
                    {
                        var smsResponse = await _smsSender.SendAsync(new SmsSenderModel()
                        {
                            MobileNumber = user.PhoneNumber,
                            Token = token,
                            Message = $"مشترک گرامی کد عددی جهت تایید{token} را وارد نمایید \n {_appSettings.ApplicationPersianName}",
                            RecordId = user.Id,
                            SmsNumber = _appSettings.SmsNumber,
                            SmsOutBoxTypeId = 14,
                            NationalCode = user.NationalCode,
                            DisplayName = user.DisplayName
                        });
                    }
                }

                var twoFactorResponse = await _usersService.UpdateTwoFactor(user.Id, token);

                return new LoginResponse()
                {
                    IsValid = true,
                    RedirectUrl = "/two-factor",
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    UserName = user.Username,
                    PhoneNumber = user.PhoneNumber
                };
            }

            var result = await _tokenFactoryService.CreateJwtTokensAsync(user);

            var ipAddress = _httpContextAccessor?.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            ipAddress ??= _httpContextAccessor?.HttpContext.Connection?.RemoteIpAddress?.ToString();
            var browserName = _commonService.
                            ReplaceAgentBrowserName(_httpContextAccessor?.HttpContext?.Request?.Headers["User-Agent"].ToString());

            await _tokenStoreService.AddUserTokenAsync(new AddUserTokenRequest
            {
                UserId = user.Id,
                AccessToken = result.AccessToken,
                RefreshTokenSerial = result.RefreshTokenSerial,
                RefreshTokenSourceSerial = Guid.NewGuid().ToString(),
                IpAddress = ipAddress,
                BrowserName = browserName
                //RefreshTokenSourceSerial = user.Username
            });

            await _uow.SaveChangesAsync();

            var redirectUrl = string.Empty;
            var userMustBeChanged = user.ChangePasswordDateTime.GetValueOrDefault();
            var maximumTimeValid = userMustBeChanged.AddMonths(3);
            if (maximumTimeValid < DateTime.Now)
            {
                return new LoginResponse()
                {
                    IsValid = true,
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    RedirectUrl = "/change-password",
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    UserName = user.Username,
                    PhoneNumber = user.PhoneNumber,
                    UserMustBeChanged = true
                };
            }

            return new LoginResponse()
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                IsValid = true,
                UserName = user.Username,
                DisplayName = user.DisplayName,
                RedirectUrl = redirectUrl,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserMustBeChanged = false,
                NationalCode = user.NationalCode,
            };
        }

        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<bool> Logout(string refreshToken)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdValue = claimsIdentity.FindFirst(ClaimTypes.UserData)?.Value;

            await _tokenStoreService.RevokeUserBearerTokensAsync(userIdValue, refreshToken);
            await _uow.SaveChangesAsync();
            return true;
        }

        [HttpPost("create")]
        [BlockByIPAddress(Name = "Create", Seconds = 2)]
        [AllowAnonymous]
        public async Task<CreateUserResponse> Create([FromBody] CreateUser request)
        {
            if (!ModelState.IsValid)
            {
                return new CreateUserResponse(false, HelperExtension.Errors(ModelState));
            }
            if (request.UserName.ContainsFarsi(false))
            {
                return new CreateUserResponse(false, "نام کاربری نمی تواند شامل حروف فارسی باشد");
            }

            var captcha = new Captcha
            {
                CaptchaCode = request.Captcha.ToPersianNumbers(),
                ClientGuid = request.ClientGuid
            };
            if (_captchaService.Validate(captcha) == false)
            {
                return new CreateUserResponse(false, "کد امنیتی را به درستی وارد نمایید");
            }
            var result = await _usersService.Create(request);
            return new CreateUserResponse(result.IsValid, result.Message);
        }

        [HttpGet("validateUserName")]
        [AjaxOnly]
        [AllowAnonymous]
        public async Task<ResponseBase> ValidateUserName(string input)
        {
            var result = await _usersService.ValidateUserName(input);
            return result;
        }

        [HttpPost("twoFactor")]
        [AllowAnonymous]
        public async Task<LoginResponse> TwoFactor([FromBody] TwoFactorModel request)
        {
            if (!ModelState.IsValid)
            {
                return new LoginResponse(false, HelperExtension.Errors(ModelState));
            }
            var result = await _usersService.ValidateTwoFactor(request.UserName, request.Token);
            if (result.IsValid == true)
            {
                var user = await _usersService.FindUser(result.Id.Value);
                if (user != null)
                {
                    var jwtTokens = await _tokenFactoryService.CreateJwtTokensAsync(user);
                    await _tokenStoreService.AddUserTokenAsync(new AddUserTokenRequest
                    {
                        UserId = user.Id,
                        AccessToken = jwtTokens.AccessToken,
                        RefreshTokenSerial = jwtTokens.RefreshTokenSerial,
                        RefreshTokenSourceSerial = "123654"
                    });
                    await _uow.SaveChangesAsync();

                    return new LoginResponse()
                    {
                        AccessToken = jwtTokens.AccessToken,
                        RefreshToken = jwtTokens.RefreshToken,
                        IsValid = true,
                        UserName = user.Username,
                        DisplayName = user.DisplayName
                    };
                }
            }
            return new LoginResponse()
            {
                IsValid = result.IsValid,
                Message = result.Message
            };
        }

        [HttpGet("captcha")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        [AllowAnonymous]
        public ResponseBase Captcha([FromQuery] Captcha request)
        {
            var result = _captchaService.GetImage(request.ClientGuid);
            return string.IsNullOrEmpty(result) == false
                ? new ResponseBase(true, result)
                : new ResponseBase(false, "کد امنیتی ایجاد نشد!");
        }


        [HttpPost("refreshToken")]
        [AllowAnonymous]
        public async Task<LoginResponse> RefreshToken([FromBody] Token model)
        {
            var refreshTokenValue = model.RefreshToken;
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return new LoginResponse(false, "refreshToken is not set.");
            }

            var token = await _tokenStoreService.FindTokenAsync(refreshTokenValue);
            if (token == null)
            {
                return new LoginResponse(false, "Unauthorized");
            }

            var result = await _tokenFactoryService.CreateJwtTokensAsync(token.User);
            await _tokenStoreService.AddUserTokenAsync(new AddUserTokenRequest()
            {
                UserId = token.User.Id,
                AccessToken = result.AccessToken,
                RefreshTokenSerial = result.RefreshTokenSerial,
                RefreshTokenSourceSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue)
            });
            await _uow.SaveChangesAsync();

            var response = new LoginResponse()
            {
                IsValid = true,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            };
            return response;
        }


        [HttpPost("changePassword")]
        public async Task<ResponseBase> ChangePassword([FromBody] ChangePasswordSave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var response = await _usersService.ChangePassword(request);
            return response;
        }

        [HttpPost("UploadFile")]
        public async Task<ResponseBase> UploadFile([FromForm] int? attachmentTypeId, IFormFile file)
        {
            var user = await _usersService.GetCurrentUser();
            if (user == null)
            {
                return new ResponseBase(false, "مشتری مورد نظر وجود ندارد");
            }

            var response = await _attachmentService.Save(new AttachmentSave()
            {
                AttachmentTypeId = attachmentTypeId,
                FormFile = file,
                ImageOrPdf = true,
                FileName = file.FileName,
                FileType = file.ContentType,
                FileSize = Convert.ToInt32(file.Length),
            });

            return response;
        }

        [HttpPost("forgottenPassword")]
        [AllowAnonymous]
        public async Task<ResponseBase> ForgottenPassword([FromBody] ForgottenPasswordSave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var captcha = new Captcha
            {
                CaptchaCode = request.Captcha.ToPersianNumbers(),
                ClientGuid = request.ClientGuid
            };
            if (_captchaService.Validate(captcha) == false)
            {
                return new ResponseBase(false, "کد امنیتی را به درستی وارد نمایید");
            }
            var response = await _usersService.ForgottenPassword(request);
            if (response.IsValid == true)
            {
                var token = response.Message;
                if (string.IsNullOrEmpty(response.Email) == false)
                {
                    var email = new EmailSenderModel()
                    {
                        Email = response.Email,
                        Title = "لطفا رمز عبور جدید خود را تائید کنید",
                        ViewPath = "~/Views/Shared/_ResetPassword.cshtml",
                        Data = new ResetPasswordModel
                        {
                            UserName = response.UserName,
                            ApplicatioWebUrl = _appSettings.ApplicatioWebUrl,
                            ApplicationName = _appSettings.ApplicationPersianName,
                            EmailConfirmationToken = token,
                            MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString()
                        }
                    };
                    await _emailSender.Send(email);
                    response.Message = "ایمیل ریست رمز عبور برای شما ارسال گردید";
                }
                if (string.IsNullOrEmpty(response.MobileNUmber) == false)
                {
                    var smsResponse = await _smsSender.SendAsync(new SmsSenderModel()
                    {
                        MobileNumber = response.MobileNUmber,
                        Token = response.Message,
                        Message = $"لینک رمز عبور: {_appSettings.ApplicatioWebUrl}/reset-password/{response.UserName}/{token}",
                        SmsNumber = _appSettings.SmsNumber,
                        RecordId = response.UserId,
                        SmsOutBoxTypeId = 12,
                        NationalCode = response.NationalCode,
                        DisplayName = response.DisplayName,
                    });

                    response.Message = smsResponse.Message;
                }
            }
            return response;
        }

        [HttpPost("confirmResetPassword")]
        [AllowAnonymous]
        public async Task<ResponseBase> ConfirmResetPassword([FromBody] ConfirmResetPasswordSave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var response = await _usersService.ResetPasswordValidate(request.UserName, request.Token);
            return response;
        }

        [HttpPost("resetPassword")]
        [AllowAnonymous]
        public async Task<ResponseBase> ResetPassword([FromBody] ResetPasswordSave request)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseBase(false, HelperExtension.Errors(ModelState));
            }
            var response = await _usersService.ResetPassword(request);
            return response;
        }


        [HttpGet("userTokens")]
        public async Task<List<UserTokenResponse>> UserTokens()
        {
            var response = await _usersService.GetUserTokens();
            return response;
        }
    }
}
