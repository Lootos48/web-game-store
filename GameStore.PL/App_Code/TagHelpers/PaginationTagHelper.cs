using AutoMapper;
using GameStore.DomainModels.Attributes;
using GameStore.PL.DTOs;
using GameStore.PL.ViewContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GameStore.PL.App_Code.TagHelpers
{
    [IgnoreInjections]
    public class PaginationTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IMapper _mapper;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public GoodsSearchRequestDTO Filters { get; set; }

        public PaginationTagHelper(IUrlHelperFactory helperFactory, IMapper mapper)
        {
            _mapper = mapper;
            _urlHelperFactory = helperFactory;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            GamesViewsContext pageModel = ViewContext.ViewData.Model as GamesViewsContext;

            Filters = pageModel.Filters;

            output.TagName = "div";

            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            TagBuilder currentPage = CreateTag(Filters.CurrentPage, urlHelper);

            if (Filters.CurrentPage > 1)
            {
                TagBuilder previousPage = CreateTag(Filters.CurrentPage - 1, urlHelper);
                tag.InnerHtml.AppendHtml(previousPage);
            }

            tag.InnerHtml.AppendHtml(currentPage);

            if (pageModel.Games.Count >= Filters.ItemsPerPage)
            {
                TagBuilder nextPage = CreateTag(Filters.CurrentPage + 1, urlHelper);
                tag.InnerHtml.AppendHtml(nextPage);
            }

            output.Content.AppendHtml(tag);
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");

            if (pageNumber == Filters.CurrentPage)
            {
                item.AddCssClass("active");
            }
            else
            {
                GoodsSearchRequestDTO routeValues = _mapper.Map<GoodsSearchRequestDTO>(Filters);

                routeValues.CurrentPage = pageNumber;

                link.Attributes["href"] = urlHelper.Action("Index", "Games", routeValues);
            }

            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);

            return item;
        }
    }
}
