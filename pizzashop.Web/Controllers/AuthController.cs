using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

using VMLogin = pizzashop.Repository.ViewModels.Login;

namespace pizzashop.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    private readonly IEMailService _emailService;

    private readonly IWebHostEnvironment _webHostEnvironment;

    private static readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new ConcurrentDictionary<string, DateTime>();


    public AuthController(IAuthService authService, IJwtService jwtService, IEMailService emailService, IWebHostEnvironment webHostEnvironment)
    {
        _authService = authService;
        _jwtService = jwtService;
        _emailService = emailService;
        _webHostEnvironment = webHostEnvironment;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        var userdata = SessionUtils.GetUser(HttpContext);

        if (userdata == null)
            return View();
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login(VMLogin model)
    {
        if (ModelState.IsValid)
        {
            var user = await _authService.AuthenticateUser(model.Email, model.Password);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalis Email or Password";
                return View();
            }

            var token = _jwtService.GenerateJwtToken(user.Email, user.Userloginid, user.Role.Rolename, user.Username, user.User.Profileimg);

            CookieUtils.SaveJWTToken(Response, token, model.RememberMe);

            if (model.RememberMe)
            {
                CookieUtils.SaveUserData(Response, user);
            }

            SessionUtils.SetUser(HttpContext, user);

            return RedirectToAction("Index", "Home");
        }
        return View();

    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult Logout()
    {
        // Clear out all the Cookie data
        CookieUtils.ClearCookies(HttpContext);

        // Clear out all the Session data
        SessionUtils.ClearSession(HttpContext);

        return RedirectToAction("Login");
    }


    [HttpGet]
    public IActionResult Forget()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Forget(Forget viewmodal)
    {

        if (ModelState.IsValid)
        {
            if (await _authService.UserExistsAsync(viewmodal))
            {

                var token = _jwtService.GenerateForgetToken(viewmodal.Email);
                // Create reset password link
                var resetLink = Url.Action("ResetPassword", "Auth",
                    new { token = token }, Request.Scheme);

                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "D:/pizzashop_nTier/pizzashop.Web/wwwroot/images/pizzashop_logo.png");
                var bodyBuilder = new BodyBuilder();
                var image = bodyBuilder.LinkedResources.Add(imagePath);
                image.ContentId = "pizzashoplogo";
                bodyBuilder.HtmlBody = $@"
    <!DOCTYPE html>
    <html lang='en'>

    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
       <title>mail</title>
    </head>

    <body>
        <div style='width:600px; border: 1px solid #000;'>

            <div style='background-color:#3c6b9e;display:flex;align-items: center;justify-content: center;'>
                  <img src=""cid:pizzashoplogo"" alt='PizzaShop Logo' style='width:80px;height:80px;'>
                  <span style='font-size: 55px; color: #fff; font-weight: 500;padding-left:3px'>PIZZASHOP</span>
            </div>

            <div style='background-color: #dcdcdc54; padding: 5px;'>
              <p>Pizza Shop,</p>
               <p>Please <a href='{resetLink}' style='color:#007bff;'><u>Click</u></a>
                to reset your password.</p>
               <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
               <p><span style='color: #eab250;'>Important Note:</span> For security reasons, the link will expire in 24 
                hours. If you did not request a password reset, please ignore this email or contact our support team 
                immediately.</p>
            </div>
       </div>
   </body>
</html>";

                await _emailService.SendResetPasswordEmail(viewmodal.Email, bodyBuilder);

                TempData["SuccessMessage"] = "Email Send Sucessfully";
                return View();
            }
            else
            {
                TempData["ErrorMessage"] = "User Not Found";
                return View();
            }
        }
        else
        {
            return View();
        }
    }

    private void BlacklistToken(string tokenId, DateTime expiration)
    {
        _blacklistedTokens.TryAdd(tokenId, expiration);
    }

    private bool IsTokenBlacklisted(string tokenId)
    {
        if (_blacklistedTokens.TryGetValue(tokenId, out DateTime blacklistExpiration))
        {
            if (blacklistExpiration > DateTime.UtcNow)
                return true;
            else
            {
                DateTime removed;
                // Remove expired blacklist entry (should rarely happen when using a far-future date)
                _blacklistedTokens.TryRemove(tokenId, out removed);
                return false;
            }
        }
        return false;
    }

    [HttpGet]
    public IActionResult ResetPassword()
    {
        var token = Request.Query["token"];
        var principal = _jwtService?.ValidateToken(token);
        if (principal == null)
        {
            TempData["ErrorMessage"] = "Invalid or expired token.";
            return View();
        }

        var tokenId = principal.Claims.FirstOrDefault(c => c.Type == "TokenId")?.Value;
        if (string.IsNullOrEmpty(tokenId) || IsTokenBlacklisted(tokenId))
        {
            TempData["ErrorMessage"] = "This password reset link has already been used or is invalid.";
            return View();
        }

        string email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        // Checking if the email is present in the query string
        if (string.IsNullOrEmpty(email))
        {
            // If no email is passed in the query string, return an error
            TempData["ErrorMessage"] = "User Not Found";
            return View();
        }

        // You can now use the email, e.g., check if it exists in the database
        // If everything is fine, you can pass the email to the view
        ViewBag.Email = email;
        ViewBag.Token = token;

        // Return the ResetPassword view
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPassword model)
    {
         var token = model.token;
            var principal = _jwtService?.ValidateToken(token);
            if (principal == null)
            {
                TempData["ErrorMessage"] = "Invalid or expired token.";
                return View(model);
            }

            // Extract TokenId claim.
            var tokenId = principal.Claims.FirstOrDefault(c => c.Type == "TokenId")?.Value;
            if (string.IsNullOrEmpty(tokenId))
            {
                TempData["ErrorMessage"] = "Invalid token payload.";
                return View(model);
            }

            // Check if the token is already blacklisted.
            if (IsTokenBlacklisted(tokenId))
            {
                TempData["ErrorMessage"] = "This token has already been used.";
                return View(model);
            }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _authService.ResetPassword(model);
        
        BlacklistToken(tokenId, DateTime.UtcNow.AddYears(100));

        // Redirect user to login page or show success message
        CookieUtils.ClearCookies(HttpContext);
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Auth");
    }



}
