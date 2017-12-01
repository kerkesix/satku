using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Web.Extensions.TagHelpers
{
    public class SpaViewTagHelper : TagHelper
    {
        public string Name { get; set; }
        public string LayoutClass { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(LayoutClass))
            {
                LayoutClass = "container";
            }

            // Output format is: 
            // <div id="@PageId" class="spapage @hiddenClass">
            //     <div id="@PageId-layout" class="container-fluid">

            // Change this element itself to output div
            output.TagName = "div";

            var content = await output.GetChildContentAsync();
            var inner = content.GetContent();

            output.Attributes.SetAttribute("id", Name);
            output.Attributes.SetAttribute("class", $"spapage preloaded");

            // Create inner div, and content inside that
            output.Content.SetHtmlContent($"<div id='{Name}-layout' class='{LayoutClass}'>" + inner + "</div>");
        }
    }
}