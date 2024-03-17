using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ViewModel;
using ViewModel.UserEducation;
using ViewModel.UserJob;
using ViewModel.UserLanguage;

namespace Services
{
    public interface IUserLanguageService
    {
        Task<List<UserLanguageResponse>> Get(int? userId = null);
        Task<ResponseBase> Save(UserLanguageSave request);
        Task<ResponseBase> Delete(int id);
    }
    public class UserLanguageService : IUserLanguageService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserLanguage> _userLanguages;
        private readonly ICommonService _commonService;

        public UserLanguageService(ICommonService commonService,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork uow)
        {
            _commonService = commonService;
            _contextAccessor = httpContextAccessor;
            _uow = uow;
            _userLanguages = uow.Set<UserLanguage>();
        }
        public async Task<List<UserLanguageResponse>> Get(int? userId = null)
        {
            if(userId == null)
            {
                userId = GetCurrentUserId();
            }
           
            if (userId == 0)
            {
                return new List<UserLanguageResponse>();
            }
            var response = (from userLanguage in _userLanguages.Where(w => w.UserId == userId)
                            from language in _uow.Set<Entities.Language>().Where(w => w.Id == userLanguage.LanguageId)
                            from languageLevel in _uow.Set<LanguageLevel>().Where(w => w.Id == userLanguage.LevelId)
                            select new UserLanguageResponse()
                            {
                                Id = userLanguage.Id,
                                Language = language.Name,
                                Level = languageLevel.Name,
                                LanguageId = language.Id,
                                LevelId = languageLevel.Id
                            }).ToList();
            return response;
        }
        public async Task<ResponseBase> Save(UserLanguageSave request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return new ResponseBase(false);
            }

            if (request.Id > 0)
            {
                var find = await _userLanguages.FirstOrDefaultAsync(w => w.Id == request.Id && w.UserId == userId);
                if (find is null)
                {
                    return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
                }
                find.LanguageId = request.LanguageId;
                find.LevelId = request.LevelId;
            }
            else
            {
                _userLanguages.Add(new UserLanguage
                {
                    UserId = userId,
                    LanguageId = request.LanguageId,
                    LevelId = request.LevelId
                });
            }
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
        public async Task<ResponseBase> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return new ResponseBase(false);
            }
            var find = await _userLanguages.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (find is null)
            {
                return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
            }
            _userLanguages.Remove(find);
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }

        private int GetCurrentUserId()
        {
            var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
            var userId = userDataClaim?.Value;
            return string.IsNullOrWhiteSpace(userId) ? 0 : int.Parse(userId);
        }

    }
}
