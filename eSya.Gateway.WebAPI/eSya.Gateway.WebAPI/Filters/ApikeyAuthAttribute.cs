using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eSya.Gateway.WebAPI.Filters
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class ApikeyAuthAttribute : Attribute, IAsyncActionFilter

    {
        private const string ApikeyHeaderName = "Apikey";
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Key accessing through query string

            //var potentialApikey = context.HttpContext.Request.Query["Apikey"].ToString();
            //if (potentialApikey =="")
            //{
            //    context.Result = new UnauthorizedResult();
            //         return;
            //}

            //Key accessing through header

            if (!context.HttpContext.Request.Headers.TryGetValue(ApikeyHeaderName, out var potentialApikey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apikey = configuration.GetValue<string>(key: "Apikey");
            var stringpotentialApikey = potentialApikey.ToString();
            //if (!apikey.Equals(potentialApikey))

            if (!apikey.Equals(stringpotentialApikey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
        }
    }
}
