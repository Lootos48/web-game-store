using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.PL.Middlewares
{
    public class RequestLocalizationCookiesMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieRequestCultureProvider Provider { get; }

        public RequestLocalizationCookiesMiddleware(RequestDelegate next, IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            _next = next;

            Provider =
                requestLocalizationOptions
                    .Value
                    .RequestCultureProviders
                    .Where(x => x is CookieRequestCultureProvider)
                    .Cast<CookieRequestCultureProvider>()
                    .FirstOrDefault();
        }

        public Task InvokeAsync(HttpContext context)
        {
            if (Provider != null)
            {
                var feature = context.Features.Get<IRequestCultureFeature>();

                if (feature != null)
                {
                    context.Response
                        .Cookies
                        .Append(
                            Provider.CookieName,
                            CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture)
                        );
                }
            }

            return _next.Invoke(context);
        }
    }
}
