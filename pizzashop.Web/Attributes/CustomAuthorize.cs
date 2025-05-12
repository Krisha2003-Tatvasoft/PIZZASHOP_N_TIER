// using pizzashop.Service;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Filters;
// using System.Security.Claims;
// using pizzashop.Service.Utils;
// using pizzashop.Service.Interfaces;

// namespace AuthenticationDemo.Attributes
// {
//     /// <summary>
//     /// Extend with Attribute Class to make this class an Attribute.
//     /// IAuthorizationFilter Interface used to implement the OnAuthorization lifeCycle method.
//     /// </summary>
//     public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
//     {
//         private readonly string[] _roles;

//         public CustomAuthorizeAttribute(params string[] roles)
//         {
//             _roles = roles;
//         }

//         public void OnAuthorization(AuthorizationFilterContext context)
//         {
//             // Inject JwtService to use in Middleware.
//             var jwtService = context.HttpContext.RequestServices.GetService(typeof(IJwtService)) as IJwtService;

//             // Get the token from Cookie
//             var token = CookieUtils.GetJWTToken(context.HttpContext.Request);

//             // Validate Token
//             var principal = jwtService?.ValidateToken(token ?? "");
//             if (principal == null)
//             {
//                 context.Result = new RedirectToActionResult("Login", "Auth", null);
//                 return;
//             }

//             context.HttpContext.User = principal;

//             if (_roles.Length > 0)
//             {
//                 // Get Role Claim from the principal
//                 var userRole = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
//                 if (!_roles.Contains(userRole))
//                 {
//                     context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
//                 }
//             }
//         }
//     }
// }


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using Microsoft.AspNetCore.Authorization;
using pizzashop.web.Attributes;
using Microsoft.AspNetCore.Http;


namespace pizzashop.web.Attributes
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string? _module;
        private readonly string? _permission;

        private readonly string[]? _allowedRoleIds;

        public CustomAuthorizeAttribute(string? module = null, string? permission = null
        , string[]? allowedRoleIds = null)
        {
            _module = module;
            _permission = permission;
            _allowedRoleIds = allowedRoleIds;
        }

        // Required method from IAuthorizationFilter
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Call the async version synchronously (Not recommended, but quick fix)
            OnAuthorizationAsync(context).GetAwaiter().GetResult();
        }

        // Async authorization method
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService(typeof(IJwtService)) as IJwtService;
            var permissionService = context.HttpContext.RequestServices.GetService(typeof(IRolePerService)) as IRolePerService;

            var token = CookieUtils.GetJWTToken(context.HttpContext.Request);
            var principal = jwtService?.ValidateToken(token ?? "");

            if (principal == null)
            {
                string? customerToken = CookieUtils.GetCustomerToken(context.HttpContext.Request);
                principal = jwtService?.ValidateToken(customerToken ?? "");
            }

            if (principal == null)
            {
                context.HttpContext.Response.Cookies.Delete("PermissionData");
                SessionUtils.ClearSession(context.HttpContext);
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            context.HttpContext.User = principal;

            // Extract the user's role (assuming only one role exists)
            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (_allowedRoleIds != null && _allowedRoleIds.Length > 0)
            {
                if (string.IsNullOrEmpty(role) || !_allowedRoleIds.Contains(role))
                {
                    context.Result = new RedirectToActionResult("HttpStatusCodeHandler", "Error", 401);
                    return;
                }
            }

            // If no module is provided, only authentication is required, skip permission check
            if (string.IsNullOrEmpty(_module) || string.IsNullOrEmpty(_permission))
            {
                return; // Authenticated users are allowed
            }


            // Check if the single role has the required permission
            bool hasPermission = await permissionService!.HasPermissionAsync(role, _module, _permission);

            if (!hasPermission)
            {
                context.Result = new RedirectToActionResult("403", "Error", null);
            }
        }
    }

}
