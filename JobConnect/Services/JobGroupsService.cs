using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel;

namespace Services
{
    public interface IJobGroupsService
    {
        Task<List<LabelValue>> Get();
    }
    public class JobGroupsService : IJobGroupsService
    {
        private readonly IUnitOfWork _uow;
        public JobGroupsService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<List<LabelValue>> Get()
        {
            return await _uow.Set<Group>().Select(w => new LabelValue
            {
                Label = w.Name,
                Value = w.Id
            }).ToListAsync();
        }
    }
}
