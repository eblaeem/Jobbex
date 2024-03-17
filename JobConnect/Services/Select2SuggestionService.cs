using DataLayer.Context;
using Entities;
using Microsoft.EntityFrameworkCore;
using ViewModel;
using ViewModel.Select2Suggestion;

namespace Services
{
    public interface ISelect2SuggestionService
    {
        Task<List<LabelValue>> Get(string url);
        Task<ResponseBase> Save(Select2SuggestionSave request);
    }
    public class Select2SuggestionService : ISelect2SuggestionService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Select2Suggestion> _select2Suggestions;
        private readonly IUsersService _usersService;
        public Select2SuggestionService(IUnitOfWork uow, IUsersService usersService)
        {
            _uow = uow;
            _select2Suggestions = _uow.Set<Select2Suggestion>();
            _usersService = usersService;
        }

        public async Task<List<LabelValue>> Get(string url)
        {
            var currentUserId = _usersService.GetCurrentUserId();
            var response = await _select2Suggestions
                .Where(a => a.UserId == currentUserId && a.Url == url).OrderByDescending(o => o.DateTime).Take(5)
                .Select(s => new LabelValue()
                {
                    Label = s.Text.Trim(),
                    //Value = s.Value      
                }).AsNoTracking().ToListAsync();
            return response;
        }
        public async Task<ResponseBase> Save(Select2SuggestionSave request)
        {
            var currentUserId = _usersService.GetCurrentUserId();
            var find = await _select2Suggestions.FirstOrDefaultAsync(a => a.UserId == currentUserId
             && a.Url == request.Url && a.Value == request.Value);
            if (find is null)
            {
                _select2Suggestions.Add(new Select2Suggestion()
                {
                    Value = request.Value.ToString(),
                    Text = request.Text.Trim(),
                    Url = request.Url,
                    DateTime = DateTime.Now,
                    UserId = currentUserId,
                });
            }
            else
            {
                find.DateTime = DateTime.Now;
            }

            await _uow.SaveChangesAsync();
            return new ResponseBase(true);
        }
    }
}
