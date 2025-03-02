using Microsoft.AspNetCore.Http;
using pizzashop.Repository.Models;
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
            if (user != null)
            {
                string userData = JsonSerializer.Serialize(user);
                httpContext.Session.SetString("UserData", userData);

                // Store simple string in Session
                //httpContext.Session.SetString("UserId", user.Id.ToString());
            }
        }

        /// <summary>
        /// Method to retrieve user details from session
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static User? GetUser(HttpContext httpContext)
        {
            // Check session first
            string? userData = httpContext.Session.GetString("UserData");

            if (string.IsNullOrEmpty(userData))
            {
                // If session is empty, check the cookie
                httpContext.Request.Cookies.TryGetValue("UserData", out userData);
            }

            return string.IsNullOrEmpty(userData) ? null : JsonSerializer.Deserialize<User>(userData);
        }

        /// <summary>
        /// Method to clear all Session data
        /// </summary>
        /// <param name="httpContext"></param>
        public static void ClearSession(HttpContext httpContext) => httpContext.Session.Clear();
    }
}