﻿using pizzashop.Entity.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using pizzashop.Entity.ViewModels;

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

            // Set the cookie to expire in 1 hour if not "remember me"
            if (!rememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddHours(1); // Non-persistent for 1 hour
            }

            if (rememberMe)
            {
                cookieOptions.Expires = DateTime.UtcNow.AddDays(3); // Persistent for 30 days
            }

            response.Cookies.Append("SuperSecretAuthToken", token, cookieOptions);
        }

        public static string? GetJWTToken(HttpRequest request)
        {
            _ = request.Cookies.TryGetValue("SuperSecretAuthToken", out string? token);
            return token;
        }

        public static string? GetCustomerToken(HttpRequest request)
        {
            _ = request.Cookies.TryGetValue("CustomerToken", out string? token);
            return token;
        }


        /// Save User data to Cookies
        public static void SaveUserData(HttpResponse response, Userslogin user)
        {

            CookieData userdata = new CookieData
            {
                Email = user.Email,
                Userid = user.Userid,
                Rolename = user.Role.Rolename,
                Username = user.Username,
                Image = user.User.Profileimg
            };

            string userData = JsonSerializer.Serialize(userdata);

            // Store user details in a cookie for 3 days
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(3),
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
             httpContext.Response.Cookies.Delete("PermissionData");
        }

        public static void SavePermissionData(HttpResponse response, List<RolePermission> permissionData)
        {
            var model = new PermissionRequest { Permissions = permissionData };
            var options = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            };

            var json = JsonSerializer.Serialize(model);
            response.Cookies.Append("PermissionData", json, options);
        }

        public static string? GetCustomerToken(object request)
        {
            throw new NotImplementedException();
        }
    }
}