using System.Diagnostics;
using AuthenticationDemo.Attributes;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Web.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using Microsoft.AspNetCore.Authorization;

namespace pizzashop.Web.Controllers;

[CustomAuthorize]
public class ProfileController : Controller
{

    private readonly IProfileService _ProfileService;

    public ProfileController(IProfileService profileService)
    {
        _ProfileService = profileService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [CustomAuthorize]
    [HttpGet]
    public IActionResult ChagePassword()
    {
        return View();
    }

    [CustomAuthorize]
    [HttpPost]
    public async Task<IActionResult> ChagePasswordAsync(ChangePassword viewmodel)
    {
        if(!ModelState.IsValid)
        {
            return View(viewmodel);
        }

        CookieData user = SessionUtils.GetUser(HttpContext);
        if (viewmodel.Newpassword != viewmodel.ConfirmPassword)
        {
            return View();
        }
        else
        {
            if (user != null)
            {
                await _ProfileService.ChangePassword(user.Email, viewmodel);
                CookieUtils.ClearCookies(HttpContext);
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Auth");
            }
        }
        return RedirectToAction("Login", "Auth");
    }

    [HttpGet]
    public async Task<IActionResult> UserProfile()
    {
        CookieData user = SessionUtils.GetUser(HttpContext);
        return View(await _ProfileService.UserProfile(user.Email));
    }

    [HttpPost]
    public async Task<IActionResult> UserProfile(UserProfile viewmodel)
    {
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);
            await _ProfileService.UpdateProfile(user.Userid, viewmodel);
            return RedirectToAction("UserProfile", "Profile");
        }
        else
        {
            return View(await _ProfileService.UserProfile(viewmodel.Email));
        }

    }


}
