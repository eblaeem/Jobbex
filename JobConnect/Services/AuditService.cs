using DataLayer.Context;
using DNTPersianUtils.Core;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel.Audit;

namespace Services
{
    public interface IAuditService
    {
        Task<List<AuditResponse>> Get(AuditFilter filter);
    }
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Audit> _audits;
        public AuditService(IUnitOfWork unitOfWork)
        {
            _uow = unitOfWork;
            _audits = _uow.Set<Audit>();
        }
        public async Task<List<AuditResponse>> Get(AuditFilter filter)
        {
            var keyValues = "{\"Id\":" + filter.Id + "}";
            var query = from audit in _audits.Where(w => w.KeyValues == keyValues && w.TableName == filter.TableName)
                        from user in _uow.Set<User>().Where(w => w.Id == audit.UserId)
                        select new AuditResponse
                        {
                            DateTime = audit.DateTime,
                            Id = audit.Id,
                            NationalCode = user.NationalCode,
                            UserName = user.Username,
                            PersonName = user.DisplayName,
                            TypeName = audit.TypeName,
                            NewValues = audit.NewValues,
                            OldValues = audit.OldValues,
                            IpAddress = audit.IpAddress
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

            query.OrderBy(q => q.DateTime);

            var totalRowCount = await query.CountAsync();
            if (totalRowCount <= 0)
            {
                return new List<AuditResponse>();
            }

            var response = query
                .Skip(filter.PageNumber * filter.PageSize).Take(filter.PageSize)
                .ToList();

            var userIds = response.Select(s => s.Id);
            foreach (var item in response)
            {
                if (response.IndexOf(item) == 0)
                {
                    item.TotalRowCount = totalRowCount;
                }
                item.DateString = item.DateTime.ToShortPersianDateTimeString();
            }
            return response;
        }
    }
}
