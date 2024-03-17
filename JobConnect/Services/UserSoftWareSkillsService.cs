using DataLayer.Context;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ViewModel;
using ViewModel.UserEducation;
using ViewModel.UserLanguage;
using ViewModel.UserSoftwareSkill;

namespace Services
{

    public interface IUserSoftWareSkillsService
    {
        Task<List<UserSoftwareSkillResponse>> Get(int? userId = null);
        Task<ResponseBase> Save(UserSoftwareSkillSave request);
        Task<ResponseBase> Delete(int id);
    }
    public class UserSoftWareSkillsService : IUserSoftWareSkillsService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserSoftwareSkill> _userSoftwareSkills;
        private readonly ICommonService _commonService;

        public UserSoftWareSkillsService(ICommonService commonService,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork uow)
        {
            _commonService = commonService;
            _contextAccessor = httpContextAccessor;
            _uow = uow;
            _userSoftwareSkills = uow.Set<UserSoftwareSkill>();
        }
        public async Task<List<UserSoftwareSkillResponse>> Get(int? userId = null)
        {
            if (userId == null)
            {
                userId = GetCurrentUserId();
            }
            if (userId == 0)
            {
                return new List<UserSoftwareSkillResponse>();
            }
            var softWareLevels = _commonService.GetSkillLevels();
            var response = (from userSoftwareSkill in _userSoftwareSkills.Where(w => w.UserId == userId)
                            from softwareSkill in _uow.Set<Entities.SoftwareSkill>().Where(w => w.Id == userSoftwareSkill.SoftwareSkillId)
                            select new
                            {
                                userSoftwareSkill,
                                softwareSkill
                            }).ToList().Select(item => new UserSoftwareSkillResponse
                            {
                                Id = item.userSoftwareSkill.Id,
                                SoftwareSkillId = item.softwareSkill.Id,
                                SoftwareSkillName = item.softwareSkill.Name,
                                LevelId = softWareLevels.FirstOrDefault(w => w.Value.Value == item.userSoftwareSkill.LevelId).Value,
                                LevelName = softWareLevels.FirstOrDefault(w => w.Value.Value == item.userSoftwareSkill.LevelId).Label
                            }).ToList();
            return response;
        }
        public async Task<ResponseBase> Save(UserSoftwareSkillSave request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return new ResponseBase(false);
            }

            if (request.Id > 0)
            {
                var find = await _userSoftwareSkills.FirstOrDefaultAsync(w => w.Id == request.Id && w.UserId == userId);
                if (find is null)
                {
                    return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
                }
                find.SoftwareSkillId = request.SoftwareSkillId;
                find.LevelId = request.LevelId;
            }
            else
            {
                _userSoftwareSkills.Add(new UserSoftwareSkill
                {
                    UserId = userId,
                    SoftwareSkillId = request.SoftwareSkillId,
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
            var find = await _userSoftwareSkills.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            if (find is null)
            {
                return new ResponseBase(false, "درخواست مورد نظر وجود ندارد");
            }
            _userSoftwareSkills.Remove(find);
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
