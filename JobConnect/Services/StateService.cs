using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel;
using ViewModel.States;

namespace Services
{
    public interface IStateService
    {
        Task<List<LabelValue>> Get();
        Task<List<GroupStateResponse>> GetGroupStates();
        Task<ResponseBase> Save(StateSave request);
        Task<int> TotalAsync();
    }
    public class StateService : IStateService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<StateModel> _states;
        public StateService(IUnitOfWork uow)
        {
            _uow = uow;
            _states = _uow.Set<StateModel>();
        }

        public async Task<List<LabelValue>> Get()
        {
            var response = await _states.Select(s => new LabelValue()
            {
                Label = s.PersianName.Trim(),
                Value = s.Id
            }).AsNoTracking().ToListAsync();
            return response;
        }
        public async Task<List<GroupStateResponse>> GetGroupStates()
        {
            var response = await (from state in _states
                                  from city in _uow.Set<City>().Where(c => c.LocationTypeId == 1 &&
                                  c.StateId == state.Id)
                                  group new { state, city } by state into g
                                  select new GroupStateResponse
                                  {
                                      Label = g.Min(w => w.state.PersianName),
                                      Options = g.Select(s => new LabelValue
                                      {
                                          Label = s.city.PersianName,
                                          Value = s.city.Id,
                                          Data = s.state.Id.ToString(),
                                      }).ToList()
                                  }
                       ).ToListAsync();
            return response;
        }
        public async Task<ResponseBase> Save(StateSave request)
        {
            var find = _states.Find(request.Id);
            if (find == null)
            {
                _states.Add(new StateModel()
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
            return await _states.CountAsync();
        }
    }
}
