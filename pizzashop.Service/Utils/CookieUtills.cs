using pizzashop.Repository.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace pizzashop.Service.Utils
{
    public static class CookieUtils
    {
        /// <summary>
        /// Save JWT Token to Cookies
        /// </summary>
        /// <param name="response"></param>
        /// <param name="token"></param>
        public static void SaveJWTToken(HttpResponse response, string token, bool rememberMe)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true
            };

            if (rememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddDays(30); // Persistent for 30 days
            }

            response.Cookies.Append("SuperSecretAuthToken", token, cookieOptions);
        }

        public static string? GetJWTToken(HttpRequest request)
        {
            _ = request.Cookies.TryGetValue("SuperSecretAuthToken", out string? token);
            return token;
        }

        /// <summary>
        /// Save User data to Cookies
        /// </summary>
        /// <param name="response"></param>
        /// <param name="user"></param>
        public static void SaveUserData(HttpResponse response, Userslogin user)
        {
            string userData = JsonSerializer.Serialize(user);

            // Store user details in a cookie for 3 days
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(30),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            };
            response.Cookies.Append("UserData", userData, cookieOptions);
        }

        public static void ClearCookies(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("SuperSecretAuthToken");
            httpContext.Response.Cookies.Delete("UserData");
        }
    }
}