using Microsoft.AspNetCore.Http;
using pizzashop.Entity.Models;
using pizzashop.Entity.ViewModels;
using System.Text.Json;

namespace pizzashop.Service.Utils
{
    public static class SessionUtils
    {
        /// <summary>
        /// Method to store user details in session
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="user"></param>
        public static void SetUser(HttpContext httpContext, Userslogin user)
        {
            CookieData userdata= new CookieData{
                Email=user.Email,
                Userid=user.Userid,
                Rolename=user.Role.Rolename,
                Username=user.Username,
                Image = user.User.Profileimg
               };
            if (user != null)
            {
                string userData = JsonSerializer.Serialize(userdata);
                httpContext.Session.SetString("UserData", userData);

            }
        }

        /// <summary>
        /// Method to retrieve user details from session
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static CookieData? GetUser(HttpContext httpContext)
        {
            // Check session first
            string? userData = httpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userData))
            {
                // If session is empty, check the cookie
                httpContext.Request.Cookies.TryGetValue("UserData", out userData);
            }

            return string.IsNullOrEmpty(userData) ? null : JsonSerializer.Deserialize<CookieData>(userData);
        }

        /// <summary>
        /// Method to clear all Session data
        /// </summary>
        /// <param name="httpContext"></param>
        public static void ClearSession(HttpContext httpContext) => httpContext.Session.Clear();
    }
}