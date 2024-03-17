using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class SelectOptionsTagParameter
    {
        public string Url { get; set; }
        public string DropdownParent { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string ParentId { get; set; }
        public string ChangeUrl { get; set; }
        public bool IsDisabled { get; set; }
        public string AdditionalData { get; set; }
        public bool IsMultiple { get; set; }
        public bool? AllowClear { get; set; }

        public bool IsTag { get; set; }

        public string Name { get; set; }
        public string Id { get; set; }

        public string Columns { get; set; }
        [JsonProperty("placeholder")]
        public string PlaceHolder { get; set; }
        public bool ReplaceChar { get; set; }
        public bool SelectFirst { get; set; }
        public bool TemplateList { get; set; }
    }
}
