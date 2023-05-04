using GameStore.DomainModels.Attributes;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace GameStore.PL.App_Code.TagHelpers
{
    [IgnoreInjections]
    [HtmlTargetElement("Quote")]
    public class QuoteTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "Quote";
            output.TagMode = TagMode.StartTagAndEndTag;

            StringBuilder sb = BuildHtmlContent(output);

            output.Content.SetHtmlContent(sb.ToString());
        }

        private StringBuilder BuildHtmlContent(TagHelperOutput output)
        {
            StringBuilder sb = new StringBuilder();
            string innerHtml = output.GetChildContentAsync().Result.GetContent();
            sb.AppendFormat($"<i><q>{innerHtml}</q></i>");
            return sb;
        }
    }
}
