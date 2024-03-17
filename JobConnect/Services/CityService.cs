using DataLayer.Context;
using ViewModel;
using ViewModel.Cities;

namespace Services
{
    public interface ICityService
    {
        Task<List<LabelValue>> Get();
        Task<ResponseBase> Save(CitySave request);
    }
    public class CityService : ICityService
    {
        private readonly IUnitOfWork _uow;

        public CityService(IUnitOfWork uow)
        {
            _uow = uow;
           
        }

        public async Task<List<LabelValue>> Get()
        {
            return null;
        }
        public async Task<ResponseBase> Save(CitySave request)
        {
            return new ResponseBase(true);
        }
    }
}
