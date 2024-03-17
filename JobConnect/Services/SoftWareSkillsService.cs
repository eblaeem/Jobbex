using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel;

namespace Services
{
    public interface ISoftWareSkillsService
    {
        Task<List<LabelValue>> Get();
        Task<List<LabelValue>> GetPopularTags();
    }
    public class SoftWareSkillsService : ISoftWareSkillsService
    {
        private readonly IUnitOfWork _uow;
        public SoftWareSkillsService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<List<LabelValue>> Get()
        {
            return await _uow.Set<SoftwareSkill>().Select(w => new LabelValue
            {
                Label = w.Name,
                Value = w.Id
            }).ToListAsync();
        }
        public async Task<List<LabelValue>> GetPopularTags()
        {
            var query = await (from softwareSkill in _uow.Set<SoftwareSkill>()
                         from userSoftwareSkill in _uow.Set<UserSoftwareSkill>().Where(w => w.SoftwareSkillId == softwareSkill.Id)
                         group softwareSkill by userSoftwareSkill.SoftwareSkillId into g
                         orderby g.Count() descending
                         select new LabelValue
                         {
                             Label = g.Min(w => w.Name),
                             Value = g.Key
                         }).ToListAsync();
            var response = query.Take(10).ToList();
            return response;
        }
    }
}
