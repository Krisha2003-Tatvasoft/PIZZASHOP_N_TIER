using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

using VMLogin = pizzashop.Repository.ViewModels.Login;

namespace pizzashop.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        var user = SessionUtils.GetUser(HttpContext);

        if (user == null)
            return View();
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login(VMLogin model)
    {
        var user = await _authService.AuthenticateUser(model.Email, model.Password);
        if (user == null)
        {
            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }

        var token = _jwtService.GenerateJwtToken(user.Email, user.Userloginid, user.Role.Rolename);

        CookieUtils.SaveJWTToken(Response, token, model.RememberMe);

        if (model.RememberMe)
        {
            CookieUtils.SaveUserData(Response, user);
        }

        SessionUtils.SetUser(HttpContext, user);

        return RedirectToAction("Index", "Home");
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

}
