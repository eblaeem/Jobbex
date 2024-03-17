using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ViewModel;
using ViewModel.Company;
using ViewModel.Setting;
using ViewModel.User;

namespace Services
{
    public interface IUsersService
    {
        Task<List<UserResponse>> Get(UserFilter request);
        Task<string> GetSerialNumber(int userId);
        Task<User> FindUser(string username, string password);
        Task<UserCompanyResponse> FindUserCompany(string userName, string password);
        ValueTask<User> FindUser(int userId);
        Task<ForgottenPasswordResponse> ForgottenPassword(ForgottenPasswordSave request);
        Task<ResponseBase> ResetPasswordValidate(string userName, string token);

        Task UpdateUserLastActivityDate(int userId);
        ValueTask<User> GetCurrentUser();
        int GetCurrentUserId();
        Task<ResponseBase> Create(CreateUser user);
        Task<ResponseBase> CreateCompanyUser(CreateUserAdmin request);

        Task<ResponseBase> ChangePassword(ChangePasswordSave request);
        Task<ResponseBase> ResetPassword(ResetPasswordSave request);
        Task<ResponseBase> UpdateTwoFactor(int userId, string token);
        Task<ResponseBase> ValidateTwoFactor(string userName, string token);
        Task<ResponseBase> ValidateUserName(string userName);

        Task<string> GetCurrentNationalCode();

        Task<List<LabelValue>> GetUsers();
        Task<ResponseBase> UserIsAdmin(int userId);
        Task<User> CurrentUserIsAdmin();
        List<LabelValue> GetUserRoles(int userId);
        Task<List<LabelValue>> Search(string term);
        Task<List<UserTokenResponse>> GetUserTokens();
        Task<List<UserAdminResponse>> GetAdminUsers();
        Task<int> Count();
    }

    public class UsersService : IUsersService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<User> _users;
        private readonly DbSet<UserToken> _usersTokens;
        private readonly DbSet<UserRole> _usersRoles;
        private readonly DbSet<UserEducation> _userEducations;
        private readonly ISecurityService _securityService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICommonService _commonService;
        private readonly AppSettings _appSettings;
        private readonly EmailConfig _emailOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailSender _emailSender;
        public UsersService(
            IUnitOfWork uow,
            IHttpContextAccessor contextAccessor,
            ICommonService commonService,
            IOptionsSnapshot<AppSettings> options,
            IOptionsSnapshot<EmailConfig> emailOptions,
            IServiceProvider serviceProvider,
            IEmailSender emailSender,
            ISecurityService securityService)
        {
            _uow = uow;
            _users = _uow.Set<User>();
            _usersTokens = _uow.Set<UserToken>();
            _usersRoles = _uow.Set<UserRole>();
            _userEducations = _uow.Set<UserEducation>();
            _contextAccessor = contextAccessor;
            _commonService = commonService;
            _appSettings = options.Value;
            _serviceProvider = serviceProvider;
            _emailOptions = emailOptions.Value;
            _emailSender = emailSender;
            _securityService = securityService;
        }
        public async ValueTask<User> FindUser(int userId)
        {
            return await _users.FindAsync(userId);
        }

        public async Task<User> FindUser(string username, string password)
        {
            var passwordHash = _securityService.GetSha256Hash(password);
            var result = await _users.FirstOrDefaultAsync(x => (x.Username == username) && x.Password == passwordHash);
            return result;
        }
        public async Task<UserCompanyResponse> FindUserCompany(string username, string password)
        {
            var passwordHash = _securityService.GetSha256Hash(password);
            var user = await _users.FirstOrDefaultAsync(x => (x.Username == username) && x.Password == passwordHash);

            if (user is null)
            {
                var message = "نام کاربری یا رمز عبور اشتباه می باشد";
                return new UserCompanyResponse(false, message);
            }
            if (user.IsActive == false)
            {
                var message = "اکانت شما غیر فعال می باشد";
                return new UserCompanyResponse(false, message);
            }

            var company = await _uow.Set<Company>().FirstOrDefaultAsync(w => w.UserId == user.Id);
            if (company == null)
            {
                return new UserCompanyResponse(false, "شرکت مورد نظر وجود ندارد");
            }
            return new UserCompanyResponse
            {
                IsValid = true,
                CompanyId = company.Id,
                DisplayName = user.DisplayName ?? company.Title,
                UserId = user.Id,
                UserName = user.Username,
                SerialNumber = user.SerialNumber,
            };
        }

        public async Task<string> GetSerialNumber(int userId)
        {
            var user = await FindUser(userId);
            return user.SerialNumber;
        }

