using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClubCore.TagHelpers
{
    [HtmlTargetElement("app-pager")]
    public class PagerTagHelper : TagHelper
    {

        const int MaxShowPage = 8;
        private TagHelperOutput _output;
        private int _totalpage;

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("page-number")]
        public int PageNumber { get; set; }

        [HtmlAttributeName("page-size")]
        public int PageSize { get; set; }

        [HtmlAttributeName("total-page-number")]
        public long TotalPageNumber { get; set; }


        [HtmlAttributeName("show-first-last")]
        public bool ShowFirstLast { get; set; }


        [HtmlAttributeName("first-text")]
        public string FirstText { get; set; }


        [HtmlAttributeName("last-text")]
        public string LastText { get; set; }


        [HtmlAttributeName("parametr-name")]
        public string ParametrName { get; set; }


        [HtmlAttributeName("is-ajax")]
        public bool IsAjax { get; set; }


        [HtmlAttributeName("ajax-container")]
        public string AjaxContainer { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.Attributes.Add("class", "pagination");
            _output = output;

            if (string.IsNullOrEmpty(ParametrName))
            {
                ParametrName = "page";
            }
            var totalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(TotalPageNumber) / Convert.ToDecimal(PageSize))));
            _totalpage = totalPages;

            if (totalPages <= 1)
            {
                output.SuppressOutput();
                return;
            }

            var start = 1;
            var end = _totalpage;
            if (totalPages > MaxShowPage)
            {
                GetStartPage(out start, out end);
            }
            if (ShowFirstLast)
            {
                AppendPage(start == 1 && PageNumber == 0 ? "disabled" : "", FirstText);
            }
            for (var i = start; i <= end; i++)
            {
                AppendPage(GetClassName(i), i.ToString());
            }
            if (ShowFirstLast)
            {
                AppendPage(PageNumber == totalPages ? "disabled" : "", LastText);
            }
            output.Content.AppendHtml("</ul>");
        }

        private void AppendPage(string className, string pageNumber)
        {
            var li = new TagBuilder("li");
            var a = new TagBuilder("a");
            var pageText = pageNumber;

            if (pageNumber == FirstText)
            {
                pageNumber = "1";
                pageText = FirstText;
            }
            if (pageNumber == LastText)
            {
                pageNumber = _totalpage.ToString();
                pageText = LastText;
            }
            if (string.IsNullOrEmpty(className) == false)
            {
                li.AddCssClass(className);
            }
            if (IsAjax)
            {
                li.InnerHtml.AppendHtml(AppendAjaxMode(pageNumber, pageText));
            }
            else
            {
                var href = $"/{Controller}/{Action}?{ParametrName}={pageNumber}";
                a.MergeAttribute("href", href);
                a.InnerHtml.Append(pageText);
                li.InnerHtml.AppendHtml(a);
            }

            _output.Content.AppendHtml(li);
        }

        private void GetStartPage(out int start, out int end)
        {
            const int dividedMaxShowPage = MaxShowPage / 2;

            start = Convert.ToInt32(PageNumber);
            if (start == 0)
            {
                start = 1;
            }

            if (start > 1)
            {
                start = start - dividedMaxShowPage;
                if (start <= 1)
                {
                    start = 1;
                }
            }


            end = PageNumber + MaxShowPage - 1;
            if (end >= _totalpage)
            {
                end = _totalpage;
            }
            if (end - start > MaxShowPage)
            {
                end = end - dividedMaxShowPage + 1;
            }
        }

        private TagBuilder AppendAjaxMode(string pageNumber, string pageText)
        {
            var a = new TagBuilder("a");
            a.MergeAttribute("data-ajax", "true");
            a.MergeAttribute($"data-url", $"/{Controller}/{Action}");
            a.MergeAttribute($"data-{ParametrName}", pageNumber);
            a.MergeAttribute("title", pageNumber);
            a.MergeAttribute("data-ajax-container", AjaxContainer);
            a.InnerHtml.Append(pageText);
            return a;
        }
        private string GetClassName(int i)
        {
            var className = i == PageNumber ? "active" : "";
            if (PageNumber == 0 && i == 1)
            {
                className = "active";
            }
            return className;
        }
    }
}
