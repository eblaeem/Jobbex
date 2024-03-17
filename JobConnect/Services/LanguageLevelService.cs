using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace Services
{
    public interface ILanguageLevelService
    {
        Task<List<LabelValue>> Get();
    }
    public class LanguageLevelService : ILanguageLevelService
    {
        private readonly IUnitOfWork _uow;
        public LanguageLevelService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<List<LabelValue>> Get()
        {
            return await _uow.Set<LanguageLevel>().Select(w => new LabelValue
            {
                Label = w.Name,
                Value = w.Id
            }).ToListAsync();
        }
    }
}
