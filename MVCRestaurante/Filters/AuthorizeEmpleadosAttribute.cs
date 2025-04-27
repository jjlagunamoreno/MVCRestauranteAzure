using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVCRestaurante.Filters
{
    public class AuthorizeEmpleadosAttribute : Microsoft.AspNetCore.Authorization.AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Managed" },
                    { "action", "ErrorAcceso" }
                });
            }
        }
    }
}
