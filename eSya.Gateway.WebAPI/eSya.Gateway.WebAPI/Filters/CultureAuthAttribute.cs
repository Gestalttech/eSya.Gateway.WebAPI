using eSya.Gateway.DL.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eSya.Gateway.WebAPI.Filters
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class CultureAuthAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var requestlang=context.HttpContext.Request.GetTypedHeaders().AcceptLanguage;

            var userLangs = context.HttpContext.Request.Headers["Accept-Language"].ToString();

            var lang = string.Empty;

            if (!string.IsNullOrEmpty(userLangs))
            {
                 lang = userLangs.Split(',').FirstOrDefault();

            }

            var defaultlang = "en-US";

            if (lang != null)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(defaultlang);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
            await next();

        }
    }
}
