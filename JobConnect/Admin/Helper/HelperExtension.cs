using Microsoft.AspNetCore.Mvc.ModelBinding;
using ViewModel;

namespace Admin.Helper
{
    public static class HelperExtension
    {
        public static string Errors(ModelStateDictionary modelState)
        {
            if (modelState.IsValid)
                return null;

            var list = modelState.Where(ms => ms.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors }).ToList();
            var errors = (from state in list
                          from modelError in state.Errors
                          select new LabelValue
                          {
                              //Value = state.Key,
                              Label = modelError.ErrorMessage.ToLower()
                                  .Replace("the", "")
                                  .Replace("field is required.", "الزامی میباشد.")
                          }).ToList();
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(errors.ToList());
            return data;
        }
        public static string Separator(this int number)
        {
            if (number == 0)
            {
                return "0";
            }
            return number.ToString("#,##");
        }
    }
}
