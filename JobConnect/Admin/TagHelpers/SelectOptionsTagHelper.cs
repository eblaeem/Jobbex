using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Asn1.Cms;
using Services;
using System.Text.Encodings.Web;
using ViewModel;
using ViewModel.States;

namespace Admin.TagHelpers
{
    [HtmlTargetElement("app-select")]
    [HtmlTargetElement("app-select", Attributes = "asp-for")]
    [HtmlTargetElement("app-select", Attributes = "asp-items")]
    public class SelectOptionsTagHelper : TagHelper
    {
        #region Parametrs
        [HtmlAttributeName("name")]
        public string Name { get; set; }

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("asp-items")]
        public ModelExpression Items { get; set; }

        [HtmlAttributeName("value")]
        public string Value { get; set; }

        [HtmlAttributeName("key")]
        public string Key { get; set; }

        [HtmlAttributeName("dropdown-parent")]
        public string DropdownParent { get; set; }

        [HtmlAttributeName("parent-id")]
        public string ParentId { get; set; }

        [HtmlAttributeName("is-disabled")]
        public bool IsDisabled { get; set; }

        [HtmlAttributeName("is-read-only")]
        public bool IsReadOnly { get; set; }


        [HtmlAttributeName("additional-data")]
        public string AdditionalData { get; set; }

        [HtmlAttributeName("multiple")]
        public bool Multiple { get; set; }

        public bool IsTag { get; set; }

        [HtmlAttributeName("check-from-sejam")]
        public string CheckFromSejam { get; set; }

        [HtmlAttributeName("change-url")]
        public string ChangeUrl { get; set; }

        [HtmlAttributeName("columns")]
        public Type Columns { set; get; }

        [HtmlAttributeName("allow-clear")]
        public bool? AllowClear { get; set; }

        [HtmlAttributeName("Url")]
        public string Url { get; set; }

        [HtmlAttributeName("placeholder")]
        public string Placeholder { get; set; }

        public bool ReplaceChar { get; set; } = true;

        [HtmlAttributeName("select-first")]
        public bool SelectFirst { get; set; } = false;


        [HtmlAttributeName("template-list")]
        public bool TemplateList { get; set; } = false;

        #endregion

        private readonly ISecurityService _securityService;
        private readonly ICommonService _commonService;
        public SelectOptionsTagHelper(ISecurityService securityService,
            ICommonService commonService)
        {
            _securityService = securityService;
            _commonService = commonService;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("width", "100%");
            var list = new List<string>()
            {
                "btn",
                "dropdown-toggle",
                "btn-light",
                "form-control"
            };
            foreach (var item in list)
            {
                output.AddClass(item, HtmlEncoder.Default);
            }


            var json = new SelectOptionsTagParameter
            {
                Url = Url,
                DropdownParent = DropdownParent,
                Key = Key,
                Value = Value,
                ParentId = ParentId,
                ChangeUrl = ChangeUrl,
                IsDisabled = IsDisabled,
                AdditionalData = AdditionalData,
                IsMultiple = Multiple,
                IsTag = IsTag,
                AllowClear = AllowClear,
                PlaceHolder = Placeholder,
                ReplaceChar = ReplaceChar,
                SelectFirst = SelectFirst,
                TemplateList = TemplateList
            };

            if (string.IsNullOrEmpty(Name) == false)
            {
                json.Name = Name;
                json.Id = Name.Replace(".", "_");
            }

            if (For != null)
            {
                json.Name = For.Name;
                json.Id = For.Name.Replace(".", "_");

                //_baseTagHelper.AddRequired(output, For);
                //json.PlaceHolder = _baseTagHelper.AddPlaceholder(output, For);
            }
            output.Attributes.Add("Id", json.Id);
            output.Attributes.Add("Name", json.Name);
            output.Attributes.Add("Key", Key);
            output.Attributes.Add("Value", Value);
            if (IsReadOnly)
            {
                output.Attributes.Add("readonly", "readonly");
            }
            if (IsDisabled)
            {
                output.Attributes.Add("disabled", "disabled");
            }

            if (Items != null)
            {
                var placeholder = For?.Metadata?.DisplayName;
                if (string.IsNullOrEmpty(placeholder))
                {
                    placeholder = Placeholder;
                }

                output.Attributes.Add("placeholder", placeholder);
                if (Items.Model is List<LabelValue> results)
                {
  
                    if (json.SelectFirst is true)
                    {
                        results.FirstOrDefault().Selected = true;
                    }

                    var html = $"<option placeholder='{placeholder}'></option>";
                    foreach (var data in results)
                    {
                        if (Key == data.Value.ToString() || data.Selected)
                        {
                            html += $"<option data='{data.Data}' selected value='{data.Value}'>{data.Label.Trim()}</option>";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(data.Label) == false)
                            {
                                html += $"<option data='{data.Data}' value='{data.Value}'>{data.Label.Trim()}</option>";
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(For?.Metadata?.DisplayName) == false)
                    {
                        json.PlaceHolder = For?.Metadata?.DisplayName;
                    }
                    output.Content.AppendHtml(html);

                    if (Multiple == true)
                    {
                        output.PostElement.AppendHtml($"<a class='selectAll' title='انتخاب همه' target='{json.Id}'><i class='fa fa-check-double'></i></a>");
                    }
                }
                if (Items.Model is List<GroupStateResponse> states)
                {
                    var html = $"<option placeholder='{Placeholder}'></option>";

                    foreach (var item in states)
                    {
                        var stateId = item.Options.FirstOrDefault().Data;
                        //html += $"<optgroup label='{item.Label}' value='{stateId}'>";
                        foreach (var data in item.Options)
                        {
                            if (Key == data.Value.ToString() || data.Selected)
                            {
                                html += $"<option data-state-id='{stateId}' selected value='{data.Value}'>{data.Label.Trim()} ({item.Label})</option>";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(data.Label) == false)
                                {
                                    html += $"<option data-state-id='{stateId}' value='{data.Value}'>{data.Label.Trim()}({item.Label})</option>";
                                }
                            }
                        }
                        html += "</optgroup>";
                    }
                    output.Content.AppendHtml(html);
                }
            }


            if (Columns == null)
            {
                AddAttributes(output, json);
                return;
            }
            var instance = Activator.CreateInstance(Columns);
            if (!(instance is IDataTable table))
            {
                AddAttributes(output, json);
                return;
            }

            AddAttributes(output, json);
        }
        private void AddAttributes(TagHelperOutput output, SelectOptionsTagParameter json)
        {
            var result = JsonConvert.SerializeObject(json, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            output.Attributes.Add("parameter", result);
        }
    }
}
