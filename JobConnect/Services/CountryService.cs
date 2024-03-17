using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel;
using ViewModel.Countries;

namespace Services
{
    public interface ICountryService
    {
        Task<List<LabelValue>> Get();
        Task<ResponseBase> Save(CountrySave request);
        Task<int> TotalAsync();
    }
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Country> _countries;
        public CountryService(IUnitOfWork uow)
        {
            _uow = uow;
            _countries = _uow.Set<Country>();
        }

        public async Task<List<LabelValue>> Get()
        {
            var response = await _countries.Select(s => new LabelValue()
            {
                Label = s.PersianName.Trim(),
                Value = s.Id
            }).AsNoTracking().ToListAsync();
            return response;
        }
        public async Task<ResponseBase> Save(CountrySave request)
        {
            var find = _countries.Find(request.Id);
            if (find == null)
            {
                _countries.Add(new Country()
                {
                    Id = request.Id,
                    PersianName = request.Name
                });
            }
            else
            {
                find.PersianName = request.Name;
            }
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
        public async Task<int> TotalAsync()
        {
            return await _countries.CountAsync();
        }
    }
}
