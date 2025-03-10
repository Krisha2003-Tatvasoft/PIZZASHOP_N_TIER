using AuthenticationDemo.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeKit;
using pizzashop.Entity.ViewModels;
using pizzashop.Service.Interfaces;
using pizzashop.Service.Utils;

namespace pizzashop.Web.Controllers;


[CustomAuthorize]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IEMailService _emailService;


    private readonly IWebHostEnvironment _webHostEnvironment;

    public UserController(IUserService userService, IWebHostEnvironment webHostEnvironment, IEMailService eMailService)
    {
        _userService = userService;
        _webHostEnvironment = webHostEnvironment;
        _emailService = eMailService;
    }

    [HttpGet]
    public async Task<IActionResult> UserList(int page = 1, int pageSize = 5, string search = "", string SortColumn = "", string SortOrder = "")
    {
        var (userlist, totalUsers) = await _userService.GetUserTable(page, pageSize, search, SortColumn, SortOrder);

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalUsers = totalUsers;
        ViewBag.SortColumn = SortColumn;
        ViewBag.SortOrder = SortOrder;

        ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);

        return Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                  ? PartialView("_ShowUserList", userlist)
                  : View(userlist);

    }

    [HttpGet]
    public async Task<IActionResult> AddNewUser()
    {
        return View(await _userService.GetAddNewUser());
    }

    [HttpPost]
    public async Task<IActionResult> AddNewUser(AddNewUser model)
    {
        if (ModelState.IsValid)
        {

            CookieData user = SessionUtils.GetUser(HttpContext);

            if (await _userService.PostAddNewUser(model, user.Userid))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "D:/PIZZASHOP_N_TIER/pizzashop.Web/wwwroot/images/pizzashop_logo.png");
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
                 <img src=""cid:pizzashoplogo""  alt='PizzaShop Logo' style='width:80px;height:80px;'>
                  <span style='padding-left:3px;font-size: 55px; color: #fff; font-weight: 500;'>PIZZASHOP</span>
                </div>

               <div style='background-color: #dcdcdc54; padding: 5px;'>
                    <p>Welcome to Pizza Shop,</p>
                    <p>Please find the details below for login into your account.</p>
                    <div style='border:2px solid #000;padding:5px'>
                       <h2>Login Details:</h2>
                       <h3>Username:{model.Username}</h3>
                       <h3>Temporary Password:{model.Password}</h3>
                    </div>
                    <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                </div>
            </div>
        </body>
     </html>";

                await _emailService.SendResetPasswordEmail(model.Email, bodyBuilder);
                TempData["SuccessMessage"] = "User Added Sucessfully";
                return RedirectToAction("UserList", "user");
            }
            else
            {
                TempData["ErrorMessage"] = "Email Already Exists";
                ModelState.Remove("Countryid");

                return View(await _userService.GetAddNewUser());
            }
        }
        else
        {
            ModelState.Remove("Countryid");
            return View(await _userService.GetAddNewUser());
        }

    }

    [HttpGet]
    public async Task<IActionResult> EditUserAsync(int id)
    {
        return View(await _userService.GetUpdate(id));
    }


    [HttpPost]
    public async Task<IActionResult> EditUserAsync(AddNewUser viewmodel)
    {
        if (ModelState.IsValid)
        {
            if (await _userService.PostUpdate(viewmodel))
            {
                TempData["SuccessMessage"] = "User Updated Sucessfully";
                return RedirectToAction("UserList", "User");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid User";
                return View();
            }
        }
        else
        {
           
            return View(await _userService.GetUpdate(viewmodel.Userid));
        }

    }

    [HttpPost]
    public async Task<IActionResult> DeleteUserAsync(int id)
    {
        if (await _userService.Delete(id))
        {
            TempData["SuccessMessage"] = "User Deleted Sucessfully";
            return RedirectToAction("UserList", "User");
        }
        else
        {
            return View();
        }
    }

}
