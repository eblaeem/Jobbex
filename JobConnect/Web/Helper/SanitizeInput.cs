using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
namespace Api.Helper
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SanitizeInputAttribute : ActionFilterAttribute
    {
        private readonly IHtmlSanitizer _sanitizer;

        public SanitizeInputAttribute()
        {
            _sanitizer = new HtmlSanitizer();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionArguments != null)
            {
                foreach (var parameter in filterContext.ActionArguments.ToList())
                {
                    if(parameter.Value is null)
                    {
                        continue;
                    }

                    var type = parameter.Value.GetType();
                    if (type == typeof(string))
                    {
                        var sanitized = _sanitizer.Sanitize(parameter.Value.ToString());
                        filterContext.ActionArguments[parameter.Key] = sanitized;
                    }
                    else
                    {
                        var properties = parameter.Value.GetType()
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(string) &&
                                        x.GetGetMethod(true).IsPublic && x.GetSetMethod(true).IsPublic);

                        foreach (var propertyInfo in properties)
                        {
                            if (propertyInfo.GetValue(parameter.Value) != null)
                            {
                                var value = propertyInfo.GetValue(parameter.Value).ToString();
                                propertyInfo.SetValue(parameter.Value, _sanitizer.Sanitize(value));
                            }
                        }
                    }
                }
            }
        }
    }
}
