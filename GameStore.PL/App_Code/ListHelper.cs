using GameStore.PL.Util.Localizers.Interfaces;
using GameStore.PL.Util.Localizers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace GameStore.PL.App_Code
{
    public static class ListHelper
    {
        public static HtmlString JoinListIntoOneString(this IHtmlHelper html, IEnumerable<string> list, string separator)
        {
            string result = "";
            result = string.Join(separator, list);

            return new HtmlString(result);
        }

        public static HtmlString JoinListIntoOneString<T>(this IHtmlHelper html, 
            IEnumerable<T> list, 
            Expression<Func<T, string>> field, 
            string separator) where T : class, ILocalizable
        {
            List<string> strList = new List<string>();
            foreach (var item in list)
            {
                strList.Add(FieldLocalizer.GetLocalizedField(field, item));
            }

            return JoinListIntoOneString(html, strList, separator);
        }
    }
}
