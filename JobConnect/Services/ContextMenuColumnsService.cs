using Newtonsoft.Json;
using ViewModel;

namespace Services
{
    public interface IContextMenuColumnsService
    {
        string Users();
    }
    public class ContextMenuColumnsService : IContextMenuColumnsService
    {
        public string Users()
        {
            var columns = new List<ContextMenuColumnsModel>()
            {
                new ContextMenuColumnsModel
                {
                    Name="ثبت ضمایم",
                    Url="/attachment/save"
                },
            };
            return JsonConvert.SerializeObject(columns);
        }
    }
}
