using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;

namespace GameStore.PL.Filters
{
    public class ArgumentDecoderFilterAttribute : ActionFilterAttribute, IActionFilter
    {
        private readonly string _argumentName;

        public ArgumentDecoderFilterAttribute(string argumentName)
        {
            _argumentName = argumentName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var argument = context.ActionArguments[_argumentName];
            var decodedArgument = HttpUtility.UrlDecode(argument.ToString());
            context.ActionArguments[_argumentName] = decodedArgument;

            base.OnActionExecuting(context);
        }
    }
}
