using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel;
using ViewModel.UserAccessCode;

namespace Services
{
    public interface IUserAccessCodeService
    {
        Task<List<UserAccessCodeResponse>> Get(UserAccessCodeFilter filter);
        Task<List<int>> Get(int userId);
        Task<ResponseBase> Save(UserAccessCodeSave request);
    }
    public class UserAccessCodeService : IUserAccessCodeService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<UserAccessCode> _userAccessCodes;
        public UserAccessCodeService(IUnitOfWork uow)
        {
            _uow = uow;
            _userAccessCodes = _uow.Set<UserAccessCode>();
        }
        public async Task<List<UserAccessCodeResponse>> Get(UserAccessCodeFilter filter)
        {
            var query = from accessCode in _uow.Set<AccessCode>()
                        from userAccessCode in _userAccessCodes.Where(w => w.UserId == filter.UserId)
                        .Where(w => w.AccessCodeId == accessCode.Id).DefaultIfEmpty()
                        select new UserAccessCodeResponse
                        {
                            Id = userAccessCode.Id,
                            UserId = userAccessCode.UserId,
                            AccessCodeId = accessCode.Id,
                            AccessCodeName = accessCode.Name,
                            AccessCodeNumber = accessCode.Number,
                            Granted = userAccessCode != null ? true : false
                        };
            if (filter.AccessCodeId > 0)
            {
                query = query.Where(w => w.AccessCodeId == filter.AccessCodeId);
            }
            return await query.OrderBy(w=>w.AccessCodeNumber).ToListAsync();
        }
        public async Task<ResponseBase> Save(UserAccessCodeSave request)
        {
            var find = await _userAccessCodes.FirstOrDefaultAsync(w => w.UserId == request.UserId &&
                 w.AccessCodeId == request.AccessCodeId);
            if (request.Type == true)
            {
                if (find is null)
                {
                    _userAccessCodes.Add(new UserAccessCode
                    {
                        UserId = request.UserId,
                        AccessCodeId = request.AccessCodeId
                    });
                    await _uow.SaveChangesAsync();
                }
                return new ResponseBase(true);
            }
            _userAccessCodes.Remove(find);
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
        public async Task<List<int>> Get(int userId)
        {
            var query = from accessCode in _uow.Set<AccessCode>()
                        from userAccessCode in _userAccessCodes.Where(w => w.UserId == userId
                        && w.AccessCodeId == accessCode.Id)
                        select accessCode.Number;

            return await query.ToListAsync();
        }
    }
}