        public async Task UpdateUserLastActivityDate(int userId)
        {
            var user = await FindUser(userId);
            if (user.LastLoggedIn != null)
            {
                var updateLastActivityDate = TimeSpan.FromMinutes(2);
                var currentUtc = DateTimeOffset.UtcNow;
                var timeElapsed = currentUtc.Subtract(user.LastLoggedIn.Value);
                if (timeElapsed < updateLastActivityDate)
                {
                    return;
                }
            }
            user.LastLoggedIn = DateTime.Now;
            await _uow.SaveChangesAsync();
        }

        public int GetCurrentUserId()
        {
            var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
            var userId = userDataClaim?.Value;
            return string.IsNullOrWhiteSpace(userId) ? 0 : int.Parse(userId);
        }

        public ValueTask<User> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            return FindUser(userId);
        }
        public async Task<ResponseBase> Create(CreateUser request)
        {
            request.UserName = request.UserName.Trim().ToEnglishNumbers();

            var find = _users.FirstOrDefault(f => f.Username == request.UserName);
            if (find != null)
            {
                return new ResponseBase(false, "کاربر مورد نظر وجود دارد");
            }

            var userName = request.UserName.RemoveAllWhitespaces();
            var user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                LastLoggedIn = DateTime.Now,
                Email = request.Email,
                DisplayName = $"{request.FirstName} {request.LastName}",
                IsActive = true,
                PhoneNumber = request.PhoneNumber,
                Username = userName,
                Password = _securityService.GetSha256Hash(request.Password),
                SerialNumber = Guid.NewGuid().ToString("N"),
                DateTime = DateTime.Now,
                ChangePasswordDateTime = DateTime.Now
            };
            _users.Add(user);
            await _uow.SaveChangesAsync();

            _usersRoles.Add(new UserRole
            {
                RoleId = 1,
                UserId = user.Id
            });
            await _uow.SaveChangesAsync();

            if (_appSettings.EnableEmailConfirmation && string.IsNullOrEmpty(request.Email) == false)
            {
                var token = _commonService.GenerateToken();
                await _emailSender.Send(new EmailSenderModel()
                {
                    Email = request.Email,
                    Title = "لطفا اکانت خود را تائید کنید",
                    ViewPath = "~/Views/Shared/_RegisterEmailConfirmation.cshtml",
                    Data = new RegisterEmailConfirmation
                    {
                        User = user,
                        EmailConfirmationToken = token,
                        EmailSignature = _emailOptions.FromAddress,
                        MessageDateTime = DateTime.UtcNow.ToLongPersianDateTimeString(),
                        ApplicatioWebUrl = _appSettings.ApplicationPersianName
                    }
                });

                return new CreateUserResponse()
                {
                    IsValid = true,
                    Message = " ایمیلی برای شما ارسال گردید لطفا اکانت خود را تایید نمایید.",
                };
            }

            return new ResponseBase(true, string.Empty);
        }
        public async Task<ResponseBase> CreateCompanyUser(CreateUserAdmin request)
        {
            if (request.UserName.ContainsFarsi(false))
            {
                return new CreateUserResponse(false, "نام کاربری نمی تواند شامل حروف فارسی باشد");
            }

            var find = _users.FirstOrDefault(f => f.Username == request.UserName);
            if (find != null)
            {
                _uow.Set<UserRole>().Add(new UserRole
                {
                    UserId = find.Id,
                    RoleId = 2
                });
                await _uow.SaveChangesAsync();

                return new ResponseBase(true)
                {
                    Id = find.Id
                };
            }

            if (string.IsNullOrEmpty(request.PhoneNumber) == false)
            {
                if (_users.FirstOrDefault(w => w.PhoneNumber == request.PhoneNumber) != null)
                {
                    return new ResponseBase(false, "کاربری با شماره همراه مورد نظر وجود دارد");
                }
            }
            var user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                DisplayName = $"{request.FirstName} {request.LastName}",
                IsActive = true,
                PhoneNumber = request.PhoneNumber,
                Username = request.UserName,
                Password = _securityService.GetSha256Hash(request.Password),
                SerialNumber = Guid.NewGuid().ToString("N"),
                DateTime = DateTime.Now
            };
            _users.Add(user);
            await _uow.SaveChangesAsync();

            _uow.Set<Company>().Add(new Company
            {
                UserId = user.Id,
                Phone = user.PhoneNumber,
                Title = $"{request.FirstName} {request.LastName}"
            });

            _uow.Set<UserRole>().Add(new UserRole
            {
                UserId = user.Id,
                RoleId = 2
            });

            await _uow.SaveChangesAsync();

            return new ResponseBase(true)
            {
                Id = user.Id
            };
        }
        public async Task<ForgottenPasswordResponse> ForgottenPassword(ForgottenPasswordSave request)
        {
            var user = await _users.FirstOrDefaultAsync(x => x.Email == request.Email);
            if (user == null)
            {
                return new ForgottenPasswordResponse(false, "ایمیل مورد نظر وجود ندارد");
            }
            user.ResetPasswordToken = GenerateToken();
            await _uow.SaveChangesAsync();
            return new ForgottenPasswordResponse(true, user.ResetPasswordToken)
            {
                UserName = user.Username,
                Email = user.Email,
                MobileNUmber = user.PhoneNumber,
                UserId = user.Id,
                DisplayName = user.DisplayName,
                NationalCode = user.NationalCode
            };
        }
        public async Task<ResponseBase> ResetPasswordValidate(string userName, string token)
        {
            var user = await _users.FirstOrDefaultAsync(w => w.Username == userName
            || w.NationalCode == userName);
            if (user == null)
            {
                return new ResponseBase(false, "نام کاربری مورد نظر وجود ندارد");
            }

            if (user.IsActive == false)
            {
                return new ResponseBase(false, "اکانت مورد نظر غیر فعال شده است");
            }
            if (user.ResetPasswordToken != token)
            {
                return new ResponseBase(false, "اطلاعات ارسال شده معتبر نمی باشد");
            }
            return new ResponseBase(true);
        }
        public async Task<ResponseBase> ChangePassword(ChangePasswordSave request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return new ResponseBase(false, "رمز عبور  با تکرار رمز عبور یکسان نمی باشد ");
            }
            var user = await GetCurrentUser();

            var currentPasswordHash = _securityService.GetSha256Hash(request.CurrentPassword);
            if (user.Password != currentPasswordHash)
            {
                return new ResponseBase(false, "رمز عبور شما اشتباه می باشد");
            }
            user.Password = _securityService.GetSha256Hash(request.NewPassword);
            user.SerialNumber = Guid.NewGuid().ToString("N");
            user.ChangePasswordDateTime = DateTime.Now;
            await _uow.SaveChangesAsync();
            return new ResponseBase(true, string.Empty);
        }
        public async Task<ResponseBase> ResetPassword(ResetPasswordSave request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return new ResponseBase(false, "رمز عبور با تکرار رمز عبور یکسان نمی باشد");
            }
            var validate = await ResetPasswordValidate(request.UserName, request.Token);
            if (validate.IsValid == false)
            {
                return validate;
            }

            var user = await _users.FindAsync(request.UserName);
            user.Password = _securityService.GetSha256Hash(request.NewPassword);
            await _uow.SaveChangesAsync();
            return new ResponseBase(true, string.Empty);
        }
        public async Task<ResponseBase> UpdateTwoFactor(int userId, string token)
        {
            var find = _users.Find(userId);
            if (find == null)
            {
                return new ResponseBase(false, "کاربر مورد نظر وجود ندارد");
            }
            find.TwoFactorCode = token;
            await _uow.SaveChangesAsync();
            return new ResponseBase(true, string.Empty);
        }

        public async Task<ResponseBase> ValidateTwoFactor(string userName, string token)
        {
            var user = _users.FirstOrDefault(f => f.Username == userName);
            if (user == null)
            {
                return new ResponseBase(false, "کاربر مورد نظر وجود ندارد");
            }
            if (user.TwoFactorCode != token)
            {
                return new ResponseBase(false, "کد وارد شده معتبر نمی باشد");
            }
            user.IsActive = true;
            await _uow.SaveChangesAsync();
            return new ResponseBase(true) { Id = user.Id };
        }

        public async Task<ResponseBase> ValidateUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return new ResponseBase(false, "کاربر مورد نظر وجود دارد");
            }
            var user = await _users.FirstOrDefaultAsync(f => f.Username == userName);
            if (user != null)
            {
                return new ResponseBase(false, "کاربر مورد نظر وجود دارد");
            }
            return new ResponseBase(true);
        }

        public async Task<string> GetCurrentNationalCode()
        {
            var user = await GetCurrentUser();
            return user is not null ? user.NationalCode : string.Empty;
        }

        public async Task<List<LabelValue>> GetUsers()
        {
            return await _users.Select(s => new LabelValue()
            {
                Label = s.DisplayName,
                Value = s.Id
            }).ToListAsync();
        }

        public async Task<List<UserResponse>> Get(UserFilter filter)
        {
            var query = from user in _users.Where(c => c.Hidden == false).TagWith("OptionRecompile")
                        select new UserResponse
                        {
                            Id = user.Id,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            IsActive = user.IsActive,
                            NationalCode = user.NationalCode,
                            PhoneNumber = user.PhoneNumber,
                            Username = user.Username,
                            DateTime = user.DateTime
                        };

            var fromDate = filter.FromDate.ToGregorianDateTime();
            var toDate = filter.ToDate.ToGregorianDateTime();
            if (fromDate is not null)
            {
                query = query.Where(w => w.DateTime.Value.Date >= fromDate.Value.Date);
            }
            if (toDate is not null)
            {
                query = query.Where(w => w.DateTime.Value.Date <= toDate.Value.Date);
            }

            if (filter.UserId > 0)
            {
                query = query.Where(w => w.Id == filter.UserId);
            }

            if (string.IsNullOrEmpty(filter.PhoneNumber) == false)
            {
                query = query.Where(w => w.PhoneNumber.Contains(filter.PhoneNumber.Trim()));
            }


            if (string.IsNullOrEmpty(filter.Username) == false)
            {
                query = query.Where(w => w.Username.Contains(filter.Username.Trim()));
            }
            if (string.IsNullOrEmpty(filter.NationalCode) == false)
            {
                query = query.Where(w => w.NationalCode.Contains(filter.NationalCode.Trim()));
            }

            if (string.IsNullOrEmpty(filter.Sort))
            {
                filter.Sort = "id desc";
            }
            if (filter.Sort.Contains("DateString"))
            {
                filter.Sort = filter.Sort.Replace("DateString", "DateTime");
            }

            query = query.OrderBy(filter.Sort);

            var totalRowCount = await query.CountAsync();
            if (totalRowCount <= 0)
            {
                return new List<UserResponse>();
            }

            var response = await query.Skip(filter.PageNumber * filter.PageSize).Take(filter.PageSize).ToListAsync();
            if (response.Any() == false)
            {
                return new List<UserResponse>();
            }
            response.FirstOrDefault().TotalRowCount = totalRowCount;
            foreach (var item in response)
            {
                item.DateString = item.DateTime.ToShortPersianDateTimeString();
            }
            return response;
        }

        public async Task<ResponseBase> UserIsAdmin(int userId)
        {
            var find = await _uow.Set<UserRole>().AnyAsync(a => a.UserId == userId && a.RoleId == 1);
            if (!find)
            {
                return new ResponseBase(false, "دسترسی به این بخش را ندارید");
            }
            return new ResponseBase(true);
        }

        public async Task<User> CurrentUserIsAdmin()
        {
            var user = await GetCurrentUser();
            if (user == null)
            {
                return null;
            }
            var find = await _uow.Set<UserRole>().AnyAsync(a => a.UserId == user.Id && a.RoleId == 1);
            return find == false ? null : user;
        }

        public List<LabelValue> GetUserRoles(int userId)
        {
            var response = (from userRole in _uow.Set<UserRole>().Where(c => c.UserId == userId)
                            from role in _uow.Set<Role>().Where(c => c.Id == userRole.RoleId)
                            select new LabelValue
                            {
                                Label = role.Name,
                                Value = userRole.RoleId
                            }
                       ).ToList();
            return response;
        }

        public async Task<List<LabelValue>> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return new List<LabelValue>();
            }
            term = term.Trim().ApplyCorrectYeKe();
            return await _users.Where(w => w.DisplayName.Contains(term) || w.NationalCode.Contains(term)
                            || w.Username.Contains(term))
               .Select(s => new LabelValue()
               {
                   Label = $"{s.DisplayName}({s.NationalCode})",
                   Value = s.Id
               }).Take(10).ToListAsync();
        }
        public async Task<List<UserTokenResponse>> GetUserTokens()
        {
            var userTokens = new List<UserTokenResponse>();
            var user = await GetCurrentUser();
            foreach (var item in _usersTokens.Where(x => x.UserId == user.Id).ToList())
            {
                userTokens.Add(new UserTokenResponse
                {
                    IpAddress = item.IpAddress,
                    BrowserName = item.BrowserName,
                    AccessTokenExpiresDateTime = item.AccessTokenExpiresDateTime.ToShortPersianDateTimeString(),
                });
            }
            return userTokens;
        }
        public async Task<List<UserAdminResponse>> GetAdminUsers()
        {
            var response = await (from userRole in _uow.Set<UserRole>().Where(w => w.RoleId == 1)
                                  from user in _users.Where(w => w.Id == userRole.UserId).
                                  Where(w => w.Username != "admin")
                                  select new UserAdminResponse
                                  {
                                      DisplayName = $"{user.DisplayName} ({user.NationalCode})",
                                      Id = user.Id,
                                      NationalCode = user.NationalCode,
                                      UserName = user.Username
                                  }).ToListAsync();

            return response;
        }

        public async Task<int> Count()
        {
            return await _users.CountAsync();
        }
        private string GenerateToken()
        {
            var rand = new Random();
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 15)
                .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}
