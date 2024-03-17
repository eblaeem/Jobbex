using FluentValidation;

namespace ViewModel.Select2Suggestion
{
    public class Select2SuggestionSave
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
    }
    public class Select2SuggestionSaveValidator : AbstractValidator<Select2SuggestionSave>
    {
        public Select2SuggestionSaveValidator()
        {
            RuleFor(x => x.Value).NotEmpty().NotNull();
            RuleFor(x => x.Text).NotEmpty().NotNull();
            RuleFor(x => x.Url).NotEmpty().NotNull();
        }
    }
}
