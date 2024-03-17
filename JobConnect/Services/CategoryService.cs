using DataLayer.Context;
using ViewModel;
using ViewModel.Category;

namespace Services
{
    public interface ICategoryService
    {
        Task<List<LabelValue>> Get();
        Task<ResponseBase> Save(CategorySave request);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;

        }

        public async Task<List<LabelValue>> Get()
        {
            return null;
        }
        public async Task<ResponseBase> Save(CategorySave request)
        {
            return new ResponseBase(true);
        }
    }

}
