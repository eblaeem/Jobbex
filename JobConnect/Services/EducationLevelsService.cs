using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using ViewModel;
using ViewModel.EducationLevels;

namespace Services
{
    public interface IEducationLevelsService
    {
        Task<List<LabelValue>> Get();
        Task<ResponseBase> Save(EducationLevelsSave request);
    }
    public class EducationLevelsService : IEducationLevelsService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Entities.EducationLevel> _educationLevelss;
        public EducationLevelsService(IUnitOfWork uow)
        {
            _uow = uow;
            _educationLevelss = _uow.Set<Entities.EducationLevel>();
        }

        public async Task<List<LabelValue>> Get()
        {
            var response = await _educationLevelss.Select(s => new LabelValue()
            {
                Label = s.Name.Trim(),
                Value = s.Id
            }).AsNoTracking().ToListAsync();
            return response;
        }
        public async Task<ResponseBase> Save(EducationLevelsSave request)
        {
            var find = _educationLevelss.Find(request.Id);
            if (find == null)
            {
                _educationLevelss.Add(new Entities.EducationLevel()
                {
                    Id = request.Id,
                    Name = request.Name
                });
            }
            else
            {
                find.Name = request.Name;
            }
            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }

    }
}
