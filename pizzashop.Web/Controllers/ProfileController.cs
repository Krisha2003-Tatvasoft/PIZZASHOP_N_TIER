using System.Diagnostics;
// using AuthenticationDemo.Attributes;
using pizzashop.web.Attributes;
using Microsoft.AspNetCore.Mvc;
using pizzashop.Web.Models;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace pizzashop.Web.Controllers;

[CustomAuthorize]
public class ProfileController : Controller
{

    private readonly IProfileService _ProfileService;

    private readonly IAuthService _authService;

    public ProfileController(IProfileService profileService, IAuthService authService)
    {
        _ProfileService = profileService;
        _authService = authService;
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
        if (!ModelState.IsValid)
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
                var authUser = await _authService.AuthenticateUser(user.Email, viewmodel.OldPassword);
                if (authUser == null)
                {
                    TempData["ErrorMessage"] = "Old Password is Incorrect";
                    return View();
                }
                await _ProfileService.ChangePassword(user.Email, viewmodel);
                CookieUtils.ClearCookies(HttpContext);
                HttpContext.Session.Clear();
                TempData["SuccessMessage"] = "Password Updated Sucessfully";
                return RedirectToAction("Login", "Auth");
            }
        }
        return RedirectToAction("Login", "Auth");
    }

    [HttpGet]
    [CustomAuthorize]
    public async Task<IActionResult> UserProfile(string from)
    {
        CookieData user = SessionUtils.GetUser(HttpContext);
        if (user != null)
        {
            if(from == "Order")
            {
                ViewBag.UseOrderLayout = true;
            }
            else
            {
                ViewBag.UseOrderLayout = false;
            }
            
            return  View(await _ProfileService.UserProfile(user.Email));
        }
        else
        {
            user = SessionUtils.GetUser(HttpContext);
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpPost]
    [CustomAuthorize]
    public async Task<IActionResult> UserProfile(UserProfile viewmodel, [FromForm(Name = "from")] string? from)
    {
        if (from == "Order")
        {
            ViewBag.UseOrderLayout = true;
             ViewBag.From = from;
        }
        else
        {
            ViewBag.UseOrderLayout = false;
            ViewBag.From = "master";
        }
        if (ModelState.IsValid)
        {
            CookieData user = SessionUtils.GetUser(HttpContext);

            if (await _ProfileService.UpdateProfile(user.Userid, viewmodel) == false)
            {
                TempData["ErrorMessage"] = "Username Already Exist.";

                viewmodel.Countries = await _ProfileService.GetCountryAsync();
                viewmodel.States = await _ProfileService.GetStatesByCountryAsync(viewmodel.Countryid);
                viewmodel.Cities = await _ProfileService.GetCitiesByStateAsync(viewmodel.Stateid);
                return View(viewmodel);
            }
            TempData["SuccessMessage"] = "Profile Updated Sucessfully";
            return RedirectToAction("UserProfile", "Profile", new { from = from });
        }
        else
        {
            viewmodel.Countries = await _ProfileService.GetCountryAsync();
            viewmodel.States = await _ProfileService.GetStatesByCountryAsync(viewmodel.Countryid);
            viewmodel.Cities = await _ProfileService.GetCitiesByStateAsync(viewmodel.Stateid);
            return View(viewmodel);
        }

    }


}
