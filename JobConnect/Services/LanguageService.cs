using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel;

namespace Services
{
    public interface ILanguageService
    {
        Task<List<LabelValue>> Get();
    }
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWork _uow;
        public LanguageService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<List<LabelValue>> Get()
        {
            return await _uow.Set<Language>().Select(w => new LabelValue
            {
                Label = w.Name,
                Value = w.Id
            }).ToListAsync();
        }
    }
}
